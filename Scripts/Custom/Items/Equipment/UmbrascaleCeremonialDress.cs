using System;

using Server;

namespace Server.Items
{
    public class UmbrascaleCeremonialDress : BaseOuterTorso
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public UmbrascaleCeremonialDress()
            : base(0xB2B9)
        {
            Name = "Umbrascale Ceremonial Dress";
            Hue = 0x0ADD;
            Weight = 3.0;
            LootType = LootType.Blessed;

            SAAbsorptionAttributes.EaterFire = 10;
            Attributes.BonusInt = 4;
            Attributes.BonusHits = 2;
            Attributes.SpellDamage = 10;
            Attributes.LowerManaCost = 5;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public UmbrascaleCeremonialDress(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
