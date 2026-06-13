using System;
using Server;

namespace Server.Items
{
    public class FabeledSash : BodySash
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public FabeledSash()
        {
            Name = "Fabled Sash";
            Hue = 68;
            LootType = LootType.Regular;

            Attributes.AttackChance = 5;
            Attributes.DefendChance = 5;
            Attributes.BonusStam = 5;
            Attributes.LowerManaCost = 5;

            SkillBonuses.SetValues(0, SkillName.Chivalry, 5.0);
        }

        public FabeledSash(Serial serial) : base(serial)
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
