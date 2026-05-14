using System;

using Server;

namespace Server.Items
{
    // Mage sash: +5 mana, +2 mana regen, +1 FCR. Light str req (10).
    public class ArtiosVineWrap : BodySash
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ArtiosVineWrap()
        {
            Name = "Artio's Vine Wrap";
            Hue = 0x0497;
            Weight = 1.0;
            LootType = LootType.Regular;

            Attributes.BonusMana = 5;
            Attributes.RegenMana = 2;
            Attributes.CastRecovery = 1;

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public ArtiosVineWrap(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
