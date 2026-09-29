using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a halloweave spider corpse")]
    public class HalloweaveSpider : BaseCreature, IHalloweenCemeteryCreature
    {
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Regular; } }

        [Constructable]
        public HalloweaveSpider() : base(AIType.AI_Necro, FightMode.Closest, 10, 1, 0.3, 0.3)
        {
            Name = "Halloweave Spider";
            Body = 173;
            BaseSoundID = 0x183; // spider chitter

            SetStr(150, 300);
            SetDex(200, 300);
            SetInt(560, 620);

            SetHits(600, 800);
            SetStam(200, 300);
            SetMana(200);

            SetDamage(15, 30);

            SetDamageType(ResistanceType.Cold, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 40, 55);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 70, 90);
            SetResistance(ResistanceType.Energy, 20, 35);

            SetSkill(SkillName.Necromancy, 80.0, 110.0);
            SetSkill(SkillName.SpiritSpeak, 85.0, 110.0);
            SetSkill(SkillName.MagicResist, 80.0, 100.0);
            SetSkill(SkillName.Tactics, 85.0, 100.0);
            SetSkill(SkillName.Wrestling, 95.0, 110.0);
            SetSkill(SkillName.Poisoning, 10.0, 30.0);

            Fame = 6000;
            Karma = -6000;

            VirtualArmor = 70;

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 65.0;

            SetWeaponAbility(WeaponAbility.BleedAttack);

            ApplyHalloweenHue();
        }

        private void ApplyHalloweenHue()
        {
            double roll = Utility.RandomDouble();

            if (roll < 0.05) // 5% ultra-rare
            {
                switch (Utility.Random(3))
                {
                    case 0: Hue = 1161; break; // Candy Corn Orange
                    case 1: Hue = 2129; break; // Slime Green
                    case 2: Hue = 1175; break; // Spectral Purple
                }
                Name = "a halloweave stalker";
            }
            else if (roll < 0.20) // 15% uncommon
            {
                switch (Utility.Random(3))
                {
                    case 0: Hue = 1358; break; // Vivid Pumpkin
                    case 1: Hue = 1272; break; // Toxic Green
                    case 2: Hue = 1153; break; // Spider Silk White
                }
            }
            else // Common
            {
                Hue = 1108; // Charcoal
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.50)
                c.DropItem(new BlackberryPumpkinPie() { Amount = Utility.RandomMinMax(1, 3) });

            if (Utility.RandomDouble() < 0.10)
                c.DropItem(new HalloweenEventBag());
        }

        public override void GenerateLoot()
        {
            if (!Controlled)
            {
                AddLoot(LootPack.Rich);
                AddLoot(LootPack.MedScrolls);
            }
        }

        public override bool CanAngerOnTame { get { return false; } }

        public HalloweaveSpider(Serial serial) : base(serial)
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
            SetWeaponAbility(WeaponAbility.BleedAttack);
        }
    }
}
