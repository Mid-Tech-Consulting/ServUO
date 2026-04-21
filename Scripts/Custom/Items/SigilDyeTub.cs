using Server.Targeting;

namespace Server.Items
{
    public class SigilDyeTub : DyeTub, Engines.VeteranRewards.IRewardItem
    {
        private bool m_IsRewardItem;

        [Constructable]
        public SigilDyeTub()
        {
            Hue = DyedHue = 0x000B;
            Redyable = false;
            LootType = LootType.Blessed;
            Name = "Sigil Dye Tub";
        }

        public SigilDyeTub(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !Engines.VeteranRewards.RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.SendLocalizedMessage(TargetMessage);
            from.Target = new InternalTarget(this);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Core.ML && m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_IsRewardItem = reader.ReadBool();
        }

        private class InternalTarget : Target
        {
            private readonly SigilDyeTub m_Tub;

            public InternalTarget(SigilDyeTub tub)
                : base(1, false, TargetFlags.None)
            {
                m_Tub = tub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is Item item))
                {
                    from.SendLocalizedMessage(m_Tub.FailMessage);
                    return;
                }

                if (!from.InRange(m_Tub.GetWorldLocation(), 1) || !from.InRange(item.GetWorldLocation(), 1))
                {
                    from.SendLocalizedMessage(500446); // That is too far away.
                    return;
                }

                if (item.Parent is Mobile)
                {
                    from.SendLocalizedMessage(500861); // Can't Dye clothing that is being worn.
                    return;
                }

                if (item is BaseWeapon || item is BaseArmor || item is BaseClothing
                    || item is BaseJewel || item is BaseShield || item is BaseTalisman)
                {
                    item.Hue = m_Tub.DyedHue;
                    from.PlaySound(0x23E);
                    return;
                }

                if (item is IDyable dyable)
                {
                    if (dyable.Dye(from, m_Tub))
                        from.PlaySound(0x23E);
                    return;
                }

                from.SendLocalizedMessage(m_Tub.FailMessage);
            }
        }
    }
}
