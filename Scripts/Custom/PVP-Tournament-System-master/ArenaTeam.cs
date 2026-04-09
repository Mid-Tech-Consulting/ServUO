using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.TournamentSystem
{
    public enum ArenaTeamType
    {
        None = 0,
        Single = 1,
        Twosome = 2,
        Foursome = 4,
        Temp = 5
    }

    [PropertyObject]
    public class ArenaTeam
    {
        public static readonly int InactiveHue = 32;

        private string m_Name;
        private bool m_Active;
        private bool m_Temp;
        private Mobile m_TeamLeader;
        private ArenaTeamType m_Type;
        private int m_TournamentWins;
        private int m_TournamentLosses;
        private int m_TournamentChampionships;
        private int m_Wins;
        private int m_Losses;
        private int m_Draws;
        private double m_Points;
        private DateTime m_NextRename;

        private long m_DamageTaken;
        private long m_DamageGiven;

        private int m_SingleElimWins;
        private int m_BestOf3Wins;
        private int m_LastManStandingWins;
        private int m_CTFWins;

        private List<Mobile> m_Fighters = new List<Mobile>();

        [CommandProperty(AccessLevel.Counselor)]
        public string Name { get { return m_Name; } set { m_Name = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active { get { return m_Active; } set { m_Active = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile TeamLeader
        {
            get
            {
                if (m_TeamLeader == null && m_Fighters.Count > 0)
                    m_TeamLeader = m_Fighters[0];

                return m_TeamLeader;
            }
            set { m_TeamLeader = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ArenaTeamType TeamType { get { return m_Type; } set { m_Type = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TournamentWins { get { return m_TournamentWins; } set { m_TournamentWins = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TournamentLosses { get { return m_TournamentLosses; } set { m_TournamentLosses = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TournamentChampionships { get { return m_TournamentChampionships; } set { m_TournamentChampionships = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Wins { get { return m_Wins; } set { m_Wins = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Losses { get { return m_Losses; } set { m_Losses = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Draws { get { return m_Draws; } set { m_Draws = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Points { get { return m_Points; } set { m_Points = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextRename { get { return m_NextRename; } set { m_NextRename = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SingleElimWins { get { return m_SingleElimWins; } set { m_SingleElimWins = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BestOf3Wins { get { return m_BestOf3Wins; } set { m_BestOf3Wins = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LastManStandingWins { get { return m_LastManStandingWins; } set { m_LastManStandingWins = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CTFWins { get { return m_CTFWins; } set { m_CTFWins = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public long DamageGiven { get { return m_DamageGiven; } set { m_DamageGiven = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public long DamageTaken { get { return m_DamageTaken; } set { m_DamageTaken = value; } }

        public List<Mobile> Fighters
        {
            get
            {
                if (m_Fighters == null)
                    m_Fighters = new List<Mobile>();

                return m_Fighters;
            }
        }

        private static List<ArenaTeam> m_Teams = new List<ArenaTeam>();
        public static List<ArenaTeam> Teams { get { return m_Teams; } }

        public static void DefragTeams()
        {
            for (int i = 0; i < m_Teams.Count; i++)
            {
                for (int j = 0; j < m_Teams[i].Fighters.Count; j++)
                {
                    if (m_Teams[i].Fighters[j] == null || m_Teams[i].Fighters[j].Deleted)
                        m_Teams[i].Kick(m_Teams[i].Fighters[j]);
                }
            }
        }

        public ArenaTeam(Mobile leader)
        {
            m_TeamLeader = leader;
            m_Fighters.Add(leader);
            m_TournamentWins = 0;
            m_TournamentLosses = 0;
            m_Wins = 0;
            m_Losses = 0;
            m_Draws = 0;
            m_Active = false;
            m_Type = ArenaTeamType.None;
            m_NextRename = DateTime.UtcNow;
        }

        public ArenaTeam()
        {
            m_Temp = true;
            m_Type = ArenaTeamType.Temp;
            m_Active = true;
        }

        public override string ToString()
        {
            if (m_Name != null)
                return m_Name;

            return "...";
        }

        public void RegisterTeam()
        {
            m_Teams.Add(this);
            m_Active = true;
        }

        public void RegisterWin(ArenaTeam defeated, ArenaFight fight, ArenaFightType type)
        {
            if (m_Temp)
                return;

            double award = fight.IsTournament ? 20 : 10;

            if (type != ArenaFightType.LastManStanding && defeated != null && defeated.Points > m_Points)
                award += Math.Max(20, Math.Sqrt(defeated.Points - m_Points));
            else if (type == ArenaFightType.LastManStanding)
                award += fight.Teams.Count / 2;

            SendMessageToFighters(String.Format("Your team has earned {0} points for achieving victory in {1}!", (int)award, ArenaHelper.GetFightType(type)));

            m_Points += award;
        }

        public void AddFighter(Mobile mob)
        {
            AddFighter(mob, true);
        }

        public void AddFighter(Mobile mob, bool exists)
        {
            if (!m_Fighters.Contains(mob))
                m_Fighters.Add(mob);

            if (exists && !m_Temp)
                OnFighterChange();
        }

        public void SendMessageToFighters(string message)
        {
            foreach (Mobile m in m_Fighters)
            {
                if (m != null)
                    m.SendMessage(ArenaHelper.ParticipantMessageHue, message);
            }
        }

        /// <summary>
        /// justdied needs to be passed in as region OnDeath is called before a mobile.Alive is set
        /// </summary>
        /// <param name="justdied">the mobile that just died</param>
        /// <returns></returns>
        public bool AllDead(Mobile justdied)
        {
            return !Fighters.Any(
                    mob => justdied != mob && 
                    mob.Alive && 
                    mob.NetState != null &&
                    mob.Region != null &&
                    mob.Region.IsPartOf<FightRegion>());
        }

        public bool AllDead()
        {
            return !Fighters.Any(
                    mob => mob.Alive &&
                    mob.NetState != null &&
                    mob.Region != null &&
                    mob.Region.IsPartOf<FightRegion>());
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)1); //Version!!!

            writer.Write(m_SingleElimWins);
            writer.Write(m_BestOf3Wins);
            writer.Write(m_LastManStandingWins);
            writer.Write(m_CTFWins);

            writer.Write(m_DamageGiven);
            writer.Write(m_DamageTaken);

            writer.Write(m_NextRename);
            writer.Write(m_Points);
            writer.Write(m_Name);
            writer.Write(m_Active);
            writer.Write(m_TeamLeader);
            writer.Write(m_TournamentWins);
            writer.Write(m_TournamentLosses);
            writer.Write(m_TournamentChampionships);
            writer.Write(m_Wins);
            writer.Write(m_Losses);
            writer.Write(m_Draws);
            writer.Write((int)m_Type);

            writer.Write(m_Fighters.Count);

            for (int i = 0; i < m_Fighters.Count; ++i)
                writer.Write(m_Fighters[i]);
        }

        public ArenaTeam(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_SingleElimWins = reader.ReadInt();
                    m_BestOf3Wins = reader.ReadInt();
                    m_LastManStandingWins = reader.ReadInt();
                    m_CTFWins = reader.ReadInt();
                    goto case 0;
                case 0:
                    m_DamageGiven = reader.ReadLong();
                    m_DamageTaken = reader.ReadLong();

                    m_NextRename = reader.ReadDateTime();

                    m_NextRename = DateTime.UtcNow;
                    m_Points = reader.ReadDouble();

                    m_Name = reader.ReadString();
                    m_Active = reader.ReadBool();
                    m_TeamLeader = reader.ReadMobile();
                    m_TournamentWins = reader.ReadInt();
                    m_TournamentLosses = reader.ReadInt();
                    m_TournamentChampionships = reader.ReadInt();
                    m_Wins = reader.ReadInt();
                    m_Losses = reader.ReadInt();
                    m_Draws = reader.ReadInt();
                    m_Type = (ArenaTeamType)reader.ReadInt();
                    break;
            }

            if (m_Fighters == null)
                m_Fighters = new List<Mobile>();

            bool checkchange = false;
            int count = reader.ReadInt();

            for (int i = 0; i < count; ++i)
            {
                Mobile mob = reader.ReadMobile();

                if (mob != null && !m_Fighters.Contains(mob))
                    m_Fighters.Add(mob);
                else
                    checkchange = true;
            }

            // For now, any team w/ BaseCreature is not added
            if (m_Fighters.All(f => f is PlayerMobile))
            {
                m_Teams.Add(this);
            }

            if (checkchange)
            {
                OnFighterChange();
            }
        }

        public static bool HasTeam(Mobile from)
        {
            foreach (ArenaTeam team in m_Teams)
            {
                if (team.Fighters.Contains(from))
                    return true;
            }
            return false;
        }

        public static bool HasTeam(Mobile from, ArenaTeamType type)
        {
            foreach (ArenaTeam team in m_Teams)
            {
                if (team.Fighters.Contains(from) && team.TeamType == type)
                    return true;
            }
            return false;
        }

        public static ArenaTeam GetTeam(Mobile from, ArenaTeamType type)
        {
            return GetTeam(from, type, false);
        }

        public static ArenaTeam GetTeam(Mobile from, ArenaTeamType type, bool create)
        {
            foreach (ArenaTeam team in m_Teams)
            {
                if (team.Fighters.Contains(from) && team.TeamType == type)
                    return team;
            }

            if (create)
            {
                ArenaTeam team = new ArenaTeam(from);
                team.TeamType = type;
                string name = String.Format("Team {0}", from.Name);

                if (ArenaTeam.NameExists(name))
                {
                    for (int i = 0; i < 5000; i++)
                    {
                        name = name + i.ToString();

                        if (!NameExists(name))
                            break;

                    }
                }

                team.Name = name;
                team.RegisterTeam();
                from.SendMessage(0x32, "{0} Arena Team auto-created.", type.ToString());

                return team;
            }

            return null;
        }

        public static bool IsTeamLeader(Mobile from, ArenaTeam team)
        {
            return (team.TeamLeader == from);
        }

        public static List<ArenaTeam> GetTeams(Mobile from)
        {
            List<ArenaTeam> list = new List<ArenaTeam>();

            foreach (ArenaTeam team in m_Teams)
            {
                foreach (Mobile mob in team.Fighters)
                {
                    if (from == mob && !list.Contains(team))
                        list.Add(team);
                }
            }
            return list;
        }

        public void OnFighterChange()
        {
            if (m_Fighters == null)
            {
                m_Active = false;
                return;
            }

            switch (m_Fighters.Count)
            {
                default:
                case 0: m_Active = false; break;
                case 1:
                    {
                        if (m_Type != ArenaTeamType.Single) m_Active = false;
                        else m_Active = true;
                        break;
                    }
                case 2:
                    {
                        if (m_Type != ArenaTeamType.Twosome) m_Active = false;
                        else m_Active = true;
                        break;
                    }
                case 4:
                    {
                        if (m_Type != ArenaTeamType.Foursome) m_Active = false;
                        else m_Active = true;
                        break;
                    }
            }

            if (m_Fighters.Count > 0 && !m_Fighters.Contains(m_TeamLeader))
            {
                m_TeamLeader = m_Fighters[0];
                string subject = "New Leader";
                string text1 = String.Format("{0} is now the new team leader of {1} by default.", m_TeamLeader.Name, m_Name);
                string text2 = String.Format("You are now the new team leader of {0} by default.", m_Name);
                foreach (Mobile mob in m_Fighters)
                {
                    ArenaHelper.DoTeamPM(mob, subject, mob == m_TeamLeader ? text1 : text2, m_TeamLeader);
                }
            }
            else if (m_Fighters.Count == 0)
                DisbandTeam();

        }

        private void DisbandTeam()
        {
            if (m_Fighters.Count > 0)
                return;

            if (m_Teams.Contains(this))
                m_Teams.Remove(this);
        }

        public Mobile GetLeadership()
        {
            if (m_TeamLeader != null && m_TeamLeader.NetState != null)
                return m_TeamLeader;

            for (int i = 0; i < m_Fighters.Count; ++i)
            {
                if (m_Fighters[i].NetState != null)
                    return m_Fighters[i];
            }
            return null;
        }

        public void Resign(Mobile from)
        {
            if (from == null)
                return;

            if (m_Fighters.Contains(from))
                m_Fighters.Remove(from);

            from.SendMessage("You have resigned from {0}.", m_Name);
            if (m_Fighters.Count > 0)
                ArenaHelper.DoTeamPM(this, "Resignation", String.Format("{0} has resigned from {1} Arena Team.", from.Name, m_Name), TeamLeader);

            OnFighterChange();
        }

        public void Kick(Mobile from)
        {
            if (from == null || from.Deleted)
            {
                if (m_Fighters.Contains(from))
                    m_Fighters.Remove(from);
                OnFighterChange();
                return;
            }

            if (m_Fighters.Contains(from))
                m_Fighters.Remove(from);

            ArenaHelper.DoTeamPM(from, "Kick", String.Format("You have been kicked from {0} Arena Team.", m_Name), TeamLeader);
            foreach (Mobile fighter in m_Fighters)
            {
                if (fighter != m_TeamLeader)
                    ArenaHelper.DoTeamPM(fighter, "Kick", String.Format("{0} has been kicked from your arena team, {0}.", from.Name, m_Name), TeamLeader);
            }

            OnFighterChange();
        }

        public static bool NameExists(string name)
        {
            foreach (ArenaTeam team in m_Teams)
            {
                if (team.Name == name)
                    return true;
            }
            return false;
        }


        public bool CanRename(Mobile from)
        {
            if (m_TeamLeader != from || m_NextRename > DateTime.UtcNow)
                return false;

            IPooledEnumerable eable = from.Map.GetMobilesInRange(from.Location, 10);

            foreach (Mobile m in eable)
            {
                if (m is ArenaKeeper)
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        public void TryRename(Mobile from, string name)
        {
            ErrorType type = RegisterTeamGump.ValidateName(name);

            switch (type)
            {
                default:
                case ErrorType.Valid:
                    IPooledEnumerable eable = from.Map.GetMobilesInRange(from.Location, 10);
                    Mobile keeper = null;
                    foreach (Mobile m in eable)
                    {
                        if (m is ArenaKeeper)
                        {
                            keeper = m;
                            break;
                        }
                    }

                    if (keeper != null)
                    {
                        if (Banker.Withdraw(from, 50000))
                        {
                            Name = name;
                            m_NextRename = DateTime.UtcNow + TimeSpan.FromDays(7);
                            ArenaHelper.DoArenaKeeperMessage(String.Format("Your arena team is now named {0}. You must wait about a week before you can change your name again.", name), from);
                        }
                        else
                            ArenaHelper.DoArenaKeeperMessage("You lack the required funds to change your team name.", from);
                    }
                    else
                        from.SendMessage("You must rename your team near an Arena Keeper for their blessing!");

                    break;
                case ErrorType.Invalid:
                    from.SendMessage(0x23, "You have chosen an invalid name.");
                    break;
                case ErrorType.TooManyChars:
                    from.SendMessage(0x23, "The name must be no more than 16 characters.");
                    break;
                case ErrorType.NotEnoughChars:
                    from.SendMessage(0x23, "Name too short.");
                    break;
                case ErrorType.AlreadyExists:
                    from.SendMessage(0x23, "That arena team name already exists.");
                    break;
                case ErrorType.Unacceptable:
                    from.SendMessage(0x23, "That name is unacceptable.");
                    break;
            }
        }
    }
}