using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a vampire corpse")]
    public class CemeteryVampire : BaseCreature, IHalloweenCemeteryCreature
    {
        [Constructable]
        public CemeteryVampire() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 1440;
            Hue = 1150; // Spectral Ghost White skin
            Title = "the Cemetery Vampire";

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                AddItem(new Skirt() { Hue = 1975, Movable = false });
                AddItem(new FancyShirt(1170) { Movable = false });
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new ShortPants(1170) { Movable = false });
                AddItem(new FancyShirt(1975) { Movable = false });
            }

            SetStr(250, 350);
            SetDex(150, 200);
            SetInt(250, 350);

            SetHits(700, 1000);

            SetDamage(16, 24);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Cold, 75, 85);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Swords, 90.0, 105.0);
            SetSkill(SkillName.Tactics, 90.0, 105.0);
            SetSkill(SkillName.Wrestling, 90.0, 105.0);
            SetSkill(SkillName.MagicResist, 95.0, 110.0);
            SetSkill(SkillName.Anatomy, 85.0, 100.0);

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 45;

            AddItem(new Boots(1) { Movable = false });
            AddItem(new Cloak(1157) { Movable = false });
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool BardImmune { get { return !Core.SE; } }
        public override bool Unprovokable { get { return Core.SE; } }
        public override bool Uncalmable { get { return Core.SE; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public void SpawnVampireBats(Mobile target)
        {
            Map map = Map;
            if (map == null)
                return;

            int count = Utility.RandomMinMax(1, 2);
            for (int i = 0; i < count; ++i)
            {
                VampireBat bat = new VampireBat();
                bat.Team = Team;
                bat.FightMode = FightMode.Closest;

                Point3D loc = Location;
                for (int j = 0; j < 10; ++j)
                {
                    int x = X + Utility.RandomMinMax(-2, 2);
                    int y = Y + Utility.RandomMinMax(-2, 2);
                    int z = map.GetAverageZ(x, y);

                    if (map.CanFit(x, y, Z, 16, false, false))
                    {
                        loc = new Point3D(x, y, Z);
                        break;
                    }
                }

                bat.MoveToWorld(loc, map);
                bat.Combatant = target;
            }
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            if (Utility.RandomDouble() < 0.15)
                SpawnVampireBats(caster);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Utility.RandomDouble() < 0.15)
                SpawnVampireBats(attacker);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (defender != null)
            {
                defender.Stam -= Utility.Random(8, 12);
                defender.Mana -= Utility.Random(8, 12);
                Hits = Math.Min(HitsMax, Hits + 10);
                PlaySound(0x1F2); // Vampire bite sound
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.25)
                c.DropItem(new HalloweenEventBag());

            if (Utility.RandomDouble() < 0.40)
                c.DropItem(new BlackberryPumpkinPie() { Amount = Utility.RandomMinMax(2, 4) });
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems, 4);
        }

        public CemeteryVampire(Serial serial) : base(serial)
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
