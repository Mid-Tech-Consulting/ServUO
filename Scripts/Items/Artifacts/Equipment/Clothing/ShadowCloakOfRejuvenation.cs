using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTailoring), typeof(GargishShadowCloakOfRejuvenation))]
    public class ShadowCloakOfRejuvenation : Cloak
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115649; } } // Shadow Cloak Of Rejuvenation

        [Constructable]
        public ShadowCloakOfRejuvenation()
        {
            Hue = 1884;
            Attributes.RegenHits = 6;
            Attributes.RegenStam = 6;
            Attributes.RegenMana = 6;
            Attributes.LowerManaCost = 10;
            Attributes.Luck = 150;
            Attributes.CastRecovery = 3;
            SAAbsorptionAttributes.EaterKinetic = 15;
        }

        public ShadowCloakOfRejuvenation(Serial serial)
            : base(serial)
        {
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

    public class GargishShadowCloakOfRejuvenation : GargishClothWingArmor
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1115649; } } // Shadow Cloak Of Rejuvenation

        [Constructable]
        public GargishShadowCloakOfRejuvenation()
        {
            Hue = 1884;
            Attributes.RegenHits = 6;
            Attributes.RegenStam = 6;
            Attributes.RegenMana = 6;
            Attributes.LowerManaCost = 10;
            Attributes.Luck = 150;
            Attributes.CastRecovery = 3;
            SAAbsorptionAttributes.EaterKinetic = 15;
        }

        public GargishShadowCloakOfRejuvenation(Serial serial)
            : base(serial)
        {
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