using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a halloween dragon corpse")]
    public class HalloweenDragon : BaseCreature
    {
        public override double HealChance { get { return 1.0; } }

        [Constructable]
        public HalloweenDragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Halloween Dragon";
            Body = Utility.RandomList(12, 59);
            BaseSoundID = 362;
            Hue = 1161; // Glowing Blaze Orange

            SetStr(796, 825);
            SetDex(86, 105);
            SetInt(436, 475);

            SetHits(550, 650);

            SetDamage(20, 26);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 60);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Anatomy, 100.0);
            SetSkill(SkillName.Healing, 100.0);
            SetSkill(SkillName.MagicResist, 85.0);
            SetSkill(SkillName.Tactics, 110.0);
            SetSkill(SkillName.Wrestling, 90.0);
            SetSkill(SkillName.Magery, 50.0);
            SetSkill(SkillName.EvalInt, 50.0);
            SetSkill(SkillName.Meditation, 50.0);

            Fame = 18000;
            Karma = -18000;

            VirtualArmor = 60;
            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 100.0;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.Heal);
            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public HalloweenDragon(Serial serial) : base(serial)
        {
        }

        public override bool CanFly { get { return true; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override TrainingDefinition TrainingDefinition
        {
            get
            {
                return new TrainingDefinition(
                    GetType(),
                    Class.MagicalClawedAndTailed,
                    MagicalAbility.Cusidhe,
                    PetTrainingHelper.SpecialAbilityBaneDragon,
                    PetTrainingHelper.WepAbility2,
                    PetTrainingHelper.AreaEffectArea2,
                    3,
                    5);
            }
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
