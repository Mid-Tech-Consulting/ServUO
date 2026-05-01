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
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;

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
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;
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
            SkillBonuses.SetValues(3, SkillName.Wrestling, 5.0);
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;

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
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;

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

    public class TalismanOfTheBokutoMage : BaseTalisman
    {
        public override bool IsArtifact
        {
            get { return true; }
        }

        [Constructable]
        public TalismanOfTheBokutoMage()
            : base(0x2F5B)
        {
            Name = "Talisman of the Bokuto Mage";
            Hue = 0x07B7;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
            SkillBonuses.SetValues(2, SkillName.Bushido, 5.0);
            SkillBonuses.SetValues(3, SkillName.Swords, 5.0);
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;

            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 50;
        }

        public TalismanOfTheBokutoMage(Serial serial)
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

    public class TalismanOfTheStealthMage : BaseTalisman
    {
        public override bool IsArtifact
        {
            get { return true; }
        }

        [Constructable]
        public TalismanOfTheStealthMage()
            : base(0x2F5B)
        {
            Name = "Talisman of the Stealth Mage";
            Hue = 0x07B7;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
            SkillBonuses.SetValues(2, SkillName.Stealth, 5.0);
            SkillBonuses.SetValues(3, SkillName.Hiding, 5.0);
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;

            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 50;
        }

        public TalismanOfTheStealthMage(Serial serial)
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

    public class TalismanOfTheTamerMage : BaseTalisman
    {
        public override bool IsArtifact
        {
            get { return true; }
        }

        [Constructable]
        public TalismanOfTheTamerMage()
            : base(0x2F5B)
        {
            Name = "Talisman of the Tamer Mage";
            Hue = 0x07B7;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Magery, 5.0);
            SkillBonuses.SetValues(1, SkillName.EvalInt, 5.0);
            SkillBonuses.SetValues(2, SkillName.AnimalTaming, 5.0);
            SkillBonuses.SetValues(3, SkillName.AnimalLore, 5.0);
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;

            Attributes.CastSpeed = 1;
            Attributes.SpellDamage = 50;
        }

        public TalismanOfTheTamerMage(Serial serial)
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

    // ============================================================
    // Dexer talismans — universal pair: Anatomy 5 + Tactics 5.
    // Bonuses: 10 HCI / 10 DCI / 30 SSI / 50 DI on every variant
    // (parallel to the mage pool's 10 HCI / 10 DCI / 1 FC / 50 SDI).
    // Hue 1153 distinguishes them visually from the mage pool.
    // ============================================================

    public class TalismanOfTheWarrior : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheWarrior()
            : base(0x2F58)
        {
            Name = "Talisman of the Warrior";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Swords, 5.0);
            SkillBonuses.SetValues(3, SkillName.Healing, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheWarrior(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheBrute : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheBrute()
            : base(0x2F59)
        {
            Name = "Talisman of the Brute";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Macing, 5.0);
            SkillBonuses.SetValues(3, SkillName.Healing, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheBrute(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheFencer : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheFencer()
            : base(0x2F5A)
        {
            Name = "Talisman of the Fencer";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Fencing, 5.0);
            SkillBonuses.SetValues(3, SkillName.Healing, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheFencer(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheMarksman : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheMarksman()
            : base(0x2F5B)
        {
            Name = "Talisman of the Marksman";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Archery, 5.0);
            SkillBonuses.SetValues(3, SkillName.Healing, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheMarksman(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheSkirmisher : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheSkirmisher()
            : base(0x2F58)
        {
            Name = "Talisman of the Skirmisher";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Throwing, 5.0);
            SkillBonuses.SetValues(3, SkillName.Healing, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheSkirmisher(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheSamurai : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheSamurai()
            : base(0x2F59)
        {
            Name = "Talisman of the Samurai";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Bushido, 5.0);
            SkillBonuses.SetValues(3, SkillName.Parry, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheSamurai(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheNinja : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheNinja()
            : base(0x2F5A)
        {
            Name = "Talisman of the Ninja";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Ninjitsu, 5.0);
            SkillBonuses.SetValues(3, SkillName.Stealth, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheNinja(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheCrusader : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheCrusader()
            : base(0x2F5B)
        {
            Name = "Talisman of the Crusader";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Chivalry, 5.0);
            SkillBonuses.SetValues(3, SkillName.Healing, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheCrusader(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheDeathKnight : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheDeathKnight()
            : base(0x2F58)
        {
            Name = "Talisman of the Death Knight";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Necromancy, 5.0);
            SkillBonuses.SetValues(3, SkillName.SpiritSpeak, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheDeathKnight(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheMysticWarrior : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheMysticWarrior()
            : base(0x2F59)
        {
            Name = "Talisman of the Mystic Warrior";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Mysticism, 5.0);
            SkillBonuses.SetValues(3, SkillName.Focus, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheMysticWarrior(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class TalismanOfTheSpellsword : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TalismanOfTheSpellsword()
            : base(0x2F5A)
        {
            Name = "Talisman of the Spellsword";
            Hue = 1153;
            Weight = 1.0;

            SkillBonuses.SetValues(0, SkillName.Anatomy, 5.0);
            SkillBonuses.SetValues(1, SkillName.Tactics, 5.0);
            SkillBonuses.SetValues(2, SkillName.Magery, 5.0);
            SkillBonuses.SetValues(3, SkillName.Swords, 5.0);
            Attributes.AttackChance = 10;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public TalismanOfTheSpellsword(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
