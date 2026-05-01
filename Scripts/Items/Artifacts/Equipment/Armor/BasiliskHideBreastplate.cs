using System;

namespace Server.Items
{
    public class BasiliskHideBreastplate : DragonChest
    {
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115444; } } // Basilisk Hide Breastplate

        [Constructable]
        public BasiliskHideBreastplate() 
        {
            Resource = CraftResource.None;
            Hue = 1366;
            AbsorptionAttributes.EaterDamage = 10;
            Attributes.BonusDex = 5;
            Attributes.BonusStam = 8;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 2;
            Attributes.RegenMana = 2;
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 30;
            Attributes.WeaponSpeed = 10;
        }

        public BasiliskHideBreastplate(Serial serial)
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
                return 14;
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
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Resource = CraftResource.None;
                this.Hue = 1366;
            }
        }
    }
}