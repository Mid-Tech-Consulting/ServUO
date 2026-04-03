using Server;
using System;

namespace Server.Items
{
    public class FemaleKimonoBearingTheCrestOfBlackthorn5 : FemaleKimono
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public FemaleKimonoBearingTheCrestOfBlackthorn5()
            : base()
        {
            ReforgedSuffix = ReforgedSuffix.Blackthorn;
            Attributes.Luck = 100;
            SAAbsorptionAttributes.EaterDamage = 15;
            Attributes.EnhancePotions = 35;
            Attributes.BonusStr = 10;
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 6;
            Hue = 132;
        }

        public FemaleKimonoBearingTheCrestOfBlackthorn5(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
			
			if (version == 0)
            {
                MaxHitPoints = 0;
                HitPoints = 0;
            }
        }
    }
}