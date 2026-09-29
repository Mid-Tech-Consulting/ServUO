using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    public interface IHalloweenCemeteryCreature
    {
    }

    #region Cemetery Pumpkin Fiend
    [CorpseName("a pumpkin fiend corpse")]
    public class CemeteryPumpkinFiend : BaseCreature, IHalloweenCemeteryCreature
    {
        [Constructable]
        public CemeteryPumpkinFiend() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Cemetery Pumpkin Fiend";
            Body = 26; // Scarecrow
            Hue = 1161; // Glowing Blaze Orange
            BaseSoundID = 0x47D;

            SetStr(300, 400);
            SetDex(150, 200);
            SetInt(400, 550);

            SetHits(800, 1200);

            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 30);
            SetDamageType(ResistanceType.Fire, 70);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 75, 85);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 90.0, 105.0);
            SetSkill(SkillName.Magery, 95.0, 110.0);
            SetSkill(SkillName.MagicResist, 90.0, 110.0);
            SetSkill(SkillName.Tactics, 90.0, 100.0);
            SetSkill(SkillName.Wrestling, 85.0, 100.0);
            SetSkill(SkillName.Meditation, 90.0, 100.0);

            Fame = 12000;
            Karma = -12000;

            VirtualArmor = 50;
        }

        public CemeteryPumpkinFiend(Serial serial) : base(serial)
        {
        }

        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.MedScrolls, 2);
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
    #endregion

    #region Crypt Ghoul
    [CorpseName("a crypt ghoul corpse")]
    public class CryptGhoul : BaseCreature, IHalloweenCemeteryCreature
    {
        [Constructable]
        public CryptGhoul() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Crypt Ghoul";
            Body = 3; // Zombie / Ghoul
            Hue = 1168; // Witches' Brew Green
            BaseSoundID = 471;

            SetStr(180, 250);
            SetDex(90, 120);
            SetInt(60, 90);

            SetHits(400, 600);

            SetDamage(12, 18);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 70.0, 85.0);
            SetSkill(SkillName.Tactics, 80.0, 95.0);
            SetSkill(SkillName.Wrestling, 80.0, 95.0);
            SetSkill(SkillName.Poisoning, 80.0, 100.0);

            Fame = 6000;
            Karma = -6000;

            VirtualArmor = 40;
        }

        public CryptGhoul(Serial serial) : base(serial)
        {
        }

        public override Poison HitPoison { get { return Poison.Greater; } }
        public override double HitPoisonChance { get { return 0.6; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
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
    #endregion

    #region Jack-o'-Lantern Apparition
    [CorpseName("an apparition corpse")]
    public class JackOLanternApparition : BaseCreature, IHalloweenCemeteryCreature
    {
        [Constructable]
        public JackOLanternApparition() : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Jack-o'-Lantern Apparition";
            Body = 0x3CA; // Phantom
            Hue = 1161; // Glowing Blaze Orange
            BaseSoundID = 0x482;

            SetStr(220, 300);
            SetDex(120, 160);
            SetInt(350, 500);

            SetHits(500, 750);

            SetDamage(14, 20);

            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 30);
            SetDamageType(ResistanceType.Fire, 30);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 65, 75);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 55, 65);

            SetSkill(SkillName.EvalInt, 85.0, 100.0);
            SetSkill(SkillName.Magery, 85.0, 100.0);
            SetSkill(SkillName.Necromancy, 90.0, 105.0);
            SetSkill(SkillName.SpiritSpeak, 90.0, 100.0);
            SetSkill(SkillName.MagicResist, 80.0, 95.0);
            SetSkill(SkillName.Tactics, 80.0, 90.0);
            SetSkill(SkillName.Wrestling, 80.0, 90.0);

            Fame = 8000;
            Karma = -8000;

            VirtualArmor = 44;
        }

        public JackOLanternApparition(Serial serial) : base(serial)
        {
        }

        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
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
    #endregion
}
