using System;
using Server;

namespace Server.Items
{
    public class BookOfBeastlyDesire : Spellbook
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1149779; } } // Corgul's Handbook on Mysticism

        [Constructable]
        public BookOfBeastlyDesire() : base()
        {
            Name = "Book of Beastly Desire";
            Hue = 98;
            LootType = LootType.Regular;

            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 4;
            Attributes.LowerManaCost = 15;
            Attributes.DefendChance = 15;
            Attributes.LowerRegCost = 15;
            Attributes.SpellDamage = 20;
            Attributes.BonusMana = 20;
        }

        public BookOfBeastlyDesire(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
