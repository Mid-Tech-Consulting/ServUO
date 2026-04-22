using System;
using Server.Items;
using Server.Spells;
using Server.Spells.Necromancy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public class ShadowlordNecroAI : NecroMageAI
    {
        public ShadowlordNecroAI(BaseCreature m) : base(m) { }

        public override Spell GetRandomCurseSpell()
        {
            Spell spell = base.GetRandomCurseSpell();

            if (spell is BloodOathSpell)
                return new EvilOmenSpell(m_Mobile, null);

            return spell;
        }
    }


    public enum ShadowlordType
    {
        Astaroth,
        Faulinei,
        Nosfentor        
    };
    
    [CorpseName("a shadowlord corpse")]
    public class Shadowlord : BaseCreature
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances { get { return m_Instances; } }

        private ShadowlordType m_Type;
        public virtual Type[] ArtifactDrops { get { return _ArtifactTypes; } }

        private Type[] _ArtifactTypes = new Type[]
        {
            typeof(Abhorrence),         typeof(CaptainJohnesBlade),             typeof(Craven),
            typeof(Equivocation),       typeof(GargishCaptainJohnesBlade),      typeof(GargishEquivocation),
            typeof(GargishPincer),      typeof(Pincer),                         typeof(Hellspire),
            typeof(GargishHellspire)
        };

        [CommandProperty(AccessLevel.GameMaster)]
        public ShadowlordType Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public Shadowlord()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);

            m_Type = (ShadowlordType)Utility.Random(3);
            Name = m_Type.ToString();

            Body = 146;
            BaseSoundID = 0x4B0;

            SetStr(981, 1078);
            SetDex(1003, 1114);
            SetInt(1098, 1245);

            SetHits(50000, 55000);
            SetStam(1003, 1114);

            SetDamage(25, 30);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 70, 80);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Magery, 120.0);
            SetSkill(SkillName.Meditation, 140.0);
            SetSkill(SkillName.MagicResist, 110.2, 120.0);
            SetSkill(SkillName.Tactics, 110.1, 115.0);
            SetSkill(SkillName.Wrestling, 110.1, 115.0);
			SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);
            SetSkill(SkillName.Anatomy, 10.0, 20.0);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 20;
            Hue = 902;
            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();

            SetSpecialAbility(SpecialAbility.LifeDrain);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("#{0}", 1154453 + (int)m_Type); // Shadowlord of ..
        }

        public Shadowlord(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override bool AlwaysMurderer { get { return true; } }

        protected override BaseAI ForcedAI { get { return new ShadowlordNecroAI(this); } }

        public override int GetAngerSound() { return 1550; }
        public override int GetHurtSound() { return 1552; }
        public override int GetDeathSound() { return 1551; }
        
        public class InternalSelfDeleteTimer : Timer
        {
            private Shadowlord Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(180))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((Shadowlord)p);
            }
            protected override void OnTick()
            {
                if (Mare.Map != Map.Internal)
                {
                    Mare.Delete();
                    Stop();
                }
            }
        }

        public static Shadowlord Spawn(Point3D platLoc, Map platMap)
        {
            if (m_Instances.Count > 0)
                return null;

            Shadowlord creature = new Shadowlord();
            creature.Home = platLoc;
            creature.RangeHome = 4;
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 6);
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.HighScrolls, 2);
        }

        public override void OnDrainLife(Mobile victim)
        {
            if (Map == null)
                return;

            ArrayList list = new ArrayList();
            int count = 0;
            IPooledEnumerable eable = GetMobilesInRange(20);

            foreach (Mobile m in eable)
            {
                if (m is DarkWisp)
                {
                    count++;
                    continue;
                }

                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            eable.Free();

            if (count < 4 && list.Count > 0)
            {
                (new DarkWisp()).MoveToWorld(new Point3D(Location), Map);
            }

            foreach (Mobile m in list)
            {
                double teleportchance = (double)Hits / HitsMax;

                if (teleportchance < Utility.RandomDouble() && m.Alive)
                {
                    m.MoveToWorld(new Point3D(Location), Map);
                }
            }
        }

        public override void OnDeath(Container c)
        {
            List<DamageStore> rights = GetLootingRights();

            foreach (DamageStore ds in rights.Where(s => s.m_HasRight))
            {
                int luck = ds.m_Mobile is PlayerMobile ? ((PlayerMobile)ds.m_Mobile).RealLuck : ds.m_Mobile.Luck;
                double chance =
                    luck <= 500 ? 0.05 :
                    luck <= 1000 ? 0.10 :
                    luck <= 2000 ? 0.15 :
                    luck <= 3000 ? 0.20 : 0.25;

                if (Utility.RandomDouble() < chance)
                {
                    Mobile m = ds.m_Mobile;
                    Item artifact = Loot.Construct(ArtifactDrops[Utility.Random(ArtifactDrops.Length)]);

                    if (artifact != null)
                    {
                        if (m.Backpack == null || !m.Backpack.TryDropItem(m, artifact, false))
                        {
                            m.BankBox.DropItem(artifact);
                            m.SendMessage("For your valor in combating the fallen beast, a special reward has been placed in your bankbox.");
                        }
                        else
                            m.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special reward has been bestowed on you.
                    }
                }
            }

            // Drop 2 guaranteed Legendary items into the corpse regardless of luck.
            if (Core.HS && RandomItemGenerator.Enabled)
            {
                for (int i = 0; i < 2; i++)
                {
                    Item legendary = Core.AOS
                        ? Loot.RandomArmorOrShieldOrWeaponOrJewelry()
                        : Loot.RandomArmorOrShieldOrWeapon();

                    if (legendary == null)
                        continue;

                    if (RunicReforging.GenerateRandomArtifactItem(legendary, 0, Utility.RandomMinMax(1250, 1300)))
                        c.DropItem(legendary);
                    else
                        legendary.Delete();
                }
            }

            base.OnDeath(c);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)m_Type);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Type = (ShadowlordType)reader.ReadInt();

                        break;
                    }
            }

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }
}
