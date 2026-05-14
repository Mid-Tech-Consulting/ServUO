using System;

using Server;

namespace Server.Items
{
    // Gargoyle counterpart to ArtiosVineWrap. Same stats, gargoyle sash slot.
    public class GargishArtiosVineWrap : GargishSash
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishArtiosVineWrap()
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

        public GargishArtiosVineWrap(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
