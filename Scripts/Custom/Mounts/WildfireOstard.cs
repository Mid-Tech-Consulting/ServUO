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
            Hue = 1175;

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

            // Rare color rolls — independent chances for special hues.
            if (Utility.Random(250) == 0) Hue = 2048;
            if (Utility.Random(500) == 0) Hue = 2206;
            if (Utility.Random(100) == 0) Hue = 2216;
            if (Utility.Random(1000) == 0) Hue = 2210;
            if (Utility.Random(1000) == 0) Hue = 2228;
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
