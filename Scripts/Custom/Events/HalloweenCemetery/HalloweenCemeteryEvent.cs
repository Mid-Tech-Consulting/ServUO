using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Custom.Events.HalloweenCemetery
{
    public class HalloweenCemeteryEvent
    {
        public static readonly string SavePath = Path.Combine("Saves/Misc", "HalloweenCemeteryEvent.bin");

        public static bool Enabled { get; set; } = true;
        public static double DyeDropChance { get; set; } = 0.04;       // 4.0% base chance
        public static double SpiderMountChance { get; set; } = 0.005;  // 0.5% base chance (1 in 200)
        public static bool LuckAffectsDrops { get; set; } = true;
        public static int MinPumpkins { get; set; } = 1;
        public static int MaxPumpkins { get; set; } = 4;

        // Cemetery Bounding Boxes (Trammel & Felucca)
        public static readonly Rectangle2D[] Cemeteries = new Rectangle2D[]
        {
            new Rectangle2D(1330, 1440, 70, 70), // Britain Cemetery
            new Rectangle2D(2410, 1085, 45, 45), // Cove Cemetery
            new Rectangle2D(1260, 3700, 45, 45), // Jhelom Cemetery
            new Rectangle2D(4515, 1300, 45, 45), // Moonglow Cemetery
            new Rectangle2D(2715, 830, 75, 75),  // Vesper Cemetery
            new Rectangle2D(700, 1090, 45, 45),  // Yew Cemetery
            new Rectangle2D(5200, 3640, 50, 40), // Lost Lands City of the Dead (T2A)
            new Rectangle2D(5810, 1450, 45, 40)  // Fire Dungeon Entrance Cemetery
        };

        // Standard Spawning Locations for Cemetery Event Hubs
        public struct CemeteryHubLocation
        {
            public string Name;
            public Point3D Location;
            public Rectangle2D Bounds;
            public CemeteryHubLocation(string name, Point3D loc, Rectangle2D bounds) { Name = name; Location = loc; Bounds = bounds; }
        }

        public static readonly CemeteryHubLocation[] HubLocations = new CemeteryHubLocation[]
        {
            new CemeteryHubLocation("Britain Cemetery", new Point3D(1385, 1488, 10), new Rectangle2D(1330, 1440, 70, 70)),
            new CemeteryHubLocation("Cove Cemetery", new Point3D(2439, 1098, 0), new Rectangle2D(2410, 1085, 45, 45)),
            new CemeteryHubLocation("Jhelom Cemetery", new Point3D(1286, 3723, 0), new Rectangle2D(1260, 3700, 45, 45)),
            new CemeteryHubLocation("Moonglow Cemetery", new Point3D(4543, 1317, 0), new Rectangle2D(4515, 1300, 45, 45)),
            new CemeteryHubLocation("Vesper Cemetery", new Point3D(2767, 856, 0), new Rectangle2D(2715, 830, 75, 75)),
            new CemeteryHubLocation("Yew Cemetery", new Point3D(725, 1128, 0), new Rectangle2D(700, 1090, 45, 45))
        };

        public static void Initialize()
        {
            EventSink.CreatureDeath += OnCreatureDeath;
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;

            CommandSystem.Register("HalloweenEvent", AccessLevel.Player, OnHalloweenEventCommand);
            CommandSystem.Register("HalloweenSetup", AccessLevel.Player, OnHalloweenSetupCommand);
            CommandSystem.Register("HalloweenCleanup", AccessLevel.Player, OnHalloweenCleanupCommand);
            CommandSystem.Register("HalloweenCemetery", AccessLevel.Player, OnHalloweenCemeteryCommand);
            CommandSystem.Register("HalloweenRewards", AccessLevel.Player, OnHalloweenRewardsCommand);
            CommandSystem.Register("HalloweenReward", AccessLevel.Player, OnHalloweenRewardsCommand);
            CommandSystem.Register("SplitPumpkin", AccessLevel.Player, OnSplitPumpkinCommand);
            CommandSystem.Register("SplitPumpkins", AccessLevel.Player, OnSplitPumpkinCommand);
        }

        [Usage("HalloweenEvent")]
        [Description("Opens the Halloween Cemetery Event admin menu.")]
        public static void OnHalloweenEventCommand(CommandEventArgs e)
        {
            if (e.Mobile.AccessLevel < AccessLevel.Counselor)
            {
                e.Mobile.SendMessage(0x22, "You must be a staff member (Counselor or higher) to use this command. Your current rank is {0}.", e.Mobile.AccessLevel);
                return;
            }

            e.Mobile.CloseGump(typeof(HalloweenCemeteryAdminGump));
            e.Mobile.SendGump(new HalloweenCemeteryAdminGump());
        }

        [Usage("HalloweenSetup")]
        [Description("Spawns cemetery event stones, carvers, and monsters across cemeteries.")]
        public static void OnHalloweenSetupCommand(CommandEventArgs e)
        {
            if (e.Mobile.AccessLevel < AccessLevel.Counselor)
            {
                e.Mobile.SendMessage(0x22, "You must be a staff member (Counselor or higher) to use this command. Your current rank is {0}.", e.Mobile.AccessLevel);
                return;
            }

            SetupEventHubs(e.Mobile);
        }

        [Usage("HalloweenCleanup")]
        [Description("Removes cemetery event stones, carvers, and event mobs.")]
        public static void OnHalloweenCleanupCommand(CommandEventArgs e)
        {
            if (e.Mobile.AccessLevel < AccessLevel.Counselor)
            {
                e.Mobile.SendMessage(0x22, "You must be a staff member (Counselor or higher) to use this command. Your current rank is {0}.", e.Mobile.AccessLevel);
                return;
            }

            CleanupEventHubs(e.Mobile);
        }

        [Usage("HalloweenRewards")]
        [Description("Opens the Halloween Cemetery Event Rewards menu.")]
        public static void OnHalloweenRewardsCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile)
            {
                e.Mobile.CloseGump(typeof(HalloweenRewardGump));
                e.Mobile.SendGump(new HalloweenRewardGump(e.Mobile));
            }
        }

        [Usage("SplitPumpkin")]
        [Description("Targets a stack of Halloween Pumpkins to open the split stack gump.")]
        public static void OnSplitPumpkinCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || !from.Alive)
                return;

            from.SendMessage(0x35, "Target the stack of Halloween Pumpkins you wish to split:");
            from.Target = new SplitPumpkinTarget();
        }

        private class SplitPumpkinTarget : Target
        {
            public SplitPumpkinTarget() : base(3, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || !from.Alive)
                    return;

                if (targeted is HalloweenEventPumpkin pumpkin)
                {
                    if (pumpkin.Deleted)
                        return;

                    if (!pumpkin.IsAccessibleTo(from) || !from.InRange(pumpkin.GetWorldLocation(), 3) || !from.CanSee(pumpkin))
                    {
                        from.SendLocalizedMessage(500446); // That is too far away.
                        return;
                    }

                    if (pumpkin.Amount <= 1)
                    {
                        from.SendMessage(0x22, "That stack only has 1 pumpkin and cannot be split.");
                        return;
                    }

                    pumpkin.BeginSplit(from);
                }
                else
                {
                    from.SendMessage(0x22, "That is not a stack of Halloween Pumpkins.");
                }
            }
        }

        [Usage("HalloweenCemetery [setup|remove]")]
        [Description("Opens the Halloween Cemetery Event admin menu or sets up/removes event spawners.")]
        public static void OnHalloweenCemeteryCommand(CommandEventArgs e)
        {
            if (e.Length > 0)
            {
                string arg = e.GetString(0).ToLower();
                if (arg == "setup")
                {
                    OnHalloweenSetupCommand(e);
                    return;
                }
                else if (arg == "remove" || arg == "cleanup")
                {
                    OnHalloweenCleanupCommand(e);
                    return;
                }
            }

            OnHalloweenEventCommand(e);
        }

        public static bool IsInCemetery(Map map, Point3D loc)
        {
            if (map == null || (map != Map.Trammel && map != Map.Felucca))
                return false;

            for (int i = 0; i < Cemeteries.Length; i++)
            {
                if (Cemeteries[i].Contains(loc))
                    return true;
            }

            return false;
        }

        public static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            if (!Enabled)
                return;

            BaseCreature bc = e.Creature as BaseCreature;
            if (bc == null)
                return;

            // Check if killed in cemetery or is a cemetery event creature
            bool inCemetery = IsInCemetery(bc.Map, bc.Location);
            bool isEventCreature = bc is IHalloweenCemeteryCreature;

            if (!inCemetery && !isEventCreature)
                return;

            // Notify cemetery wave spawner if present
            IPooledEnumerable eableItems = bc.Map.GetItemsInRange(bc.Location, 50);
            foreach (Item item in eableItems)
            {
                if (item is HalloweenCemeterySpawner)
                {
                    HalloweenCemeterySpawner spawner = (HalloweenCemeterySpawner)item;
                    if (spawner.Bounds.Contains(bc.Location) || bc.InRange(spawner.Location, 40))
                    {
                        spawner.OnMobKilled(bc, e.Killer);
                        break;
                    }
                }
            }
            eableItems.Free();

            // Determine player killer
            Mobile killer = e.Killer;
            if (killer is BaseCreature)
            {
                BaseCreature pet = (BaseCreature)killer;
                if (pet.Controlled && pet.ControlMaster != null)
                    killer = pet.ControlMaster;
                else if (pet.Summoned && pet.SummonMaster != null)
                    killer = pet.SummonMaster;
            }

            PlayerMobile pm = killer as PlayerMobile;
            if (pm == null || pm.Backpack == null)
                return;

            // 1. Calculate Pumpkin Tokens
            int fame = Math.Max(100, bc.Fame);
            int basePumpkins = Utility.RandomMinMax(MinPumpkins, MaxPumpkins);

            if (isEventCreature || fame >= 8000)
                basePumpkins = Utility.RandomMinMax(basePumpkins * 2, basePumpkins * 3 + 4);

            pm.AddToBackpack(new HalloweenEventPumpkin(basePumpkins));
            pm.PlaySound(0x2E6);
            pm.SendMessage(0x35, "You harvested {0} Halloween Pumpkin{1} from {2}!", 
                basePumpkins, basePumpkins > 1 ? "s" : "", bc.Name ?? "the creature");

            // Calculate Luck Factor
            double luckBonus = 0.0;
            if (LuckAffectsDrops)
            {
                int luck = Math.Max(0, pm.RealLuck);
                luckBonus = Math.Min(0.05, (double)luck / 25000.0);
            }

            // 2. Roll for Special Hair / Beard Dye
            double dyeRollChance = DyeDropChance + luckBonus;
            if (isEventCreature)
                dyeRollChance *= 1.5;

            if (Utility.RandomDouble() < dyeRollChance)
            {
                Item dye = CreateRandomHalloweenDye();
                if (dye != null)
                {
                    pm.AddToBackpack(dye);
                    pm.PlaySound(0x5B4);
                    pm.FixedParticles(0x376A, 9, 32, 5030, dye.Hue, 0, EffectLayer.Waist);
                    pm.SendMessage(0x35, "Spooky luck! A rare {0} has appeared in your backpack!", dye.Name);
                }
            }

            // 3. Roll for Ethereal Orange Spider
            double spiderRollChance = SpiderMountChance + (luckBonus * 0.25);
            if (isEventCreature)
                spiderRollChance *= 2.0;

            if (Utility.RandomDouble() < spiderRollChance)
            {
                EtherealOrangeSpider spider = new EtherealOrangeSpider();
                pm.AddToBackpack(spider);
                pm.PlaySound(0x5B4);
                pm.FixedParticles(0x375A, 10, 15, 5017, 1161, 0, EffectLayer.Waist);
                pm.SendMessage(0x35, "THE CRYPT SHAKES! A rare Ethereal Orange Spider has crawled into your backpack!");
            }
        }

        public static Item CreateRandomHalloweenDye()
        {
            switch (Utility.Random(10))
            {
                case 0: return new CandyCornOrangeHairDye();
                case 1: return new CandyCornOrangeBeardDye();
                case 2: return new SpiderSilkWhiteHairDye();
                case 3: return new SpiderSilkWhiteBeardDye();
                case 4: return new VibrantCrimsonHairDye();
                case 5: return new VibrantCrimsonBeardDye();
                case 6: return new SpectralVenomHairDye();
                case 7: return new SpectralVenomBeardDye();
                case 8: return new SpectralAmethystHairDye();
                default: return new SpectralAmethystBeardDye();
            }
        }

        public static void SetupEventHubs(Mobile from)
        {
            int placedCount = 0;
            Map[] maps = new Map[] { Map.Trammel, Map.Felucca };

            // First clean up any existing event hubs
            CleanupEventHubs(null);

            foreach (Map map in maps)
            {
                if (map == null)
                    continue;

                foreach (CemeteryHubLocation hub in HubLocations)
                {
                    Point3D stoneLoc = hub.Location;
                    Point3D npcLoc = new Point3D(hub.Location.X + 1, hub.Location.Y, hub.Location.Z);
                    Point3D altarLoc = new Point3D(hub.Location.X - 1, hub.Location.Y, hub.Location.Z);

                    HalloweenRewardStone stone = new HalloweenRewardStone();
                    stone.MoveToWorld(stoneLoc, map);

                    JackThePumpkinCarver jack = new JackThePumpkinCarver();
                    jack.Home = npcLoc;
                    jack.RangeHome = 0;
                    jack.MoveToWorld(npcLoc, map);

                    HalloweenCemeterySpawner spawner = new HalloweenCemeterySpawner(hub.Name, hub.Bounds);
                    spawner.MoveToWorld(altarLoc, map);

                    placedCount += 3;
                }
            }

            if (from != null)
                from.SendMessage(0x35, "Successfully deployed Halloween Cemetery Event wave spawners, altars, vendors, and reward stones across Trammel and Felucca ({0} objects deployed).", placedCount);
        }

        public static void CleanupEventHubs(Mobile from)
        {
            int deletedCount = 0;
            List<Item> itemsToDelete = new List<Item>();
            List<Mobile> mobilesToDelete = new List<Mobile>();

            foreach (Item item in World.Items.Values)
            {
                if (item is HalloweenRewardStone || item is HalloweenCemeterySpawner)
                    itemsToDelete.Add(item);
            }

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob is JackThePumpkinCarver || mob is IHalloweenCemeteryCreature || 
                    mob is CemeteryVampireLord || mob is ThePumpkinKing)
                {
                    mobilesToDelete.Add(mob);
                }
            }

            foreach (Item item in itemsToDelete)
            {
                item.Delete();
                deletedCount++;
            }

            foreach (Mobile mob in mobilesToDelete)
            {
                mob.Delete();
                deletedCount++;
            }

            if (from != null)
                from.SendMessage(0x22, "Removed {0} Halloween Cemetery event objects, wave spawners, and event creatures from the world.", deletedCount);
        }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                SavePath,
                writer =>
                {
                    writer.Write((int)0); // version
                    writer.Write(Enabled);
                    writer.Write(DyeDropChance);
                    writer.Write(SpiderMountChance);
                    writer.Write(LuckAffectsDrops);
                    writer.Write(MinPumpkins);
                    writer.Write(MaxPumpkins);
                });
        }

        public static void OnLoad()
        {
            Persistence.Deserialize(
                SavePath,
                reader =>
                {
                    int version = reader.ReadInt();
                    Enabled = reader.ReadBool();
                    DyeDropChance = reader.ReadDouble();
                    SpiderMountChance = reader.ReadDouble();
                    LuckAffectsDrops = reader.ReadBool();
                    MinPumpkins = reader.ReadInt();
                    MaxPumpkins = reader.ReadInt();
                });
        }
    }

    public class HalloweenCemeteryAdminGump : Gump
    {
        public HalloweenCemeteryAdminGump() : base(100, 100)
        {
            AddPage(0);
            AddBackground(0, 0, 440, 360, 9270);
            AddBackground(15, 15, 410, 45, 9390);

            AddHtml(25, 25, 390, 25, "<BASEFONT COLOR=#FF7700 size=6><CENTER><b>Halloween Cemetery Event Control</b></CENTER></BASEFONT>", false, false);

            int y = 75;

            // Enabled Toggle
            AddHtml(30, y, 220, 20, "<BASEFONT COLOR=#FFFFFF>Event Status:</BASEFONT>", false, false);
            AddHtml(250, y, 100, 20, HalloweenCemeteryEvent.Enabled ? "<BASEFONT COLOR=#00FF00><b>ACTIVE</b></BASEFONT>" : "<BASEFONT COLOR=#FF3333><b>INACTIVE</b></BASEFONT>", false, false);
            AddButton(360, y - 2, HalloweenCemeteryEvent.Enabled ? 0x992 : 0x995, 0x992, 1, GumpButtonType.Reply, 0);
            y += 35;

            // Dye Drop Chance
            AddHtml(30, y, 240, 20, String.Format("<BASEFONT COLOR=#FFFFFF>Special Dye Drop Chance: <b>{0:P1}</b></BASEFONT>", HalloweenCemeteryEvent.DyeDropChance), false, false);
            AddButton(330, y - 2, 0x15E3, 0x15E7, 2, GumpButtonType.Reply, 0); // -1%
            AddButton(370, y - 2, 0x15E1, 0x15E5, 3, GumpButtonType.Reply, 0); // +1%
            y += 35;

            // Spider Mount Drop Chance
            AddHtml(30, y, 240, 20, String.Format("<BASEFONT COLOR=#FFFFFF>Spider Mount Chance: <b>{0:P2}</b></BASEFONT>", HalloweenCemeteryEvent.SpiderMountChance), false, false);
            AddButton(330, y - 2, 0x15E3, 0x15E7, 4, GumpButtonType.Reply, 0); // -0.1%
            AddButton(370, y - 2, 0x15E1, 0x15E5, 5, GumpButtonType.Reply, 0); // +0.1%
            y += 35;

            // Pumpkin Range
            AddHtml(30, y, 260, 20, String.Format("<BASEFONT COLOR=#FFFFFF>Pumpkin Range: <b>{0} - {1} per kill</b></BASEFONT>", HalloweenCemeteryEvent.MinPumpkins, HalloweenCemeteryEvent.MaxPumpkins), false, false);
            AddButton(330, y - 2, 0x15E3, 0x15E7, 6, GumpButtonType.Reply, 0); // -1 max
            AddButton(370, y - 2, 0x15E1, 0x15E5, 7, GumpButtonType.Reply, 0); // +1 max
            y += 40;

            // Setup / Remove Buttons
            AddButton(30, y, 0xFA5, 0xFA7, 10, GumpButtonType.Reply, 0);
            AddHtml(65, y + 2, 140, 20, "<BASEFONT COLOR=#00FF00><b>Deploy Hubs</b></BASEFONT>", false, false);

            AddButton(230, y, 0xFA5, 0xFA7, 11, GumpButtonType.Reply, 0);
            AddHtml(265, y + 2, 140, 20, "<BASEFONT COLOR=#FF3333><b>Remove Hubs</b></BASEFONT>", false, false);
            y += 40;

            // View Rewards Gump
            AddButton(30, y, 0xFA5, 0xFA7, 12, GumpButtonType.Reply, 0);
            AddHtml(65, y + 2, 250, 20, "<BASEFONT COLOR=#FFA500><b>Preview Rewards Menu</b></BASEFONT>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (from == null || from.AccessLevel < AccessLevel.Counselor)
                return;

            switch (info.ButtonID)
            {
                case 0: return; // Close
                case 1: // Toggle Enabled
                    HalloweenCemeteryEvent.Enabled = !HalloweenCemeteryEvent.Enabled;
                    from.SendMessage(0x35, "Halloween Cemetery Event is now {0}.", HalloweenCemeteryEvent.Enabled ? "ENABLED" : "DISABLED");
                    break;
                case 2: // -1% Dye
                    HalloweenCemeteryEvent.DyeDropChance = Math.Max(0.01, HalloweenCemeteryEvent.DyeDropChance - 0.01);
                    break;
                case 3: // +1% Dye
                    HalloweenCemeteryEvent.DyeDropChance = Math.Min(0.50, HalloweenCemeteryEvent.DyeDropChance + 0.01);
                    break;
                case 4: // -0.1% Spider
                    HalloweenCemeteryEvent.SpiderMountChance = Math.Max(0.001, HalloweenCemeteryEvent.SpiderMountChance - 0.001);
                    break;
                case 5: // +0.1% Spider
                    HalloweenCemeteryEvent.SpiderMountChance = Math.Min(0.10, HalloweenCemeteryEvent.SpiderMountChance + 0.001);
                    break;
                case 6: // -1 Max Pumpkin
                    if (HalloweenCemeteryEvent.MaxPumpkins > HalloweenCemeteryEvent.MinPumpkins)
                        HalloweenCemeteryEvent.MaxPumpkins--;
                    break;
                case 7: // +1 Max Pumpkin
                    HalloweenCemeteryEvent.MaxPumpkins++;
                    break;
                case 10: // Deploy Hubs
                    HalloweenCemeteryEvent.SetupEventHubs(from);
                    break;
                case 11: // Remove Hubs
                    HalloweenCemeteryEvent.CleanupEventHubs(from);
                    break;
                case 12: // Preview Rewards Menu
                    from.CloseGump(typeof(HalloweenRewardGump));
                    from.SendGump(new HalloweenRewardGump(from));
                    return;
            }

            from.CloseGump(typeof(HalloweenCemeteryAdminGump));
            from.SendGump(new HalloweenCemeteryAdminGump());
        }
    }
}
