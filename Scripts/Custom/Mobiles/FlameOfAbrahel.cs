using System;

using Server;
using Server.Items;

namespace Server.Mobiles
{
    // Tameable mount: starts as a 3-slot fire-themed horse, can be trained
    // up to 5 slots via the standard pet training gump. Three magical-school
    // training paths available: Bushido, Ninjitsu, Chivalry (bit-flagged so
    // the player picks).
    [CorpseName("a flame of abrahel corpse")]
    public class FlameOfAbrahel : BaseMount
    {
        // Per-creature override -- PetTrainingHelper.GetTrainingDefinition
        // checks this before the central Definitions table, so we keep the
        // training spec right next to the mob. Slots 3-5 means "starts at 3,
        // can be trained up to 5".
        public override TrainingDefinition TrainingDefinition
        {
            get
            {
                return new TrainingDefinition(
                    typeof(FlameOfAbrahel),
                    Class.Magical,
                    MagicalAbility.Bushido | MagicalAbility.Ninjitsu | MagicalAbility.Chivalry,
                    PetTrainingHelper.SpecialAbilityNone,
                    PetTrainingHelper.WepAbilityNone,
                    PetTrainingHelper.AreaEffectNone,
                    3,  // ControlSlotsMin -- starts as a 3-slot tame
                    5); // ControlSlotsMax -- player can train it up to 5 slots
            }
        }

        [Constructable]
        public FlameOfAbrahel()
            : this("Flame Of Abrahel")
        {
        }

        [Constructable]
        public FlameOfAbrahel(string name)
            : base(name, 0xB2, 0x3EA0, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0xA8;
            Hue = 0x04EB;

            SetStr(700);
            SetDex(180);
            SetInt(400);

            SetHits(560);
            SetStam(150);
            SetMana(400);

            SetDamage(16, 30);

            // Pure fire damage type
            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 100);

            SetResistance(ResistanceType.Physical, 70);
            SetResistance(ResistanceType.Fire, 85);
            SetResistance(ResistanceType.Cold, 50);
            SetResistance(ResistanceType.Poison, 70);
            SetResistance(ResistanceType.Energy, 70);

            SetSkill(SkillName.Wrestling, 90.0, 100.0);
            SetSkill(SkillName.Tactics, 90.0, 100.0);
            SetSkill(SkillName.MagicResist, 95.0, 100.0);
            SetSkill(SkillName.DetectHidden, 50.0, 100.0);
            SetSkill(SkillName.Focus, 50.0, 100.0);

            Fame = 18000;
            Karma = -18000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 94.0;

            // Barding difficulty 101.1: roughly Hits-based on the standard
            // formula; setting Hits 560 already lands here naturally, but
            // explicit fame/karma above keeps OSI-style display consistent.

            // Pet training: caster pet that the owner can develop along the
            // Bushido, Ninjitsu, and Chivalry skill paths via the training
            // gump. Bit-flagged so all three tracks are available.
            SetMagicalAbility(MagicalAbility.Bushido | MagicalAbility.Ninjitsu | MagicalAbility.Chivalry);
        }

        public FlameOfAbrahel(Serial serial)
            : base(serial)
        {
        }

        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Equine; } }

        public override int Meat { get { return 5; } }
        public override int Hides { get { return 12; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
