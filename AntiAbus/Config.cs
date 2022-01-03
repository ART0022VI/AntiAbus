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
        public bool IsRoundNotStarted { get; set; } = true;
        [Description("Wait message.")]
        public string Wait { get; set; } = "Wait";
        [Description("Seconds message.")]
        public string Seconds { get; set; } = "seconds";
        [Description("RA message before the round start.")]
        public string RoundNotStartedMessage { get; set; } = "<color=red>Round > The round has not started!</color>";
        [Description("RA give MicroHid message.")]
        public string GiveHidMessage { get; set; } = "<color=red>Give > There is only one MicroHid on the map!</color>";
        [Description("RA give limit message.")]
        public string LimitGiveMessage { get; set; } = "<color=red>Give > The limit for issuing items has been exhausted!</color>";
        [Description("RA force limit message.")]
        public string LimitForceMessage { get; set; } = "<color=red>Force > The limit for issuing roles has been exhausted!</color>";
        [Description("RA effect limit message.")]
        public string LimitEffectMessage { get; set; } = "<color=red>Effect > The limit for issuing effects has been exhausted!</color>";
        [Description("RA force MTF message.")]
        public string ForceMTFMessage { get; set; } = "<color=red>MTF > You can't spawn MTF</color>";
        [Description("RA force CI message.")]
        public string ForceCIMessage { get; set; } = "<color=red>CI > You can't spawn CI</color>";
        [Description("RA using FF message.")]
        public string FFMessage { get; set; } = "<color=red>FF > You can 't turn FF</color>";
        [Description("How many seconds do need to wait for the admin panel to work?")]
        public int NeedTimeMinutes { get; set; } = 3;
        [Description("Dictionary: Admin, effect, force, give, heal?")]

        public Dictionary<string, Admin> admins { get; set; } = new Dictionary<string, Admin>()
        {
            ["owner"] = new Admin()
            {
                give = 5,
                force = 4,
                effect = 2,
                heal = 5
            },
            ["gladcat"] = new Admin()
            {
                give = 3,
                force = 3,
                effect = 1,
                heal = 3
            },
            ["admin"] = new Admin()
            {
                give = 2,
                force = 2,
                effect = 0,
                heal = 2
            },
            ["vip"] = new Admin()
            {
                give = 2,
                force = 1,
                effect = 0,
                heal = 1
            }
        };
        public class Admin
        {
            public Admin()
            {
                this.give = 0;
                this.force = 0;
                this.effect = 0;
                this.heal = 0;
            }

            public int give { get; set; }

            public int force { get; set; }

            public int effect { get; set; }

            public int heal { get; set; }
        }
    }
}
