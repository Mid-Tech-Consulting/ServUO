using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom.Events.HalloweenCemetery
{
    public enum CemeteryWave
    {
        Wave1_RisingDead = 1,
        Wave2_CursedSwarm = 2,
        Wave3_CryptHorrors = 3,
        Wave4_Boss = 4,
        Cooldown = 5
    }

    public class HalloweenCemeterySpawner : Item
    {
        private string m_CemeteryName;
        private Rectangle2D m_Bounds;
        private CemeteryWave m_CurrentWave;
        private int m_KillsThisWave;
        private int m_MaxActiveMobs;
        private DateTime m_CooldownEnd;
        private DateTime m_LastKillTime;
        private List<Mobile> m_ActiveMobs;
        private Mobile m_ActiveBoss;
        private Timer m_SliceTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public string CemeteryName
        {
            get { return m_CemeteryName; }
            set { m_CemeteryName = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rectangle2D Bounds
        {
            get { return m_Bounds; }
            set { m_Bounds = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CemeteryWave CurrentWave
        {
            get { return m_CurrentWave; }
            set { SetWave(value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int KillsThisWave
        {
            get { return m_KillsThisWave; }
            set { m_KillsThisWave = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxActiveMobs
        {
            get { return m_MaxActiveMobs; }
            set { m_MaxActiveMobs = value; }
        }

        [Constructable]
        public HalloweenCemeterySpawner() : this("Cemetery", new Rectangle2D(0, 0, 0, 0))
        {
        }

        [Constructable]
        public HalloweenCemeterySpawner(string cemeteryName, Rectangle2D bounds) : base(0xED4)
        {
            Name = "Halloween Cemetery Altar";
            Hue = 1161; // Glowing Blaze Orange
            Movable = false;

            m_CemeteryName = cemeteryName;
            m_Bounds = bounds;
            m_CurrentWave = CemeteryWave.Wave1_RisingDead;
            m_KillsThisWave = 0;
            m_MaxActiveMobs = 50;
            m_ActiveMobs = new List<Mobile>();
            m_LastKillTime = DateTime.UtcNow;

            StartTimer();
        }

        public HalloweenCemeterySpawner(Serial serial) : base(serial)
        {
        }

        public int GetTargetActiveMobs()
        {
            int baseMax = m_MaxActiveMobs > 0 ? m_MaxActiveMobs : 50;
            switch (m_CurrentWave)
            {
                case CemeteryWave.Wave1_RisingDead: return baseMax;
                case CemeteryWave.Wave2_CursedSwarm: return (int)(baseMax * 0.9);
                case CemeteryWave.Wave3_CryptHorrors: return (int)(baseMax * 0.8);
                case CemeteryWave.Wave4_Boss: return Math.Min(15, (int)(baseMax * 0.25));
                default: return 0;
            }
        }

        public int GetRequiredKills(CemeteryWave wave)
        {
            switch (wave)
            {
                case CemeteryWave.Wave1_RisingDead: return 50;
                case CemeteryWave.Wave2_CursedSwarm: return 45;
                case CemeteryWave.Wave3_CryptHorrors: return 40;
                default: return 1;
            }
        }

        public string GetWaveName(CemeteryWave wave)
        {
            switch (wave)
            {
                case CemeteryWave.Wave1_RisingDead: return "The Rising Dead";
                case CemeteryWave.Wave2_CursedSwarm: return "The Cursed Swarm";
                case CemeteryWave.Wave3_CryptHorrors: return "The Crypt Horrors";
                case CemeteryWave.Wave4_Boss: return "Champion Encounter";
                case CemeteryWave.Cooldown: return "Restful Crypts";
                default: return "Active";
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(String.Format("<BASEFONT COLOR=#FF7700>{0}</BASEFONT>", m_CemeteryName));

            if (m_CurrentWave == CemeteryWave.Cooldown)
            {
                TimeSpan rem = m_CooldownEnd - DateTime.UtcNow;
                int mins = Math.Max(1, (int)rem.TotalMinutes);
                list.Add(String.Format("<BASEFONT COLOR=#A0A0A0>Status: Restful ({0}m remaining)</BASEFONT>", mins));
            }
            else if (m_CurrentWave == CemeteryWave.Wave4_Boss)
            {
                list.Add("<BASEFONT COLOR=#FF2222><b>CHAMPION BOSS ACTIVE!</b></BASEFONT>");
                if (m_ActiveBoss != null && m_ActiveBoss.Alive)
                    list.Add(String.Format("<BASEFONT COLOR=#FFA500>Boss: {0}</BASEFONT>", m_ActiveBoss.Name));
                list.Add(String.Format("<BASEFONT COLOR=#C0C0C0>Active Minions: {0}/{1}</BASEFONT>", m_ActiveMobs.Count, GetTargetActiveMobs()));
            }
            else
            {
                list.Add(String.Format("<BASEFONT COLOR=#FFA500>Wave {0}/3: {1}</BASEFONT>", (int)m_CurrentWave, GetWaveName(m_CurrentWave)));
                list.Add(String.Format("<BASEFONT COLOR=#FFFFFF>Progress: <b>{0}</b> / {1} kills</BASEFONT>", m_KillsThisWave, GetRequiredKills(m_CurrentWave)));
                list.Add(String.Format("<BASEFONT COLOR=#C0C0C0>Active Monsters: {0}/{1}</BASEFONT>", m_ActiveMobs.Count, GetTargetActiveMobs()));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 4))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (m_CurrentWave == CemeteryWave.Cooldown)
            {
                TimeSpan rem = m_CooldownEnd - DateTime.UtcNow;
                from.SendMessage(0x35, "The crypts of {0} are resting after the defeat of the Champion. The invasion resumes in {1} minutes.", m_CemeteryName, Math.Max(1, (int)rem.TotalMinutes));
            }
            else if (m_CurrentWave == CemeteryWave.Wave4_Boss)
            {
                string bName = m_ActiveBoss != null && m_ActiveBoss.Alive ? m_ActiveBoss.Name : "the Champion Boss";
                from.SendMessage(0x22, "DANGER! {0} has been awakened in {1}! Slay the Champion to claim immense Halloween spoils!", bName, m_CemeteryName);
            }
            else
            {
                int req = GetRequiredKills(m_CurrentWave);
                from.SendMessage(0x35, "{0} is currently in Wave {1}/3 ({2}). Slay {3} more monsters to summon the next wave!", 
                    m_CemeteryName, (int)m_CurrentWave, GetWaveName(m_CurrentWave), Math.Max(0, req - m_KillsThisWave));
            }
        }

        public void StartTimer()
        {
            if (m_SliceTimer != null)
                m_SliceTimer.Stop();

            m_SliceTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), OnSlice);
        }

        private void OnSlice()
        {
            if (Deleted || Map == null || Map == Map.Internal)
                return;

            // Prune dead or deleted mobs
            for (int i = m_ActiveMobs.Count - 1; i >= 0; i--)
            {
                Mobile m = m_ActiveMobs[i];
                if (m == null || m.Deleted || !m.Alive)
                    m_ActiveMobs.RemoveAt(i);
            }

            // Handle Cooldown state
            if (m_CurrentWave == CemeteryWave.Cooldown)
            {
                if (DateTime.UtcNow >= m_CooldownEnd)
                {
                    SetWave(CemeteryWave.Wave1_RisingDead);
                    BroadcastArea(0x35, "The mist deepens over {0}... The undead have risen once more! Wave 1: The Rising Dead has begun!", m_CemeteryName);
                    PlayAreaSound(0x5B6);
                }
                return;
            }

            // Handle Boss state
            if (m_CurrentWave == CemeteryWave.Wave4_Boss)
            {
                if (m_ActiveBoss == null || m_ActiveBoss.Deleted || !m_ActiveBoss.Alive)
                {
                    OnBossDefeated();
                }
                return;
            }

            // Inactivity decay: if no kills in 20 minutes and past wave 1, reset to wave 1
            if (m_CurrentWave > CemeteryWave.Wave1_RisingDead && (DateTime.UtcNow - m_LastKillTime) > TimeSpan.FromMinutes(20))
            {
                SetWave(CemeteryWave.Wave1_RisingDead);
                BroadcastArea(0x22, "The cemetery quietens down due to inactivity. The event resets to Wave 1.");
                return;
            }

            // Spawn up to TargetActiveMobs
            int targetMax = GetTargetActiveMobs();
            if (m_ActiveMobs.Count < targetMax)
            {
                int spawnCount = Math.Min(6, targetMax - m_ActiveMobs.Count);
                for (int i = 0; i < spawnCount; i++)
                {
                    SpawnNextCreature();
                }
                InvalidateProperties();
            }
        }

        public void OnMobKilled(BaseCreature bc, Mobile killer)
        {
            if (m_ActiveMobs.Contains(bc))
                m_ActiveMobs.Remove(bc);

            if (m_CurrentWave == CemeteryWave.Wave4_Boss || m_CurrentWave == CemeteryWave.Cooldown)
                return;

            m_KillsThisWave++;
            m_LastKillTime = DateTime.UtcNow;

            int req = GetRequiredKills(m_CurrentWave);

            if (m_KillsThisWave >= req)
            {
                AdvanceWave();
            }
            else if (m_KillsThisWave % 10 == 0 || (req - m_KillsThisWave) <= 5)
            {
                BroadcastArea(0x35, "[{0}] Wave {1} Progress: {2}/{3} kills ({4} remaining)", m_CemeteryName, (int)m_CurrentWave, m_KillsThisWave, req, Math.Max(0, req - m_KillsThisWave));
            }

            InvalidateProperties();
        }

        public void SetWave(CemeteryWave wave)
        {
            m_CurrentWave = wave;
            m_KillsThisWave = 0;
            m_LastKillTime = DateTime.UtcNow;
            InvalidateProperties();
        }

        public void AdvanceWave()
        {
            switch (m_CurrentWave)
            {
                case CemeteryWave.Wave1_RisingDead:
                    SetWave(CemeteryWave.Wave2_CursedSwarm);
                    BroadcastArea(0x35, "THE EARTH TREMBLES in {0}! Wave 2: The Cursed Swarm has emerged from their crypts!", m_CemeteryName);
                    PlayAreaSound(0x5B6);
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 1161, 0, 5052, 0);
                    break;

                case CemeteryWave.Wave2_CursedSwarm:
                    SetWave(CemeteryWave.Wave3_CryptHorrors);
                    BroadcastArea(0x22, "A BLOOD-RED FOG ROLLS OVER {0}! Wave 3: The Crypt Horrors have taken over the cemetery!", m_CemeteryName);
                    PlayAreaSound(0x2F3);
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 1157, 0, 5052, 0);
                    break;

                case CemeteryWave.Wave3_CryptHorrors:
                    SpawnBoss();
                    break;
            }
        }

        public void SpawnBoss()
        {
            SetWave(CemeteryWave.Wave4_Boss);

            // Cull down to 10 minion adds so the boss has center stage with minion escort
            while (m_ActiveMobs.Count > 10)
            {
                Mobile m = m_ActiveMobs[m_ActiveMobs.Count - 1];
                m_ActiveMobs.RemoveAt(m_ActiveMobs.Count - 1);
                if (m != null && !m.Deleted)
                    m.Delete();
            }

            // Spawn either Count Vladislaus or Lord Hallow
            BaseCreature boss;
            if (Utility.RandomBool())
                boss = new CemeteryVampireLord();
            else
                boss = new ThePumpkinKing();

            Point3D bossLoc = FindValidSpawnLocation();
            boss.Home = bossLoc;
            boss.RangeHome = 15;
            boss.MoveToWorld(bossLoc, Map);

            m_ActiveBoss = boss;

            // Dramatic entrance effects
            Effects.SendLocationParticles(EffectItem.Create(bossLoc, Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 1161, 0, 5052, 0);
            Effects.PlaySound(bossLoc, Map, 0x29); // Thunder
            Effects.PlaySound(bossLoc, Map, 0x208); // Fire explosion

            // Area and server notification
            string bossName = boss.Name + (boss.Title != null ? " " + boss.Title : "");
            BroadcastArea(0x22, "LIGHTNING RIPS THE NIGHT SKY! {0} HAS RISEN IN {1}!", bossName.ToUpper(), m_CemeteryName.ToUpper());

            InvalidateProperties();
        }

        private void OnBossDefeated()
        {
            string bName = m_ActiveBoss != null ? m_ActiveBoss.Name : "The Champion Boss";
            m_ActiveBoss = null;

            // Victory explosion & fanfare
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 10, 30, 1161, 0, 5030, 0);
            PlayAreaSound(0x5B4);

            BroadcastArea(0x35, "VICTORY! {0} has been slain in {1}! The spirits rest as peace returns to the crypts.", bName, m_CemeteryName);

            // Reward all participating players in the cemetery area
            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 35);
            foreach (Mobile m in eable)
            {
                if (m is PlayerMobile && m.Alive)
                {
                    PlayerMobile pm = (PlayerMobile)m;
                    int bonusPumpkins = Utility.RandomMinMax(80, 150);
                    pm.AddToBackpack(new HalloweenEventPumpkin(bonusPumpkins));
                    pm.SendMessage(0x35, "Champion Victory! You received {0} bonus Halloween Pumpkins!", bonusPumpkins);

                    // Chance for Halloween Event Bag
                    if (Utility.RandomDouble() < 0.60)
                    {
                        pm.AddToBackpack(new HalloweenEventBag());
                        pm.SendMessage(0x35, "A Halloween Event Goodie Bag has been placed in your backpack!");
                    }

                    // High chance for Rare Dye
                    if (Utility.RandomDouble() < 0.20)
                    {
                        Item dye = HalloweenCemeteryEvent.CreateRandomHalloweenDye();
                        if (dye != null)
                        {
                            pm.AddToBackpack(dye);
                            pm.SendMessage(0x35, "Spectacular! A rare {0} has appeared in your backpack!", dye.Name);
                        }
                    }

                    // Chance for Ethereal Orange Spider mount (5%)
                    if (Utility.RandomDouble() < 0.05)
                    {
                        pm.AddToBackpack(new EtherealOrangeSpider());
                        pm.SendMessage(0x35, "THE CRYPT TREMBLES! A legendary Ethereal Orange Spider mount has crawled into your backpack!");
                    }
                }
            }
            eable.Free();

            // Set 5 minute cooldown before wave 1 restarts
            m_CurrentWave = CemeteryWave.Cooldown;
            m_CooldownEnd = DateTime.UtcNow + TimeSpan.FromMinutes(5);
            InvalidateProperties();
        }

        private void SpawnNextCreature()
        {
            Type mobType = GetRandomMobType(m_CurrentWave);
            if (mobType == null)
                return;

            try
            {
                BaseCreature bc = Activator.CreateInstance(mobType) as BaseCreature;
                if (bc == null)
                    return;

                Point3D loc = FindValidSpawnLocation();
                bc.Home = loc;
                bc.RangeHome = 15;
                bc.MoveToWorld(loc, Map);

                m_ActiveMobs.Add(bc);
            }
            catch
            {
            }
        }

        private Type GetRandomMobType(CemeteryWave wave)
        {
            switch (wave)
            {
                case CemeteryWave.Wave1_RisingDead:
                    switch (Utility.Random(6))
                    {
                        case 0: return typeof(CryptGhoul);
                        case 1: return typeof(BoneKnight);
                        case 2: return typeof(SkeletalMage);
                        case 3: return typeof(Spectre);
                        case 4: return typeof(Zombie);
                        default: return typeof(Skeleton);
                    }

                case CemeteryWave.Wave2_CursedSwarm:
                    switch (Utility.Random(5))
                    {
                        case 0: return typeof(HalloweaveSpider);
                        case 1: return typeof(JackOLanternApparition);
                        case 2: return typeof(VampireBat);
                        case 3: return typeof(Mummy);
                        default: return typeof(Wraith);
                    }

                case CemeteryWave.Wave3_CryptHorrors:
                    switch (Utility.Random(5))
                    {
                        case 0: return typeof(CemeteryPumpkinFiend);
                        case 1: return typeof(CemeteryVampire);
                        case 2: return typeof(DreadSpider);
                        case 3: return typeof(Lich);
                        default: return typeof(PoisonElemental);
                    }

                case CemeteryWave.Wave4_Boss:
                    switch (Utility.Random(4))
                    {
                        case 0: return typeof(CryptGhoul);
                        case 1: return typeof(VampireBat);
                        case 2: return typeof(Wraith);
                        default: return typeof(BoneKnight);
                    }

                default:
                    return typeof(CryptGhoul);
            }
        }

        private Point3D FindValidSpawnLocation()
        {
            if (m_Bounds.Width <= 0 || m_Bounds.Height <= 0)
                return GetNearbyLocation(Location, 12);

            for (int i = 0; i < 35; i++)
            {
                int x = Utility.RandomMinMax(m_Bounds.X, m_Bounds.X + m_Bounds.Width);
                int y = Utility.RandomMinMax(m_Bounds.Y, m_Bounds.Y + m_Bounds.Height);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanFit(x, y, z, 16, false, false))
                    return new Point3D(x, y, z);
            }

            return GetNearbyLocation(Location, 12);
        }

        private Point3D GetNearbyLocation(Point3D center, int dist)
        {
            for (int i = 0; i < 20; i++)
            {
                int x = center.X + Utility.RandomMinMax(-dist, dist);
                int y = center.Y + Utility.RandomMinMax(-dist, dist);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanFit(x, y, z, 16, false, false))
                    return new Point3D(x, y, z);
            }

            return center;
        }

        public void ClearActiveMobs()
        {
            foreach (Mobile m in m_ActiveMobs)
            {
                if (m != null && !m.Deleted)
                    m.Delete();
            }
            m_ActiveMobs.Clear();

            if (m_ActiveBoss != null && !m_ActiveBoss.Deleted)
            {
                m_ActiveBoss.Delete();
                m_ActiveBoss = null;
            }
        }

        private void BroadcastArea(int hue, string format, params object[] args)
        {
            string message = String.Format(format, args);
            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 40);
            foreach (Mobile m in eable)
            {
                if (m.Player)
                    m.SendMessage(hue, message);
            }
            eable.Free();
        }

        private void PlayAreaSound(int soundID)
        {
            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 35);
            foreach (Mobile m in eable)
            {
                if (m.Player)
                    m.PlaySound(soundID);
            }
            eable.Free();
        }

        public override void OnDelete()
        {
            if (m_SliceTimer != null)
                m_SliceTimer.Stop();

            ClearActiveMobs();
            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_CemeteryName);
            writer.Write(m_Bounds);
            writer.Write((int)m_CurrentWave);
            writer.Write(m_KillsThisWave);
            writer.Write(m_MaxActiveMobs);
            writer.Write(m_CooldownEnd);
            writer.WriteMobileList(m_ActiveMobs);
            writer.Write(m_ActiveBoss);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_CemeteryName = reader.ReadString();
            m_Bounds = reader.ReadRect2D();
            m_CurrentWave = (CemeteryWave)reader.ReadInt();
            m_KillsThisWave = reader.ReadInt();
            m_MaxActiveMobs = reader.ReadInt();
            if (m_MaxActiveMobs < 50)
                m_MaxActiveMobs = 50;
            m_CooldownEnd = reader.ReadDateTime();
            m_ActiveMobs = reader.ReadStrongMobileList();
            m_ActiveBoss = reader.ReadMobile();

            StartTimer();
        }
    }
}
