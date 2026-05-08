using System;

using Server;

namespace Server.Items
{
    public class VoidskipperBoots : Boots
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public VoidskipperBoots()
        {
            Name = "Voidskipper Boots";
            Hue = 0x0AD7;
            Weight = 3.0;
            LootType = LootType.Blessed;

            Attributes.NightSight = 1;
            Attributes.Luck = 200;
            Attributes.LowerManaCost = 3;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public VoidskipperBoots(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
