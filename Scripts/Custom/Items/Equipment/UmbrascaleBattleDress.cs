using System;

using Server;

namespace Server.Items
{
    public class UmbrascaleBattleDress : BaseOuterTorso
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public UmbrascaleBattleDress()
            : base(0xB2B9)
        {
            Name = "Umbrascale Battle Dress";
            Hue = 0x0ADD;
            Weight = 3.0;
            LootType = LootType.Blessed;

            SAAbsorptionAttributes.EaterFire = 10;
            Attributes.BonusStr = 5;
            Attributes.BonusHits = 2;
            Attributes.BonusStam = 5;
            Attributes.WeaponSpeed = 5;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public UmbrascaleBattleDress(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
