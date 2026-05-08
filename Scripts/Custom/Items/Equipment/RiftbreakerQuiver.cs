using System;

using Server;

namespace Server.Items
{
    public class RiftbreakerQuiver : BaseQuiver
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public RiftbreakerQuiver()
            : base(0x2B02)
        {
            Name = "Riftbreaker Quiver";
            Hue = 0x0AD7;
            Weight = 8.0;
            LootType = LootType.Blessed;

            Capacity = 1000;
            DamageIncrease = 10;       // Archery damage modifier
            LowerAmmoCost = 30;
            WeightReduction = 30;

            Attributes.BonusDex = 3;
            Attributes.BonusStam = 5;
            Attributes.WeaponSpeed = 5;
        }

        public RiftbreakerQuiver(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
