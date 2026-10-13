using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a storm of akalabeth corpse")]
    public class StormOfAkalabeth : BaseMount
    {
        public static readonly int[] StandardHues = new int[]
        {
            1373, // Purple / Lavender
            2052, // Slate / Greenish Charcoal
            2722, // Deep Violet / Green
            1755, // Plum / Violet
            2776  // Dark Teal / Cyan
        };

        public static readonly int[] UberRareHues = new int[]
        {
            2753, // Uber Rare: Fiery Gold / Lava
            2075  // Uber Rare: Blood Flame / Deep Crimson
        };

        public static int RollRandomHue(out bool isUberRare)
        {
            int roll = Utility.Random(100); // 0 to 99

            if (roll == 0) // 1%
            {
                isUberRare = true;
                return UberRareHues[0]; // 2753
            }
            if (roll == 1) // 1%
            {
                isUberRare = true;
                return UberRareHues[1]; // 2075
            }

            isUberRare = false;
            return StandardHues[Utility.Random(StandardHues.Length)];
        }

        public static bool IsUberRareHue(int hue)
        {
            return hue == 2753 || hue == 2075;
        }

        [Constructable]
        public StormOfAkalabeth() : this("a storm of akalabeth")
        {
        }

        [Constructable]
        public StormOfAkalabeth(string name)
            : base(name, 0x0679, 0x3EE0, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0xA8;
            BodyValue = 1657; // 0x0679 - Horse_Elemental_Air

            bool uber;
            Hue = RollRandomHue(out uber);

            SetStr(455, 465);
            SetDex(175, 185);
            SetInt(295, 305);

            SetHits(405, 415);
            SetStam(150);
            SetMana(300);

            SetDamage(18, 24);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 0);
            SetDamageType(ResistanceType.Cold, 0);
            SetDamageType(ResistanceType.Poison, 0);
            SetDamageType(ResistanceType.Energy, 100); // 100% Energy Damage

            SetResistance(ResistanceType.Physical, 60);
            SetResistance(ResistanceType.Fire, 40);
            SetResistance(ResistanceType.Cold, 40);
            SetResistance(ResistanceType.Poison, 50);
            SetResistance(ResistanceType.Energy, 80);

            SetSkill(SkillName.Wrestling, 80.0, 90.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.MagicResist, 80.0);
            SetSkill(SkillName.Anatomy, 50.0, 60.0);
            SetSkill(SkillName.DetectHidden, 60.0);
            SetSkill(SkillName.Meditation, 80.0, 90.0);
            SetSkill(SkillName.Focus, 95.0, 100.0);

            Fame = 15000;
            Karma = 0;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 88.0;

            SetSpecialAbility(SpecialAbility.ConductiveBlast);
        }

        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Equine;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
        }

        public StormOfAkalabeth(Serial serial) : base(serial)
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
