using System;

namespace Server.Items
{
    public class VioletCourage : FemalePlateChest
	{
		public override bool IsArtifact { get { return true; } }

        // +20 to a random combat skill (Swords / Macing / Fencing / Archery).
        private static readonly SkillName[] m_RandomSkills =
        {
            SkillName.Swords,
            SkillName.Macing,
            SkillName.Fencing,
            SkillName.Archery,
        };

        [Constructable]
        public VioletCourage()
        {
            Hue = Utility.RandomBool() ? 0x486 : 0x490;
            Attributes.Luck = 95;
            Attributes.DefendChance = 15;
            Attributes.BonusStam = 8;
            Attributes.RegenMana = 2;
            Attributes.WeaponSpeed = 5;
            Attributes.LowerManaCost = 5;
            ArmorAttributes.LowerStatReq = 100;
            ArmorAttributes.MageArmor = 1;
            SkillBonuses.SetValues(0, m_RandomSkills[Utility.Random(m_RandomSkills.Length)], 20.0);
        }

        public VioletCourage(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063471;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 14;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 8;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 9;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
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