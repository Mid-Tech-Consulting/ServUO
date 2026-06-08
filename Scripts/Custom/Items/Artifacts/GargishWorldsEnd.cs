using System;
using Server;

namespace Server.Items
{
    public class GargishWorldsEnd : GargishApron
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishWorldsEnd() : base()
        {
            Name = "World's End";
            Hue = 18;
            LootType = LootType.Regular;

            Attributes.BonusMana = 10;
            Attributes.BonusStam = 10;
            Attributes.BonusInt = 5;
            Attributes.SpellDamage = 15;
            Attributes.AttackChance = 10;
            Attributes.RegenMana = 3;
            Attributes.LowerManaCost = 5;

            SkillBonuses.SetValues(0, SkillName.EvalInt, 10.0);
        }

        public GargishWorldsEnd(Serial serial) : base(serial)
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
