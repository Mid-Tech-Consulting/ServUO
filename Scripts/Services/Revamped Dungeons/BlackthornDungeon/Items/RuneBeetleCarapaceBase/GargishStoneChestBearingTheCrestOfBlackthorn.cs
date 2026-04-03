using Server;
using System;

namespace Server.Items
{
    public class GargishStoneChestBearingTheCrestOfBlackthorn : GargishStoneChest
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishStoneChestBearingTheCrestOfBlackthorn()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            this.Hue = 1773;
            this.Attributes.BonusMana = 10;
            this.Attributes.RegenMana = 3;
            this.Attributes.LowerManaCost = 15;
            this.Attributes.LowerRegCost = 15;
            this.Attributes.Luck = 150;
            this.ArmorAttributes.LowerStatReq = 100;
            this.ArmorAttributes.MageArmor = 1;
            this.AbsorptionAttributes.EaterFire = 15;
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

        public GargishStoneChestBearingTheCrestOfBlackthorn(Serial serial)
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