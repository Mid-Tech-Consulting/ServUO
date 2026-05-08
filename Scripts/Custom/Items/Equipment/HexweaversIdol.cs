using System;

using Server;

namespace Server.Items
{
    public class HexweaversIdol : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public HexweaversIdol()
            : base(0x2F59)
        {
            Name = "Hexweaver's Idol";
            Hue = 0x0AE1;
            Weight = 1.0;
            LootType = LootType.Blessed;

            Removal = TalismanRemoval.Ward;

            Attributes.BonusHits = 3;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 10;
            Attributes.SpellDamage = 8;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public HexweaversIdol(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
