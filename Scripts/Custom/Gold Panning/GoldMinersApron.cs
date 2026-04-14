using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishGoldMinersApron))]
    public class GoldMinersApron : HalfApron
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public GoldMinersApron()
            : base()
        {
            this.Hue = 18;
			
            Attributes.WeaponSpeed = 15;
        }

        public GoldMinersApron(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075043;
            }
        }// Crimson Cincture
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    public class GargishGoldMinersApron : GargoyleHalfApron
    {
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
            }
        }

        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishGoldMinersApron()
            : base()
        {
            this.Hue = 0x485;

            this.Attributes.BonusDex = 5;
            this.Attributes.BonusHits = 10;
            this.Attributes.RegenHits = 2;
        }

        public GargishGoldMinersApron(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075043;
            }
        }// Crimson Cincture
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}