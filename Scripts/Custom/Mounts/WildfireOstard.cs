namespace Server.Mobiles
{
    [CorpseName("a wildfire ostard corpse")]
    public class WildfireOstard : BaseMount
    {
        [Constructable]
        public WildfireOstard()
            : this("a Wildfire Ostard")
        {
        }

        [Constructable]
        public WildfireOstard(string name)
            : base(name, 0xDA, 0x3EA4, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0x275;
            Hue = 0x0AC6;

            SetStr(540);
            SetDex(100);
            SetInt(150);

            SetHits(400);
            SetStam(100);
            SetMana(150);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 25);
            SetDamageType(ResistanceType.Cold, 25);

            SetResistance(ResistanceType.Physical, 65);
            SetResistance(ResistanceType.Fire, 45);
            SetResistance(ResistanceType.Cold, 40);
            SetResistance(ResistanceType.Poison, 55);
            SetResistance(ResistanceType.Energy, 35);

            SetSkill(SkillName.Wrestling, 97.6);
            SetSkill(SkillName.Tactics, 99.0);
            SetSkill(SkillName.Poisoning, 67.1);
            SetSkill(SkillName.MagicResist, 93.9);
            SetSkill(SkillName.Magery, 43.0);
            SetSkill(SkillName.EvalInt, 36.3);

            Fame = 10000;
            Karma = -10000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 96.0;

            // Color roll: rare overrides only. 56% stays the default 0x0AC6.
            int roll = Utility.Random(10000);
            if (roll < 100) Hue = 0x07B7;                         // 1% Red (ultra rare)
            else if (roll < 250) Hue = 1153;                       // 1.5% Luna White
            else if (roll < 400) Hue = 2406;                       // 1.5% Black
            else if (roll < 1200) Hue = 2048;                      // 8% existing
            else if (roll < 2000) Hue = 2206;                      // 8% existing
            else if (roll < 2800) Hue = 2216;                      // 8% existing
            else if (roll < 3600) Hue = 2210;                      // 8% existing
            else if (roll < 4400) Hue = 2228;                      // 8% existing
            // else: default 0x0AC6 unchanged (56%)
        }

        public WildfireOstard(Serial serial)
            : base(serial)
        {
        }

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.FruitsAndVegies; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Ostard; } }
        public override int Meat { get { return 4; } }
        public override int Hides { get { return 8; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
