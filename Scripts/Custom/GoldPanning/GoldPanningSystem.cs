/* Created by Hammerhand & Milva */
/* Modified to be self-contained */

using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Mobiles;
using System.Linq;

namespace Server.Engines.Harvest
{
    public class GoldPanningSystem : HarvestSystem
    {
        private static GoldPanningSystem m_Instance;

        public static GoldPanningSystem Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new GoldPanningSystem();

                return m_Instance;
            }
        }

        private readonly List<HarvestDefinition> m_Definitions;

        private GoldPanningSystem()
        {
            m_Definitions = new List<HarvestDefinition>();
            InitializeGoldPanning();
        }

        public List<HarvestDefinition> Definitions
        {
            get
            {
                return m_Definitions;
            }
        }

        private HarvestDefinition m_Definition;

        public HarvestDefinition Definition
        {
            get { return m_Definition; }
        }

        private void InitializeGoldPanning()
        {
            HarvestResource[] res;
            HarvestVein[] veins;

            #region GoldPanning
            HarvestDefinition nugget = new HarvestDefinition();

            // Resource banks are every 8x8 tiles
            nugget.BankWidth = 8;
            nugget.BankHeight = 8;

            // Every bank holds from 5 to 10 nuggets
            nugget.MinTotal = 5;
            nugget.MaxTotal = 10;

            // A resource bank will respawn its content every 10 to 20 minutes
            nugget.MinRespawn = TimeSpan.FromMinutes(10.0);
            nugget.MaxRespawn = TimeSpan.FromMinutes(20.0);

            // Skill checking is done on the Mining skill
            nugget.Skill = SkillName.Mining;

            // Set the list of harvestable tiles
            nugget.Tiles = m_WaterTiles;
            nugget.RangedTiles = true;

            // Players must be within 2 tiles to harvest
            nugget.MaxRange = 2;

            // One nugget per harvest action
            nugget.ConsumedPerHarvest = 1;
            nugget.ConsumedPerFeluccaHarvest = 1;

            // The panning
            nugget.EffectActions = new int[] { 32 };
            nugget.EffectSounds = new int[0];
            nugget.EffectCounts = new int[] { 1 };
            nugget.EffectDelay = TimeSpan.Zero;
            nugget.EffectSoundDelay = TimeSpan.FromSeconds(8.0);

            nugget.NoResourcesMessage = "There doesn't seem to be any nuggets left here."; // There doesn't seem to be any nuggets left here.
            nugget.FailMessage = "You pan for a while, but fail to find any nuggets."; // You pan for a while, but fail to find any nuggets.
            nugget.TimedOutOfRangeMessage = "You need to be closer to the water for panning!"; // You need to be closer to the water for panning!
            nugget.OutOfRangeMessage = "You need to be closer to the water for panning!"; // You need to be closer to the water for panning!
            nugget.PackFullMessage = "You dont have room in your pack for another nugget."; // You dont have room in your pack for another nugget.
            nugget.ToolBrokeMessage = "You wore out your gold pan."; // You wore out your gold pan.

            res = new HarvestResource[]
            {
                new HarvestResource(70.0, 75.0, 80.0, "You put a shallow water [Treasure Bag] in your pack.", typeof(GoldPanningTreasureBag)),
                new HarvestResource(80.0, 80.0, 85.0, "You put a shallow water [Treasure Chest] in your pack.", typeof(GoldPanningTreasureChest)),
                new HarvestResource(85.0, 85.0, 90.0, "You put a shallow water [Treasure Chest] in your pack.", typeof(GoldPanningTreasureChest)),
                new HarvestResource(90.0, 90.0, 90.0, "You put a shallow water [Treasure Chest] in your pack.", typeof(GoldPanningTreasureChest))
            };

            veins = new HarvestVein[]
            {
                new HarvestVein(70.0, 0.0, res[0], null),
                new HarvestVein(60.0, 0.5, res[1], res[0]),
                new HarvestVein(40.0, 1.0, res[2], res[0]),
                new HarvestVein(20.0, 1.5, res[3], res[0])
            };

            nugget.Resources = res;
            nugget.Veins = veins;

            if (Core.ML)
            {
                nugget.BonusResources = new BonusHarvestResource[]
                {
                    new BonusHarvestResource(0, 88.0, null, null),	//edit was 0, 88.0,
                    new BonusHarvestResource(100, 2.0, 1072562, typeof(BlueDiamond)),
                    new BonusHarvestResource(100, 2.0, 1072567, typeof(DarkSapphire)),
                    new BonusHarvestResource(100, 2.0, 1072570, typeof(EcruCitrine)),
                    new BonusHarvestResource(100, 2.0, 1072564, typeof(FireRuby)),
                    new BonusHarvestResource(100, 2.0, 1072566, typeof(PerfectEmerald)),
                    new BonusHarvestResource(100, 2.0, 1072568, typeof(Turquoise)),//was Turquoise
                    new BonusHarvestResource(100, 2.0, 1113349, typeof(DelicateScales)),
                    new BonusHarvestResource(100, 2.0, 1026256, typeof(BrilliantAmber)),
                    new BonusHarvestResource(100, 2.0, 1032694, typeof(WhitePearl))
                };
            }

            m_Definition = nugget;
            m_Definitions.Add(nugget);
            #endregion
        }

        public virtual bool CheckTool(Mobile from, Item tool)
        {
            bool wornOut = (tool == null || tool.Deleted || (tool is IUsesRemaining && ((IUsesRemaining)tool).UsesRemaining <= 0));

            if (wornOut)
                from.SendLocalizedMessage(1044038); // You have worn out your tool!

            return !wornOut;
        }

        public virtual bool CheckHarvest(Mobile from, Item tool)
        {
            return CheckTool(from, tool);
        }

        public virtual bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            if (!CheckTool(from, tool))
                return false;

            if (from.Mounted)
            {
                from.SendMessage("You can't pan for gold while riding!"); // You can't pan for gold while riding!
                return false;
            }

            return true;
        }

        public virtual bool CheckRange(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            bool inRange = (from.Map == map && from.InRange(loc, def.MaxRange));

            if (!inRange)
                def.SendMessageTo(from, timed ? def.TimedOutOfRangeMessage : def.OutOfRangeMessage);

            return inRange;
        }

        public virtual bool CheckResources(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);
            bool available = (bank != null && bank.Current >= def.ConsumedPerHarvest);

            if (!available)
                def.SendMessageTo(from, timed ? def.DoubleHarvestMessage : def.NoResourcesMessage);

            return available;
        }

        public virtual void OnBadHarvestTarget(Mobile from, Item tool, object toHarvest)
        {
        }

        public virtual object GetLock(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            return this;
        }

        public virtual void OnConcurrentHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            from.SendMessage("You are already panning. Gold panning takes time."); // You are already panning.
        }

        public override bool BeginHarvesting(Mobile from, Item tool)
        {
            if (!CheckHarvest(from, tool))
                return false;

            EventSink.InvokeResourceHarvestAttempt(new ResourceHarvestAttemptEventArgs(from, tool, this));
            from.Target = new HarvestTarget(tool, this);
            from.SendMessage("What water do you want to pan in?"); // What water do you want to pan in?
            return true;
        }

        public virtual void FinishHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked)
        {
            from.EndAction(locked);

            if (!CheckHarvest(from, tool))
                return;

            int tileID;
            Map map;
            Point3D loc;

            if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }
            else if (!def.Validate(tileID) && !def.ValidateSpecial(tileID))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            if (!CheckRange(from, tool, def, map, loc, true))
                return;
            else if (!CheckResources(from, tool, def, map, loc, true))
                return;
            else if (!CheckHarvest(from, tool, def, toHarvest))
                return;

            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

            if (bank == null)
                return;

            HarvestVein vein = bank.Vein;

            if (vein != null)
                vein = MutateVein(from, tool, def, bank, toHarvest, vein);

            if (vein == null)
                return;

            HarvestResource primary = vein.PrimaryResource;
            HarvestResource fallback = vein.FallbackResource;
            HarvestResource resource = MutateResource(from, tool, def, map, loc, vein, primary, fallback);

            double skillBase = from.Skills[def.Skill].Base;

            Type type = null;

            if (CheckHarvestSkill(map, loc, from, resource, def))
            {
                type = GetResourceType(from, tool, def, map, loc, resource);

                if (type != null)
                    type = MutateType(type, from, tool, def, map, loc, resource);

                if (type != null)
                {
                    Item item = Construct(type, from, tool);

                    if (item == null)
                    {
                        type = null;
                    }
                    else
                    {
                        int amount = def.ConsumedPerHarvest;
                        int feluccaAmount = def.ConsumedPerFeluccaHarvest;

                        if (item is BaseGranite)
                            feluccaAmount = 3;

                        Caddellite.OnHarvest(from, tool, this, item);

                        //The whole harvest system is kludgy and I'm sure this is just adding to it.
                        if (item.Stackable)
                        {
                            int racialAmount = (int)Math.Ceiling(amount * 1.1);
                            int feluccaRacialAmount = (int)Math.Ceiling(feluccaAmount * 1.1);

                            bool eligableForRacialBonus = (def.RaceBonus && from.Race == Race.Human);
                            bool inFelucca = map == Map.Felucca && !Siege.SiegeShard;

                            if (eligableForRacialBonus && inFelucca && bank.Current >= feluccaRacialAmount && 0.1 > Utility.RandomDouble())
                                item.Amount = feluccaRacialAmount;
                            else if (inFelucca && bank.Current >= feluccaAmount)
                                item.Amount = feluccaAmount;
                            else if (eligableForRacialBonus && bank.Current >= racialAmount && 0.1 > Utility.RandomDouble())
                                item.Amount = racialAmount;
                            else
                                item.Amount = amount;

                            // Void Pool Rewards
                            item.Amount += WoodsmansTalisman.CheckHarvest(from, type, this);
                        }

                        if (from.AccessLevel == AccessLevel.Player)
                        {
                            bank.Consume(amount, from);
                        }

                        if (Give(from, item, def.PlaceAtFeetIfFull))
                        {
                            SendSuccessTo(from, item, resource);
                        }
                        else
                        {
                            SendPackFullTo(from, item, def, resource);
                            item.Delete();
                        }

                        BonusHarvestResource bonus = def.GetBonusResource();
                        Item bonusItem = null;

                        if (bonus != null && bonus.Type != null && skillBase >= bonus.ReqSkill)
                        {
                            if (bonus.RequiredMap == null || bonus.RequiredMap == from.Map)
                            {
                                bonusItem = Construct(bonus.Type, from, tool);
                                Caddellite.OnHarvest(from, tool, this, bonusItem);

                                if (Give(from, bonusItem, true))	//Bonuses always allow placing at feet, even if pack is full irregrdless of def
                                {
                                    bonus.SendSuccessTo(from);
                                }
                                else
                                {
                                    bonusItem.Delete();
                                }
                            }
                        }

                        EventSink.InvokeResourceHarvestSuccess(new ResourceHarvestSuccessEventArgs(from, tool, item, bonusItem, this));
                    }

                    #region High Seas
                    OnToolUsed(from, tool, item != null);
                    #endregion
                }

                // Siege rules will take into account axes and polearms used for lumberjacking
                if (tool is IUsesRemaining && (tool is BaseHarvestTool || tool is Pickaxe || tool is SturdyPickaxe || tool is GargoylesPickaxe || Siege.SiegeShard))
                {
                    IUsesRemaining toolWithUses = (IUsesRemaining)tool;

                    toolWithUses.ShowUsesRemaining = true;

                    if (toolWithUses.UsesRemaining > 0)
                        --toolWithUses.UsesRemaining;

                    if (toolWithUses.UsesRemaining < 1)
                    {
                        tool.Delete();
                        def.SendMessageTo(from, def.ToolBrokeMessage);
                    }
                }
            }

            if (type == null)
                def.SendMessageTo(from, def.FailMessage);

            OnHarvestFinished(from, tool, def, vein, bank, resource, toHarvest);
        }

        public virtual bool CheckHarvestSkill(Map map, Point3D loc, Mobile from, HarvestResource resource, HarvestDefinition def)
        {
            return from.Skills[def.Skill].Value >= resource.ReqSkill && from.CheckSkill(def.Skill, resource.MinSkill, resource.MaxSkill);
        }

        public virtual void OnToolUsed(Mobile from, Item tool, bool caughtSomething)
        {
        }

        public virtual void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
            if (Core.ML)
                from.RevealingAction();
        }

        public virtual Item Construct(Type type, Mobile from, Item tool)
        {
            try
            {
                return Activator.CreateInstance(type) as Item;
            }
            catch
            {
                return null;
            }
        }

        public virtual HarvestVein MutateVein(Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
        {
            return vein;
        }

        public virtual void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
        {
            resource.SendSuccessTo(from);
        }

        public virtual void SendPackFullTo(Mobile from, Item item, HarvestDefinition def, HarvestResource resource)
        {
            def.SendMessageTo(from, def.PackFullMessage);
        }

        public virtual bool Give(Mobile m, Item item, bool placeAtFeet)
        {
            if (m.PlaceInBackpack(item))
                return true;

            if (!placeAtFeet)
                return false;

            Map map = m.Map;

            if (map == null || map == Map.Internal)
                return false;

            List<Item> atFeet = new List<Item>();

            IPooledEnumerable eable = m.GetItemsInRange(0);

            foreach (Item obj in eable)
                atFeet.Add(obj);

            eable.Free();

            for (int i = 0; i < atFeet.Count; ++i)
            {
                Item check = atFeet[i];

                if (check.StackWith(m, item, false))
                    return true;
            }

            ColUtility.Free(atFeet);

            item.MoveToWorld(m.Location, map);
            return true;
        }

        public virtual Type MutateType(Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            return from.Region.GetResource(type);
        }

        public virtual Type GetResourceType(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            if (resource.Types.Length > 0)
                return resource.Types[Utility.Random(resource.Types.Length)];

            return null;
        }

        public virtual HarvestResource MutateResource(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestVein vein, HarvestResource primary, HarvestResource fallback)
        {
            bool racialBonus = (def.RaceBonus && from.Race == Race.Elf);

            if (vein.ChanceToFallback > (Utility.RandomDouble() + (racialBonus ? .20 : 0)))
                return fallback;

            double skillValue = from.Skills[def.Skill].Value;

            if (fallback != null && (skillValue < primary.ReqSkill || skillValue < primary.MinSkill))
                return fallback;

            return primary;
        }

        public virtual bool OnHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked, bool last)
        {
            if (!CheckHarvest(from, tool))
            {
                from.EndAction(locked);
                return false;
            }

            int tileID;
            Map map;
            Point3D loc;

            if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                from.EndAction(locked);
                OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }
            else if (!def.Validate(tileID) && !def.ValidateSpecial(tileID))
            {
                from.EndAction(locked);
                OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }
            else if (!CheckRange(from, tool, def, map, loc, true))
            {
                from.EndAction(locked);
                return false;
            }
            else if (!CheckResources(from, tool, def, map, loc, true))
            {
                from.EndAction(locked);
                return false;
            }
            else if (!CheckHarvest(from, tool, def, toHarvest))
            {
                from.EndAction(locked);
                return false;
            }

            DoHarvestingEffect(from, tool, def, map, loc);

            new HarvestSoundTimer(from, tool, this, def, toHarvest, locked, last).Start();

            return !last;
        }

        public virtual void DoHarvestingSound(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            if (def.EffectSounds.Length > 0)
                from.PlaySound(Utility.RandomList(def.EffectSounds));
        }

        public virtual void DoHarvestingEffect(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
        {
            from.Direction = from.GetDirectionTo(loc);

            if (!from.Mounted)
            {
                if (Core.SA)
                {
                    from.Animate(AnimationType.Attack, Utility.RandomList(def.EffectActions));
                }
                else
                {
                    from.Animate(Utility.RandomList(def.EffectActions), 5, 1, true, false, 0);
                }
            }
        }

        public virtual HarvestDefinition GetDefinition(int tileID)
        {
            return GetDefinition(tileID, null);
        }

        public virtual HarvestDefinition GetDefinition(int tileID, Item tool)
        {
            HarvestDefinition def = null;

            for (int i = 0; def == null && i < m_Definitions.Count; ++i)
            {
                HarvestDefinition check = m_Definitions[i];

                if (check.Validate(tileID))
                    def = check;
            }

            return def;
        }

        #region High Seas
        public virtual HarvestDefinition GetDefinitionFromSpecialTile(int tileID)
        {
            HarvestDefinition def = null;

            for (int i = 0; def == null && i < m_Definitions.Count; ++i)
            {
                HarvestDefinition check = m_Definitions[i];

                if (check.ValidateSpecial(tileID))
                    def = check;
            }

            return def;
        }
        #endregion

        public virtual void StartHarvesting(Mobile from, Item tool, object toHarvest)
        {
            if (!CheckHarvest(from, tool))
                return;

            int tileID;
            Map map;
            Point3D loc;

            if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            HarvestDefinition def = GetDefinition(tileID, tool);

            if (def == null)
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            if (!CheckRange(from, tool, def, map, loc, false))
                return;
            else if (!CheckResources(from, tool, def, map, loc, false))
                return;
            else if (!CheckHarvest(from, tool, def, toHarvest))
                return;

            object toLock = GetLock(from, tool, def, toHarvest);

            if (!from.BeginAction(toLock))
            {
                OnConcurrentHarvest(from, tool, def, toHarvest);
                return;
            }

            new HarvestTimer(from, tool, this, def, toHarvest, toLock).Start();
            OnHarvestStarted(from, tool, def, toHarvest);
        }

        public virtual bool GetHarvestDetails(Mobile from, Item tool, object toHarvest, out int tileID, out Map map, out Point3D loc)
        {
            if (toHarvest is Static && !((Static)toHarvest).Movable)
            {
                Static obj = (Static)toHarvest;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = obj.Map;
                loc = obj.GetWorldLocation();
            }
            else if (toHarvest is StaticTarget)
            {
                StaticTarget obj = (StaticTarget)toHarvest;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = from.Map;
                loc = obj.Location;
            }
            else if (toHarvest is LandTarget obj)
            {
                tileID = obj.TileID;
                map = from.Map;
                loc = obj.Location;
            }
            else
            {
                tileID = 0;
                map = null;
                loc = Point3D.Zero;
                return false;
            }

            return map != null && map != Map.Internal;
        }

        #region Enhanced Client
        public static void TargetByResource(TargetByResourceMacroEventArgs e)
        {
            Mobile m = e.Mobile;
            Item tool = e.Tool;

            GoldPanningSystem system = null;
            HarvestDefinition def = null;
            object toHarvest;

            if (tool is IHarvestTool)
            {
                // Note: This assumes the tool's system is GoldPanningSystem
                system = (GoldPanningSystem)((IHarvestTool)tool).HarvestSystem;
            }

            if (system != null)
            {
                // Gold panning specific logic if needed
                if (FindValidTile(m, system.Definition, out toHarvest))
                {
                    system.StartHarvesting(m, tool, toHarvest);
                    return;
                }

                system.OnBadHarvestTarget(m, tool, new LandTarget(new Point3D(0, 0, 0), Map.Felucca));
            }
        }

        private static bool FindValidTile(Mobile m, HarvestDefinition definition, out object toHarvest)
        {
            Map map = m.Map;
            toHarvest = null;

            if (m == null || map == null || map == Map.Internal)
                return false;

            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, false);

                    if (tiles.Length > 0)
                    {
                        foreach (var tile in tiles)
                        {
                            int id = (tile.ID & 0x3FFF) | 0x4000;

                            if (definition.Validate(id))
                            {
                                toHarvest = new StaticTarget(new Point3D(x, y, tile.Z), tile.ID);
                                return true;
                            }
                        }
                    }

                    LandTile lt = map.Tiles.GetLandTile(x, y);

                    if (definition.Validate(lt.ID))
                    {
                        toHarvest = new LandTarget(new Point3D(x, y, lt.Z), map);
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryHarvestGrave(Mobile m)
        {
            Map map = m.Map;

            if (map == null)
                return false;

            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, false);

                    foreach (var tile in tiles)
                    {
                        int itemID = tile.ID;

                        if (itemID == 0xED3 || itemID == 0xEDF || itemID == 0xEE0 || itemID == 0xEE1 || itemID == 0xEE2 || itemID == 0xEE8)
                        {
                            if (m is PlayerMobile player)
                            {
                                QuestSystem qs = player.Quest;

                                if (qs is WitchApprenticeQuest)
                                {
                                    if (qs.FindObjective(typeof(FindIngredientObjective)) is FindIngredientObjective obj && !obj.Completed && obj.Ingredient == Ingredient.Bones)
                                    {
                                        player.SendLocalizedMessage(1055037); // You finish your grim work, finding some of the specific bones listed in the Hag's recipe.
                                        obj.Complete();

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public static bool TryHarvestShrooms(Mobile m)
        {
            Map map = m.Map;

            if (map == null)
                return false;

            for (int x = m.X - 1; x <= m.X + 1; x++)
            {
                for (int y = m.Y - 1; y <= m.Y + 1; y++)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(x, y, false);

                    foreach (var tile in tiles)
                    {
                        int itemID = tile.ID;

                        if (itemID == 0xD15 || itemID == 0xD16)
                        {
                            if (m is PlayerMobile player)
                            {
                                QuestSystem qs = player.Quest;

                                if (qs is WitchApprenticeQuest)
                                {
                                    if (qs.FindObjective(typeof(FindIngredientObjective)) is FindIngredientObjective obj && !obj.Completed && obj.Ingredient == Ingredient.RedMushrooms)
                                    {
                                        player.SendLocalizedMessage(1055036); // You slice a red cap mushroom from its stem.
                                        obj.Complete();

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        #endregion

        private static int[] m_WaterTiles = new int[]
        {
            0x00A8, 0x00AB,
            0x0136, 0x0137,
            0x5797, 0x579C,
            0x746E, 0x7485,
            0x7490, 0x74AB,
            0x74B5, 0x75D5
        };
    }
}

namespace Server
{
    public interface IChopable
    {
        void OnChop(Mobile from);
    }

    public interface IHarvestTool : IEntity
    {
        Engines.Harvest.HarvestSystem HarvestSystem { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FurnitureAttribute : Attribute
    {
        public FurnitureAttribute()
        {
        }

        private static bool IsNotChoppables(Item item)
        {
            return _NotChoppables.Any(t => t == item.GetType());
        }

        private static Type[] _NotChoppables = new Type[]
        {
            typeof(CommodityDeedBox), typeof(ChinaCabinet), typeof(PieSafe), typeof(AcademicBookCase), typeof(JewelryBox),
            typeof(WoodenBookcase), typeof(Countertop), typeof(Mailbox)
        };

        public static bool Check(Item item)
        {
            if (item == null)
            {
                return false;
            }

            if (IsNotChoppables(item))
            {
                return false;
            }

            if (item.GetType().IsDefined(typeof(FurnitureAttribute), false))
            {
                return true;
            }

            if (item is AddonComponent && ((AddonComponent)item).Addon != null && ((AddonComponent)item).Addon.GetType().IsDefined(typeof(FurnitureAttribute), false))
            {
                return true;
            }

            return false;
        }
    }
}/* Created by Hammerhand & Milva */

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Harvest
{
	public class GoldPanning : HarvestSystem
	{
		private static GoldPanning m_System;

		public static GoldPanning System
		{
			get
			{
				if ( m_System == null )
					m_System = new GoldPanning();

				return m_System;
			}
		}

		private HarvestDefinition m_Definition;

		public HarvestDefinition Definition
		{
			get{ return m_Definition; }
		}

        private GoldPanning()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

            #region GoldPanning
            HarvestDefinition nugget = new HarvestDefinition();

			// Resource banks are every 8x8 tiles
            nugget.BankWidth = 8;
            nugget.BankHeight = 8;

			// Every bank holds from 5 to 10 nuggets
            nugget.MinTotal = 5;
            nugget.MaxTotal = 10;

			// A resource bank will respawn its content every 10 to 20 minutes
            nugget.MinRespawn = TimeSpan.FromMinutes(10.0);
            nugget.MaxRespawn = TimeSpan.FromMinutes(20.0);

			// Skill checking is done on the Mining skill
            nugget.Skill = SkillName.Mining;

			// Set the list of harvestable tiles
            nugget.Tiles = m_WaterTiles;
            nugget.RangedTiles = true;

			// Players must be within 2 tiles to harvest
            nugget.MaxRange = 2;

			// One nugget per harvest action
            nugget.ConsumedPerHarvest = 1;
            nugget.ConsumedPerFeluccaHarvest = 1;

			// The panning
            nugget.EffectActions = new int[] { 32 };
            nugget.EffectSounds = new int[0];
            nugget.EffectCounts = new int[] { 1 };
            nugget.EffectDelay = TimeSpan.Zero;
            nugget.EffectSoundDelay = TimeSpan.FromSeconds(8.0);

            nugget.NoResourcesMessage = "There doesn't seem to be any nuggets left here."; // There doesn't seem to be any nuggets left here.
            nugget.FailMessage = "You pan for a while, but fail to find any nuggets."; // You pan for a while, but fail to find any nuggets.
            nugget.TimedOutOfRangeMessage = "You need to be closer to the water for panning!"; // You need to be closer to the water for panning!
            nugget.OutOfRangeMessage = "You need to be closer to the water for panning!"; // You need to be closer to the water for panning!
            nugget.PackFullMessage = "You dont have room in your pack for another nugget."; // You dont have room in your pack for another nugget.
            nugget.ToolBrokeMessage = "You wore out your gold pan."; // You wore out your gold pan.

			res = new HarvestResource[]
				{
					new HarvestResource( 70.0, 75.0, 80.0, "You put a shallow water [Treasure Bag] in your pack.", typeof( GoldPanningTreasureBag ) ),
					new HarvestResource( 80.0, 80.0, 85.0, "You put a shallow water [Treasure Chest] in your pack.", typeof( GoldPanningTreasureChest ) ),
					new HarvestResource( 85.0, 85.0, 90.0, "You put a shallow water [Treasure Chest] in your pack.", typeof( GoldPanningTreasureChest ) ),
					new HarvestResource( 90.0, 90.0, 90.0, "You put a shallow water [Treasure Chest] in your pack.", typeof( GoldPanningTreasureChest ) )
                    
				};

			veins = new HarvestVein[]
				{
					new HarvestVein( 70.0, 0.0, res[0], null ),
                    new HarvestVein( 60.0, 0.5, res[1], res[0] ),
                    new HarvestVein( 40.0, 1.0, res[2], res[0] ),
                    new HarvestVein( 20.0, 1.5, res[3], res[0] )
				};

            nugget.Resources = res;
            nugget.Veins = veins;

            if (Core.ML)
            {
                nugget.BonusResources = new BonusHarvestResource[]
				{
					new BonusHarvestResource( 0, 88.0, null, null ),	//edit was 0, 88.0,
					new BonusHarvestResource( 100, 2.0, 1072562, typeof( BlueDiamond ) ),
					new BonusHarvestResource( 100, 2.0, 1072567, typeof( DarkSapphire ) ),
					new BonusHarvestResource( 100, 2.0, 1072570, typeof( EcruCitrine ) ),
					new BonusHarvestResource( 100, 2.0, 1072564, typeof( FireRuby ) ),
					new BonusHarvestResource( 100, 2.0, 1072566, typeof( PerfectEmerald ) ),
					new BonusHarvestResource( 100, 2.0, 1072568, typeof( Turquoise ) ),//was Turquoise
					new BonusHarvestResource( 100, 2.0, 1113349, typeof( DelicateScales ) ),
					new BonusHarvestResource( 100, 2.0, 1026256, typeof( BrilliantAmber ) ),
					new BonusHarvestResource( 100, 2.0, 1032694, typeof( WhitePearl ) )
				};
            }

            m_Definition = nugget;
            Definitions.Add(nugget);
			#endregion
		}

		public override void OnConcurrentHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
            from.SendMessage("You are already panning. Gold panning takes time."); // You are already panning.
		}

		private static Map SafeMap( Map map )
		{
			if ( map == null || map == Map.Internal )
				return Map.Trammel;

			return map;
		}

		public override void OnHarvestStarted( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			base.OnHarvestStarted( from, tool, def, toHarvest );

			int tileID;
			Map map;
			Point3D loc;

			if ( GetHarvestDetails( from, tool, toHarvest, out tileID, out map, out loc ) )
				Timer.DelayCall( TimeSpan.FromSeconds( 1.5 ), 
					delegate
					{
						if( Core.ML )
							from.RevealingAction();

						Effects.SendLocationEffect( loc, map, 0x352D, 16, 4 );
						Effects.PlaySound( loc, map, 0x364 );
					} );
		}

		public override void OnHarvestFinished( Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested )
		{
			base.OnHarvestFinished( from, tool, def, vein, bank, resource, harvested );

			if ( Core.ML )
				from.RevealingAction();
		}

		public override object GetLock( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			return this;
		}

		public override bool BeginHarvesting( Mobile from, Item tool )
		{
			if ( !base.BeginHarvesting( from, tool ) )
				return false;

			from.SendMessage( "What water do you want to pan in?" ); // What water do you want to pan in?
			return true;
		}

		public override bool CheckHarvest( Mobile from, Item tool )
		{
			if ( !base.CheckHarvest( from, tool ) )
				return false;

			if ( from.Mounted )
			{
                from.SendMessage("You can't pan for gold while riding!"); // You can't pan for gold while riding!
				return false;
			}

			return true;
		}

		public override bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( !base.CheckHarvest( from, tool, def, toHarvest ) )
				return false;

			if ( from.Mounted )
			{
                from.SendMessage("You can't pan for gold while riding!"); // You can't pan for gold while riding!
				return false;
			}

			return true;
		}

		private static int[] m_WaterTiles = new int[]
			{
				0x00A8, 0x00AB,
				0x0136, 0x0137,
				0x5797, 0x579C,
				0x746E, 0x7485,
				0x7490, 0x74AB,
				0x74B5, 0x75D5
			};
	}
}
