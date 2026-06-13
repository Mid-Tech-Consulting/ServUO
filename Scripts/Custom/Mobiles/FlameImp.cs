using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a flame imp corpse")]
    public class FlameImp : BaseCreature
    {
        [Constructable]
        public FlameImp() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a flame imp";
            Body = 74; // Imp body
            BaseSoundID = 0x1A0;
            Hue = 1260; // Fiery red

            SetStr(100, 125);
            SetDex(100, 150);
            SetInt(100, 150);

            SetHits(80, 100);

            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 100);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Fire, 90, 100);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.EvalInt, 70.1, 80.0);
            SetSkill(SkillName.Magery, 70.1, 85.0);
            SetSkill(SkillName.MagicResist, 70.1, 85.0);
            SetSkill(SkillName.Tactics, 60.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 1500;
            Karma = -1500;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public FlameImp(Serial serial) : base(serial)
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
        }
    }
}
