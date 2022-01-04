using System;
using Qurre;
using Qurre.API;
using Qurre.API.Events;
using MEC;
using Qurre.API.Objects;
using Qurre.API.Controllers;
using System.Collections.Generic;
using static AntiAbus.Config;

namespace AntiAbus
{
    public class AntiAbus : Plugin
    {
        public static bool Enabled { get; internal set; }
        public override string Developer => "KoToXleB#4663";
        public override string Name => "AntiAbus";
        public override Version Version => new Version(1, 0, 0);
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
        }
        public void UnregisterEvents()
        {
            CustomConfigs.Remove(CustomConfig);
            if (!CustomConfig.IsEnable) return;

            Qurre.Events.Server.SendingRA -= OnSendRA;
        }
        public void OnSendRA(SendingRAEvent ev)
        {
            if (!(ev.CommandSender.SenderId == null))
            {
                if (!(ev.CommandSender.SenderId == "1"))
                {
                    if (CustomConfig.admins.ContainsKey($"{ev.Player.GroupName}"))
                    {
                        if (!CustomConfig.admins.ContainsKey(ev.CommandSender.SenderId))
                        {
                            CustomConfig.admins.Add(ev.CommandSender.SenderId, new Admin());
                        }
                    }
                    else return;
                    if (!Round.Started)
                    {
                        if (CustomConfig.IsRoundNotStarted)
                        {
                            ev.ReplyMessage = CustomConfig.RoundNotStartedMessage;
                            ev.Success = false;
                            ev.Allowed = false;
                            return;
                        }
                    }
                    else
                    {
                        if (Round.ElapsedTime.Minutes < CustomConfig.NeedTimeMinutes)
                        {
                            ev.ReplyMessage = $"<color=red>Time > {CustomConfig.Wait} {CustomConfig.NeedTimeMinutes * 60 - Round.ElapsedTime.Seconds} {CustomConfig.Seconds}</color>";
                            ev.Success = false;
                            ev.Allowed = false;
                            return;
                        }
                        if (ev.Name == "give")
                        {
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
                            }
                            else CustomConfig.admins[ev.CommandSender.SenderId].give++;
                        }

                        if (ev.Name == "forceclass")
                        {
                            if (CustomConfig.admins[ev.CommandSender.SenderId].force >= CustomConfig.admins[$"{ev.Player.GroupName}"].force)
                            {
                                ev.ReplyMessage = CustomConfig.LimitForceMessage;
                                ev.Success = false;
                                ev.Allowed = false;
                            }
                            else CustomConfig.admins[ev.CommandSender.SenderId].force++;
                        }

                        if (ev.Name == "effect")
                        {
                            if (CustomConfig.admins[ev.CommandSender.SenderId].effect >= CustomConfig.admins[$"{ev.Player.GroupName}"].effect)
                            {
                                ev.ReplyMessage = CustomConfig.LimitEffectMessage;
                                ev.Success = false;
                                ev.Allowed = false;
                            }
                            else CustomConfig.admins[ev.CommandSender.SenderId].effect++;
                        }
                        if (ev.Name == "server_event")
                        {
                            Log.Info($"{ev.Name} {ev.Args[0]}");
                            ev.Allowed = false;
                            if (ev.Args[0].ToLower() == "force_mtf_respawn")
                            {
                                ev.Success = false;
                                ev.ReplyMessage = CustomConfig.ForceMTFMessage;
                            }
                            else if (ev.Args[0].ToLower() == "force_ci_respawn")
                            {
                                ev.Success = false;
                                ev.ReplyMessage = CustomConfig.ForceCIMessage;
                            }
                        }
                        if (ev.Name == "setconfig")
                        {
                            Log.Info($"{ev.Name} {ev.Args[0]}");
                            ev.Allowed = false;
                            ev.Success = false;
                            ev.ReplyMessage = CustomConfig.FFMessage;
                        }
                        /*
                        if (ev.Name == "mute" || ev.Name == "unmute")
                        {
                            Log.Info($"{ev.Name} {ev.Args[0]}");
                            ev.Allowed = false;
                            ev.Success = false;
                            ev.ReplyMessage = "<color=red>mutes > Не мутить людей!</color>";
                        }*/
                    }
                }
            }
        }
    }
}
