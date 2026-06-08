using System;
using System.Collections.Generic;
using Server;
using Server.Engines.CannedEvil;

namespace Server.Custom.Events
{
    public class CustomChampTheme
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public int Hue { get; set; }
        public int Body { get; set; }

        // Stats
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Int { get; set; }
        public int Hits { get; set; }
        public int Stam { get; set; }
        public int Mana { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }

        // Resistances
        public int PhysicalResist { get; set; }
        public int FireResist { get; set; }
        public int ColdResist { get; set; }
        public int PoisonResist { get; set; }
        public int EnergyResist { get; set; }

        // Spawn waves (Level 1, 2, 3, 4)
        public Type[][] SpawnTypes { get; set; }

        // Reward lists (Clean custom artifacts)
        public Type[] UniqueList { get; set; }
        public Type[] SharedList { get; set; }
        public Type[] DecorativeList { get; set; }

        // Champion Spawn System mapping
        public ChampionSpawnType SpawnType { get; set; }
        public ChampionSkullType SkullType { get; set; }

        public CustomChampTheme(string name)
        {
            Name = name;
            Title = null;
            Hue = 0;
            Body = 0x3E7;

            Str = 1500;
            Dex = 1500;
            Int = 1500;
            Hits = 320000;
            Stam = 750;
            Mana = 1200;
            MinDamage = 35;
            MaxDamage = 45;

            PhysicalResist = 80;
            FireResist = 65;
            ColdResist = 85;
            PoisonResist = 20;
            EnergyResist = 75;

            SkullType = ChampionSkullType.None;
        }
    }
}
