using Server.Targeting;

namespace Server.Items
{
    public class CursedRemovalDeed : Item
    {
        [Constructable]
        public CursedRemovalDeed() : base(0x14F0)
        {
            Weight = 1.0;
            Name = "Cursed Removal Deed";
            LootType = LootType.Blessed;
            Hue = 1175;
        }

        public CursedRemovalDeed(Serial serial) : base(serial)
        {
        }

        public override bool DisplayLootType { get { return false; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Target a cursed item to remove the cursed status.");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            from.SendMessage("Target the cursed item to restore.");
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
            private readonly CursedRemovalDeed m_Deed;

            public InternalTarget(CursedRemovalDeed deed) : base(1, false, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Deed.Deleted)
                    return;

                Item item = target as Item;

                if (!(item is BaseClothing || item is BaseArmor || item is BaseShield ||
                      item is BaseWeapon || item is BaseJewel || item is BaseTalisman))
                {
                    from.SendMessage("You cannot use that on this item.");
                    return;
                }

                if (item.LootType != LootType.Cursed)
                {
                    from.SendMessage("This item is not cursed.");
                    return;
                }

                item.LootType = LootType.Regular;
                from.SendMessage("This item is no longer cursed.");
                m_Deed.Delete();
            }
        }
    }
}
