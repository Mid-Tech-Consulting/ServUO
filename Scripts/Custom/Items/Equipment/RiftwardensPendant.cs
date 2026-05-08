using System;

using Server;

namespace Server.Items
{
    public class RiftwardensPendant : BaseNecklace
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public RiftwardensPendant()
            : base(0x9E2B)
        {
            Name = "Riftwarden's Pendant";
            Hue = 0x0AE1;
            Weight = 1.0;
            LootType = LootType.Blessed;

            Attributes.BonusInt = 2;
            Attributes.BonusMana = 8;
            Attributes.SpellDamage = 8;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public RiftwardensPendant(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
