using System;

namespace Server.Items
{
    public class TalismanOfTheMystic : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheMystic()
            : base(0x2F58)
        {
            Name = "Talisman of the Mystic";
            Hue = 0x07B7;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
            SkillBonuses.SetValues(2, SkillName.Mysticism, 5.0);
            SkillBonuses.SetValues(3, SkillName.Focus, 5.0);

            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 50;
        }

        public TalismanOfTheMystic(Serial serial)
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

    public class TalismanOfTheNecromancer : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheNecromancer()
            : base(0x2F59)
        {
            Name = "Talisman of the Necromancer";
            Hue = 0x07B7;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
            SkillBonuses.SetValues(2, SkillName.Necromancy, 5.0);
            SkillBonuses.SetValues(3, SkillName.SpiritSpeak, 5.0);

            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 50;
        }

        public TalismanOfTheNecromancer(Serial serial)
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

    public class TalismanOfTheSpellweaver : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheSpellweaver()
            : base(0x2F5A)
        {
            Name = "Talisman of the Spellweaver";
            Hue = 0x07B7;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
            SkillBonuses.SetValues(2, SkillName.Spellweaving, 5.0);
            SkillBonuses.SetValues(3, SkillName.Alchemy, 5.0);

            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 50;
        }

        public TalismanOfTheSpellweaver(Serial serial)
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

    public class TalismanOfThePureMage : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfThePureMage()
            : base(0x2F5B)
        {
            Name = "Talisman of the Pure Mage";
            Hue = 0x07B7;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
            SkillBonuses.SetValues(2, SkillName.Inscribe, 5.0);
            SkillBonuses.SetValues(3, SkillName.Alchemy, 5.0);

            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 50;
        }

        public TalismanOfThePureMage(Serial serial)
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
