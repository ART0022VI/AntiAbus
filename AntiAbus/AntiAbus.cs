using System;
using Qurre;
using Qurre.API;
using Qurre.API.Events;
using MEC;
using Qurre.API.Objects;
using Qurre.API.Controllers;
using System.Collections.Generic;
using static AntiAbus.Config;
using System.Linq;

namespace AntiAbus
{
    public class AntiAbus : Plugin
    {
        public override string Developer => "KoT0XleB#4663";
        public override string Name => "AntiAbus";
        public override Version Version => new Version(2, 1, 0);
        public override int Priority => int.MinValue;
        public override void Enable() => RegisterEvents();
        public override void Disable() => UnregisterEvents();
        public Config CustomConfig { get; private set; }
        public void RegisterEvents()
        {
            CustomConfig = new Config();
            CustomConfigs.Add(CustomConfig);
            if (!CustomConfig.IsEnable) return;

            Qurre.Events.Server.SendingRA += OnSendRA;
            Qurre.Events.Round.Restart += OnRestart;
        }
        public void UnregisterEvents()
        {
            CustomConfigs.Remove(CustomConfig);
            if (!CustomConfig.IsEnable) return;

            Qurre.Events.Server.SendingRA -= OnSendRA;
            Qurre.Events.Round.Restart -= OnRestart;
        }
        public void OnRestart()
        {
            CustomConfig.Reload();
        }
        public void AddDonatorAdmin(SendingRAEvent ev)
        {
            // Если админ не хост
            if (!(ev.Player.IsHost) && !(ev.Player.Sender.SenderId == null) && !(ev.Player.Sender.SenderId == "1"))
            {
                // Если роль амдина содержится в конфиге
                if (CustomConfig.admins.ContainsKey($"{ev.Player.GroupName}"))
                {
                    // Если админ не содержится в конфиге, то мы его добавляем
                    if (!CustomConfig.admins.ContainsKey(ev.Player.Sender.SenderId))
                    {
                        CustomConfig.admins.Add(ev.Player.Sender.SenderId, new Admin());
                    }
                }
            }
        }
        public void OnSendRA(SendingRAEvent ev)
        {
            // Добавление админа в конфиги
            AddDonatorAdmin(ev);
            // Содержится ли админ в конфигах
            if (CustomConfig.admins.ContainsKey(ev.Player.Sender.SenderId))
            {
                // Если ожидание раунда
                if (Round.Waiting)
                {
                    if (ev.Name == "forcestart")
                    {
                        ev.ReplyMessage = "<color=green>Раунд > Вы начали раунд!</color>";
                        ev.Success = true;
                        ev.Allowed = true;
                        return;
                    }
                    else if (CustomConfig.IsRoundWaiting)
                    {
                        ev.ReplyMessage = CustomConfig.RoundWaitingMessage;
                        ev.Success = false;
                        ev.Allowed = false;
                        return;
                    }
                }
                // Если конец раунда
                if (Round.Ended)
                {
                    if (CustomConfig.IsRoundEnded)
                    {
                        ev.ReplyMessage = CustomConfig.RoundEndedMessage;
                        ev.Success = false;
                        ev.Allowed = false;
                        return;
                    }
                }
                // Перед началом раунда
                //if (Round.ElapsedTime.Minutes < CustomConfig.NeedTimeMinutes)
                //{
                //    ev.ReplyMessage = $"<color=red>Время > {CustomConfig.Wait} {CustomConfig.NeedTimeMinutes * 60 - Round.ElapsedTime.Seconds} {CustomConfig.Seconds}</color>";
                //    ev.Success = false;
                 //   ev.Allowed = false;
                //    return;
                //}
                // Ввод команды с админ панели
                switch (ev.Name)
                {
                    // Выдача предметов
                    case "give":
                        {
                            // Пылесос
                            if (ev.Args[1] == "16")
                            {
                                ev.ReplyMessage = CustomConfig.GiveHidMessage;
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (CustomConfig.admins[ev.CommandSender.SenderId].give >= CustomConfig.admins[$"{ev.Player.GroupName}"].give)
                            {
                                ev.ReplyMessage = CustomConfig.LimitGiveMessage;
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            else CustomConfig.admins[ev.CommandSender.SenderId].give++;
                        }
                        break;
                    // Выдача ролей
                    case "forceclass":
                        {
                            if (CustomConfig.admins[ev.CommandSender.SenderId].force >= CustomConfig.admins[$"{ev.Player.GroupName}"].force)
                            {
                                ev.ReplyMessage = CustomConfig.LimitForceMessage;
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            else CustomConfig.admins[ev.CommandSender.SenderId].force++;
                        }
                        break;
                    // Выдача эффектов
                    case "effect":
                        {
                            if (CustomConfig.admins[ev.CommandSender.SenderId].effect >= CustomConfig.admins[$"{ev.Player.GroupName}"].effect)
                            {
                                ev.ReplyMessage = CustomConfig.LimitEffectMessage;
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            else
                            {
                                CustomConfig.admins[ev.CommandSender.SenderId].effect++;
                            }
                        }
                        break;
                    // Выдача ноуклипа
                    case "noclip":
                        {
                            ev.ReplyMessage = "Извините, донатеры, КотоХлеб выключил Ноуклип.";
                            ev.Success = false;
                            ev.Allowed = false;
                        }
                        break;
                    // Выдача ноуклипа
                    case "god":
                        {
                            ev.ReplyMessage = "Извините, донатеры, КотоХлеб выключил Годмод.";
                            ev.Success = false;
                            ev.Allowed = false;
                        }
                        break;
                    // Спавн отрядов
                    case "server_event":
                        {
                            //Log.Info($"{ev.Name} {ev.Args[0]}");
                            if (ev.Args[0].ToLower() == "force_mtf_respawn" || ev.Args[0].ToLower() == "force_ci_respawn")
                            {
                                if (CustomConfig.admins[ev.CommandSender.SenderId].call >= CustomConfig.admins[$"{ev.Player.GroupName}"].call)
                                {
                                    ev.ReplyMessage = CustomConfig.ForceMTForChaosMessage;
                                    ev.Success = false;
                                    ev.Allowed = false;
                                }
                                else
                                {
                                    ev.ReplyMessage = "<color=green>Отряды > Вы вызвали отряды МОГ или Хаос</color>";
                                    CustomConfig.admins[ev.CommandSender.SenderId].call++;
                                }
                            }
                        }
                        break;
                    // Огонь по своим
                    case "setconfig":
                        {
                            //Log.Info($"{ev.Name} {ev.Args[0]}");
                            ev.Allowed = false;
                            ev.Success = false;
                            ev.ReplyMessage = CustomConfig.FFMessage;
                        }
                        break;
                    // Изменение размера
                    case "size":
                        {
                            if (ev.Args.Length <= 0)
                            {
                                ev.ReplyMessage = "Введите size [ID] [X] [Y] [Z]";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (ev.Args.Length != 4)
                            {
                                ev.ReplyMessage = "Введите size [ID] [X] [Y] [Z]";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (ev.Args[0] == ev.Player.Id.ToString())
                            {
                                if (float.Parse(ev.Args[1]) >= 0.85f && float.Parse(ev.Args[1]) <= 1.15f) // size 2 x
                                {
                                    if (float.Parse(ev.Args[2]) >= 0.85f && float.Parse(ev.Args[1]) <= 1.15f) // size 2 x y
                                    {
                                        if (float.Parse(ev.Args[3]) >= 0.85f && float.Parse(ev.Args[3]) <= 1.15f) // size 2 x y z
                                        {
                                            ev.Success = true;
                                            ev.Allowed = true;
                                            return;
                                        }
                                        else
                                        {
                                            ev.ReplyMessage = "Размер не должен быть больше 1.15 или меньше 0.85";
                                            ev.Success = false;
                                            ev.Allowed = false;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        ev.ReplyMessage = "Размер не должен быть больше 1.15 или меньше 0.85";
                                        ev.Success = false;
                                        ev.Allowed = false;
                                        return;
                                    }
                                }
                                else
                                {
                                    ev.ReplyMessage = "Размер не должен быть больше 1.15 или меньше 0.85";
                                    ev.Success = false;
                                    ev.Allowed = false;
                                    return;
                                }
                            }
                        }
                        break;
                    // Изменение масштаба
                    case "scale":
                        {
                            if (ev.Args.Length <= 0)
                            {
                                ev.ReplyMessage = "Введите scale [Размер]";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (ev.Args.Length != 2)
                            {
                                ev.ReplyMessage = "Введите scale [Размер]";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (ev.Args[0] != ev.Player.Id.ToString())
                            {
                                ev.ReplyMessage = "Вы можете только себе изменить размер.";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (float.Parse(ev.Args[1]) >= 0.85f && float.Parse(ev.Args[1]) <= 1.15f)
                            {
                                ev.Success = true;
                                ev.Allowed = true;
                                return;
                            }
                            else
                            {
                                ev.ReplyMessage = "Размер не должен быть больше 1.15 или меньше 0.85";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                        }
                    // Выдача хилки
                    case "hp":
                        {
                            if (ev.Args.Length <= 0)
                            {
                                ev.ReplyMessage = "<color=red>Введите hp [id] [amount]</color>";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (ev.Args.Length != 2)
                            {
                                ev.ReplyMessage = "<color=red>Введите hp [id] [amount]</color>";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (int.Parse(ev.Args[0]) != ev.Player.Id)
                            {
                                ev.ReplyMessage = "<color=red>Введите свой айди: hp [id] [amount]</color>";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (CustomConfig.admins[ev.CommandSender.SenderId].heal >= CustomConfig.admins[$"{ev.Player.GroupName}"].heal)
                            {
                                ev.ReplyMessage = "<color=red>Хилка > Вы исчерпали свой лимит выдачи ХП</color>";
                                ev.Success = false;
                                ev.Allowed = false;
                            }
                            else
                            {
                                var hp = 50;
                                if (ev.Player.Team == Team.SCP)
                                {
                                    if (Int32.Parse(ev.Args[1]) >= 1000)
                                    {
                                        hp = 500;
                                    }
                                }
                                else if (ev.Player.Team != Team.SCP)
                                {
                                    if (Int32.Parse(ev.Args[1]) >= 100)
                                    {
                                        hp = 100;
                                    }
                                }
                                ev.Player.Hp = hp;
                                ev.Allowed = false;
                                ev.Success = true;
                                ev.ReplyMessage = $"<color=green>Хилка > Вам было выдано {hp} ХП.</color>";
                                CustomConfig.admins[ev.CommandSender.SenderId].heal++;
                            }
                        }
                        break;
                    default: return;
                }
            }
        }
    }
}