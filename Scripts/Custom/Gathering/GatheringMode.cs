using System;
using System.Collections.Generic;
using System.Linq;

using Server.Commands;
using Server.Engines.Harvest;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Engines.Gathering
{
    public enum GatheringMode
    {
        None = 0,
        Mining,
        Lumberjacking,
        Fishing
    }

    /// <summary>
    /// Auto-gather quality-of-life feature. While a player has gathering mode
    /// active, a passive timer keeps harvesting nearby valid tiles -- no
    /// movement required. When the area runs dry the player gets a one-shot
    /// "move on" notification. Uses the existing Mining / Lumberjacking
    /// pipeline so skill checks, resource depletion, swing delays, and tool
    /// durability all behave exactly as a manual harvest. The classic
    /// double-click-and-target flow is untouched.
    /// </summary>
    public static class GatheringModeSystem
    {
        // Scan radius is taken from each HarvestDefinition's MaxRange (2 for
        // both Mining and Lumberjacking), matching the harvest pipeline's own
        // CheckRange so we never target a tile we can't actually reach.

        // Timer cadence. We poll every second and gate by per-player cooldown
        // so the actual swing pace matches CycleCooldown rather than the tick.
        private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1.0);

        // Minimum spacing between successful harvest starts. The harvest pipeline
        // itself runs ~2.5s (effect 1.6s + sound 0.9s); this cooldown is the
        // pause AFTER that, so total visible cycle = ~cooldown. Mining is tuned
        // tighter than lumberjacking because rock veins deplete much faster
        // and the perceived swing tempo on OSI is brisker. Fishing pacing
        // matches the OSI bobber animation ~8s.
        private static readonly TimeSpan MiningCycleCooldown = TimeSpan.FromSeconds(1.5);
        private static readonly TimeSpan LumberjackingCycleCooldown = TimeSpan.FromSeconds(5.0);
        private static readonly TimeSpan FishingCycleCooldown = TimeSpan.FromSeconds(8.0);

        // Tinkering skill threshold + ingot cost for auto-create tools.
        // Matches DefTinkering.cs: Hatchet=30, Shovel/Pickaxe=40, all 4 iron ingots.
        private const double HatchetSkill = 30.0;
        private const double MiningToolSkill = 40.0;
        private const int ToolIngotCost = 4;

        // Fishing pole: Carpentry recipe (with Tailoring secondary) per OSI:
        // 68.4 Carpentry / 40.0 Tailoring, 5 boards or logs + 5 cloth.
        private const double FishingPoleCarpentrySkill = 68.4;
        private const double FishingPoleTailoringSkill = 40.0;
        private const int FishingPoleBoardCost = 5;
        private const int FishingPoleClothCost = 5;

        // How long to wait between "nothing here, move on" notifications when
        // the player is sitting in an empty area.
        private static readonly TimeSpan EmptyNotifyCooldown = TimeSpan.FromSeconds(10);

        private static readonly Dictionary<PlayerMobile, GatheringMode> _Modes =
            new Dictionary<PlayerMobile, GatheringMode>();

        private static readonly Dictionary<PlayerMobile, DateTime> _NextHarvestAllowed =
            new Dictionary<PlayerMobile, DateTime>();

        private static readonly Dictionary<PlayerMobile, DateTime> _NextEmptyNotify =
            new Dictionary<PlayerMobile, DateTime>();

        // Players who've toggled "auto-cut logs to boards" on. Lumberjacking-only.
        private static readonly HashSet<PlayerMobile> _AutoCutLogs =
            new HashSet<PlayerMobile>();

        // Players who've toggled "auto-create tools" on (uses Tinkering + ingots).
        private static readonly HashSet<PlayerMobile> _AutoCreateTools =
            new HashSet<PlayerMobile>();

        // Players who've toggled "auto-smelt ores" on. Mining-only.
        private static readonly HashSet<PlayerMobile> _AutoSmelt =
            new HashSet<PlayerMobile>();

        // Players who've toggled "auto-cut fish into steaks" on. Fishing-only.
        // Skips BigFish and BaseMagicFish subclasses (quest / display fish).
        private static readonly HashSet<PlayerMobile> _AutoCutFish =
            new HashSet<PlayerMobile>();

        public static bool IsAutoCutFish(PlayerMobile pm)
        {
            return _AutoCutFish.Contains(pm);
        }

        public static void SetAutoCutFish(PlayerMobile pm, bool on)
        {
            if (on)
            {
                if (_AutoCutFish.Add(pm))
                    pm.SendMessage(0x40, "Fish will be auto-cut into fish steaks (quest fish are skipped).");
            }
            else
            {
                if (_AutoCutFish.Remove(pm))
                    pm.SendMessage(0x40, "Auto-cut fish disabled.");
            }
        }

        public static bool IsAutoSmelt(PlayerMobile pm)
        {
            return _AutoSmelt.Contains(pm);
        }

        public static void SetAutoSmelt(PlayerMobile pm, bool on)
        {
            if (on)
            {
                if (_AutoSmelt.Add(pm))
                    pm.SendMessage(0x40, "Ore will be smelted automatically when near a forge or fire beetle.");
            }
            else
            {
                if (_AutoSmelt.Remove(pm))
                    pm.SendMessage(0x40, "Auto-smelt disabled.");
            }
        }

        public static bool IsAutoCutLogs(PlayerMobile pm)
        {
            return _AutoCutLogs.Contains(pm);
        }

        public static void SetAutoCutLogs(PlayerMobile pm, bool on)
        {
            if (on)
            {
                if (_AutoCutLogs.Add(pm))
                    pm.SendMessage(0x40, "Logs will now be cut into boards automatically.");
            }
            else
            {
                if (_AutoCutLogs.Remove(pm))
                    pm.SendMessage(0x40, "Auto-cut logs disabled.");
            }
        }

        public static bool IsAutoCreateTools(PlayerMobile pm)
        {
            return _AutoCreateTools.Contains(pm);
        }

        public static void SetAutoCreateTools(PlayerMobile pm, bool on)
        {
            if (on)
            {
                if (_AutoCreateTools.Add(pm))
                    pm.SendMessage(0x40, "Tools will be auto-created from iron ingots when you run out.");
            }
            else
            {
                if (_AutoCreateTools.Remove(pm))
                    pm.SendMessage(0x40, "Auto-create tools disabled.");
            }
        }

        private static Timer _Timer;

        public static void Initialize()
        {
            CommandSystem.Register("gather", AccessLevel.Player, OnGatherCommand);
            CommandSystem.Register("fish", AccessLevel.Player, OnGatherCommand);
            CommandSystem.Register("fishing", AccessLevel.Player, OnGatherCommand);
            CommandSystem.Register("gatherinfo", AccessLevel.Player, OnGatherInfoCommand);
        }

        // Diagnostic command -- walks through the same checks TryGather uses
        // for the calling player and reports exactly what's happening. Run
        // this when "auto-fish does nothing" to see which gate is closing.
        private static void OnGatherInfoCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile pm))
                return;

            GatheringMode mode = GetMode(pm);
            pm.SendMessage(0x40, "Mode: {0}", mode);
            pm.SendMessage(0x40, "Map: {0} at ({1},{2},{3})", pm.Map, pm.X, pm.Y, pm.Z);
            pm.SendMessage(0x40, "Toggles -> autoCutLogs:{0} autoCreate:{1} autoSmelt:{2} autoCutFish:{3}",
                IsAutoCutLogs(pm), IsAutoCreateTools(pm), IsAutoSmelt(pm), IsAutoCutFish(pm));

            // Pack inventory -- count fish so we can verify cut-fish has work.
            Container pack0 = pm.Backpack;
            if (pack0 != null)
            {
                int fishCount = 0;
                int bigFishCount = 0;
                int magicFishCount = 0;
                int hsFishCount = 0;
                int rareFishCount = 0;
                foreach (Item it in pack0.Items)
                {
                    if (it == null || it.Deleted) continue;
                    if (it.GetType() == typeof(Fish))
                        fishCount++;
                    else if (it is BigFish)
                        bigFishCount++;
                    else if (it is BaseMagicFish)
                        magicFishCount++;
                    else if (it is RareFish)
                        rareFishCount++;
                    else if (it is BaseHighseasFish)
                        hsFishCount++;
                }
                pm.SendMessage(0x40, "Pack fish: plain={0} big={1} magic={2} highseas={3} rare={4}",
                    fishCount, bigFishCount, magicFishCount, hsFishCount, rareFishCount);
            }

            if (mode == GatheringMode.None)
            {
                pm.SendMessage(0x22, "No gathering mode active. Open the gump and pick one.");
                return;
            }

            pm.SendMessage(0x40, "Alive: {0} | Mounted: {1} | Flying: {2} | Hidden: {3}",
                pm.Alive, pm.Mounted, pm.Flying, pm.Hidden);

            if (SpellHelper.CheckCombat(pm))
                pm.SendMessage(0x22, "* In combat (heat-of-battle). Auto-gather is paused.");

            if (WeightOverloading.IsOverloaded(pm))
                pm.SendMessage(0x22, "* Overweight. Auto-gather is paused.");

            Item tool = FindTool(pm, mode);
            if (tool == null)
                pm.SendMessage(0x22, "* No matching tool found in pack/hand.");
            else
                pm.SendMessage(0x40, "Tool: {0} ({1}) layer={2}", tool.GetType().Name, tool.Serial, tool.Layer);

            HarvestDefinition def;
            switch (mode)
            {
                case GatheringMode.Mining: def = Mining.System.OreAndStone; break;
                case GatheringMode.Fishing: def = Fishing.System.Definition; break;
                default: def = Lumberjacking.System.Definition; break;
            }

            pm.SendMessage(0x40, "Range: {0}, Tiles[]: {1}, RangedTiles: {2}, SpecialTiles: {3}",
                def.MaxRange, def.Tiles?.Length, def.RangedTiles, def.SpecialTiles?.Length);

            object target = ScanForTarget(pm, mode);
            if (target == null)
            {
                pm.SendMessage(0x22, "* Scan found no valid target within {0} tiles.", def.MaxRange);
                ReportNearbyTiles(pm, def);
            }
            else
            {
                if (target is LandTarget lt)
                    pm.SendMessage(0x40, "Target: LandTarget id=0x{0:X4} at ({1},{2},{3})",
                        lt.TileID, lt.Location.X, lt.Location.Y, lt.Location.Z);
                else if (target is StaticTarget st)
                    pm.SendMessage(0x40, "Target: StaticTarget id=0x{0:X4} at ({1},{2},{3})",
                        st.ItemID, st.Location.X, st.Location.Y, st.Location.Z);
            }
        }

        // Dump the actual tile IDs around the player so we can see whether
        // m_WaterTiles / mining-tile arrays cover what the map has.
        private static void ReportNearbyTiles(PlayerMobile pm, HarvestDefinition def)
        {
            Map map = pm.Map;
            if (map == null) return;

            int range = Math.Min(def.MaxRange, 4);
            var seen = new HashSet<int>();

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    int x = pm.X + dx;
                    int y = pm.Y + dy;

                    var land = map.Tiles.GetLandTile(x, y);
                    int landID = land.ID;
                    if (seen.Add(landID))
                    {
                        pm.SendMessage(0x40, "  land 0x{0:X4} at ({1},{2}) validates? {3}",
                            landID, x, y, def.Validate(landID));
                    }

                    var statics = map.Tiles.GetStaticTiles(x, y, true);
                    foreach (var s in statics)
                    {
                        int adjusted = (s.ID & 0x3FFF) | 0x4000;
                        if (seen.Add(s.ID))
                        {
                            pm.SendMessage(0x40, "  static itemID=0x{0:X4} (adj 0x{1:X4}) at ({2},{3}) validates? {4}",
                                s.ID, adjusted, x, y, def.Validate(adjusted));
                        }
                    }
                }
            }
        }

        private static void OnGatherCommand(CommandEventArgs e)
        {
            if (e.Mobile is PlayerMobile pm)
                OpenMenu(pm);
        }

        public static void OpenMenu(PlayerMobile pm)
        {
            pm.CloseGump(typeof(GatheringModeGump));
            pm.SendGump(new GatheringModeGump(pm));
        }

        public static void AddContextMenuEntry(PlayerMobile pm, Mobile from, System.Collections.Generic.List<Server.ContextMenus.ContextMenuEntry> list)
        {
            if (from == pm && pm.Alive)
                list.Add(new GatheringModeMenuEntry(pm));
        }

        private class GatheringModeMenuEntry : Server.ContextMenus.ContextMenuEntry
        {
            private readonly PlayerMobile m_Owner;

            // Cliloc 1044105 displays "Mining" in standard UO clients --
            // not a perfect label since this menu picks Mining/Lumberjacking
            // both, but it's guaranteed to be present and readable in any
            // modern client's cliloc data. Swap if a "Gathering Mode" cliloc
            // becomes available.
            public GatheringModeMenuEntry(PlayerMobile owner) : base(1044105)
            {
                m_Owner = owner;
            }

            public override void OnClick()
            {
                OpenMenu(m_Owner);
            }
        }

        public static GatheringMode GetMode(PlayerMobile pm)
        {
            return _Modes.TryGetValue(pm, out GatheringMode mode) ? mode : GatheringMode.None;
        }

        public static void SetMode(PlayerMobile pm, GatheringMode mode)
        {
            if (mode == GatheringMode.None)
            {
                if (_Modes.Remove(pm))
                {
                    _NextEmptyNotify.Remove(pm);
                    _NextHarvestAllowed.Remove(pm);
                    pm.SendMessage(0x40, "Gathering mode disabled.");
                }

                StopTimerIfIdle();
                return;
            }

            _Modes[pm] = mode;
            _NextEmptyNotify.Remove(pm);
            _NextHarvestAllowed.Remove(pm);

            string label;
            string hint;
            switch (mode)
            {
                case GatheringMode.Fishing:
                    label = "fishing";
                    hint = "Stand near water (or on a boat) and the system will fish for you.";
                    break;
                case GatheringMode.Lumberjacking:
                    label = "lumberjacking";
                    hint = "Stand near a tree and the system will gather for you.";
                    break;
                default:
                    label = "mining";
                    hint = "Stand near a vein and the system will gather for you.";
                    break;
            }
            pm.SendMessage(0x40, "You begin {0}. {1}", label, hint);

            EnsureTimerRunning();
        }

        private static void EnsureTimerRunning()
        {
            if (_Timer == null)
            {
                _Timer = Timer.DelayCall(TickInterval, TickInterval, Tick);
                _Timer.Start();
            }
        }

        private static void StopTimerIfIdle()
        {
            if (_Modes.Count == 0 && _Timer != null)
            {
                _Timer.Stop();
                _Timer = null;
            }
        }

        private static void Tick()
        {
            if (_Modes.Count == 0)
            {
                StopTimerIfIdle();
                return;
            }

            // Snapshot so we can mutate _Modes inside (e.g. auto-disable on
            // tool exhaustion) without invalidating the iterator.
            var snapshot = _Modes.ToArray();

            foreach (var kvp in snapshot)
            {
                var pm = kvp.Key;
                var mode = kvp.Value;

                if (pm.Deleted || pm.NetState == null)
                {
                    _Modes.Remove(pm);
                    _NextEmptyNotify.Remove(pm);
                    _NextHarvestAllowed.Remove(pm);
                    _AutoCutLogs.Remove(pm);
                    _AutoCreateTools.Remove(pm);
                    _AutoSmelt.Remove(pm);
                    _AutoCutFish.Remove(pm);
                    continue;
                }

                TryGather(pm, mode);

                if (mode == GatheringMode.Lumberjacking && IsAutoCutLogs(pm))
                    AutoCutLogs(pm);

                if (mode == GatheringMode.Mining && IsAutoSmelt(pm))
                    AutoSmeltOres(pm);

                if (mode == GatheringMode.Fishing && IsAutoCutFish(pm))
                    AutoCutFish(pm);
            }

            StopTimerIfIdle();
        }

        private static void TryGather(PlayerMobile pm, GatheringMode mode)
        {
            // Hidden players are not allowed to auto-gather. Stealth + passive
            // harvest is too strong for PvP / VvV. The classic manual click
            // path still reveals on swing as normal -- this only gates the
            // automatic loop.
            if (!pm.Alive || pm.Mounted || pm.Flying || pm.Hidden || pm.Map == null || pm.Map == Map.Internal)
                return;

            if (SpellHelper.CheckCombat(pm))
                return; // skip silently during combat — resumes once combat heat fades

            // Per-player cycle cooldown. Prevents firing back-to-back swings the
            // moment the harvest pipeline's BeginAction lock clears.
            DateTime now = DateTime.UtcNow;
            if (_NextHarvestAllowed.TryGetValue(pm, out DateTime allowed) && now < allowed)
                return;

            if (WeightOverloading.IsOverloaded(pm))
            {
                NotifyOnce(pm, "You are too encumbered to gather. Drop weight to continue.");
                return;
            }

            Item tool = FindTool(pm, mode);
            if (tool == null)
            {
                // Tools exhausted -- try auto-create if the player opted in.
                if (IsAutoCreateTools(pm))
                {
                    tool = TryCreateTool(pm, mode);
                }

                if (tool == null)
                {
                    string toolDesc;
                    switch (mode)
                    {
                        case GatheringMode.Mining: toolDesc = "pickaxe or shovel"; break;
                        case GatheringMode.Fishing: toolDesc = "fishing pole"; break;
                        default: toolDesc = "axe"; break;
                    }
                    pm.SendMessage(0x22, "You have no {0} in your pack. Gathering mode disabled.", toolDesc);
                    SetMode(pm, GatheringMode.None);
                    return;
                }
            }

            object target = ScanForTarget(pm, mode);
            if (target == null)
            {
                if (mode == GatheringMode.Fishing)
                    NotifyOnce(pm, "You need to be closer to the water to fish!");
                else
                    NotifyOnce(pm, "Nothing left here. Move to the next spot.");
                return;
            }

            switch (mode)
            {
                case GatheringMode.Mining:
                    Mining.System.StartHarvesting(pm, tool, target);
                    break;
                case GatheringMode.Lumberjacking:
                    Lumberjacking.System.StartHarvesting(pm, tool, target);
                    break;
                case GatheringMode.Fishing:
                    Fishing.System.StartHarvesting(pm, tool, target);
                    break;
            }

            // Stamp the cooldown regardless of whether StartHarvesting actually
            // succeeded -- if it bailed (concurrent lock, range, etc.), waiting
            // a cycle before retrying is still the right behaviour.
            TimeSpan cooldown;
            switch (mode)
            {
                case GatheringMode.Mining: cooldown = MiningCycleCooldown; break;
                case GatheringMode.Fishing: cooldown = FishingCycleCooldown; break;
                default: cooldown = LumberjackingCycleCooldown; break;
            }

            _NextHarvestAllowed[pm] = now + cooldown;
        }

        // Walk the player's backpack and carve every fish into raw fish
        // steaks. Players who want to keep certain fish (e.g. for quests)
        // simply toggle this off in the gump. Requires a hatchet or knife
        // in pack (same as the manual UO mechanic: double-click cutting
        // tool, target fish).
        private static void AutoCutFish(PlayerMobile pm)
        {
            Container pack = pm.Backpack;
            if (pack == null)
                return;

            // BaseKnife (Cleaver, Butcher Knife, Dagger, Skinning Knife,
            // etc.) is the canonical fish-carve tool. BaseAxe (Hatchet)
            // is also accepted.
            Item tool = pack.FindItemByType(typeof(BaseAxe))
                       ?? pack.FindItemByType(typeof(BaseKnife));

            if (tool == null)
            {
                NotifyOnce(pm, "You need a hatchet or knife in your pack to cut fish.");
                return;
            }

            // Carve regular Fish -> RawFishSteak (4 per fish).
            var fishList = pack.FindItemsByType<Fish>();
            if (fishList != null)
            {
                foreach (Fish fish in fishList)
                {
                    if (fish == null || fish.Deleted)
                        continue;

                    fish.Carve(pm, tool);
                }
            }

            // BigFish are also ICarvable. Magic fish (Prized / Wondrous /
            // TrulyRare / Peculiar) are not carvable in stock UO, so they
            // naturally stay intact regardless of this toggle.
            var bigFishList = pack.FindItemsByType<BigFish>();
            if (bigFishList != null)
            {
                foreach (BigFish bf in bigFishList)
                {
                    if (bf == null || bf.Deleted)
                        continue;

                    bf.Carve(pm, tool);
                }
            }

            // High Seas / Pub 57 fish (Walleye, Pike, Bass, Trout, Catfish,
            // Salmon, Sunfish, Perch, Bream, Shiner, deep-water + dungeon
            // species, etc.). RareFish are skipped — they record the fisher
            // and date and are kept as trophies.
            var hsFishList = pack.FindItemsByType<BaseHighseasFish>();
            if (hsFishList != null)
            {
                foreach (BaseHighseasFish hs in hsFishList)
                {
                    if (hs == null || hs.Deleted || hs is RareFish)
                        continue;

                    hs.Carve(pm, tool);
                }
            }
        }

        // Auto-craft a replacement tool from materials in the player's pack.
        // Returns the new tool on success, null on failure. Mining /
        // lumberjacking craft via Tinkering + iron ingots; fishing crafts a
        // pole via Carpentry + Tailoring + boards/logs + cloth.
        private static Item TryCreateTool(PlayerMobile pm, GatheringMode mode)
        {
            Container pack = pm.Backpack;
            if (pack == null)
                return null;

            if (mode == GatheringMode.Fishing)
                return TryCreateFishingPole(pm, pack);

            double required = mode == GatheringMode.Mining ? MiningToolSkill : HatchetSkill;
            string toolName = mode == GatheringMode.Mining ? "shovel" : "hatchet";

            if (pm.Skills.Tinkering.Value < required)
            {
                pm.SendMessage(0x22, "You need {0} Tinkering skill to create a {1}. Gathering mode disabled.",
                    (int)required, toolName);
                return null;
            }

            if (pack.GetAmount(typeof(IronIngot)) < ToolIngotCost)
            {
                pm.SendMessage(0x22, "You need {0} iron ingots in your pack to create a {1}. Gathering mode disabled.",
                    ToolIngotCost, toolName);
                return null;
            }

            if (!pack.ConsumeTotal(typeof(IronIngot), ToolIngotCost))
            {
                pm.SendMessage(0x22, "Failed to consume iron ingots for tool creation.");
                return null;
            }

            Item newTool = mode == GatheringMode.Mining
                ? (Item)new Shovel()
                : new Hatchet();

            pack.DropItem(newTool);
            pm.SendMessage(0x40, "You craft a new {0} from your iron ingots.", toolName);
            return newTool;
        }

        private static Item TryCreateFishingPole(PlayerMobile pm, Container pack)
        {
            if (pm.Skills.Carpentry.Value < FishingPoleCarpentrySkill)
            {
                pm.SendMessage(0x22, "You need {0:0.0} Carpentry skill to create a fishing pole. Gathering mode disabled.",
                    FishingPoleCarpentrySkill);
                return null;
            }

            if (pm.Skills.Tailoring.Value < FishingPoleTailoringSkill)
            {
                pm.SendMessage(0x22, "You need {0:0.0} Tailoring skill to create a fishing pole. Gathering mode disabled.",
                    FishingPoleTailoringSkill);
                return null;
            }

            // Boards preferred over raw logs to mirror the OSI carpentry recipe.
            // Players who only have logs still get a pole crafted -- the helper
            // burns logs as a fallback so the experience matches "I have wood
            // and cloth in my pack".
            int boards = pack.GetAmount(typeof(Board));
            int logs = pack.GetAmount(typeof(Log));

            if (boards + logs < FishingPoleBoardCost)
            {
                pm.SendMessage(0x22, "You need {0} boards or logs to craft a fishing pole. Gathering mode disabled.",
                    FishingPoleBoardCost);
                return null;
            }

            int cloth = pack.GetAmount(typeof(Cloth)) + pack.GetAmount(typeof(BoltOfCloth)) * 50;

            if (cloth < FishingPoleClothCost)
            {
                pm.SendMessage(0x22, "You need {0} cloth in your pack to craft a fishing pole. Gathering mode disabled.",
                    FishingPoleClothCost);
                return null;
            }

            int boardsToConsume = Math.Min(boards, FishingPoleBoardCost);
            int logsToConsume = FishingPoleBoardCost - boardsToConsume;

            if (boardsToConsume > 0 && !pack.ConsumeTotal(typeof(Board), boardsToConsume))
            {
                pm.SendMessage(0x22, "Failed to consume boards for fishing pole creation.");
                return null;
            }

            if (logsToConsume > 0 && !pack.ConsumeTotal(typeof(Log), logsToConsume))
            {
                pm.SendMessage(0x22, "Failed to consume logs for fishing pole creation.");
                return null;
            }

            if (!pack.ConsumeTotal(typeof(Cloth), FishingPoleClothCost))
            {
                // Fallback: ConsumeTotal won't unravel BoltOfCloth automatically.
                int needed = FishingPoleClothCost;
                Item bolt = pack.FindItemByType(typeof(BoltOfCloth));

                if (bolt != null)
                {
                    bolt.Delete();
                    Cloth unraveled = new Cloth(50 - needed) { Hue = bolt.Hue };
                    pack.DropItem(unraveled);
                }
                else
                {
                    pm.SendMessage(0x22, "Failed to consume cloth for fishing pole creation.");
                    return null;
                }
            }

            Item newPole = new FishingPole();
            pack.DropItem(newPole);
            pm.SendMessage(0x40, "You craft a new fishing pole from your boards and cloth.");
            return newPole;
        }

        private static void NotifyOnce(PlayerMobile pm, string text)
        {
            DateTime now = DateTime.UtcNow;
            if (_NextEmptyNotify.TryGetValue(pm, out DateTime next) && now < next)
                return;

            _NextEmptyNotify[pm] = now + EmptyNotifyCooldown;
            pm.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, false, text);
        }

        // Walks the player's backpack and smelts any BaseOre stacks into ingots,
        // provided a forge or fire beetle is within 2 tiles. Mirrors the skill
        // checks and "burn the impurities" failure mode from BaseOre.OnTarget so
        // the auto-path produces identical results to manual smelting.
        private static void AutoSmeltOres(PlayerMobile pm)
        {
            Container pack = pm.Backpack;
            if (pack == null)
                return;

            // Check pack first so we don't bother forge-scanning when there's no ore.
            var ores = pack.FindItemsByType<BaseOre>();
            if (ores == null || ores.Count == 0)
                return;

            if (!IsForgeNearby(pm))
            {
                NotifyOnce(pm, "You need a forge or fire beetle nearby to smelt.");
                return;
            }

            foreach (BaseOre ore in ores)
            {
                if (ore == null || ore.Deleted)
                    continue;

                SmeltOre(pm, ore);
            }
        }

        private static bool IsForgeNearby(PlayerMobile pm)
        {
            Map map = pm.Map;
            if (map == null || map == Map.Internal)
                return false;

            // Items with ForgeAttribute (vanilla Forge) or known forge tile IDs.
            IPooledEnumerable<Item> items = map.GetItemsInRange(pm.Location, 2);
            foreach (Item item in items)
            {
                if (IsForgeObject(item))
                {
                    items.Free();
                    return true;
                }
            }
            items.Free();

            // Mobiles with ForgeAttribute (Fire Beetle, etc.). Skip dead bonded
            // pets, matching BaseOre.IsForge.
            IPooledEnumerable<Mobile> mobs = map.GetMobilesInRange(pm.Location, 2);
            foreach (Mobile m in mobs)
            {
                if (m.IsDeadBondedPet)
                    continue;

                if (m.GetType().IsDefined(typeof(Server.Engines.Craft.ForgeAttribute), false))
                {
                    mobs.Free();
                    return true;
                }
            }
            mobs.Free();

            // Static tiles -- forge tile ranges from BaseOre.IsForge.
            var statics = map.Tiles.GetStaticTiles(pm.X, pm.Y, true);
            for (int i = 0; i < statics.Length; i++)
            {
                int id = statics[i].ID;
                if (id == 4017 || (id >= 6522 && id <= 6569))
                    return true;
            }

            return false;
        }

        private static bool IsForgeObject(object obj)
        {
            if (obj is Mobile && ((Mobile)obj).IsDeadBondedPet)
                return false;

            if (obj.GetType().IsDefined(typeof(Server.Engines.Craft.ForgeAttribute), false))
                return true;

            int itemID = 0;
            if (obj is Item)
                itemID = ((Item)obj).ItemID;

            return itemID == 4017 || (itemID >= 6522 && itemID <= 6569);
        }

        // Replicates BaseOre.InternalTarget.OnTarget's smelt branch -- skill
        // gate, success / failure (impurity burn), and ingot output.
        private static void SmeltOre(PlayerMobile pm, BaseOre ore)
        {
            double difficulty;

            switch (ore.Resource)
            {
                default: difficulty = 50.0; break;
                case CraftResource.DullCopper: difficulty = 65.0; break;
                case CraftResource.ShadowIron: difficulty = 70.0; break;
                case CraftResource.Copper: difficulty = 75.0; break;
                case CraftResource.Bronze: difficulty = 80.0; break;
                case CraftResource.Gold: difficulty = 85.0; break;
                case CraftResource.Agapite: difficulty = 90.0; break;
                case CraftResource.Verite: difficulty = 95.0; break;
                case CraftResource.Valorite: difficulty = 99.0; break;
            }

            // Smelter's talisman bypasses the skill gate when matching resource.
            bool talisman = false;
            SmeltersTalisman t = pm.FindItemOnLayer(Layer.Talisman) as SmeltersTalisman;
            if (t != null && t.Resource == ore.Resource)
                talisman = true;

            if (difficulty > 50.0 && difficulty > pm.Skills[SkillName.Mining].Value && !talisman)
                return; // silent skip — high-tier ore stays as ore until skill grows

            // 0x19B7 is the small "ore pile" graphic which needs >= 2 to make 1 ingot.
            if (ore.ItemID == 0x19B7 && ore.Amount < 2)
                return;

            double minSkill = difficulty - 25.0;
            double maxSkill = difficulty + 25.0;

            if (talisman || pm.CheckTargetSkill(SkillName.Mining, ore, minSkill, maxSkill))
            {
                int toConsume = ore.Amount;
                if (toConsume <= 0)
                    return;

                if (toConsume > 30000)
                    toConsume = 30000;

                int ingotAmount;
                if (ore.ItemID == 0x19B7)
                {
                    ingotAmount = toConsume / 2;
                    if (toConsume % 2 != 0)
                        --toConsume;
                }
                else if (ore.ItemID == 0x19B9)
                {
                    ingotAmount = toConsume * 2;
                }
                else
                {
                    ingotAmount = toConsume;
                }

                BaseIngot ingot = ore.GetIngot();
                ingot.Amount = ingotAmount;

                if (ore.HasSocket<Caddellite>())
                    ingot.AttachSocket(new Caddellite());

                ore.Consume(toConsume);
                pm.AddToBackpack(ingot);

                if (talisman && t != null)
                    t.UsesRemaining--;
            }
            else
            {
                // Skill check failed — burn impurities, ore amount halves.
                if (ore.Amount < 2)
                {
                    if (ore.ItemID == 0x19B9)
                        ore.ItemID = 0x19B8;
                    else
                        ore.ItemID = 0x19B7;
                }
                else
                {
                    ore.Amount /= 2;
                }
            }
        }

        // Convert any logs in the player's backpack to boards using their axe.
        // Skill checks are handled by BaseLog.Axe -> TryCreateBoards. Each
        // log stack converts instantly via ScissorHelper (no swing delay).
        private static void AutoCutLogs(PlayerMobile pm)
        {
            Container pack = pm.Backpack;
            if (pack == null)
                return;

            BaseAxe axe = pack.FindItemByType(typeof(BaseAxe)) as BaseAxe;
            if (axe == null)
                return;

            // Snapshot in case Axe modifies the container during iteration.
            var logs = pack.FindItemsByType<BaseLog>();
            if (logs == null || logs.Count == 0)
                return;

            foreach (BaseLog log in logs)
            {
                if (log.Deleted)
                    continue;

                log.Axe(pm, axe);
            }
        }

        private static Item FindTool(PlayerMobile pm, GatheringMode mode)
        {
            Container pack = pm.Backpack;

            switch (mode)
            {
                case GatheringMode.Mining:
                    {
                        if (pack == null) return null;
                        Item tool = pack.FindItemByType(typeof(Pickaxe));
                        if (tool == null)
                            tool = pack.FindItemByType(typeof(Shovel));
                        return tool;
                    }
                case GatheringMode.Fishing:
                    {
                        // Fishing requires the pole to be equipped (UO mechanic).
                        // Fishing.StartHarvesting auto-equips a pack pole on
                        // first cast, so subsequent ticks find it on the hand
                        // layer. Check both so we work whether the pole is
                        // already in hand or still sitting in the pack.
                        Item held = pm.FindItemOnLayer(Layer.OneHanded)
                                    ?? pm.FindItemOnLayer(Layer.TwoHanded);

                        if (held is FishingPole)
                            return held;

                        return pack?.FindItemByType(typeof(FishingPole));
                    }
                default:
                    {
                        if (pack == null) return null;
                        // Any axe variant -- BaseAxe is the common ancestor.
                        return pack.FindItemByType(typeof(BaseAxe));
                    }
            }
        }

        private static object ScanForTarget(PlayerMobile pm, GatheringMode mode)
        {
            Map map = pm.Map;
            if (map == null)
                return null;

            HarvestDefinition def;
            switch (mode)
            {
                case GatheringMode.Mining: def = Mining.System.OreAndStone; break;
                case GatheringMode.Fishing: def = Fishing.System.Definition; break;
                default: def = Lumberjacking.System.Definition; break;
            }

            int range = def.MaxRange;

            // Mining: land tiles only (mountain/cave land tile IDs).
            // Lumberjacking: static tiles only (trees).
            // Fishing: BOTH -- coastal water can be either land tiles OR
            //   placed-static water; the harvest definition's Tiles array
            //   holds a mix and uses RangedTiles to pair (min,max) ranges
            //   so we delegate the matching to def.Validate() rather than
            //   iterating the array ourselves.
            bool checkLand = mode != GatheringMode.Lumberjacking;
            bool checkStatics = mode == GatheringMode.Lumberjacking || mode == GatheringMode.Fishing;

            object bestTarget = null;
            int bestDist = int.MaxValue;

            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    int dist = Math.Max(Math.Abs(dx), Math.Abs(dy));
                    if (dist >= bestDist)
                        continue;

                    int x = pm.X + dx;
                    int y = pm.Y + dy;

                    if (checkLand)
                    {
                        var landTile = map.Tiles.GetLandTile(x, y);
                        if (def.Validate(landTile.ID) && HasResources(def, map, x, y))
                        {
                            int z = map.GetAverageZ(x, y);
                            bestTarget = new LandTarget(new Point3D(x, y, z), map);
                            bestDist = dist;
                            continue;
                        }
                    }

                    if (checkStatics)
                    {
                        var staticTiles = map.Tiles.GetStaticTiles(x, y, true);
                        for (int i = 0; i < staticTiles.Length; i++)
                        {
                            int adjusted = (staticTiles[i].ID & 0x3FFF) | 0x4000;
                            if (!def.Validate(adjusted))
                                continue;

                            if (!HasResources(def, map, x, y))
                                continue;

                            bestTarget = new StaticTarget(
                                new Point3D(x, y, staticTiles[i].Z),
                                staticTiles[i].ID);
                            bestDist = dist;
                            break;
                        }
                    }
                }
            }

            return bestTarget;
        }

        private static bool HasResources(HarvestDefinition def, Map map, int x, int y)
        {
            HarvestBank bank = def.GetBank(map, x, y);
            return bank != null && bank.Current >= def.ConsumedPerHarvest;
        }
    }
}
