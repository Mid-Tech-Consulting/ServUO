using System;

namespace Server.Items
{
    public class BoglingHideMukluks : Boots
    {
        public override bool IsArtifact { get { return true; } }

        // Human pool: no Throwing (gargoyle-only skill).
        private static readonly SkillName[] m_RandomSkills =
        {
            SkillName.Wrestling,
            SkillName.Swords,
            SkillName.Macing,
            SkillName.Fencing,
            SkillName.Archery,
            SkillName.Bushido,
            SkillName.Chivalry,
            SkillName.Ninjitsu,
        };

        [Constructable]
        public BoglingHideMukluks()
            : base(0x03FA)
        {
            Name = "Bogling Hide Mukluks";
            Weight = 1.0;
            LootType = LootType.Blessed;

            StrRequirement = 10;
            MaxHitPoints = 255;
            HitPoints = 255;

            Attributes.BonusStr = 10;
            Attributes.BonusDex = 10;
            Attributes.BonusInt = 10;
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;

            SkillBonuses.SetValues(0, m_RandomSkills[Utility.Random(m_RandomSkills.Length)], 10.0);
        }

        public BoglingHideMukluks(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishBoglingHideMukluks : LeatherTalons
    {
        public override bool IsArtifact { get { return true; } }

        // Gargoyle pool: no Archery (humans/elves only).
        private static readonly SkillName[] m_RandomSkills =
        {
            SkillName.Wrestling,
            SkillName.Swords,
            SkillName.Macing,
            SkillName.Fencing,
            SkillName.Throwing,
            SkillName.Bushido,
            SkillName.Chivalry,
            SkillName.Ninjitsu,
        };

        [Constructable]
        public GargishBoglingHideMukluks()
        {
            Name = "Gargish Bogling Hide Mukluks";
            Weight = 1.0;
            LootType = LootType.Blessed;

            StrRequirement = 10;
            MaxHitPoints = 255;
            HitPoints = 255;

            Attributes.BonusStr = 10;
            Attributes.BonusDex = 10;
            Attributes.BonusInt = 10;
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;

            SkillBonuses.SetValues(0, m_RandomSkills[Utility.Random(m_RandomSkills.Length)], 10.0);
        }

        public GargishBoglingHideMukluks(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
