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
        private XiuLiNPC m_NPC;
        private Dictionary<Layer, Item> m_Items;
        private Dictionary<Layer, int> m_RepairCosts;
        private int m_TotalCost;

        public RepairAllGump(Mobile owner, XiuLiNPC npc)
            : base(50, 50)
        {
            m_Owner = owner;
            m_NPC = npc;
            m_Items = new Dictionary<Layer, Item>();
            m_RepairCosts = new Dictionary<Layer, int>();
            m_TotalCost = 0;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 760, 600, 302);
            AddLabel(270, 20, 37, @"Equipment Repair Service");

            CollectEquipmentInfo();
            DisplayEquipmentList();

            AddButton(220, 550, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(270, 550, 0x44, "One-click repair all equipment (" + m_TotalCost + " Gold)");
        }

        private bool TryDeductGold(Mobile from, int amount)
        {
            if (from.Backpack == null)
                return false;

            // 先尝试从背包扣除
            if (from.Backpack.ConsumeTotal(typeof(Gold), amount))
                return true;

            // 如果背包钱不够，尝试从银行扣除剩余部分
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
            Layer[] layers = new Layer[]
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

            foreach (Layer layer in layers)
            {
                Item item = m_Owner.FindItemOnLayer(layer);
                m_Items[layer] = item;

                if (item != null && IsRepairable(item))
                {
                    int cost = CalculateRepairCost(item);
                    m_RepairCosts[layer] = cost;
                    if (cost > 0)
                    {
                        m_TotalCost += cost;
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
                {
                    toConsume = (bw.MaxHitPoints - bw.HitPoints) * 2 * consumeFix;
                }
            }
            else if (item is BaseArmor)
            {
                BaseArmor ba = item as BaseArmor;
                if (ba.MaxHitPoints <= 10)
                    return 0;

                consumeFix = GetMaterialCost(ba.Resource);
                if (ba.HitPoints < ba.MaxHitPoints)
                {
                    toConsume = (ba.MaxHitPoints - ba.HitPoints) * 2 * consumeFix;
                }
            }
            else if (item is BaseJewel)
            {
                BaseJewel bj = item as BaseJewel;
                if (bj.MaxHitPoints <= 10)
                    return 0;

                consumeFix = GetMaterialCost(bj.Resource);
                if (bj.HitPoints < bj.MaxHitPoints)
                {
                    toConsume = (bj.MaxHitPoints - bj.HitPoints) * 2 * consumeFix;
                }
            }
            else if (item is BaseClothing)
            {
                BaseClothing bjc = item as BaseClothing;
                if (bjc.MaxHitPoints <= 10)
                    return 0;

                consumeFix = GetMaterialCost(bjc.Resource);
                if (bjc.HitPoints < bjc.MaxHitPoints)
                {
                    toConsume = (bjc.MaxHitPoints - bjc.HitPoints) * 2 * consumeFix;
                }
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
            {
                DisplayEquipmentItem(column1X, ref y, layer, itemSpacing);
            }

            y = 60;
            foreach (Layer layer in column2Layers)
            {
                DisplayEquipmentItem(column2X, ref y, layer, itemSpacing);
            }
        }

        private void DisplayEquipmentItem(int x, ref int y, Layer layer, int itemSpacing)
        {
            Item item = m_Items[layer];
            int cost = m_RepairCosts[layer];

            //AddLabel(x, y, 0x44, GetLayerName(layer) + ":");
            
            if (item != null)
            {
                AddItem(x + 50, y - 5, item.ItemID, item.Hue);
                AddLabel(x + 130, y, GetTextHue(item), GetDurabilityString(item));
                
                if (cost > 0)
                {
                    AddLabel(x + 200, y, 0x44, cost + " Gold");
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

        private string GetLayerName(Layer layer)
        {
            switch (layer)
            {
                case Layer.Helm: return "頭盔";
                case Layer.Neck: return "項鍊";
                case Layer.Earrings: return "耳環";
                case Layer.Shirt: return "襯衫";
                case Layer.Arms: return "護臂";
                case Layer.Gloves: return "手套";
                case Layer.Ring: return "戒指";
                case Layer.Talisman: return "護身符";
                case Layer.InnerTorso: return "內衣";
                case Layer.Bracelet: return "手鐲";
                case Layer.MiddleTorso: return "中衣";
                case Layer.OuterTorso: return "外衣";
                case Layer.Pants: return "褲子";
                case Layer.InnerLegs: return "內褲";
                case Layer.OuterLegs: return "護腿";
                case Layer.Shoes: return "鞋子";
                case Layer.Waist: return "腰帶";
                case Layer.Cloak: return "披風";
                case Layer.FirstValid: return "武器";
                case Layer.TwoHanded: return "副手";
                default: return layer.ToString();
            }
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

        private Layer GetLayerFromButtonID(int buttonID)
        {
            return (Layer)(buttonID - 100);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID == 1)
            {
                if (m_TotalCost > 0)
                {
                    if (HasEnoughGold(from, m_TotalCost))
                    {
                        if (TryDeductGold(from, m_TotalCost))
                        {
                            foreach (KeyValuePair<Layer, Item> kvp in m_Items)
                            {
                                if (kvp.Value != null && m_RepairCosts[kvp.Key] > 0)
                                {
                                    RepairItem(kvp.Value);
                                }
                            }
                            from.SendMessage("You paid " + m_TotalCost + " gold coins to repair all equipment");
                            m_NPC.SayTo(from, "You paid " + m_TotalCost + " gold coins to repair all equipment");
                            Effects.PlaySound(from.Location, from.Map, 0x2A);
                            from.SendGump(new RepairAllGump(from, m_NPC));
                        }
                    }
                    else
                    {
                        from.SendMessage("You don't have enough gold to pay for the repairs.");
                        m_NPC.SayTo(from, "You don't have enough gold to pay for the repairs.");
                        from.SendGump(new RepairAllGump(from, m_NPC));
                    }
                }
                else
                {
                    from.SendMessage(" No equipment requires repair requires repair.");
                    m_NPC.SayTo(from, " No equipment requires repair requires repair..");
                }
            }
            else if (info.ButtonID >= 100)
            {
                Layer layer = GetLayerFromButtonID(info.ButtonID);
                if (m_Items.ContainsKey(layer) && m_Items[layer] != null && m_RepairCosts[layer] > 0)
                {
                    Item item = m_Items[layer];
                    int cost = m_RepairCosts[layer];

                    if (HasEnoughGold(from, cost))
                    {
                        if (TryDeductGold(from, cost))
                        {
                            RepairItem(item);
                            from.SendMessage("You paid " + cost + " gold coins to repair " + item.Name +".");
                            m_NPC.SayTo(from, "You paid " + cost + " gold coins to repair " + item.Name +".");
                            Effects.PlaySound(from.Location, from.Map, 0x2A);
                            from.SendGump(new RepairAllGump(from, m_NPC));
                        }
                    }
                    else
                    {
                        from.SendMessage("You don't have enough gold to pay for the repairs.");
                        m_NPC.SayTo(from, "You don't have enough gold to pay for the repairs.");
                        from.SendGump(new RepairAllGump(from, m_NPC));
                    }
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

    public class RepairNPCAll : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public RepairNPCAll() : base("Equipment Repairman")
        {
            Body = 689;
            Hue = 1153;
            CantWalk = true;
        }

        public override void InitSBInfo()
        {
        }

        public RepairNPCAll(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
