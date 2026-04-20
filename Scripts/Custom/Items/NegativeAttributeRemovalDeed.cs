using Server.Targeting;

namespace Server.Items
{
    public class NegativeAttributeRemovalDeed : Item
    {
        [Constructable]
        public NegativeAttributeRemovalDeed() : base(0x14F0)
        {
            Weight = 1.0;
            Name = "Attribute Removal Deed";
            LootType = LootType.Blessed;
            Hue = 1175;
        }

        public NegativeAttributeRemovalDeed(Serial serial) : base(serial)
        {
        }

        public override bool DisplayLootType { get { return false; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Target your item to remove:");
            list.Add("Antique, Massive, No Repair, Prized, Unwieldly.");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            from.SendMessage("Target the item to remove a negative attribute from.");
            from.Target = new InternalTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            LootType = LootType.Blessed;
        }

        private class InternalTarget : Target
        {
            private readonly NegativeAttributeRemovalDeed m_Deed;

            public InternalTarget(NegativeAttributeRemovalDeed deed) : base(1, false, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Deed.Deleted)
                    return;

                NegativeAttributes neg = GetNegativeAttributes(target);

                if (neg == null)
                {
                    from.SendMessage("You cannot use that on this item.");
                    return;
                }

                if (!RemoveFirstNegative(neg))
                {
                    from.SendMessage("This item has no negative attributes to remove.");
                    return;
                }

                if (target is Item item)
                    item.InvalidateProperties();

                from.SendMessage("You removed a negative attribute from this item.");
                m_Deed.Delete();
            }

            private static NegativeAttributes GetNegativeAttributes(object target)
            {
                if (target is BaseArmor armor) return armor.NegativeAttributes;
                if (target is BaseWeapon weapon) return weapon.NegativeAttributes;
                if (target is BaseJewel jewel) return jewel.NegativeAttributes;
                return null;
            }

            private static bool RemoveFirstNegative(NegativeAttributes neg)
            {
                if (neg.Antique != 0) { neg.Antique = 0; return true; }
                if (neg.Massive != 0) { neg.Massive = 0; return true; }
                if (neg.NoRepair != 0) { neg.NoRepair = 0; return true; }
                if (neg.Prized != 0) { neg.Prized = 0; return true; }
                if (neg.Unwieldly != 0) { neg.Unwieldly = 0; return true; }
                return false;
            }
        }
    }
}
