using System;

namespace Server.Items
{
    public abstract class BaseResourceSatchel : Container
    {
        public const int DefaultWeightReduction = 50;

        private int _WeightReduction;

        [CommandProperty(AccessLevel.GameMaster)]
        public int WeightReduction
        {
            get { return _WeightReduction; }
            set
            {
                int val = Math.Clamp(value, 0, 100);

                if (_WeightReduction != val)
                {
                    int oldEffective = GetTotal(TotalType.Weight);
                    _WeightReduction = val;
                    int newEffective = GetTotal(TotalType.Weight);
                    int delta = newEffective - oldEffective;

                    if (delta != 0)
                    {
                        if (Parent is Item parentItem)
                            parentItem.UpdateTotal(this, TotalType.Weight, delta);
                        else if (Parent is Mobile parentMobile)
                            parentMobile.UpdateTotal(this, TotalType.Weight, delta);
                        else if (HeldBy != null)
                            HeldBy.UpdateTotal(this, TotalType.Weight, delta);
                    }

                    InvalidateWeight();
                    InvalidateProperties();
                }
            }
        }

        public abstract Type[] HoldTypes { get; }

        public BaseResourceSatchel(int id)
            : base(id)
        {
            _WeightReduction = DefaultWeightReduction;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_WeightReduction != 0)
                list.Add(1072210, _WeightReduction.ToString()); // Weight reduction: ~1_PERCENTAGE~%
        }

        public override int GetTotal(TotalType type)
        {
            int total = base.GetTotal(type);

            if (type == TotalType.Weight && _WeightReduction > 0)
                total -= (int)(total * ((double)_WeightReduction / 100.0));

            return total;
        }

        public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            if (type == TotalType.Weight && _WeightReduction > 0 && sender != this && delta != 0 && !sender.IsVirtualItem)
            {
                int oldEffective = GetTotal(TotalType.Weight);

                base.UpdateTotal(sender, type, delta);

                int newEffective = GetTotal(TotalType.Weight);
                int effectiveDelta = newEffective - oldEffective;
                int diff = effectiveDelta - delta;

                if (diff != 0)
                {
                    if (Parent is Item parentItem)
                        parentItem.UpdateTotal(sender, TotalType.Weight, diff);
                    else if (Parent is Mobile parentMobile)
                        parentMobile.UpdateTotal(sender, TotalType.Weight, diff);
                    else if (HeldBy != null)
                        HeldBy.UpdateTotal(sender, TotalType.Weight, diff);
                }
            }
            else
            {
                base.UpdateTotal(sender, type, delta);
            }
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!CheckType(item))
            {
                if (message)
                    m.SendLocalizedMessage(1074836); // The container can not hold that type of object.

                return false;
            }

            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        public bool CheckType(Item item)
        {
            Type type = item.GetType();

            foreach (var t in HoldTypes)
            {
                if (type == t || type.IsSubclassOf(t))
                {
                    return true;
                }
            }

            return false;
        }

        public void InvalidateWeight()
        {
            if (RootParent is Mobile)
            {
                Mobile m = (Mobile)RootParent;

                m.UpdateTotals();
            }
        }

        public override void AddItem(Item dropped)
        {
            base.AddItem(dropped);

            InvalidateWeight();
        }

        public override void RemoveItem(Item dropped)
        {
            base.RemoveItem(dropped);

            InvalidateWeight();
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);
            InvalidateWeight();
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile m)
                m.UpdateTotals();
            else if (parent is Item i && i.RootParent is Mobile rm)
                rm.UpdateTotals();
        }

        public BaseResourceSatchel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_WeightReduction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _WeightReduction = reader.ReadInt();
        }
    }

    [Flipable(0xA272, 0xA273)]
    public class MinersSatchel : BaseResourceSatchel
    {
        public override int LabelNumber { get { return 1158773; } } // Miner's Satchel

        public override Type[] HoldTypes { get { return new Type[] { typeof(BaseOre), typeof(BaseIngot), typeof(Granite), typeof(Saltpeter) }; } }

        [Constructable]
        public MinersSatchel()
            : base(0xA272)
        {
        }

        public MinersSatchel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    [Flipable(0xA274, 0xA275)]
    public class LumbjacksSatchel : BaseResourceSatchel
    {
        public override int LabelNumber { get { return 1158772; } } // Lumberjack's Satchel

        public override Type[] HoldTypes { get { return new Type[] { typeof(BaseLog), typeof(Board) }; } }

        [Constructable]
        public LumbjacksSatchel()
            : base(0xA274)
        {
        }

        public LumbjacksSatchel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}