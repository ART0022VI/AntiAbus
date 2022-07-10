using Qurre.API.Addons;
using System.Collections.Generic;
using System.ComponentModel;
using static AntiAbus.AntiAbus;

namespace AntiAbus
{
    public class Config : IConfig
    {
        [Description("Plugin Name")]
        public string Name { get; set; } = "AntiAbus";

        [Description("Enable the plugin?")]
        public bool IsEnable { get; set; } = true;
        [Description("Disable the ability to use the admin panel before the round start?")]
        public bool IsRoundWaiting { get; set; } = true;
        [Description("Disable the ability to use the admin panel after the end of the round?")]
        public bool IsRoundEnded { get; set; } = true;
        [Description("Wait message.")]
        public string Wait { get; set; } = "Подождите"; // "Wait";
        [Description("Seconds message.")]
        public string Seconds { get; set; } = "секунд"; // "seconds";
        [Description("RA message before the round start.")]
        public string RoundWaitingMessage { get; set; } = "<color=red>Раунд > Раунд ещё не начался!</color>"; // "<color=red>Round > The round has not started!</color>";
        [Description("RA message before the round start.")]
        public string RoundEndedMessage { get; set; } = "<color=red>Раунд > Раунд ещё не закончился!</color>"; // "<color=red>Round > The round has ended!</color>";
        [Description("RA give MicroHid message.")]
        public string GiveHidMessage { get; set; } = "<color=red>Предметы > На карте уже есть МикроХид!</color>"; // "<color=red>Give > There is only one MicroHid on the map!</color>";
        [Description("RA give limit message.")]
        public string LimitGiveMessage { get; set; } = "<color=red>Предметы > Вы исчерпали свой лимит выдачи предметов!</color>"; // "<color=red>Give > The limit for issuing items has been exhausted!</color>";
        [Description("RA force limit message.")]
        public string LimitForceMessage { get; set; } = "<color=red>Роли > Вы исчерпали свой лимит выдачи ролей!</color>"; // "<color=red>Force > The limit for issuing roles has been exhausted!</color>";
        [Description("RA effect limit message.")]
        public string LimitEffectMessage { get; set; } = "<color=red>Эффекты > Вы исчерпали свой лимит выдачи эффектов</color>"; // "<color=red>Effect > The limit for issuing effects has been exhausted!</color>";
        [Description("RA force MTF message.")]
        public string ForceMTForChaosMessage { get; set; } = "<color=red>Отряды > Вы исчерпали свой лимит на вызов отрядов Мог/Хаос</color>"; // "<color=red>MTF > You can't spawn MTF</color>";
        //[Description("RA force CI message.")]
        //public string ForceCIMessage { get; set; } = "<color=red>ХАОС > Ты не можешь вызвать Хаоситов</color>"; // "<color=red>CI > You can't spawn CI</color>";
        [Description("RA using FF message.")]
        public string FFMessage { get; set; } = "<color=red>ФФ > Вы не можете включить 'Огонь по Своим'</color>"; // "<color=red>FF > You can 't turn FF</color>";
        [Description("How many seconds do need to wait for the admin panel to work?")]
        public int NeedTimeMinutes { get; set; } = 3;
        [Description("Dictionary: Admin, effect, force, give, heal?")]

        public Dictionary<string, Admin> admins { get; set; } = new Dictionary<string, Admin>()
        {
            ["owner"] = new Admin()
            {
                give = 5,
                force = 4,
                effect = 5,
                heal = 1,
                call = 2
            },
            ["gladcat"] = new Admin()
            {
                give = 4,
                force = 3,
                effect = 2,
                heal = 1,
                call = 1
            },
            ["admin"] = new Admin()
            {
                give = 3,
                force = 3,
                effect = 1,
                heal = 1,
                call = 0
            },
            ["vip"] = new Admin()
            {
                give = 2,
                force = 1,
                effect = 0,
                heal = 0,
                call = 0
            }
        };
        public class Admin
        {
            public int give { get; set; }

            public int force { get; set; }

            public int effect { get; set; }

            public int heal { get; set; }

            public int call { get; set; }

            public Admin()
            {
                this.give = 0;
                this.force = 0;
                this.effect = 0;
                this.heal = 0;
                this.call = 0;
            }
        }
    }
}
