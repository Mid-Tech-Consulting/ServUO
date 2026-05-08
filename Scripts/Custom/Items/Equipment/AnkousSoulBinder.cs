using System;

using Server;

namespace Server.Items
{
    public class AnkousSoulBinder : BodySash
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public AnkousSoulBinder()
        {
            Name = "Ankou's Soul Binder";
            Hue = 0x0AD7;
            Weight = 1.0;
            LootType = LootType.Blessed;

            Attributes.BonusMana = 2;
            Attributes.CastSpeed = 1;
            Attributes.LowerManaCost = 8;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public AnkousSoulBinder(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
