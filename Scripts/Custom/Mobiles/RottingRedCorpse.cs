using System;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a rotting corpse")]
    public class RottingRedCorpse : BaseCreature
    {
        [Constructable]
        public RottingRedCorpse()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rotting red corpse";
            Body = 155; 
            BaseSoundID = 471;
            Hue = 33;

            SetStr(301, 350);
            SetDex(100);
            SetInt(151, 200);

            SetHits(500);
            SetStam(150);
            SetMana(0);

            SetDamage(8, 10);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Poison, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.Poisoning, 130.0);
            SetSkill(SkillName.MagicResist, 150.0);
            SetSkill(SkillName.Tactics, 110.0);
            SetSkill(SkillName.Wrestling, 100.1, 110.0);

            Fame = 6000;
            Karma = -6000;

            VirtualArmor = 50;
            
            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 144.9;
        }

        public RottingRedCorpse(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override double DispelDifficulty
        {
            get
            {
                return 100.0;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 35.0;
            }
        }

        public override TribeType Tribe { get { return TribeType.Undead; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
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
