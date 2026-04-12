using System;
using Server.Gumps;
using Server.Network;

namespace Server.Mobiles
{
    // =========================================================
    //  Shared base — handles all player/pet damage immunity
    //  and 2-minute inactivity auto-delete
    // =========================================================
    public abstract class BaseTrainingCreature : BaseCreature
    {
        private static readonly TimeSpan InactivityTimeout = TimeSpan.FromMinutes(2);

        private DateTime _lastAttackedTime;
        private InactivityTimer _inactivityTimer;

        protected BaseTrainingCreature(AIType ai, FightMode mode, int range, int irange, double passiveSpeed, double activeSpeed)
            : base(ai, mode, range, irange, passiveSpeed, activeSpeed)
        {
            CantWalk = true;
            GuardImmune = true;
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

            SetDamage(1, 1);

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

            SetDamage(1, 1);

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

            SetDamage(1, 1);

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

            SetDamage(1, 1);

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
    //  Training Master NPC — double-click to open creature menu
    // =========================================================
    public class TrainingMaster : BaseCreature
    {
        [Constructable]
        public TrainingMaster()
            : base(AIType.AI_Vendor, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Training Master";
            Title = "the training master";
            Body = 400;
            CantWalk = true;
            Blessed = true;
        }

        public TrainingMaster(Serial serial) : base(serial) { }

        public override bool IsInvulnerable => true;
        public override bool ClickTitle => true;

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(Location, 4))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.CloseGump(typeof(TrainingGump));
            from.SendGump(new TrainingGump(this));
        }

        public void SpawnTrainingCreature(Mobile from, int type)
        {
            BaseCreature creature;

            switch (type)
            {
                case 0: creature = new TrainingElemental(); break;
                case 1: creature = new TrainingHumanFighter(); break;
                case 2: creature = new TrainingOrcWarrior(); break;
                case 3: creature = new TrainingGargoyle(); break;
                default: return;
            }

            creature.MoveToWorld(new Point3D(X + 2, Y, Z), Map);
            from.SendMessage("A training creature has been summoned!");
        }

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
            AddLabel(58, 87, 0, "Human Fighter");

            AddButton(20, 115, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(58, 117, 0, "Orc Warrior");

            AddButton(20, 145, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddLabel(58, 147, 0, "Gargoyle");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null || from.Deleted || _master == null || _master.Deleted)
                return;

            if (!from.InRange(_master.Location, 4))
            {
                from.SendLocalizedMessage(500446);
                return;
            }

            switch (info.ButtonID)
            {
                case 1: _master.SpawnTrainingCreature(from, 0); break; // Elemental
                case 2: _master.SpawnTrainingCreature(from, 1); break; // Human Fighter
                case 3: _master.SpawnTrainingCreature(from, 2); break; // Orc Warrior
                case 4: _master.SpawnTrainingCreature(from, 3); break; // Gargoyle
            }
        }
    }
}
