using System;

namespace Server.Items
{
    public class DefenderOfTheMagus : MetalShield
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DefenderOfTheMagus() 
        {
            Hue = 590;
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 25;
            Attributes.CastRecovery = 2;
            ArmorAttributes.SoulCharge = 30;
            AbsorptionAttributes.EaterFire = 15;
            Attributes.Luck = 150;
            Attributes.LowerRegCost = 15;
            //Random Resonance:
            switch (Utility.Random(5))
            {
                case 0:
                    AbsorptionAttributes.ResonanceCold = 10;
                    break;
                case 1:
                    AbsorptionAttributes.ResonanceFire = 10;
                    break;
                case 2:
                    AbsorptionAttributes.ResonanceKinetic = 10;
                    break;
                case 3:
                    AbsorptionAttributes.ResonancePoison = 10;
                    break;
                case 4:
                    AbsorptionAttributes.ResonanceEnergy = 10;
                    break;
            }
            //Random Resist:
            switch (Utility.Random(5))
            {
                case 0:
                    ColdBonus = 10;
                    break;
                case 1:
                    FireBonus = 10;
                    break;
                case 2:
                    PhysicalBonus = 10;
                    break;
                case 3:
                    PoisonBonus = 10;
                    break;
                case 4:
                    EnergyBonus = 10;
                    break;
            }

        }

        public DefenderOfTheMagus(Serial serial)
            : base(serial)
        {
        }
        
        public override int LabelNumber { get{return 1113851;} }// Defender of the Magus

        public override int BasePhysicalResistance
        {
            get
            {
                return 0;
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
                return 0;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 0;
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
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
