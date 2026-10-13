using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Custom.Events.TurkeyEvent
{
    public class TurkeyEvent
    {
        public static readonly string SavePath = Path.Combine("Saves/Misc", "NovemberTurkeyEvent.bin");

        public static bool Enabled { get; set; } = false;
        public static int FeathersPerKill { get; set; } = 250;
        public static int TurkeysPerFacet { get; set; } = 5;

        public static readonly List<EventGiantTurkey> ActiveTurkeys = new List<EventGiantTurkey>();
        private static Timer m_SpawnTimer;

        // Forest Bounding Rectangles
        private static readonly Rectangle2D[] FeluccaForests = new Rectangle2D[]
        {
            new Rectangle2D(550, 750, 600, 600),   // Yew Deep Forest
            new Rectangle2D(2300, 400, 500, 500),  // Minoc / Vesper Woods
            new Rectangle2D(1400, 1900, 500, 600), // Britain / Trinsic Forests
            new Rectangle2D(900, 2000, 400, 500),  // Skara Brae Woods
            new Rectangle2D(600, 2400, 600, 500)   // Spiritwood
        };

        private static readonly Rectangle2D[] TokunoForests = new Rectangle2D[]
        {
            new Rectangle2D(900, 150, 400, 500),   // Isamu Jima
            new Rectangle2D(650, 950, 400, 350),   // Makoto Jima
            new Rectangle2D(150, 550, 500, 400)    // Homare Jima
        };

        private static readonly Rectangle2D[] MalasForests = new Rectangle2D[]
        {
            new Rectangle2D(700, 400, 1200, 500),  // Northern Wilds
            new Rectangle2D(1800, 800, 600, 600),  // Grimswind / Corrupted Forest
            new Rectangle2D(900, 1300, 800, 500)   // Southern Forest
        };

        public static void Initialize()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;

            CommandSystem.Register("TurkeyEvent", AccessLevel.Player, OnTurkeyEventCommand);
            CommandSystem.Register("NovemberEvent", AccessLevel.Player, OnTurkeyEventCommand);
            CommandSystem.Register("TurkeyAdmin", AccessLevel.Player, OnTurkeyEventCommand);
            CommandSystem.Register("HarvestEvent", AccessLevel.Player, OnTurkeyEventCommand);
            CommandSystem.Register("TurkeyRewards", AccessLevel.Player, OnTurkeyRewardsCommand);
            CommandSystem.Register("TurkeyReward", AccessLevel.Player, OnTurkeyRewardsCommand);
            CommandSystem.Register("HarvestRewards", AccessLevel.Player, OnTurkeyRewardsCommand);
            CommandSystem.Register("GetFeathers", AccessLevel.Counselor, OnGetFeathersCommand);
            CommandSystem.Register("GiveFeathers", AccessLevel.Counselor, OnGetFeathersCommand);

            m_SpawnTimer = Timer.DelayCall(TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(60.0), CheckSpawns);
        }

        [Usage("TurkeyEvent")]
        [Description("Opens the November Giant Turkey Event admin control panel.")]
        public static void OnTurkeyEventCommand(CommandEventArgs e)
        {
            if (e.Mobile.AccessLevel < AccessLevel.Counselor)
            {
                e.Mobile.SendMessage(0x22, "You must be a staff member (Counselor or higher) to use this command. Your current rank is {0}.", e.Mobile.AccessLevel);
                return;
            }

            e.Mobile.CloseGump(typeof(TurkeyEventAdminGump));
            e.Mobile.SendGump(new TurkeyEventAdminGump());
        }

        [Usage("TurkeyRewards")]
        [Description("Opens the Turkey Event rewards turn-in menu.")]
        public static void OnTurkeyRewardsCommand(CommandEventArgs e)
        {
            if (e.Mobile != null)
            {
                e.Mobile.CloseGump(typeof(TurkeyRewardGump));
                e.Mobile.SendGump(new TurkeyRewardGump(e.Mobile));
            }
        }

        [Usage("GetFeathers [amount]")]
        [Description("Gives Turkey Feathers to your backpack for testing (default 5,000).")]
        public static void OnGetFeathersCommand(CommandEventArgs e)
        {
            int amount = 5000;
            if (e.Length > 0)
            {
                int.TryParse(e.GetString(0), out amount);
                if (amount <= 0)
                    amount = 5000;
            }

            if (e.Mobile != null && e.Mobile.Backpack != null)
            {
                e.Mobile.AddToBackpack(new TurkeyFeather(amount));
                e.Mobile.SendMessage(0x35, "Added {0:N0} Turkey Feathers to your backpack for testing.", amount);
                e.Mobile.PlaySound(0x2E6);
            }
        }

        public static int GetAliveCount(Map map)
        {
            if (map == null)
                return 0;

            int count = 0;
            for (int i = 0; i < ActiveTurkeys.Count; i++)
            {
                EventGiantTurkey t = ActiveTurkeys[i];
                if (t != null && !t.Deleted && t.Alive && t.Map == map)
                    count++;
            }
            return count;
        }

        public static void RegisterAlive(EventGiantTurkey turkey)
        {
            if (turkey != null && !turkey.Deleted && !ActiveTurkeys.Contains(turkey))
            {
                ActiveTurkeys.Add(turkey);
            }
        }

        public static void OnTurkeyKilled(EventGiantTurkey turkey)
        {
            if (turkey != null)
            {
                ActiveTurkeys.Remove(turkey);
            }

            // Schedule a respawn check after delay (e.g. 15 seconds)
            Timer.DelayCall(TimeSpan.FromSeconds(15.0), CheckSpawns);
        }

        public static void CheckSpawns()
        {
            if (!Enabled)
                return;

            // Clean up deleted/null references
            ActiveTurkeys.RemoveAll(t => t == null || t.Deleted || !t.Alive);

            // Maintain 5 in Felucca, 5 in Tokuno, 5 in Malas
            SpawnForFacet(Map.Felucca, FeluccaForests);
            SpawnForFacet(Map.Tokuno, TokunoForests);
            SpawnForFacet(Map.Malas, MalasForests);
        }

        private static void SpawnForFacet(Map map, Rectangle2D[] zones)
        {
            if (map == null || zones == null || zones.Length == 0)
                return;

            int current = GetAliveCount(map);
            int needed = TurkeysPerFacet - current;

            for (int i = 0; i < needed; i++)
            {
                Point3D loc = FindForestLocation(map, zones);
                if (loc != Point3D.Zero)
                {
                    EventGiantTurkey turkey = new EventGiantTurkey();
                    turkey.MoveToWorld(loc, map);
                    turkey.Home = loc;
                    turkey.RangeHome = 30;

                    ActiveTurkeys.Add(turkey);
                }
            }
        }

        public static Point3D FindForestLocation(Map map, Rectangle2D[] zones)
        {
            if (map == null || zones == null || zones.Length == 0)
                return Point3D.Zero;

            for (int attempt = 0; attempt < 50; attempt++)
            {
                Rectangle2D zone = zones[Utility.Random(zones.Length)];
                int x = Utility.RandomMinMax(zone.X, zone.X + zone.Width);
                int y = Utility.RandomMinMax(zone.Y, zone.Y + zone.Height);
                int z = map.GetAverageZ(x, y);

                if (map.CanSpawnMobile(x, y, z))
                {
                    return new Point3D(x, y, z);
                }
            }

            return Point3D.Zero;
        }

        public static void RespawnAll()
        {
            CleanupTurkeys();
            CheckSpawns();
        }

        public static void CleanupTurkeys()
        {
            for (int i = ActiveTurkeys.Count - 1; i >= 0; i--)
            {
                EventGiantTurkey t = ActiveTurkeys[i];
                if (t != null && !t.Deleted)
                {
                    t.Delete();
                }
            }
            ActiveTurkeys.Clear();
        }

        private static void OnSave(WorldSaveEventArgs e)
        {
            try
            {
                string dir = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (FileStream fs = new FileStream(SavePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write((int)0); // version
                    bw.Write(Enabled);
                    bw.Write(FeathersPerKill);
                    bw.Write(TurkeysPerFacet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving TurkeyEvent: {0}", ex);
            }
        }

        private static void OnLoad()
        {
            if (!File.Exists(SavePath))
                return;

            try
            {
                using (FileStream fs = new FileStream(SavePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int version = br.ReadInt32();
                    Enabled = br.ReadBoolean();
                    FeathersPerKill = br.ReadInt32();
                    TurkeysPerFacet = br.ReadInt32();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading TurkeyEvent: {0}", ex);
            }
        }
    }
}
