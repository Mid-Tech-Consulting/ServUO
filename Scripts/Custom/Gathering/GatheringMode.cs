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
        Lumberjacking
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
        // and the perceived swing tempo on OSI is brisker.
        private static readonly TimeSpan MiningCycleCooldown = TimeSpan.FromSeconds(1.5);
        private static readonly TimeSpan LumberjackingCycleCooldown = TimeSpan.FromSeconds(5.0);

        // Tinkering skill threshold + ingot cost for auto-create tools.
        // Matches DefTinkering.cs: Hatchet=30, Shovel/Pickaxe=40, all 4 iron ingots.
        private const double HatchetSkill = 30.0;
        private const double MiningToolSkill = 40.0;
        private const int ToolIngotCost = 4;

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

            string label = mode == GatheringMode.Mining ? "mining" : "lumberjacking";
            pm.SendMessage(0x40, "You begin {0}. Stand near a vein and the system will gather for you.", label);

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
                    continue;
                }

                TryGather(pm, mode);

                if (mode == GatheringMode.Lumberjacking && IsAutoCutLogs(pm))
                    AutoCutLogs(pm);

                if (mode == GatheringMode.Mining && IsAutoSmelt(pm))
                    AutoSmeltOres(pm);
            }

            StopTimerIfIdle();
        }

        private static void TryGather(PlayerMobile pm, GatheringMode mode)
        {
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
                    string toolDesc = mode == GatheringMode.Mining
                        ? "pickaxe or shovel"
                        : "axe";
                    pm.SendMessage(0x22, "You have no {0} in your pack. Gathering mode disabled.", toolDesc);
                    SetMode(pm, GatheringMode.None);
                    return;
                }
            }

            object target = ScanForTarget(pm, mode);
            if (target == null)
            {
                NotifyOnce(pm, "Nothing left here. Move to the next spot.");
                return;
            }

            if (mode == GatheringMode.Mining)
                Mining.System.StartHarvesting(pm, tool, target);
            else
                Lumberjacking.System.StartHarvesting(pm, tool, target);

            // Stamp the cooldown regardless of whether StartHarvesting actually
            // succeeded -- if it bailed (concurrent lock, range, etc.), waiting
            // a cycle before retrying is still the right behaviour.
            TimeSpan cooldown = mode == GatheringMode.Mining
                ? MiningCycleCooldown
                : LumberjackingCycleCooldown;

            _NextHarvestAllowed[pm] = now + cooldown;
        }

        // Auto-craft a replacement tool from iron ingots in the player's pack.
        // Returns the new tool on success, null on failure (skill or ingots
        // missing). Skill cap is the same threshold as DefTinkering.cs.
        private static Item TryCreateTool(PlayerMobile pm, GatheringMode mode)
        {
            Container pack = pm.Backpack;
            if (pack == null)
                return null;

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
            if (pack == null)
                return null;

            if (mode == GatheringMode.Mining)
            {
                Item tool = pack.FindItemByType(typeof(Pickaxe));
                if (tool == null)
                    tool = pack.FindItemByType(typeof(Shovel));
                return tool;
            }

            // Any axe variant — BaseAxe is the common ancestor.
            return pack.FindItemByType(typeof(BaseAxe));
        }

        private static object ScanForTarget(PlayerMobile pm, GatheringMode mode)
        {
            Map map = pm.Map;
            if (map == null)
                return null;

            HarvestDefinition def = mode == GatheringMode.Mining
                ? Mining.System.OreAndStone
                : Lumberjacking.System.Definition;

            int[] tiles = def.Tiles;
            int range = def.MaxRange;

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

                    if (mode == GatheringMode.Mining)
                    {
                        var landTile = map.Tiles.GetLandTile(x, y);
                        if (Array.IndexOf(tiles, landTile.ID) < 0)
                            continue;

                        // Skip tiles whose resource bank is depleted -- otherwise we'd
                        // keep retargeting the same nearest tile after its bank empties
                        // and quietly fail every cycle while other in-range banks sit
                        // unused.
                        if (!HasResources(def, map, x, y))
                            continue;

                        int z = map.GetAverageZ(x, y);
                        bestTarget = new LandTarget(new Point3D(x, y, z), map);
                        bestDist = dist;
                    }
                    else
                    {
                        var staticTiles = map.Tiles.GetStaticTiles(x, y, true);
                        for (int i = 0; i < staticTiles.Length; i++)
                        {
                            int adjusted = (staticTiles[i].ID & 0x3FFF) | 0x4000;
                            if (Array.IndexOf(tiles, adjusted) < 0)
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
