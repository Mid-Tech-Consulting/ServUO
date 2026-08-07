using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.CannedEvil;

namespace Server.Custom.Events
{
    public static class CustomChampThemes
    {
        public static readonly List<CustomChampTheme> All = new List<CustomChampTheme>();

        public static void Initialize()
        {
            // Build and register custom themes
            Register(BuildBigBeast());

            // You can easily register more custom champ spawns here in the future!
            // Register(BuildSomeOtherBoss());
        }

        public static void Register(CustomChampTheme theme)
        {
            // Set up standard ChampionSpawnInfo using our generic CustomChampion mobile
            ChampionSpawnInfo info = new ChampionSpawnInfo(
                theme.Name,
                typeof(CustomChampion),
                new string[] { "Stalker", "Hunter", "Slayer" },
                theme.SpawnTypes
            );

            // Register dynamically in the Champion System
            ChampionSpawnInfo.Register(info);

            // Assign the dynamically mapped ChampionSpawnType index
            theme.SpawnType = (ChampionSpawnType)(ChampionSpawnInfo.DynamicTable.Count - 1);

            All.Add(theme);

            Utility.WriteConsoleColor(ConsoleColor.Green, String.Format("Registered Custom Champion Spawn: {0} (SpawnType ID: {1})", theme.Name, (int)theme.SpawnType));
        }

        public static CustomChampTheme Find(ChampionSpawnType type)
        {
            int index = (int)type;
            foreach (CustomChampTheme t in All)
            {
                if ((int)t.SpawnType == index)
                    return t;
            }
            return null;
        }

        public static CustomChampTheme Find(string name)
        {
            return All.Find(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private static CustomChampTheme BuildBigBeast()
        {
            CustomChampTheme t = new CustomChampTheme("Big Beast");
            t.Body = 0x3E7;
            t.Hue = 0;

            // Stats
            t.Str = 1500;
            t.Dex = 1500;
            t.Int = 1500;
            t.Hits = 320000;
            t.Stam = 750;
            t.Mana = 1200;
            t.MinDamage = 35;
            t.MaxDamage = 45;

            // Resistances
            t.PhysicalResist = 80;
            t.FireResist = 65;
            t.ColdResist = 85;
            t.PoisonResist = 20;
            t.EnergyResist = 75;

            // Spawn waves (Level 1, 2, 3, 4)
            t.SpawnTypes = new Type[][]
            {
                new Type[] { typeof(Skeleton2), typeof(Zombie) }, // Level 1 (Undead Slayer)
                new Type[] { typeof(FlameImp), typeof(Gargoyle) }, // Level 2 (Demon/Abyss Slayer)
                new Type[] { typeof(Dragon), typeof(Drake), typeof(GreaterDragon) }, // Level 3 (Reptile Slayer - Dragons, Drakes, Greater Dragons)
                new Type[] { typeof(PoisonElemental), typeof(TerathanAvenger) } // Level 4 (Arachnid Slayer)
            };

            // Custom Clean Artifact Rewards
            t.UniqueList = new Type[] { typeof(DyingDiamonds), typeof(GargishDyingDiamonds), typeof(CapeOfCourage), typeof(GargishCapeOfCourage), typeof(HoodedCoverings), typeof(GargishHoodedCoverings), typeof(NeckGuard), typeof(GargishNeckGuard) };
            t.SharedList = new Type[] { typeof(BeastlyTunic), typeof(GargishBeastlyTunic), typeof(BookOfBeastlyDesire), typeof(TwiceMittens), typeof(GargishTwiceMittens), typeof(WorldsEnd), typeof(GargishWorldsEnd), typeof(DivineKryss), typeof(DivineKryssGargish), typeof(DivineSpear), typeof(DivineSpearGargish) };
            t.DecorativeList = new Type[] { typeof(BramblewoodSleeves), typeof(GargishBramblewoodSleeves), typeof(FabeledSash), typeof(GargishFabeledSash) };

            return t;
        }
    }
}
