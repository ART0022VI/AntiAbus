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
        [Description("RA block giving items message.")]
        public string BlockItemMessage { get; set; } = "<color=red>Предметы > Вы не можете получить эти вещи!</color>"; // "<color=red>Give > There is only one MicroHid on the map!</color>";
        [Description("RA give limit message.")]
        public string LimitGiveMessage { get; set; } = "<color=red>Предметы > Вы исчерпали свой лимит выдачи предметов!</color>"; // "<color=red>Give > The limit for issuing items has been exhausted!</color>";
        [Description("RA force limit message.")]
        public string LimitForceMessage { get; set; } = "<color=red>Роли > Вы исчерпали свой лимит выдачи ролей!</color>"; // "<color=red>Force > The limit for issuing roles has been exhausted!</color>";
        [Description("RA effect limit message.")]
        public string LimitEffectMessage { get; set; } = "<color=red>Эффекты > Вы исчерпали свой лимит выдачи эффектов</color>"; // "<color=red>Effect > The limit for issuing effects has been exhausted!</color>";
        [Description("RA force MTF message.")]
        public string ForceMTForChaosMessage { get; set; } = "<color=red>Отряды > Вы исчерпали свой лимит на вызов отрядов Мог/Хаос</color>"; // "<color=red>MTF > You can't spawn MTF</color>";
        public List<ItemType> BlockItems { get; set; } = new List<ItemType>()
        {
            ItemType.MicroHID,
            ItemType.SCP2176,
            ItemType.ParticleDisruptor,
            ItemType.SCP244a,
            ItemType.SCP244b
        };
        [Description("RA using FF message.")]
        public string FFMessage { get; set; } = "<color=red>ФФ > Вы не можете включить 'Огонь по Своим'</color>"; // "<color=red>FF > You can 't turn FF</color>";
        [Description("How many seconds do need to wait for the admin panel to work?")]
        public int NeedTimeMinutes { get; set; } = 1;
        [Description("Давать возможность донатерам спавнить отряды?")] // Give donaters the opportunity to spawn squads?
        public bool DoTeamRespawn { get; set; } = false;
        [Description("Делать ожидание использования админ панелью после начала раунда?")] // Should I wait for the admin panel to be used after the start of the round?
        public bool DoTimeAfterRoundStarted { get; set; } = false;
        [Description("Dictionary: Admin, effect, force, give, heal?")]

        public Dictionary<string, Admin> admins { get; set; } = new Dictionary<string, Admin>()
        {
            ["owner"] = new Admin()
            {
                give = 5,
                force = 4,
                effect = 5,
                heal = 1,
                call = 2,
                god = 1,
                noclip = 2,
                bypass = 1
            },
            ["gladcat"] = new Admin()
            {
                give = 4,
                force = 3,
                effect = 2,
                heal = 1,
                call = 1,
                god = 1,
                noclip = 1,
                bypass = 1
            },
            ["admin"] = new Admin()
            {
                give = 3,
                force = 3,
                effect = 1,
                heal = 1,
                call = 0,
                god = 0,
                noclip = 0,
                bypass = 0
            },
            ["vip"] = new Admin()
            {
                give = 2,
                force = 1,
                effect = 0,
                heal = 0,
                call = 0,
                god = 0,
                noclip = 0,
                bypass = 0
            },
        };
        public class Admin
        {
            public int give { get; set; }

            public int force { get; set; }

            public int effect { get; set; }

            public int heal { get; set; }

            public int call { get; set; }

            public int god { get; set; }

            public int noclip { get; set; }
            public int bypass { get; set; }

            public Admin()
            {
                this.give = 0;
                this.force = 0;
                this.effect = 0;
                this.heal = 0;
                this.call = 0;
                this.god = 0;
                this.noclip = 0;
                this.bypass = 0;
            }
        }
    }
}
