using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    // =========================================================
    //  Shared base — handles all player/pet damage immunity
    //  and 2-minute inactivity auto-delete
    // =========================================================
    public abstract class BaseTrainingCreature : BaseCreature
    {
        private static readonly TimeSpan InactivityTimeout = TimeSpan.FromMinutes(1);

        private DateTime _lastAttackedTime;
        private InactivityTimer _inactivityTimer;

        public TrainingMaster Master { get; set; }
        public Mobile Summoner { get; set; }

        protected BaseTrainingCreature(AIType ai, FightMode mode, int range, int irange, double passiveSpeed, double activeSpeed)
            : base(ai, mode, range, irange, passiveSpeed, activeSpeed)
        {
            CantWalk = true;
            GuardImmune = true;
        }

        public BaseTrainingCreature(Serial serial) : base(serial) { }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            Master?.RemoveCreature(this);
        }

        public override bool AutoDispel => true;
        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 1;
        public override Poison PoisonImmune => Poison.Deadly;
        public override bool BreathImmune => true;

        // Called whenever the creature is placed on a real map —
        // covers both fresh spawns and server restarts after deserialization.
        protected override void OnMapChange(Map oldMap)
        {
            base.OnMapChange(oldMap);

            if (Map != null && Map != Map.Internal)
            {
                _inactivityTimer?.Stop();
                _lastAttackedTime = DateTime.UtcNow;
                _inactivityTimer = new InactivityTimer(this);
                _inactivityTimer.Start();
            }
        }

        // Reset the inactivity clock whenever a player (or their pet) swings at us.
        private void RegisterAttack(Mobile from)
        {
            if (from is PlayerMobile || (from is BaseCreature bc && (bc.Controlled || bc.BardTarget == this)))
                _lastAttackedTime = DateTime.UtcNow;
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (!(attacker is PlayerMobile player))
                return;

            // 15% chance per hit to attempt a stat gain, bypassing the global stat timer
            // so dedicated training actually produces results.
            if (Utility.RandomDouble() >= 0.15)
                return;

            BaseWeapon weapon = player.Weapon as BaseWeapon;
            if (weapon == null)
                return;

            SkillInfo info = player.Skills[weapon.Skill]?.Info;
            if (info == null)
                return;

            bool primaryUp   = GetStatLock(player, (SkillCheck.Stat)info.Primary)   == StatLockType.Up;
            bool secondaryUp = GetStatLock(player, (SkillCheck.Stat)info.Secondary) == StatLockType.Up;

            if (primaryUp && secondaryUp)
            {
                // Mirror TryStatGain: 75% primary, 25% secondary
                SkillCheck.IncreaseStat(player, Utility.Random(4) == 0
                    ? (SkillCheck.Stat)info.Secondary
                    : (SkillCheck.Stat)info.Primary);
            }
            else if (primaryUp)
                SkillCheck.IncreaseStat(player, (SkillCheck.Stat)info.Primary);
            else if (secondaryUp)
                SkillCheck.IncreaseStat(player, (SkillCheck.Stat)info.Secondary);
        }

        private static StatLockType GetStatLock(Mobile m, SkillCheck.Stat stat)
        {
            switch (stat)
            {
                case SkillCheck.Stat.Str: return m.StrLock;
                case SkillCheck.Stat.Dex: return m.DexLock;
                case SkillCheck.Stat.Int: return m.IntLock;
                default: return StatLockType.Locked;
            }
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            RegisterAttack(from);

            if (from is BaseCreature creature && (creature.Controlled || creature.BardTarget == this))
                damage = 0;
            else if (from is PlayerMobile)
                damage = 0;
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            RegisterAttack(caster);

            if (caster is BaseCreature c && c.GetMaster() is PlayerMobile)
                scalar = 0.0;
            else if (caster is PlayerMobile)
                scalar = 0.0;
        }

        public override void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            RegisterAttack(from);

            if (from is BaseCreature creature && creature.GetMaster() is PlayerMobile)
                damage = 0;
            else if (from is PlayerMobile)
                damage = 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
            writer.Write(Master);
            writer.Write(Summoner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                Master = reader.ReadMobile() as TrainingMaster;
                Summoner = reader.ReadMobile();
            }
        }

        // ---- Inactivity timer ----
        private class InactivityTimer : Timer
        {
            private readonly BaseTrainingCreature _creature;

            public InactivityTimer(BaseTrainingCreature creature)
                : base(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30))
            {
                _creature = creature;
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                if (_creature.Deleted)
                {
                    Stop();
                    return;
                }

                if (DateTime.UtcNow - _creature._lastAttackedTime >= InactivityTimeout)
                {
                    _creature.Delete();
                    Stop();
                }
            }
        }
    }

    // =========================================================
    //  Training Elemental  (original creature, renamed)
    // =========================================================
    [CorpseName("a training elemental corpse")]
    public class TrainingElemental : BaseTrainingCreature
    {
        [Constructable]
        public TrainingElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a training elemental";
            Body = 111;
            BaseSoundID = 268;
            Hue = 1910;

            SetStr(226, 255);
            SetDex(126, 145);
            SetInt(71, 92);

            SetHits(136, 153);

            SetDamage(0, 0);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 50.1, 95.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 4500;
            Karma = -4500;
        }

        public TrainingElemental(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    // =========================================================
    //  Training Human Fighter
    // =========================================================
    [CorpseName("a training fighter corpse")]
    public class TrainingHumanFighter : BaseTrainingCreature
    {
        [Constructable]
        public TrainingHumanFighter()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a training fighter";
            Body = 400;
            BaseSoundID = 0x45A;

            SetStr(176, 200);
            SetDex(96, 115);
            SetInt(36, 55);

            SetHits(106, 120);

            SetDamage(0, 0);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 45.1, 75.0);
            SetSkill(SkillName.Swords, 60.1, 100.0);
            SetSkill(SkillName.Tactics, 60.1, 100.0);

            Fame = 3000;
            Karma = -3000;
        }

        public TrainingHumanFighter(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    // =========================================================
    //  Training Orc Warrior
    // =========================================================
    [CorpseName("a training orc corpse")]
    public class TrainingOrcWarrior : BaseTrainingCreature
    {
        [Constructable]
        public TrainingOrcWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a training orc warrior";
            Body = 17;
            BaseSoundID = 0x45A;

            SetStr(196, 225);
            SetDex(81, 105);
            SetInt(36, 60);

            SetHits(118, 135);

            SetDamage(0, 0);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.1, 75.0);
            SetSkill(SkillName.Tactics, 60.1, 95.0);
            SetSkill(SkillName.Wrestling, 55.1, 90.0);

            Fame = 3500;
            Karma = -3500;
        }

        public TrainingOrcWarrior(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    // =========================================================
    //  Training Gargoyle
    // =========================================================
    [CorpseName("a training gargoyle corpse")]
    public class TrainingGargoyle : BaseTrainingCreature
    {
        [Constructable]
        public TrainingGargoyle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a training gargoyle";
            Body = 4;
            BaseSoundID = 372;

            SetStr(146, 175);
            SetDex(76, 95);
            SetInt(81, 105);

            SetHits(88, 105);

            SetDamage(0, 0);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 5, 10);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 50.1, 85.0);
            SetSkill(SkillName.Tactics, 55.1, 80.0);
            SetSkill(SkillName.Wrestling, 50.1, 85.0);

            Fame = 3500;
            Karma = -3500;
        }

        public TrainingGargoyle(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    // =========================================================
    //  Training Golem
    // =========================================================
    [CorpseName("a training golem corpse")]
    public class TrainingGolem : BaseTrainingCreature
    {
        [Constructable]
        public TrainingGolem()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            Name = "a training golem";
            Body = 752;

            SetStr(226, 255);
            SetDex(76, 100);
            SetInt(101, 125);

            SetHits(136, 153);

            SetDamage(0, 0);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 100);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 10, 25);
            SetResistance(ResistanceType.Energy, 30, 45);

            SetSkill(SkillName.MagicResist, 60.0, 100.0);
            SetSkill(SkillName.Tactics, 60.0, 100.0);
            SetSkill(SkillName.Wrestling, 60.1, 100.0);

            Fame = 3500;
            Karma = -3500;
        }

        public TrainingGolem(Serial serial) : base(serial) { }

        public override int GetAngerSound() => 541;
        public override int GetDeathSound() => 545;
        public override int GetAttackSound() => 562;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    // =========================================================
    //  Training Master NPC — double-click to open creature menu
    // =========================================================
    public class TrainingMaster : BaseCreature
    {
        private const int VendorCreatureLimit = 6;

        private static readonly TimeSpan GreetCooldown = TimeSpan.FromSeconds(60);
        private readonly Dictionary<Mobile, DateTime> _greeted = new Dictionary<Mobile, DateTime>();
        private readonly List<BaseTrainingCreature> _activeCreatures = new List<BaseTrainingCreature>();

        public void RemoveCreature(BaseTrainingCreature creature)
        {
            _activeCreatures.Remove(creature);
        }

        [Constructable]
        public TrainingMaster()
            : base(AIType.AI_Vendor, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Training Master";
            Body = 400;
            CantWalk = true;
            CanMove = false;
            Blessed = true;

            AddItem(new Server.Items.HoodedShroudOfShadows());
        }

        public TrainingMaster(Serial serial) : base(serial) { }

        public override bool IsInvulnerable => true;
        public override bool ClickTitle => true;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (!(m is PlayerMobile) || !m.InRange(Location, 5) || Utility.InRange(oldLocation, Location, 5))
                return;

            if (_greeted.TryGetValue(m, out DateTime last) && DateTime.UtcNow - last < GreetCooldown)
                return;

            _greeted[m] = DateTime.UtcNow;
            Say("Hail warrior! Double click me to choose your sparring partner.");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(Location, 8))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.CloseGump(typeof(TrainingGump));
            from.SendGump(new TrainingGump(this));
        }

        public void SpawnTrainingCreature(Mobile from, int type)
        {
            _activeCreatures.RemoveAll(c => c.Deleted);

            if (_activeCreatures.Count >= VendorCreatureLimit)
            {
                from.SendMessage("The training area is at capacity. Please wait for a creature to become available.");
                return;
            }

            if (_activeCreatures.Exists(c => c.Summoner == from))
            {
                from.SendMessage("You already have a training creature active. Defeat it before summoning another.");
                return;
            }

            BaseTrainingCreature creature;

            switch (type)
            {
                case 0: creature = new TrainingElemental(); break;
                case 1: creature = new TrainingHumanFighter(); break;
                case 2: creature = new TrainingOrcWarrior(); break;
                case 3: creature = new TrainingGargoyle(); break;
                case 4: creature = new TrainingGolem(); break;
                default: return;
            }

            creature.Master = this;
            creature.Summoner = from;
            _activeCreatures.Add(creature);

            creature.MoveToWorld(from.Location, from.Map);
            from.SendMessage("A training creature has been summoned!");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            _activeCreatures.RemoveAll(c => c.Deleted);
            writer.Write(_activeCreatures.Count);
            foreach (BaseTrainingCreature c in _activeCreatures)
                writer.Write(c);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
            {
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    if (reader.ReadMobile() is BaseTrainingCreature c && !c.Deleted)
                        _activeCreatures.Add(c);
                }
            }
        }
    }

    // =========================================================
    //  Training Gump — creature selection menu
    // =========================================================
    public class TrainingGump : Gump
    {
        private readonly TrainingMaster _master;

        public TrainingGump(TrainingMaster master) : base(150, 150)
        {
            _master = master;

            AddPage(0);
            AddBackground(0, 0, 230, 210, 9200);
            AddLabel(55, 15, 1153, "Choose Training Opponent");

            AddButton(20, 55,  4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(58, 57, 0, "Elemental");

            AddButton(20, 85,  4005, 4007, 2, GumpButtonType.Reply, 0);
            AddLabel(58, 87, 0, "Orc Warrior");

            AddButton(20, 115, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(58, 117, 0, "Gargoyle");

            AddButton(20, 145, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(58, 147, 0, "Golem");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || from.Deleted || _master == null || _master.Deleted)
                return;

            if (!from.InRange(_master.Location, 8))
            {
                from.SendLocalizedMessage(500446);
                return;
            }

            switch (info.ButtonID)
            {
                case 1: _master.SpawnTrainingCreature(from, 0); break; // Elemental
                case 2: _master.SpawnTrainingCreature(from, 2); break; // Orc Warrior
                case 3: _master.SpawnTrainingCreature(from, 3); break; // Gargoyle
                case 4: _master.SpawnTrainingCreature(from, 4); break; // Golem
            }
        }
    }
}
