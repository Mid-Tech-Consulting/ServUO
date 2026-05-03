using System;

namespace Server.Items
{
    public class OrcishVisage : OrcHelm
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public OrcishVisage()
        {
            Hue = 0x592;
            ArmorAttributes.SelfRepair = 3;
            Attributes.BonusStr = 10;
            Attributes.BonusStam = 5;
            Attributes.LowerManaCost = 8;
            Attributes.RegenHits = 3;
            AbsorptionAttributes.EaterKinetic = 10;
            SkillBonuses.SetValues(0, SkillName.Lumberjacking, 20.0);
        }

        public OrcishVisage(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070691;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 10;
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