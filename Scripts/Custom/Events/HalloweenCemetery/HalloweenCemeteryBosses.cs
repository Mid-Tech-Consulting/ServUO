using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles
{
    #region Cemetery Vampire Lord (Champion Boss)
    [CorpseName("a vampire lord corpse")]
    public class CemeteryVampireLord : BaseCreature, IHalloweenCemeteryCreature
    {
        private DateTime m_NextBloodNova;
        private DateTime m_NextBatSwarm;
        private DateTime m_NextTeleport;

        [Constructable]
        public CemeteryVampireLord() : base(AIType.AI_NecroMage, FightMode.Closest, 12, 1, 0.2, 0.4)
        {
            Name = "Count Vladislaus";
            Title = "the Cemetery Vampire Lord";
            Body = 0x190;
            Hue = 1150; // Ghostly Spectral Pale
            BaseSoundID = 1440;

            SetStr(800, 1000);
            SetDex(200, 260);
            SetInt(900, 1100);

            SetHits(14000, 16000);
            SetMana(1000);
            SetStam(250);

            SetDamage(22, 32);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 35, 45); // Weak to fire
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 75, 85);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 110.0, 120.0);
            SetSkill(SkillName.Magery, 110.0, 120.0);
            SetSkill(SkillName.Necromancy, 115.0, 125.0);
            SetSkill(SkillName.SpiritSpeak, 110.0, 120.0);
            SetSkill(SkillName.Meditation, 100.0, 110.0);
            SetSkill(SkillName.MagicResist, 115.0, 125.0);
            SetSkill(SkillName.Tactics, 110.0, 120.0);
            SetSkill(SkillName.Wrestling, 110.0, 120.0);
            SetSkill(SkillName.Anatomy, 100.0, 115.0);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 65;

            // Spooky Vampire Lord Attire
            AddItem(new Cloak(1) { Movable = false }); // Pitch black cloak
            AddItem(new FancyShirt(1175) { Movable = false }); // Spectral purple shirt
            AddItem(new LongPants(1) { Movable = false });
            AddItem(new ThighBoots(1170) { Movable = false });

            // Glowing Vampire Longsword
            Longsword sword = new Longsword();
            sword.Hue = 1157; // Blood Crimson
            sword.Movable = false;
            AddItem(sword);

            m_NextBloodNova = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            m_NextBatSwarm = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextTeleport = DateTime.UtcNow + TimeSpan.FromSeconds(8);
        }

        public CemeteryVampireLord(Serial serial) : base(serial)
        {
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool BardImmune { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool BleedImmune { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null || !Alive || Deleted || Map == null)
                return;

            DateTime now = DateTime.UtcNow;

            // 1. Teleport Strike
            if (now >= m_NextTeleport && 0.3 > Utility.RandomDouble())
            {
                m_NextTeleport = now + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 16));
                DoTeleportStrike();
            }

            // 2. Blood Nova
            if (now >= m_NextBloodNova)
            {
                m_NextBloodNova = now + TimeSpan.FromSeconds(Utility.RandomMinMax(18, 25));
                DoBloodNova();
            }

            // 3. Bat Swarm
            if (now >= m_NextBatSwarm)
            {
                m_NextBatSwarm = now + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 35));
                DoBatSwarm();
            }
        }

        private void DoTeleportStrike()
        {
            Mobile target = Combatant as Mobile;
            if (target == null || !target.Alive || target.Map != Map || !InRange(target, 12))
                return;

            Point3D targetLoc = target.Location;
            for (int i = 0; i < 10; ++i)
            {
                int x = targetLoc.X + Utility.RandomMinMax(-1, 1);
                int y = targetLoc.Y + Utility.RandomMinMax(-1, 1);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanFit(x, y, z, 16, false, false))
                {
                    // Smoke effect at old position
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 15, 1150, 0, 5023, 0);
                    PlaySound(0x379);

                    MoveToWorld(new Point3D(x, y, z), Map);

                    // Smoke effect at new position
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 15, 1150, 0, 5023, 0);
                    PlaySound(0x1FE);

                    Say(1049532, "Blood calls to blood!");
                    break;
                }
            }
        }

        private void DoBloodNova()
        {
            PlaySound(0x1F7);
            FixedParticles(0x3709, 10, 30, 5052, 1157, 0, EffectLayer.LeftFoot);

            Say("*unleashes a swirling tempest of cursed blood!*");

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 8);
            int drainTotal = 0;

            foreach (Mobile m in eable)
            {
                if (m != this && CanBeHarmful(m, false) && (m.Player || (m is BaseCreature && ((BaseCreature)m).Controlled)))
                {
                    int damage = Utility.RandomMinMax(30, 50);
                    AOS.Damage(m, this, damage, 40, 0, 60, 0, 0);
                    m.FixedParticles(0x374A, 10, 15, 5013, 1157, 0, EffectLayer.Waist);
                    drainTotal += damage / 2;
                }
            }
            eable.Free();

            if (drainTotal > 0 && Hits < HitsMax)
            {
                Hits = Math.Min(HitsMax, Hits + drainTotal);
                PlaySound(0x202);
            }
        }

        private void DoBatSwarm()
        {
            PlaySound(0x19B);
            Say("*summons a voracious swarm of vampire bats!*");

            int count = Utility.RandomMinMax(2, 3);
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
                    int z = Map.GetAverageZ(x, y);

                    if (Map.CanFit(x, y, z, 16, false, false))
                    {
                        loc = new Point3D(x, y, z);
                        break;
                    }
                }

                bat.MoveToWorld(loc, Map);
                if (Combatant != null)
                    bat.Combatant = Combatant;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.HighScrolls, 3);
            AddLoot(LootPack.Gems, 8);
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

            m_NextBloodNova = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextBatSwarm = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextTeleport = DateTime.UtcNow + TimeSpan.FromSeconds(10);
        }
    }
    #endregion

    #region The Pumpkin King (Champion Boss)
    [CorpseName("the pumpkin king's remains")]
    public class ThePumpkinKing : BaseCreature, IHalloweenCemeteryCreature
    {
        private DateTime m_NextPumpkinBarrage;
        private DateTime m_NextVineEntangle;
        private DateTime m_NextSummonFiends;

        [Constructable]
        public ThePumpkinKing() : base(AIType.AI_Mage, FightMode.Closest, 12, 1, 0.2, 0.4)
        {
            Name = "Lord Hallow";
            Title = "the Pumpkin King";
            Body = 26; // Giant Scarecrow
            Hue = 1161; // Glowing Blaze Orange
            BaseSoundID = 0x47D;

            SetStr(900, 1100);
            SetDex(180, 240);
            SetInt(800, 1000);

            SetHits(15000, 18000);
            SetMana(1000);
            SetStam(250);

            SetDamage(24, 35);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 60);

            SetResistance(ResistanceType.Physical, 70, 80);
            SetResistance(ResistanceType.Fire, 80, 90); // Very resistant to fire
            SetResistance(ResistanceType.Cold, 35, 45); // Weak to cold
            SetResistance(ResistanceType.Poison, 75, 85);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetSkill(SkillName.EvalInt, 115.0, 125.0);
            SetSkill(SkillName.Magery, 115.0, 125.0);
            SetSkill(SkillName.Meditation, 105.0, 115.0);
            SetSkill(SkillName.MagicResist, 120.0, 130.0);
            SetSkill(SkillName.Tactics, 115.0, 125.0);
            SetSkill(SkillName.Wrestling, 115.0, 125.0);
            SetSkill(SkillName.Anatomy, 100.0, 110.0);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 70;

            m_NextPumpkinBarrage = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            m_NextVineEntangle = DateTime.UtcNow + TimeSpan.FromSeconds(16);
            m_NextSummonFiends = DateTime.UtcNow + TimeSpan.FromSeconds(25);
        }

        public ThePumpkinKing(Serial serial) : base(serial)
        {
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool BardImmune { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool BleedImmune { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null || !Alive || Deleted || Map == null)
                return;

            DateTime now = DateTime.UtcNow;

            // 1. Flaming Pumpkin Barrage
            if (now >= m_NextPumpkinBarrage)
            {
                m_NextPumpkinBarrage = now + TimeSpan.FromSeconds(Utility.RandomMinMax(14, 20));
                DoPumpkinBarrage();
            }

            // 2. Vine Entangle
            if (now >= m_NextVineEntangle)
            {
                m_NextVineEntangle = now + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30));
                DoVineEntangle();
            }

            // 3. Summon Pumpkin Fiends
            if (now >= m_NextSummonFiends)
            {
                m_NextSummonFiends = now + TimeSpan.FromSeconds(Utility.RandomMinMax(35, 50));
                DoSummonFiends();
            }
        }

        private void DoPumpkinBarrage()
        {
            Say("*hurls exploding flaming pumpkins across the graveyard!*");
            PlaySound(0x208);

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 10);
            foreach (Mobile m in eable)
            {
                if (m != this && CanBeHarmful(m, false) && (m.Player || (m is BaseCreature && ((BaseCreature)m).Controlled)))
                {
                    MovingParticles(m, 0xF88, 7, 0, false, true, 1161, 0, 9502, 4019, 0x160, 0);
                    int damage = Utility.RandomMinMax(25, 45);
                    Timer.DelayCall(TimeSpan.FromSeconds(0.5), () =>
                    {
                        if (m != null && m.Alive && !m.Deleted)
                        {
                            AOS.Damage(m, this, damage, 30, 70, 0, 0, 0);
                            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x36BD, 15, 20, 1161, 0, 5044, 0);
                            m.PlaySound(0x307);
                        }
                    });
                }
            }
            eable.Free();
        }

        private void DoVineEntangle()
        {
            Say("*thorny pumpkin vines erupt from the cold graves!*");
            PlaySound(0x524);

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 8);
            foreach (Mobile m in eable)
            {
                if (m != this && CanBeHarmful(m, false) && (m.Player || (m is BaseCreature && ((BaseCreature)m).Controlled)))
                {
                    m.Paralyze(TimeSpan.FromSeconds(4));
                    m.FixedParticles(0x374A, 10, 20, 5017, 1168, 0, EffectLayer.Waist);
                    m.SendMessage(0x22, "Cursed pumpkin vines wrap tightly around your legs, holding you fast!");
                }
            }
            eable.Free();
        }

        private void DoSummonFiends()
        {
            Say("*Awaken, my pumpkin fiends! Feed on their fear!*");
            PlaySound(0x218);

            for (int i = 0; i < 2; ++i)
            {
                CemeteryPumpkinFiend fiend = new CemeteryPumpkinFiend();
                fiend.Team = Team;
                fiend.FightMode = FightMode.Closest;

                Point3D loc = Location;
                for (int j = 0; j < 10; ++j)
                {
                    int x = X + Utility.RandomMinMax(-3, 3);
                    int y = Y + Utility.RandomMinMax(-3, 3);
                    int z = Map.GetAverageZ(x, y);

                    if (Map.CanFit(x, y, z, 16, false, false))
                    {
                        loc = new Point3D(x, y, z);
                        break;
                    }
                }

                fiend.MoveToWorld(loc, Map);
                if (Combatant != null)
                    fiend.Combatant = Combatant;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.HighScrolls, 3);
            AddLoot(LootPack.Gems, 8);
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

            m_NextPumpkinBarrage = DateTime.UtcNow + TimeSpan.FromSeconds(15);
            m_NextVineEntangle = DateTime.UtcNow + TimeSpan.FromSeconds(20);
            m_NextSummonFiends = DateTime.UtcNow + TimeSpan.FromSeconds(30);
        }
    }
    #endregion
}
