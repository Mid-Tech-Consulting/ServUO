using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Regions;
using Server.Engines.ArenaSystem;

namespace Server.TournamentSystem
{
    [PropertyObject]
    public abstract class PVPTournamentSystem
    {
        #region Getters and Setters
        private string m_Name;

        private Timer m_Timer;
        private PVPTournamentSystem m_LinkedSystem;
        private TournamentStone m_Stone;
        private bool m_UseLinked;
        private bool m_InUse;
        private bool m_Active;
        private bool m_CanDoTourneys;
        private Region m_FightRegion;
        private Region m_AudienceRegion;
        private ArenaFight m_CurrentFight;
        private Tournament m_CurrentTournament;
        private Mobile m_ArenaKeeper;
        private TournamentBoard m_TournamentBoard;
        private StatsBoard m_StatsBoard;
        private TeamsBoard m_TeamsBoard;
        private PrizeChest m_Chest;
        private ArenaBell m_Bell;
        private List<Tournament> m_Tournaments = new List<Tournament>();
        private Dictionary<Mobile, Item> m_Wagers = new Dictionary<Mobile, Item>();
        private List<ArenaFight> m_Queue = new List<ArenaFight>();
        private List<FlagHolder> m_FlagHolders = new List<FlagHolder>();

        public Timer Timer { get { return m_Timer; } set { m_Timer = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public PVPTournamentSystem LinkedSystem { get { return m_LinkedSystem; } set { m_LinkedSystem = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TournamentStone Stone { get { return m_Stone; } set { m_Stone = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool InUse
        { 
            get { return m_InUse; } 
            set 
            { 
                m_InUse = value;

                if (!m_InUse)
                    CheckQueue();

                if (m_Stone != null)
                    m_Stone.InvalidateProperties();
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active 
        { 
            get { return m_Active; }
            set
            { 
                m_Active = value; 
                
                if (m_Active) 
                    m_ArenaKeeper = SpawnArenaKeeper();

                if (m_Stone != null)
                    m_Stone.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CanDoTourneys { get { return m_CanDoTourneys; } set { m_CanDoTourneys = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Region FightRegion
        { 
            get { return m_FightRegion; } 
            set { m_FightRegion = value;  } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Region AudienceRegion 
        { 
            get { return m_AudienceRegion; } 
            set { m_AudienceRegion = value; } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaFight CurrentFight
        {
            get { return m_CurrentFight; }
            set
            {
                m_CurrentFight = value; 
                
                if (m_Stone != null)
                    m_Stone.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Tournament CurrentTournament { get { return m_CurrentTournament; } set { m_CurrentTournament = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile ArenaKeeper 
        { 
            get 
            { 
                if (m_ArenaKeeper == null && m_Active) 
                    m_ArenaKeeper = SpawnArenaKeeper(); 

                return m_ArenaKeeper;
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TournamentBoard TournamentBoard { get { return m_TournamentBoard; } set { m_TournamentBoard = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public StatsBoard StatsBoard { get { return m_StatsBoard; } set { m_StatsBoard = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public PrizeChest Chest { get { return m_Chest; } set { m_Chest = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TeamsBoard TeamsBoard { get { return m_TeamsBoard; } set { m_TeamsBoard = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaBell Bell { get { return m_Bell; } set { m_Bell = value; } }

        public List<Tournament> Tournaments { get { return m_Tournaments; } }
        public Dictionary<Mobile, Item> Wagers { get { return m_Wagers; } }
        public List<ArenaFight> Queue { get { return m_Queue; } }
        public List<FlagHolder> FlagHolders { get { return m_FlagHolders; } }

        public bool CanUse 
        { 
            get 
            {
                if (m_Tournaments.Count == 0)
                    return !m_InUse;
                else
                {
                    m_Tournaments.Sort();
                    return !m_InUse && DateTime.Now + TournamentSetupTime < m_Tournaments[0].StartTime;
                }
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UseLinked
        {
            get { return m_UseLinked; }
            set
            {
                if (value != m_UseLinked)
                {
                    m_UseLinked = value;

                    if (value)
                        SetLinkedSystem();
                    else
                        m_LinkedSystem = null;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public FlagHolder FlagHolder1
        {
            get 
            {
                if(m_FlagHolders.Count > 0)
                    return m_FlagHolders[0];
                return null;
            }
            set 
            {
                if (m_FlagHolders.Count > 0)
                    m_FlagHolders.RemoveAt(0);

                m_FlagHolders.Insert(0, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public FlagHolder FlagHolder2
        {
            get
            {
                if (m_FlagHolders.Count > 1)
                    return m_FlagHolders[1];
                return null;
            }
            set
            {
                if (m_FlagHolders.Count > 1)
                    m_FlagHolders.RemoveAt(1);

                m_FlagHolders.Insert(1, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public FlagHolder FlagHolder3
        {
            get
            {
                if (m_FlagHolders.Count > 2)
                    return m_FlagHolders[2];
                return null;
            }
            set
            {
                if (m_FlagHolders.Count > 2)
                    m_FlagHolders.RemoveAt(2);

                m_FlagHolders.Insert(2, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public FlagHolder FlagHolder4
        {
            get
            {
                if (m_FlagHolders.Count > 3)
                    return m_FlagHolders[3];
                return null;
            }
            set
            {
                if (m_FlagHolders.Count > 3)
                    m_FlagHolders.RemoveAt(3);

                m_FlagHolders.Insert(3, value);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Name
        {
            get { return m_Name; }
            set
            {
                var old = m_Name;

                if (old != value && ValidateName(value))
                {
                    m_Name = value;

                    OnNameChange(old); // we need to update tournament stats!
                }
            }
        }
        #endregion

        #region static props

        private static List<PVPTournamentSystem> m_SystemList = new List<PVPTournamentSystem>();
        public static List<PVPTournamentSystem> SystemList { get { return m_SystemList; } }

        #endregion

        #region Overrideable Props
        public virtual bool NoClearRegion { get { return Config.NoClearRegion; } }

        public virtual TimeSpan TournamentSetupTime { get { return Config.TournamentSetupTime; } }
        public virtual bool CTFRandomStart { get { return Config.CTFRandomStart; } }

        public virtual int TeamAHue { get { return Config.TeamAHue; } } // Red
        public virtual int TeamBHue { get { return Config.TeamBHue; } } // Blue

        public virtual TimeSpan StartDelay { get { return Config.StartDelay; } }
        public virtual TimeSpan EjectDelay { get { return Config.EjectDelay; } }
        public virtual TimeSpan PreFightDelay { get { return Config.PreFightDelay; } }
        public virtual TimeSpan DefaultDuration { get { return Config.DefaultDuration; } }
        public virtual int MinLastManStanding { get { return Config.MinLastManStanding; } }

        public virtual bool AudienceRegionNoAttack { get { return Config.AudienceRegionNoAttack; } }
        public virtual bool AudienceRegionNoSpells { get { return Config.AudienceRegionNoSpells; } }

        public virtual bool RandomizeStartLocations { get { return Config.RandomizeStartLocations; } }

		public virtual bool ForceFightType { get { return false; } }
		public virtual ArenaFightType ForcedFightType { get { return ArenaFightType.SingleElimination; } }

        public virtual bool CanUseAlternateArena { get { return m_LinkedSystem != null; } }

		public virtual Point3D FlagHolder1Loc { get { return Point3D.Zero; } }
		public virtual Point3D FlagHolder2Loc { get { return Point3D.Zero; } }
        public virtual Point3D FlagHolder3Loc { get { return Point3D.Zero; } }
        public virtual Point3D FlagHolder4Loc { get { return Point3D.Zero; } }

        public virtual FightRegion GetFightRegion { get { return new FightRegion(this); } }
        public virtual AudienceRegion GetAudienceRegion { get { return new AudienceRegion(this); } }

        public virtual Rectangle2D[] FightingRegionBounds { get { return Definition.FightingRegionBounds; } }
        public virtual Rectangle2D[] AudienceRegionBounds { get { return Definition.AudienceRegionBounds; } }
        public virtual Rectangle2D RandomStartBounds { get { return Definition.RandomStartBounds; } }

        public virtual Rectangle2D KickZone { get { return Definition.KickZone; } }
        public virtual Rectangle2D WallArea { get { return Definition.WallArea; } }

        public virtual Point3D StoneLocation { get { return Definition.StoneLocation; } }
        public virtual Point3D TeamAStartLocation { get { return Definition.TeamAStartLocation; } }
        public virtual Point3D TeamBStartLocation { get { return Definition.TeamBStartLocation; } }
        public virtual Point3D ArenaKeeperLocation { get { return Definition.ArenaKeeperLocation; } }
        public virtual Point3D TeamAWageDisplay { get { return Definition.TeamAWageDisplay; } }
        public virtual Point3D TeamBWageDisplay { get { return Definition.TeamBWageDisplay; } }
        public virtual Point3D StatsBoardLocation { get { return Definition.StatsBoardLocation; } }
        public virtual Point3D TeamsBoardLocation { get { return Definition.TeamsBoardLocation; } }
        public virtual Point3D TournamentInfoBoardLocation { get { return Definition.TournamentInfoBoardLocation; } }
        public virtual Point3D ChestLocation { get { return Definition.ChestLocation; } }
        public virtual Point3D BellLocation { get { return Definition.BellLocation; } }

        public virtual Map ArenaMap { get { return null; } }
        public virtual int WallItemID { get { return 0x80; } }

        public abstract ArenaDefinition Definition { get; }
        public abstract string DefaultName { get; }

        #endregion

        public PVPTournamentSystem(TournamentStone stone)
        {
            m_Name = DefaultName;
            m_Stone = stone;
            m_Stone.System = this;
            m_CanDoTourneys = true;

            Timer.DelayCall(InitializeSystem);
        }

        public virtual void InitializeSystem()
        {
            m_FightRegion = GetFightRegion;
            m_AudienceRegion = GetAudienceRegion;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            m_SystemList.Add(this);
            m_UseLinked = true;
            SetLinkedSystem();

            SetupFlagHolders();

            m_StatsBoard = new StatsBoard(this);
            m_StatsBoard.MoveToWorld(StatsBoardLocation, ArenaMap);

            if (TeamsBoardLocation != Point3D.Zero)
            {
                m_TeamsBoard = new TeamsBoard();
                m_TeamsBoard.MoveToWorld(TeamsBoardLocation, ArenaMap);
            }

            m_TournamentBoard = new TournamentBoard(this);
            m_TournamentBoard.MoveToWorld(TournamentInfoBoardLocation, ArenaMap);

            m_Chest = new PrizeChest(this);
            m_Chest.MoveToWorld(ChestLocation, ArenaMap);
            m_Chest.Visible = false;

            m_Bell = new ArenaBell(this, 0x4C5C);
            m_Bell.MoveToWorld(BellLocation, ArenaMap);

            OnSystemConfigured();
        }

        public virtual void OnSystemConfigured()
        {
        }

        public bool ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return PVPTournamentSystem.SystemList.All(sys => sys.Name != name);
        }

        public ArenaFight ConstructArenaFight(ArenaFightType type, Tournament tourney = null)
        {
            if (ForcedFightType != ArenaFightType.SingleElimination)
            {
                type = ForcedFightType;
            }

            switch (type)
            {
                default:
                case ArenaFightType.SingleElimination: return new SingleEliminationFight(this, tourney);
                case ArenaFightType.BestOf3: return new BestOf3Fight(this, tourney);
                case ArenaFightType.LastManStanding: return new LastManStandingFight(this, tourney);
                case ArenaFightType.CaptureTheFlag: return new CaptureTheFlagFight(this, tourney);
            }
        }

        private void OnNameChange(string oldArena)
        {
            foreach (var stat in PVPTournamentStats.TournamentStats.Where(s => s.Arena == oldArena))
            {
                stat.UpdateArenaName(this);
            }
        }

        public override string ToString()
        {
            return "...";
        }

        public void AddTournament(Tournament tourney)
        {
            if (!m_Tournaments.Contains(tourney))
            {
                m_Tournaments.Add(tourney);
                TournamentBoard.UpdateBoards();
            }
        }

        public void RemoveTournament(Tournament tourney)
        {
            if (m_Tournaments.Contains(tourney))
            {
                m_Tournaments.Remove(tourney);
                TournamentBoard.UpdateBoards();
            }
        }

        public virtual Mobile SpawnArenaKeeper()
        {
            if (m_ArenaKeeper != null || m_Stone == null)
                return null;

            ArenaKeeper arenaKeeper = new ArenaKeeper(this);
            arenaKeeper.MoveToWorld(ArenaKeeperLocation, ArenaMap);
            arenaKeeper.Home = ArenaKeeperLocation;
            arenaKeeper.RangeHome = 5;

            return arenaKeeper;
        }

        public virtual bool CanRegisterTournament(Mobile from)
        {
            if (m_Tournaments.Count >= Config.MaxTournaments)
                return false;

            foreach (Tournament tourney in m_Tournaments)
                if (tourney.Creator == from)
                    return false;

            return m_CanDoTourneys;
        }

        public virtual bool BeginPrefight()
        {
            return true;
        }

        public virtual bool BeginFight()
        {
            return true;
        }

        public virtual void OnTick()
        {
        }

        public virtual void OnBeforeFight()
        {
        }

        public virtual void OnTickFight()
        {
        }

        public virtual void EndFight(bool istournament, ArenaTeam winner, ArenaTeam loser, ArenaFight fight)
        {
            foreach (FlagHolder holder in m_FlagHolders)
            {
                if (holder != null)
                    holder.Hue = 0;
            }
        }

        public virtual void SetupFlags(CaptureTheFlagFight fight)
		{
            if (fight == null || m_FlagHolders.Count < 2)
                return;

            ArenaTeam a = fight.TeamA;
            ArenaTeam b = fight.TeamB;

            int huea = TeamAHue;
            int hueb = TeamBHue;

            if (CTFRandomStart && 0.5 > Utility.RandomDouble())
            {
                a = fight.TeamB;
                b = fight.TeamA;
                huea = TeamBHue;
                hueb = TeamAHue;
            }

			if(FlagHolders.Count > 1 && FlagHolders[0] != null && FlagHolders[1] != null)
			{
				fight.Flag1 = new CTFFlag(fight, a, 0x15B7, huea);
                fight.Flag2 = new CTFFlag(fight, b, 0x15B6, hueb);

                FlagHolders[0].Owner = a;
                FlagHolders[0].AddFlag(fight.Flag1);
                FlagHolders[0].Hue = huea;

                FlagHolders[1].Owner = b;
                FlagHolders[1].AddFlag(fight.Flag2);
                FlagHolders[1].Hue = hueb;
			}
			else
				fight.CancelFight(CancelReason.SystemError);
		}

        public void SetupFlagHolders()
        {
            if (FlagHolder1Loc != Point3D.Zero)
            {
                FlagHolder holder = new FlagHolder(this);
                m_FlagHolders.Add(holder);
                holder.MoveToWorld(FlagHolder1Loc, ArenaMap);
            }

            if (FlagHolder2Loc != Point3D.Zero)
            {
                FlagHolder holder = new FlagHolder(this);
                m_FlagHolders.Add(holder);
                holder.MoveToWorld(FlagHolder2Loc, ArenaMap);
            }

            if (FlagHolder3Loc != Point3D.Zero)
            {
                FlagHolder holder = new FlagHolder(this);
                m_FlagHolders.Add(holder);
                holder.MoveToWorld(FlagHolder3Loc, ArenaMap);
            }

            if (FlagHolder4Loc != Point3D.Zero)
            {
                FlagHolder holder = new FlagHolder(this);
                m_FlagHolders.Add(holder);
                holder.MoveToWorld(FlagHolder4Loc, ArenaMap);
            }
        }
		
		public virtual bool CanDoCTF()
		{
			return m_FlagHolders.Count > 0;
		}
 
        public virtual void DoArenaKeeperMessage(string message)
        {
            if (m_ArenaKeeper != null)
                m_ArenaKeeper.Say(message);
        }

        public virtual Point3D GetRandomKickLocation()
        {
            Point3D p = StoneLocation;
            int x, y, z;

            for (int i = 0; i < 25; i++)
            {
                x = Utility.Random(KickZone.X, KickZone.Width);
                y = Utility.Random(KickZone.Y, KickZone.Height);

                if (this is KhaldunArena || this is KhaldunArenaTram)
                    z = 25;
                else
                    z = ArenaMap.GetAverageZ(x, y);

                if (ArenaMap.CanSpawnMobile(x, y, z))
                {
                    p = new Point3D(x, y, z);
                    break;
                }
            }

            return p;
        }
 
        public virtual Point3D RandomStartLocation(ArenaTeam team, bool ctf)
        {
			Point3D start;
			
			if(ctf)
			{
				if(m_FlagHolders.Count > 0 && m_FlagHolders[0].Owner == team)
					start = m_FlagHolders[0].Location;
                else if (m_FlagHolders.Count > 1 && m_FlagHolders[1].Owner == team)
					start = m_FlagHolders[1].Location;
				else
				{
					start = 0.5 > Utility.RandomDouble() ? TeamAStartLocation : TeamBStartLocation;
				}
			}
			else
				start = 0.5 > Utility.RandomDouble() ? TeamAStartLocation : TeamBStartLocation;
 
			for (int i = 0; i < 25; i++)
			{
				int x = Utility.RandomMinMax(start.X - 5, start.X + 5);
				int y = Utility.RandomMinMax(start.Y - 5, start.Y + 5);
				int z = ArenaMap.GetAverageZ(x, y);
 
				if (ArenaMap.CanSpawnMobile(x, y, z))
				{
					start = new Point3D(x, y, z);
					break;
				}
			}
				
            return start;
        }

        public virtual void DoWall()
        {
            if (WallItemID <= 0)
            {
                return;
            }

            for (int x = WallArea.Start.X; x <= WallArea.End.X; x++)
            {
                for (int y = WallArea.Start.Y; y <= WallArea.End.Y; y++)
                {
                    Point3D pnt = new Point3D(x, y, ArenaMap.GetAverageZ(x, y));

                    ArenaWall wall = new ArenaWall(this);
                    wall.MoveToWorld(pnt, ArenaMap);
                    Effects.PlaySound(pnt, ArenaMap, 0x1F6);
                }
            }
        }

        public virtual void RegionOnEnter(Mobile from)
        {
        }

        public virtual void RegionOnExit(Mobile from)
        {
        }

        public void AddWager(Dictionary<Mobile, Item> wagers)
        {
            m_Wagers = wagers;
        }

        public void ClearWagers()
        {
            m_Wagers.Clear();
        }

        public virtual bool CanRegisterFight(Mobile from)
        {
            return CanRegisterFight(from, true);
        }

        public virtual bool CanRegisterFight(Mobile from, bool checkTeam)
        {
            if (m_CurrentTournament != null)
                return false;

            List<ArenaTeam> teams = ArenaTeam.GetTeams(from);

            if (teams.Count == 0)
                return !checkTeam;

            for (int i = 0; i < teams.Count; i++)
            {
                ArenaTeam team = teams[i];

                if (team != null)
                {
                    // in queue already!
                    foreach (ArenaFight fight in m_Queue)
                    {
                        foreach(TeamInfo info in fight.Teams)
                        {
                            if (info == null)
                                continue;

                            if (team == info.Team)
                                return false;
                        }
                    }

                    // is currently fighting!
                    if (CurrentFight != null && CurrentFight.Teams.Select(x => x.Team).Any(t => t == team))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void AddToQueue(ArenaFight fight)
        {
            if (!m_Queue.Contains(fight))
            {
                foreach (var m in fight.GetFighters())
                {
                    m.SendMessage(ArenaHelper.ParticipantMessageHue, "Your fight has been added to the queue and will fight in the order in which you were added. You are {0} in the queue.", m_Queue.Count == 0 ? "next" : String.Format("number {0}", m_Queue.Count + 1));
                }

                m_Queue.Add(fight);
            }
        }

        public void RemoveFromQueue(ArenaFight fight)
        {
            if (m_Queue.Contains(fight))
                m_Queue.Remove(fight);
        }

        public void CheckQueue()
        {
            if (m_CurrentFight == null && !m_InUse && m_Active && m_Queue.Count > 0)
            {
                ArenaFight next = m_Queue[0];

                Timer.DelayCall(EjectDelay, new TimerCallback(next.OnBeforeFight));

                for (int i = 0; i < m_Queue.Count; i++)
                {
                    ArenaFight fight = m_Queue[i];
 
                    foreach (TeamInfo info in fight.Teams)
                    {
						ArenaTeam team = info.Team;
						
						if(team == null)
							continue;
					
                        foreach (Mobile m in team.Fighters)
                        {
                            if (i == 0)
                                m.SendMessage(ArenaHelper.ParticipantMessageHue, "Your teams fight will begin shortly.");
                            else if (i == 1)
                                m.SendMessage(ArenaHelper.ParticipantMessageHue, "Your team is on deck!");
                            else if (i == 2)
                                m.SendMessage(ArenaHelper.ParticipantMessageHue, "Your team is in the hole!");
                            else 
                                m.SendMessage(ArenaHelper.ParticipantMessageHue, String.Format("You have {0} fights before you can fight!", i));
                        }
                    }
                }
            }
        }

        public virtual void RemoveSystem()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_FightRegion != null)
                m_FightRegion.Unregister();

            if (m_AudienceRegion != null)
                m_AudienceRegion.Unregister();

            if (m_ArenaKeeper != null)
                m_ArenaKeeper.Delete();

            if (m_TournamentBoard != null)
                m_TournamentBoard.Delete();

            if (m_StatsBoard != null)
                m_StatsBoard.Delete();

            if (m_TeamsBoard != null)
                m_TeamsBoard.Delete();

            if (m_Chest != null)
                m_Chest.Delete();

            if (m_Bell != null)
                m_Bell.Delete();

            if (m_Stone != null && !m_Stone.Deleted)
                m_Stone.Delete();

            foreach (var holder in m_FlagHolders.Where(h => h != null && !h.Deleted))
                holder.Delete();

            if (m_CurrentFight is CaptureTheFlagFight)
            {
                var ctf = (CaptureTheFlagFight)m_CurrentFight;

                if (ctf.Flag1 != null)
                    ctf.Flag1.Delete();

                if (ctf.Flag2 != null)
                    ctf.Flag2.Delete();

                if (ctf.Flag3 != null)
                    ctf.Flag3.Delete();

                if (ctf.Flag4 != null)
                    ctf.Flag4.Delete();
            }

            m_SystemList.Remove(this);
        }

        /*public void ShowRegionBounds()
        {
            var list = new List<Static>();

            if (FightRegion != null)
            {
                foreach (var rec in FightRegion.Area)
                {
                    for (int x = rec.Start.X; x <= rec.Start.X + rec.Width; x++)
                    {
                        for (int y = rec.Start.Y; y <= rec.Start.Y + rec.Height; y++)
                        {
                            if (x == rec.Start.X || y == rec.Start.Y || x == rec.Start.X + rec.Width || y == rec.Start.Y + rec.Height)
                            {
                                var st = new Static(0x3709);
                                st.MoveToWorld(new Point3D(x, y, ArenaMap.GetAverageZ(x, y)), ArenaMap);

                                list.Add(st);
                            }
                        }
                    }
                }
            }

            if (AudienceRegion != null)
            {
                foreach (var rec in AudienceRegion.Area)
                {
                    for (int x = rec.Start.X; x <= rec.Start.X + rec.Width; x++)
                    {
                        for (int y = rec.Start.Y; y <= rec.Start.Y + rec.Height; y++)
                        {
                            if (x == rec.Start.X || y == rec.Start.Y || x == rec.Start.X + rec.Width || y == rec.Start.Y + rec.Height)
                            {
                                var st = new Static(0x3709);
                                st.Hue = 2;
                                st.MoveToWorld(new Point3D(x, y, ArenaMap.GetAverageZ(x, y)), ArenaMap);

                                list.Add(st);
                            }
                        }
                    }
                }
            }

            Timer.DelayCall<List<Static>>(TimeSpan.FromMinutes(5), lu =>
            {
                foreach (var s in lu)
                {
                    s.Delete();
                }

                Stone.ShowRegionBounds = false;
            }, list);
        }*/

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write((int)1); //Version

            writer.Write(m_Bell);

            writer.Write(m_FlagHolders.Count);

            foreach (FlagHolder holder in m_FlagHolders)
                writer.Write(holder);

            writer.Write(m_Name);

            writer.Write(m_UseLinked);
            writer.Write(m_CanDoTourneys);

            writer.Write(m_Active);
            writer.Write(m_Chest);
            writer.Write(m_StatsBoard);
            writer.Write(m_TournamentBoard);
            writer.Write(m_TeamsBoard);
            writer.Write(m_ArenaKeeper);

            writer.Write(m_Tournaments.Count);
            foreach (Tournament tourney in m_Tournaments)
            {
                tourney.Serialize(writer);
            }

            writer.Write(m_Wagers.Count);
            foreach (KeyValuePair<Mobile, Item> kvp in m_Wagers)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Bell = reader.ReadItem() as ArenaBell;
                    m_Bell.System = this;
                    goto case 0;
                case 0:

                    int c = reader.ReadInt();
                    for (int i = 0; i < c; i++)
                    {
                        FlagHolder holder = reader.ReadItem() as FlagHolder;
                        if (holder != null)
                        {
                            holder.System = this;
                            m_FlagHolders.Add(holder);
                        }
                    }

                    m_Name = reader.ReadString();

                    m_UseLinked = reader.ReadBool();
                    m_CanDoTourneys = reader.ReadBool();

                    m_Active = reader.ReadBool();
                    m_Chest = reader.ReadItem() as PrizeChest;
                    m_StatsBoard = reader.ReadItem() as StatsBoard;
                    m_TournamentBoard = reader.ReadItem() as TournamentBoard;
                    m_TeamsBoard = reader.ReadItem() as TeamsBoard;
                    m_ArenaKeeper = reader.ReadMobile();

                    if (m_Chest != null)
                        m_Chest.System = this;

                    if (m_TournamentBoard != null)
                        m_TournamentBoard.System = this;

                    if (m_StatsBoard != null)
                        m_StatsBoard.System = this;

                    if (m_ArenaKeeper != null && m_ArenaKeeper is ArenaKeeper)
                        ((ArenaKeeper)m_ArenaKeeper).System = this;

                    int tourneyCount = reader.ReadInt();

                    if (tourneyCount > 0)
                    {
                        if (m_Tournaments == null)
                            m_Tournaments = new List<Tournament>();

                        for (int i = 0; i < tourneyCount; ++i)
                        {
                            Tournament tourney = new Tournament(reader, this);

                            if (tourney != null && !m_Tournaments.Contains(tourney))
                                m_Tournaments.Add(tourney);
                        }
                    }

                    int wageCount = reader.ReadInt();

                    if (wageCount > 0)
                    {
                        for (int i = 0; i < wageCount; ++i)
                        {
                            Mobile leader = reader.ReadMobile();
                            Item item = reader.ReadItem();

                            if (item == null)
                                continue;

                            if (leader != null)
                            {
                                leader.BankBox.DropItem(item);
                                item.Movable = true;
                            }
                            else if (m_Chest != null)
                            {
                                m_Chest.DropItem(item);
                                item.Movable = true;
                            }
                            else
                                item.Delete();
                        }
                    }
                    break;
            }

            m_FightRegion = GetFightRegion;
            m_AudienceRegion = GetAudienceRegion;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            if (m_Chest != null)
                m_Chest.System = this;

            m_SystemList.Add(this);

            if (m_UseLinked)
                Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(SetLinkedSystem));

            if (version == 0)
            {
                m_Bell = new ArenaBell(this, 0x4C5C);
                m_Bell.MoveToWorld(BellLocation, ArenaMap);
            }
        }

        public PVPTournamentSystem(GenericReader reader, TournamentStone stone)
        {
            m_Stone = stone;
            m_Stone.System = this;

            Deserialize(reader);
        }

        public virtual void HandleKill(TeamInfo killerInfo, TeamInfo victimInfo, Mobile killer, Mobile victim)
        {
            killerInfo.Kills++;
        }

        public virtual void OnAfterEject(Mobile m)
        {
            if (m_ArenaKeeper != null)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), ArenaHelper.DoResurrect, m);
                Timer.DelayCall(TimeSpan.FromSeconds(3), ArenaHelper.RemoveCurses, m);
            }

            m.CloseGump(typeof(ParticipantGump));
        }

        public virtual void SetLinkedSystem()
        {
        }

        public virtual bool DoStartMessage()
        {
            return true;
        }

        public virtual bool DoStartDelayMessage()
        {
            return true;
        }

        public virtual bool DoTieMessage()
        {
            return true;
        }

        public virtual bool DoEndOfFightMessage(ArenaTeam winner, ArenaTeam loser, bool forfeit)
        {
            return true;
        }

        public bool TournamentNameExists(string name)
        {
            foreach (Tournament tourney in m_Tournaments)
            {
                if (tourney.Name == name)
                    return true;
            }
            return false;
        }

        #region Static Methods
		public static string GetFightType(ArenaFightType type)
		{
            return ArenaHelper.GetFightType(type);
		}
 
        public static PVPTournamentSystem GetRegionalSystem(Region region)
        {
            if (region is FightRegion)
                return ((FightRegion)region).System;

            if (region is AudienceRegion)
                return ((AudienceRegion)region).System;

            return null;
        }

        public static int MobileNotoriety(Mobile source, IDamageable damageable)
        {
            Mobile target = damageable as Mobile;

            if (target is Mobile)
            {
                if (target.Blessed ||
                        (target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || 
                        target is PlayerVendor || target is TownCrier)
                    return Notoriety.Invulnerable;

                if (target.AccessLevel > AccessLevel.Player)
                    return Notoriety.CanBeAttacked;

                if (IsEnemy(source, target))
                    return Notoriety.Enemy;

                if (IsFriendly(source, target))
                    return Notoriety.Ally;
            }

            return Server.Misc.NotorietyHandlers.MobileNotoriety(source, damageable);
        }

        public static bool IsEnemy(Mobile source, Mobile target)
        {
            if (target is BaseCreature && ((BaseCreature)target).GetMaster() != null)
                target = ((BaseCreature)target).GetMaster();

            PVPTournamentSystem system = GetRegionalSystem(source.Region);

            if (system != null && system.CurrentFight != null && source.Region != null && source.Region.IsPartOf(typeof(FightRegion)) && target.Region != null && target.Region.IsPartOf(typeof(FightRegion)))
            {
                TeamInfo sourceInfo = system.CurrentFight.GetTeamInfo(source);
                TeamInfo targetInfo = system.CurrentFight.GetTeamInfo(target);

                return sourceInfo != null && targetInfo != null && sourceInfo != targetInfo;
            }

            return false;
        }

        public static bool IsFriendly(Mobile source, Mobile target)
        {
            if (target is BaseCreature && ((BaseCreature)target).GetMaster() != null)
                target = ((BaseCreature)target).GetMaster();

            PVPTournamentSystem system = GetRegionalSystem(source.Region);

            if (system != null && system.CurrentFight != null && source.Region != null && source.Region.IsPartOf(typeof(FightRegion)) && target.Region != null && target.Region.IsPartOf(typeof(FightRegion)))
            {
                TeamInfo sourceInfo = system.CurrentFight.GetTeamInfo(source);
                TeamInfo targetInfo = system.CurrentFight.GetTeamInfo(target);

                return sourceInfo != null && targetInfo != null && sourceInfo == targetInfo;
            }
            return false;
        }
        #endregion

        public class InternalTimer : Timer
        {
            private PVPTournamentSystem m_System;

            public InternalTimer(PVPTournamentSystem system) : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_System = system;
            }

            protected override void OnTick()
            {
                if (m_System == null)
                {
                    Stop();
                    return;
                }

                m_System.OnTick();

                for (int i = 0; i < m_System.Tournaments.Count; i++)
                {
                    Tournament tourney = m_System.Tournaments[i];

                    if (!m_System.InUse && m_System.CurrentTournament == null && DateTime.Now + m_System.TournamentSetupTime >= tourney.StartTime)
                    {
                        if (DateTime.Now > tourney.StartTime)
                            tourney.StartTime = DateTime.Now + m_System.TournamentSetupTime;  //Just incase server goes down etc ect.

                        m_System.CurrentTournament = tourney;
                        tourney.BeginTournament();
                    }
                    else if (tourney.WarningMessage == 1 && DateTime.Now + TimeSpan.FromHours(72) > tourney.StartTime)
                    {
                        string msg = String.Format("The {0} Tournament will begin at {1}.", tourney.Name, tourney.StartTime);
                        foreach (ArenaTeam team in tourney.Participants)
                            ArenaHelper.DoTeamPM(team, "72 Hour Reminder", msg, m_System.ArenaKeeper, TimeSpan.FromHours(73));

                        tourney.WarningMessage++;
                    }
                    else if (tourney.WarningMessage == 2 && DateTime.Now + TimeSpan.FromHours(24) > tourney.StartTime)
                    {
                        string msg = String.Format("The {0} Tournament will begin at {1}.", tourney.Name, tourney.StartTime);
                        foreach (ArenaTeam team in tourney.Participants)
                            ArenaHelper.DoTeamPM(team, "24 Hour Reminder", msg, m_System.ArenaKeeper, TimeSpan.FromHours(25));

                        tourney.WarningMessage++;
                    }
                }
            }
        }
    }
}