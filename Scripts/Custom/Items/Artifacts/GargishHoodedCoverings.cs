using System;
using Server;

namespace Server.Items
{
    public class GargishHoodedCoverings : GargishRobe
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishHoodedCoverings() : this(43)
        {
        }

        [Constructable]
        public GargishHoodedCoverings(int hue) : base(hue)
        {
            Name = "Hooded Coverings";
            LootType = LootType.Regular;

            Attributes.BonusInt = 20;
            Attributes.SpellDamage = 15;
            Attributes.CastRecovery = 2;
            Attributes.CastSpeed = 1;
            Attributes.EnhancePotions = 30;
            Attributes.RegenMana = 2;
            Attributes.RegenHits = 2;
            Attributes.LowerManaCost = 5;

            SkillBonuses.SetValues(0, SkillName.Magery, 15.0);
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            from.SendLocalizedMessage(sender.FailMessage);
            return false;
        }

        public GargishHoodedCoverings(Serial serial) : base(serial)
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
