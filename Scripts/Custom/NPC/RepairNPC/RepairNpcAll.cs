using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Gumps
{
    public class RepairAllGump : Gump
    {
        private Mobile m_Owner;
        private RepairNPCAll m_NPC;
        private Dictionary<Layer, Item> m_Items;
        private Dictionary<Layer, int> m_RepairCosts;
        private int m_TotalCost;
        private bool m_FreeRepairs;

        private static readonly Layer[] RepairLayers = new Layer[]
        {
            Layer.Helm,
            Layer.Neck,
            Layer.Earrings,
            Layer.Shirt,
            Layer.Arms,
            Layer.Gloves,
            Layer.Ring,
            Layer.Talisman,
            Layer.InnerTorso,
            Layer.Bracelet,
            Layer.MiddleTorso,
            Layer.OuterTorso,
            Layer.Pants,
            Layer.InnerLegs,
            Layer.OuterLegs,
            Layer.Shoes,
            Layer.Waist,
            Layer.Cloak,
            Layer.FirstValid,
            Layer.TwoHanded
        };

        public RepairAllGump(Mobile owner, RepairNPCAll npc)
            : base(50, 50)
        {
            m_Owner = owner;
            m_NPC = npc;
            m_FreeRepairs = npc.FreeRepairs;
            m_Items = new Dictionary<Layer, Item>();
            m_RepairCosts = new Dictionary<Layer, int>();
            m_TotalCost = 0;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 760, 600, 9270);
            AddAlphaRegion(10, 10, 740, 580);
            AddLabel(270, 20, 37, @"Equipment Repair Service");

            CollectEquipmentInfo();
            DisplayEquipmentList();

            AddButton(220, 550, 4005, 4007, 1, GumpButtonType.Reply, 0);
            string repairAllLabel = m_FreeRepairs
                ? "One-click repair all equipment (Free)"
                : "One-click repair all equipment (" + m_TotalCost + " Gold)";
            AddLabel(270, 550, 0x44, repairAllLabel);
        }

        private bool TryDeductGold(Mobile from, int amount)
        {
            if (from.Backpack == null)
                return false;

            if (from.Backpack.ConsumeTotal(typeof(Gold), amount))
                return true;

            int backpackGold = from.Backpack.GetAmount(typeof(Gold));
            int remaining = amount - backpackGold;

            if (remaining > 0 && Banker.Withdraw(from, remaining))
            {
                from.Backpack.ConsumeTotal(typeof(Gold), backpackGold);
                return true;
            }

            return false;
        }

        private bool HasEnoughGold(Mobile from, int amount)
        {
            if (from.Backpack == null)
                return false;

            int backpackGold = from.Backpack.GetAmount(typeof(Gold));
            if (backpackGold >= amount)
                return true;

            int bankGold = Banker.GetBalance(from);
            return (backpackGold + bankGold) >= amount;
        }

        private void CollectEquipmentInfo()
        {
            foreach (Layer layer in RepairLayers)
            {
                Item item = m_Owner.FindItemOnLayer(layer);
                m_Items[layer] = item;

                if (item != null && IsRepairable(item))
                {
                    int cost = CalculateRepairCost(item);
                    m_RepairCosts[layer] = cost;
                    if (cost > 0)
                    {
                        // Guard against integer overflow across all slots
                        long newTotal = (long)m_TotalCost + cost;
                        m_TotalCost = (int)Math.Min(newTotal, int.MaxValue);
                    }
                }
                else
                {
                    m_RepairCosts[layer] = 0;
                }
            }
        }

        private bool IsRepairable(Item item)
        {
            if (item is BaseWeapon || item is BaseArmor || item is BaseJewel || item is BaseClothing)
            {
                PropertyInfo maxHitPoints = item.GetType().GetProperty("MaxHitPoints");
                if (maxHitPoints != null)
                {
                    int maxHP = (int)maxHitPoints.GetValue(item, null);
                    return maxHP > 10;
                }
            }
            return false;
        }

        private int CalculateRepairCost(Item item)
        {
            int consumeFix = 5;
            int toConsume = 0;

            if (item is BaseWeapon)
            {
                BaseWeapon bw = item as BaseWeapon;
                if (bw.MaxHitPoints <= 10)
                    return 0;

                consumeFix = GetMaterialCost(bw.Resource);
                if (bw.HitPoints < bw.MaxHitPoints)
                    toConsume = (bw.MaxHitPoints - bw.HitPoints) * 2 * consumeFix;
            }
            else if (item is BaseArmor)
            {
                BaseArmor ba = item as BaseArmor;
                if (ba.MaxHitPoints <= 10)
                    return 0;

                consumeFix = GetMaterialCost(ba.Resource);
                if (ba.HitPoints < ba.MaxHitPoints)
                    toConsume = (ba.MaxHitPoints - ba.HitPoints) * 2 * consumeFix;
            }
            else if (item is BaseJewel)
            {
                BaseJewel bj = item as BaseJewel;
                if (bj.MaxHitPoints <= 10)
                    return 0;

                consumeFix = GetMaterialCost(bj.Resource);
                if (bj.HitPoints < bj.MaxHitPoints)
                    toConsume = (bj.MaxHitPoints - bj.HitPoints) * 2 * consumeFix;
            }
            else if (item is BaseClothing)
            {
                BaseClothing bjc = item as BaseClothing;
                if (bjc.MaxHitPoints <= 10)
                    return 0;

                consumeFix = GetMaterialCost(bjc.Resource);
                if (bjc.HitPoints < bjc.MaxHitPoints)
                    toConsume = (bjc.MaxHitPoints - bjc.HitPoints) * 2 * consumeFix;
            }

            return toConsume;
        }

        private int GetMaterialCost(CraftResource resource)
        {
            switch (resource)
            {
                case CraftResource.DullCopper: return 10;
                case CraftResource.ShadowIron: return 20;
                case CraftResource.Copper: return 30;
                case CraftResource.Bronze: return 40;
                case CraftResource.Gold: return 50;
                case CraftResource.Agapite: return 60;
                case CraftResource.Verite: return 70;
                case CraftResource.Valorite: return 80;
                case CraftResource.OakWood: return 10;
                case CraftResource.AshWood: return 20;
                case CraftResource.YewWood: return 30;
                case CraftResource.Heartwood: return 40;
                case CraftResource.Bloodwood: return 50;
                case CraftResource.Frostwood: return 60;
                case CraftResource.SpinedLeather: return 10;
                case CraftResource.HornedLeather: return 20;
                case CraftResource.BarbedLeather: return 30;
                case CraftResource.RedScales:
                case CraftResource.YellowScales:
                case CraftResource.BlackScales:
                case CraftResource.GreenScales:
                case CraftResource.WhiteScales:
                case CraftResource.BlueScales: return 15;
                default: return 5;
            }
        }

        private void DisplayEquipmentList()
        {
            int y = 60;
            int column1X = 20;
            int column2X = 420;
            int itemSpacing = 50;

            List<Layer> column1Layers = new List<Layer>();
            List<Layer> column2Layers = new List<Layer>();

            int halfCount = m_Items.Count / 2;
            int index = 0;
            foreach (Layer layer in m_Items.Keys)
            {
                if (index < halfCount)
                    column1Layers.Add(layer);
                else
                    column2Layers.Add(layer);
                index++;
            }

            foreach (Layer layer in column1Layers)
                DisplayEquipmentItem(column1X, ref y, layer, itemSpacing);

            y = 60;
            foreach (Layer layer in column2Layers)
                DisplayEquipmentItem(column2X, ref y, layer, itemSpacing);
        }

        private void DisplayEquipmentItem(int x, ref int y, Layer layer, int itemSpacing)
        {
            Item item = m_Items[layer];
            int cost = m_RepairCosts[layer];

            if (item != null)
            {
                AddItem(x + 50, y - 5, item.ItemID, item.Hue);
                AddLabel(x + 130, y, GetTextHue(item), GetDurabilityString(item));

                if (cost > 0)
                {
                    AddLabel(x + 200, y, 0x44, m_FreeRepairs ? "Free" : cost + " Gold");
                    AddButton(x + 280, y - 5, 0x2a3a, 0x2a3a, GetButtonID(layer), GumpButtonType.Reply, 0);
                }
                else if (IsRepairable(item))
                {
                    AddLabel(x + 200, y, 0x44, "No repair needed");
                }
                else
                {
                    AddLabel(x + 200, y, 0x44, "Non-repairable");
                }
            }
            else
            {
                AddLabel(x + 130, y, 0x44, "Null");
            }

            y += itemSpacing;
        }

        private int GetTextHue(Item item)
        {
            PropertyInfo hitPoints = item.GetType().GetProperty("HitPoints");
            PropertyInfo maxHitPoints = item.GetType().GetProperty("MaxHitPoints");
            if (hitPoints != null && maxHitPoints != null)
            {
                int hp = (int)hitPoints.GetValue(item, null);
                int maxHP = (int)maxHitPoints.GetValue(item, null);
                if (maxHP <= 10)
                    return 33;
                if (hp < maxHP * 0.3)
                    return 37;
                else if (hp < maxHP * 0.6)
                    return 53;
                else if (hp < maxHP)
                    return 88;
                else
                    return 0;
            }
            return 0;
        }

        private string GetDurabilityString(Item item)
        {
            PropertyInfo hitPoints = item.GetType().GetProperty("HitPoints");
            PropertyInfo maxHitPoints = item.GetType().GetProperty("MaxHitPoints");
            if (hitPoints != null && maxHitPoints != null)
            {
                int hp = (int)hitPoints.GetValue(item, null);
                int maxHP = (int)maxHitPoints.GetValue(item, null);
                return hp + "/" + maxHP;
            }
            return "N/A";
        }

        private int GetButtonID(Layer layer)
        {
            return 100 + (int)layer;
        }

        private bool TryGetLayerFromButtonID(int buttonID, out Layer layer)
        {
            layer = (Layer)(buttonID - 100);
            return m_Items.ContainsKey(layer);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (!from.InRange(m_NPC.Location, 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (info.ButtonID == 1)
            {
                // Re-verify which items are still equipped and recalculate cost at response time
                var toRepair = new List<KeyValuePair<Layer, Item>>();
                long freshTotal = 0;

                foreach (Layer layer in RepairLayers)
                {
                    Item snapshotItem;
                    if (!m_Items.TryGetValue(layer, out snapshotItem) || snapshotItem == null)
                        continue;

                    // Confirm item is still on this player at this layer
                    Item currentItem = from.FindItemOnLayer(layer);
                    if (currentItem == null || currentItem != snapshotItem)
                        continue;

                    int cost = CalculateRepairCost(currentItem);
                    if (cost > 0)
                    {
                        toRepair.Add(new KeyValuePair<Layer, Item>(layer, currentItem));
                        freshTotal += cost;
                    }
                }

                int totalCost = (int)Math.Min(freshTotal, int.MaxValue);

                if (totalCost > 0)
                {
                    bool doRepair = m_FreeRepairs;

                    if (!doRepair)
                    {
                        if (HasEnoughGold(from, totalCost))
                            doRepair = TryDeductGold(from, totalCost);

                        if (!doRepair)
                        {
                            from.SendMessage("You don't have enough gold to pay for the repairs.");
                            m_NPC.SayTo(from, "You don't have enough gold to pay for the repairs.");
                            from.SendGump(new RepairAllGump(from, m_NPC));
                        }
                    }

                    if (doRepair)
                    {
                        foreach (var kvp in toRepair)
                            RepairItem(kvp.Value);

                        string msg = m_FreeRepairs
                            ? "All equipment has been repaired."
                            : "You paid " + totalCost + " gold coins to repair all equipment.";
                        from.SendMessage(msg);
                        m_NPC.SayTo(from, msg);
                        Effects.PlaySound(from.Location, from.Map, 0x2A);
                        from.SendGump(new RepairAllGump(from, m_NPC));
                    }
                }
                else
                {
                    from.SendMessage("No equipment requires repair.");
                    m_NPC.SayTo(from, "No equipment requires repair.");
                }
            }
            else if (info.ButtonID >= 100)
            {
                Layer layer;
                if (!TryGetLayerFromButtonID(info.ButtonID, out layer))
                    return;

                Item snapshotItem;
                if (!m_Items.TryGetValue(layer, out snapshotItem) || snapshotItem == null)
                    return;

                // Confirm item is still on this player at this layer
                Item currentItem = from.FindItemOnLayer(layer);
                if (currentItem == null || currentItem != snapshotItem)
                {
                    from.SendMessage("That item is no longer equipped.");
                    from.SendGump(new RepairAllGump(from, m_NPC));
                    return;
                }

                int cost = CalculateRepairCost(currentItem);
                if (cost <= 0)
                {
                    from.SendGump(new RepairAllGump(from, m_NPC));
                    return;
                }

                bool doRepair = m_FreeRepairs;

                if (!doRepair)
                {
                    if (HasEnoughGold(from, cost))
                        doRepair = TryDeductGold(from, cost);

                    if (!doRepair)
                    {
                        from.SendMessage("You don't have enough gold to pay for the repairs.");
                        m_NPC.SayTo(from, "You don't have enough gold to pay for the repairs.");
                        from.SendGump(new RepairAllGump(from, m_NPC));
                    }
                }

                if (doRepair)
                {
                    RepairItem(currentItem);
                    string msg = m_FreeRepairs
                        ? currentItem.Name + " has been repaired."
                        : "You paid " + cost + " gold coins to repair " + currentItem.Name + ".";
                    from.SendMessage(msg);
                    m_NPC.SayTo(from, msg);
                    Effects.PlaySound(from.Location, from.Map, 0x2A);
                    from.SendGump(new RepairAllGump(from, m_NPC));
                }
            }
        }

        private void RepairItem(Item item)
        {
            if (item is BaseWeapon)
            {
                BaseWeapon bw = item as BaseWeapon;
                if (bw.MaxHitPoints > 10)
                {
                    bw.MaxHitPoints -= 1;
                    bw.HitPoints = bw.MaxHitPoints;
                }
            }
            else if (item is BaseArmor)
            {
                BaseArmor ba = item as BaseArmor;
                if (ba.MaxHitPoints > 10)
                {
                    ba.MaxHitPoints -= 1;
                    ba.HitPoints = ba.MaxHitPoints;
                }
            }
            else if (item is BaseJewel)
            {
                BaseJewel bj = item as BaseJewel;
                if (bj.MaxHitPoints > 10)
                {
                    bj.MaxHitPoints -= 1;
                    bj.HitPoints = bj.MaxHitPoints;
                }
            }
            else if (item is BaseClothing)
            {
                BaseClothing bjc = item as BaseClothing;
                if (bjc.MaxHitPoints > 10)
                {
                    bjc.MaxHitPoints -= 1;
                    bjc.HitPoints = bjc.MaxHitPoints;
                }
            }
        }
    }

    public class RepairNPCAll : BaseCreature
    {
        private bool m_FreeRepairs;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool FreeRepairs
        {
            get { return m_FreeRepairs; }
            set { m_FreeRepairs = value; }
        }

        [Constructable]
        public RepairNPCAll() : base(AIType.AI_Vendor, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Equipment Repairman";
            Body = 689;
            Hue = 1153;
            CantWalk = true;
        }

        public RepairNPCAll(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.Alive)
                return;

            if (from.InRange(this.Location, 3))
            {
                from.SendGump(new RepairAllGump(from, this));
            }
            else
            {
                from.SendLocalizedMessage(500446);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_FreeRepairs);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_FreeRepairs = reader.ReadBool();
        }
    }
}
