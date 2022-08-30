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
        public bool TeamIsRespawning = false;
        public void RegisterEvents()
        {
            CustomConfig = new Config();
            CustomConfigs.Add(CustomConfig);
            if (!CustomConfig.IsEnable) return;

            Qurre.Events.Server.SendingRA += OnSendRA;
            Qurre.Events.Server.SendingConsole += OnSendConsole;
            Qurre.Events.Round.Restart += OnRestart;
            Qurre.Events.Round.TeamRespawn += OnTeamRespawn;
        }
        public void UnregisterEvents()
        {
            CustomConfigs.Remove(CustomConfig);
            if (!CustomConfig.IsEnable) return;

            Qurre.Events.Server.SendingRA -= OnSendRA;
            Qurre.Events.Server.SendingConsole -= OnSendConsole;
            Qurre.Events.Round.Restart -= OnRestart;
            Qurre.Events.Round.TeamRespawn -= OnTeamRespawn;
        }
        public void OnRestart() => CustomConfig.Reload();
        public void OnTeamRespawn(TeamRespawnEvent ev) => TeamIsRespawning = true;
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
        public void OnSendConsole(SendingConsoleEvent ev)
        {
            // Содержится ли админ в конфигах
            if (CustomConfig.admins.ContainsKey(ev.Player.Sender.SenderId))
            {
                ev.Allowed = false;
            }
        } 
        public void OnSendRA(SendingRAEvent ev)
        {
            Log.Info(ev.Name);
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
                if (CustomConfig.DoTimeAfterRoundStarted && Round.ElapsedTime.Minutes < CustomConfig.NeedTimeMinutes)
                {
                    ev.ReplyMessage = $"Подождите {CustomConfig.NeedTimeMinutes} минуту от старта раунда.";
                    ev.Success = false;
                    ev.Allowed = false;
                    return;
                }
                // Ввод команды с админ панели
                switch (ev.Name)
                {
                    // Выдача предметов
                    case "give":
                        {
                            if (Round.ElapsedTime.Minutes < 3)
                            {
                                switch(byte.Parse(ev.Args[1]))
                                {
                                    case 13: 
                                    case 20: 
                                    case 21:
                                    case 23:
                                    case 24:
                                    case 25:
                                    case 30:
                                    case 31:
                                    case 39:
                                    case 40:
                                    case 41:
                                    case 47:
                                        {
                                            ev.ReplyMessage = $"Огнестрел ждите 3 минуты от старта раунда.";
                                            ev.Success = false;
                                            ev.Allowed = false;
                                        }
                                        break;
                                }
                            }
                            // Пылесос
                            if (CustomConfig.BlockItems.Contains((ItemType)byte.Parse(ev.Args[1])))
                            {
                                ev.ReplyMessage = CustomConfig.BlockItemMessage;
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            if (ev.Player.Team == Team.SCP)
                            {
                                ev.ReplyMessage = "<color=red>У SCP нет рук!</color>";
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
                            switch(ev.Args[1])
                            {
                                // SCP class
                                case "0":
                                case "3":
                                case "5":
                                case "7":
                                case "9":
                                case "10":
                                case "16":
                                case "17":
                                    {
                                        ev.ReplyMessage = "Вы не можете стать SCP!";
                                        ev.Success = false;
                                        ev.Allowed = false;
                                        return;
                                    }
                                // MTF and CI class
                                case "4":
                                case "8":
                                case "11":
                                case "12":
                                case "13":
                                case "18":
                                case "19":
                                case "20":
                                    {
                                        if (TeamIsRespawning) break;
                                        ev.ReplyMessage = "Подождите, пока отряды не заспавнятся!";
                                        ev.Success = false;
                                        ev.Allowed = false;
                                        return;
                                    }
                                default: break;
                            }
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
                                ev.ReplyMessage = "Выдан эффект на 10 секунд.";
                                ev.Success = true;
                                ev.Allowed = true;
                                Timing.CallDelayed(10, () =>
                                {
                                    ev.Player.DisableAllEffects();
                                });
                                CustomConfig.admins[ev.CommandSender.SenderId].effect++;
                            }
                        }
                        break;
                    // Выдача ноуклипа
                    case "noclip":
                        {
                            if (ev.Args[1] == "enable")
                            {
                                if (CustomConfig.admins[ev.CommandSender.SenderId].noclip >= CustomConfig.admins[$"{ev.Player.GroupName}"].noclip)
                                {
                                    ev.ReplyMessage = "Вы достаточно использовали Noclip.";
                                    ev.Success = false;
                                    ev.Allowed = false;
                                    return;
                                }
                                CustomConfig.admins[ev.CommandSender.SenderId].noclip++;
                                ev.Player.Noclip = true;
                                ev.ReplyMessage = "Выдан ноуклип на 15 секунд.";
                                ev.Success = true;
                                ev.Allowed = false;

                                Timing.CallDelayed(15, () =>
                                {
                                    ev.Player.Noclip = false;
                                    GameCore.Console.singleton.TypeCommand($"/noclip {ev.Player.Id} disable");
                                });
                            }
                        }
                        break;
                    // Выдача ноуклипа
                    case "god":
                        {
                            if (CustomConfig.admins[ev.CommandSender.SenderId].god >= CustomConfig.admins[$"{ev.Player.GroupName}"].god)
                            {
                                ev.ReplyMessage = "Вы достаточно использовали Godmod.";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            CustomConfig.admins[ev.CommandSender.SenderId].god++;
                            ev.Player.GodMode = true;

                            ev.ReplyMessage = "Godmod выдан на 15 секунд.";
                            ev.Success = true;
                            ev.Allowed = false;

                            Timing.CallDelayed(15, () =>
                            {
                                ev.Player.GodMode = false;
                            });
                        }
                        break;
                    // Возможность открывать двери без ключей
                    case "bypass":
                        {
                            if (CustomConfig.admins[ev.CommandSender.SenderId].bypass >= CustomConfig.admins[$"{ev.Player.GroupName}"].bypass)
                            {
                                ev.ReplyMessage = "Вы достаточно использовали Bypass.";
                                ev.Success = false;
                                ev.Allowed = false;
                                return;
                            }
                            CustomConfig.admins[ev.CommandSender.SenderId].bypass++;
                            ev.Player.BypassMode = true;

                            ev.ReplyMessage = "Bypass выдан на 15 секунд.";
                            ev.Success = true;
                            ev.Allowed = false;

                            Timing.CallDelayed(15, () =>
                            {
                                ev.Player.BypassMode = false;
                            });
                        }
                        break;
                    // Спавн отрядов
                    case "server_event":
                        {
                            if (CustomConfig.DoTeamRespawn)
                            {
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
                                else
                                {
                                    ev.ReplyMessage = "Донатеры не могут перезапускать раунд.";
                                    ev.Success = false;
                                    ev.Allowed = false;
                                }
                            }
                            else
                            {
                                ev.ReplyMessage = "Вы не можете спавнить отряды.";
                                ev.Success = false;
                                ev.Allowed = false;
                            }
                        }
                        break;
                    // Огонь по своим
                    case "setconfig":
                        {
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
                    case "roundlock":
                        {
                            ev.ReplyMessage = $"Донатеры не могут блокировать раунд.";
                            ev.Allowed = false;
                            ev.Success = false;
                        }
                        break;
                    case "lobbylock":
                        {
                            ev.ReplyMessage = $"Донатеры не могут блокировать раунд.";
                            ev.Allowed = false;
                            ev.Success = false;
                        }
                        break;
                    case "open":
                        {
                            ev.ReplyMessage = $"Донатеры не могут открывать двери.";
                            ev.Allowed = false;
                            ev.Success = false;
                        }
                        break;
                    default:
                        {
                            ev.ReplyMessage = $"Нельзя использовать эту команду сервера.";
                            ev.Allowed = false;
                            ev.Success = false;
                        }
                        break;
                }
            }
        }
    }
}