using System;

namespace Server.Items
{
    [Flipable(0x9F40, 0x9F3F)]
    public class UndeadWeddingVeil : BaseHat
    {
        public bool m_Transformed;
        private int m_Hue = -1;
        private int m_SaveHueMod = -1;

        public bool Transformed
        {
            get { return m_Transformed; }
            set { m_Transformed = value; }
        }
        public int CostumeHue
        {
            get { return m_Hue; }
            set { m_Hue = value; }
        }

        public override int LabelNumber { get { return 1124792; } } // Undead Wedding Veil

        [Constructable]
        public UndeadWeddingVeil() : base(0x9F40)
        {
            this.CostumeHue = 1150;
            this.Weight = 3.0;
            this.LootType = LootType.Blessed;
        }

        public UndeadWeddingVeil(Serial serial)
            : base(serial)
        {
        }

        private bool EnCostume(Mobile from)
        {
            if (from.Mounted || from.Flying) // You cannot use this while mounted or flying.
            {
                from.SendLocalizedMessage(1010097);
            }
            else if (from.IsBodyMod || from.HueMod > -1)
            {
                from.SendLocalizedMessage(1158010); // You cannot use that item in this form.
            }
            else
            {
                from.HueMod = m_Hue;
                Transformed = true;

                return true;
            }

            return false;
        }

        private void DeCostume(Mobile from)
        {
            from.HueMod = -1;
            Transformed = false;
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            else if (RootParent is Mobile && from != RootParent)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public override bool OnEquip(Mobile from)
        {
            if (!Transformed)
            {
                if (EnCostume(from))
                    return true;

                return false;
            }

            return base.OnEquip(from);
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile && Transformed)
            {
                DeCostume((Mobile)parent);
            }

            base.OnRemoved(parent);
        }

        public override bool AllowMaleWearer
        {
            get
            {
                return false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3);
            writer.Write((int)m_Hue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    m_Hue = reader.ReadInt();
                    break;
                case 2:
                    m_Hue = reader.ReadInt();
                    reader.ReadInt();
                    break;
                case 1:
                    m_Hue = reader.ReadInt();
                    reader.ReadInt();
                    reader.ReadBool();

                    m_SaveHueMod = reader.ReadInt();
                    reader.ReadInt();
                    break;
            }

            if (RootParent is Mobile && ((Mobile)RootParent).Items.Contains(this))
            {
                EnCostume((Mobile)RootParent);
            }
        }
    }
}
