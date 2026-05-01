using System;

namespace Server.Items
{
    public class VoidInfusedKilt : GargishPlateKilt
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113868; } } // Void Infused Kilt
		
        [Constructable]
        public VoidInfusedKilt()
            : base()
        {
            Hue = 2124;
            Attributes.AttackChance = 10;
            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.RegenMana = 1;
            Attributes.RegenStam = 1;
            Attributes.LowerManaCost = 8;
            AbsorptionAttributes.EaterDamage = 10;
            SkillBonuses.SetValues(0, SkillName.MagicResist, 15.0);
        }

        public VoidInfusedKilt(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
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