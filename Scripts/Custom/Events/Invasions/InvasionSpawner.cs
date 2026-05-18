using System;
using System.Collections.Generic;

using Server;
using Server.Custom.Events;
using Server.Mobiles;

namespace Server.Items
{
    // Generic area spawn controller. Concrete subclasses pin a theme so
    // [add UndeadInvasionSpawner / [add DemonInvasionSpawner / ... drop a
    // ready-themed spawner with no [props fiddling.
    //
    // - Active : on/off toggle. Off means no spawns, no upkeep.
    // - HomeRange : half-width of the spawn box (default 30 = 60x60 area).
    // - Tick interval is 60s; each tick the spawner walks its registered
    //   creatures and replaces any dead ones to bring totals back up to
    //   the theme's Population mix.
    //
    // Set Active = false and double-click to despawn everything alive.
    public abstract class InvasionSpawner : Item
    {
        public abstract InvasionTheme Theme { get; }

        private bool m_Active;
        private int m_HomeRange;
        private List<Mobile> m_Spawned;
        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                if (m_Active == value)
                    return;

                m_Active = value;

                if (m_Active)
                    StartTicking();
                else
                    StopTicking();

                // Tell the loot handler -- this is what gates whether kills
                // of this theme's mobs actually drop tokens.
                InvasionLoot.NotifyActive(Theme, m_Active);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HomeRange
        {
            get { return m_HomeRange; }
            set { m_HomeRange = Math.Max(5, Math.Min(60, value)); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LiveCount
        {
            get
            {
                Cleanup();
                return m_Spawned.Count;
            }
        }

        protected InvasionSpawner() : base(0x1F1C)
        {
            ItemID = Theme.SpawnerItemID;
            Hue = Theme.SpawnerHue;
            Name = Theme.Name + " Spawner";
            Movable = false;
            Weight = 50.0;

            m_HomeRange = 30;
            m_Spawned = new List<Mobile>();
            m_Active = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel < AccessLevel.GameMaster)
                return;

            int hue = Theme.SpawnerHue == 0 ? 0x47E : Theme.SpawnerHue;
            from.SendMessage(hue, String.Format(
                "{0} Spawner: Active={1}, HomeRange={2}, Live={3}",
                Theme.Name, m_Active, m_HomeRange, LiveCount));
            from.SendMessage(hue, "Use [props to toggle Active or change HomeRange. Set Active=false then double-click again to clear remaining spawns.");

            if (!m_Active && m_Spawned.Count > 0)
            {
                ClearSpawns();
                from.SendMessage(hue, "Cleared all live spawns from this controller.");
            }
        }

        private void StartTicking()
        {
            StopTicking();
            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60), Tick);
        }

        private void StopTicking()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        private void Tick()
        {
            if (Deleted || Map == null || Map == Map.Internal)
                return;
            if (!m_Active)
                return;

            Cleanup();

            Dictionary<Type, int> live = new Dictionary<Type, int>();
            foreach (Mobile m in m_Spawned)
            {
                Type t = m.GetType();
                int n;
                live.TryGetValue(t, out n);
                live[t] = n + 1;
            }

            foreach (KeyValuePair<Type, int> target in Theme.Population)
            {
                int have;
                live.TryGetValue(target.Key, out have);

                int missing = target.Value - have;
                for (int i = 0; i < missing; i++)
                    SpawnOne(target.Key);
            }
        }

        private void SpawnOne(Type t)
        {
            BaseCreature bc = Activator.CreateInstance(t) as BaseCreature;
            if (bc == null)
                return;

            Point3D loc = PickSpawnLocation();
            if (loc == Point3D.Zero)
            {
                bc.Delete();
                return;
            }

            bc.Home = loc;
            bc.RangeHome = m_HomeRange;

            // Roll for paragon conversion BEFORE MoveToWorld -- doing it
            // after places the creature in the world with its un-paragoned
            // OPL, and the suffix/hue refresh after InvalidateProperties is
            // unreliable for some bodies (e.g. LichLord). Setting IsParagon
            // here mirrors the standard OnBeforeSpawn flow.
            //
            // Fame bonus: +1% per 1000 Fame. Top-tier mobs (LichLord 18k,
            // SkeletalDragon 22.5k) get a meaningful boost so they actually
            // show up as paragons; trash mobs stay near the base rate.
            double chance = Theme.ParagonChance + (bc.Fame / 100000.0);
            if (chance > 0 && Utility.RandomDouble() < chance && bc.CanBeParagon)
                bc.IsParagon = true;

            bc.MoveToWorld(loc, Map);

            m_Spawned.Add(bc);
        }

        private Point3D PickSpawnLocation()
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                int x = X + Utility.RandomMinMax(-m_HomeRange, m_HomeRange);
                int y = Y + Utility.RandomMinMax(-m_HomeRange, m_HomeRange);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanSpawnMobile(x, y, z))
                    return new Point3D(x, y, z);
            }
            return Point3D.Zero;
        }

        private void Cleanup()
        {
            if (m_Spawned == null)
            {
                m_Spawned = new List<Mobile>();
                return;
            }
            m_Spawned.RemoveAll(m => m == null || m.Deleted);
        }

        private void ClearSpawns()
        {
            Cleanup();
            foreach (Mobile m in m_Spawned)
                m.Delete();
            m_Spawned.Clear();
        }

        public override void OnDelete()
        {
            base.OnDelete();
            StopTicking();
            ClearSpawns();

            // Drop our active-refcount contribution if we were running.
            if (m_Active)
                InvasionLoot.NotifyActive(Theme, false);
        }

        // Must be public so ServUO's serialization audit (which uses
        // public-only reflection) can find it on this abstract base.
        public InvasionSpawner(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_Active);
            writer.Write(m_HomeRange);
            writer.WriteMobileList(m_Spawned);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            m_Active = reader.ReadBool();
            m_HomeRange = reader.ReadInt();
            m_Spawned = new List<Mobile>(reader.ReadStrongMobileList());

            if (m_Active)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(5), StartTicking);
                // Rebuild the active-refcount across a world reload.
                InvasionLoot.NotifyActive(Theme, true);
            }
        }
    }

    public class UndeadInvasionSpawner : InvasionSpawner
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Undead; } }
        [Constructable] public UndeadInvasionSpawner() { }
        public UndeadInvasionSpawner(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class DemonInvasionSpawner : InvasionSpawner
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Demon; } }
        [Constructable] public DemonInvasionSpawner() { }
        public DemonInvasionSpawner(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ReptileInvasionSpawner : InvasionSpawner
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Reptile; } }
        [Constructable] public ReptileInvasionSpawner() { }
        public ReptileInvasionSpawner(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ArachnidInvasionSpawner : InvasionSpawner
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Arachnid; } }
        [Constructable] public ArachnidInvasionSpawner() { }
        public ArachnidInvasionSpawner(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class FeyInvasionSpawner : InvasionSpawner
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Fey; } }
        [Constructable] public FeyInvasionSpawner() { }
        public FeyInvasionSpawner(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class RepondInvasionSpawner : InvasionSpawner
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Repond; } }
        [Constructable] public RepondInvasionSpawner() { }
        public RepondInvasionSpawner(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
