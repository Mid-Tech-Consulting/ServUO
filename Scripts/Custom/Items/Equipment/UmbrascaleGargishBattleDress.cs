using System;

using Server;

namespace Server.Items
{
    public class UmbrascaleGargishBattleDress : BaseClothing
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public UmbrascaleGargishBattleDress()
            : base(0xB2BA, Layer.OuterTorso)
        {
            Name = "Umbrascale Gargish Battle Dress";
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

        public UmbrascaleGargishBattleDress(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
