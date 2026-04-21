using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a red dragon mount corpse")]
    public class DragonHildebrandt : BaseMount
    {
        [Constructable]
        public DragonHildebrandt()
            : this("a Dragon Hildebrandt")
        {
        }

        [Constructable]
        public DragonHildebrandt(string name)
            : base(name, 0x581, 0x3EDD, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.2)
        {
            BaseSoundID = 362;

            // Every Hildebrandt spawns with a unique hue in this range.
            Hue = Utility.RandomMinMax(1400, 1500);

            SetStr(396, 525);
            SetDex(286, 505);
            SetInt(286, 725);

            SetHits(648, 999);

            SetDamage(20, 48);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 55, 75);
            SetResistance(ResistanceType.Fire, 60, 77);
            SetResistance(ResistanceType.Cold, 20, 55);
            SetResistance(ResistanceType.Poison, 50, 80);
            SetResistance(ResistanceType.Energy, 30, 65);

            SetSkill(SkillName.EvalInt, 100.4, 120.0);
            SetSkill(SkillName.Magery, 90.4, 120.0);
            SetSkill(SkillName.MagicResist, 120.3, 140.0);
            SetSkill(SkillName.Tactics, 97.6, 130.0);
            SetSkill(SkillName.Wrestling, 80.5, 122.5);

            Fame = 14000;
            Karma = -14000;

            VirtualArmor = 80;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 100.1;

            switch (Utility.Random(12))
            {
                case 0: PackItem(new BloodOathScroll()); break;
                case 1: PackItem(new HorrificBeastScroll()); break;
                case 2: PackItem(new StrangleScroll()); break;
                case 3: PackItem(new VengefulSpiritScroll()); break;
            }

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public DragonHildebrandt(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 5;
        public override int Hides => 10;
        public override HideType HideType => HideType.Horned;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool CanAngerOnTame => true;

        public override void OnAfterTame(Mobile tamer)
        {
            if (Owners.Count == 0 && PetTrainingHelper.Enabled)
            {
                if (RawStr > 0)
                    RawStr = (int)Math.Max(1, RawStr * 0.5);

                if (RawDex > 0)
                    RawDex = (int)Math.Max(1, RawDex * 0.5);

                if (HitsMaxSeed > 0)
                    HitsMaxSeed = (int)Math.Max(1, HitsMaxSeed * 0.5);

                Hits = Math.Min(HitsMaxSeed, Hits);
                Stam = Math.Min(RawDex, Stam);
            }
            else
            {
                base.OnAfterTame(tamer);
            }
        }

        public override int GetAngerSound()
        {
            if (!Controlled)
                return 0x16A;

            return base.GetAngerSound();
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
            if (version < 1)
                ControlSlots = 3;
        }
    }
}
