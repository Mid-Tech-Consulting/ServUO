using System;
using Server;

namespace Server.Items
{
    public class DyingDiamonds : GoldEarrings
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DyingDiamonds()
        {
            Name = "Dying Diamonds";
            Hue = 33;
            LootType = LootType.Regular;

            Attributes.BonusInt = 10;
            Attributes.DefendChance = 10;
            Attributes.CastRecovery = 3;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 10;
            Attributes.EnhancePotions = 15;

            SkillName[] castSkills = new SkillName[] { SkillName.Magery, SkillName.Necromancy, SkillName.Mysticism, SkillName.Chivalry, SkillName.Spellweaving, SkillName.EvalInt };
            SkillBonuses.SetValues(0, castSkills[Utility.Random(castSkills.Length)], 10.0);
        }

        public DyingDiamonds(Serial serial) : base(serial)
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
