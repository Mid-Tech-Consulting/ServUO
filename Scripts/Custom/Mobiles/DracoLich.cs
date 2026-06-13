using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a skeletal dragon corpse")]
    public class DracoLich : BaseCreature
    {
        [Constructable]
        public DracoLich()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a draco lich";
            Body = 104;
            BaseSoundID = 0x488;
            Hue = 2480;

            SetStr(698, 730);
            SetDex(128, 200);
            SetInt(488, 620);

            SetHits(699, 799);

            SetDamage(24, 32);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 70, 75);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 55, 60);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 50, 65);

            SetSkill(SkillName.EvalInt, 100.1, 110.0);
            SetSkill(SkillName.Magery, 100.1, 110.0);
            SetSkill(SkillName.MagicResist, 100.3, 130.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 107.6, 115.0);
            SetSkill(SkillName.Necromancy, 100.1, 110.0);
            SetSkill(SkillName.SpiritSpeak, 100.1, 110.0);

            Fame = 22500;
            Karma = -22500;

            VirtualArmor = 85;
            
            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 144.9;
        }

        public DracoLich(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel { get { return !Controlled; } }
        public override bool BleedImmune { get { return true; } }
        public override bool ReacquireOnMovement { get { return !Controlled; } }
        public override double BonusPetDamageScalar { get { return (Core.SE) ? 3.0 : 1.0; } }
        public override int Hides { get { return 20; } }
        public override int Meat { get { return 19; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override TribeType Tribe { get { return TribeType.Undead; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 1);
            AddLoot(LootPack.Gems, 5);
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
                return 115.0;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 45.0;
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
