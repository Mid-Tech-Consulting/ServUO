using System;
using System.Collections.Generic;

using Server;
using Server.Network;
using Server.Mobiles;
using Server.Guilds;
using Server.Gumps;

namespace Server.TournamentSystem
{
    public class PlayerStatsGump : BaseTournamentGump
    {
        private int m_SortByType;
        private int m_SortByWins;

        public override int LabelHue { get { return 1150; } }

        public PlayerStatsGump(PlayerMobile from) : this(from, 0, 0)
        {
        }

        public PlayerStatsGump(PlayerMobile from, int type, int wins)
            : base(from, 20, 20)
        {
            m_SortByType = type;
            m_SortByWins = wins;
        }

        public override void AddGumpLayout()
        {
            if (m_SortByWins < 0) m_SortByWins = 5;
            else if (m_SortByWins > 5) m_SortByWins = 0;

            if (m_SortByType < 0) m_SortByType = 3;
            if (m_SortByType > 3) m_SortByType = 0;

            List<ArenaTeam> Teams;

            AddPage(0);
            AddBackground(0, 0, 800, 550, DarkBackground);
            AddBackground(316, 10, 167, 30, 9350);
            AddBackground(10, 55, 780, 485, LightBackground);

            AddHtml(0, 15, 800, 20, Center("Team Statistics"), false, false);

            AddHtml(45, 32, 100, 20, Color("#FFFFFF", "My Teams"), false, false);
            AddButton(10, 32, 4005, 4006, 1, GumpButtonType.Reply, 0);

            AddLabel(25, 70, LabelHue, "Team");
            AddLabel(180, 70, LabelHue, "Fighters");
            AddLabel(250, 70, LabelHue, "Arena");
            AddLabel(350, 70, LabelHue, "Tournament");
            AddLabel(450, 70, LabelHue, "Championships");
            AddLabel(545, 70, LabelHue, "Damage Ratio");
            AddLabel(650, 70, LabelHue, "Points");

            AddLabel(300, 55, LabelHue, "Wins/Losses");

            if (m_SortByType == 0)
                AddLabel(54, 515, 0, "Viewing All");
            else if (m_SortByType == 1)
                AddLabel(54, 515, 0, "Viewing Singles");
            else if (m_SortByType == 2)
                AddLabel(54, 515, 0, "Viewing Twosomes");
            else
                AddLabel(54, 515, 0, "Viewing Foursomes");

            AddButton(20, 515, 4011, 4012, 2, GumpButtonType.Reply, 0);

            if (m_SortByWins == 0)
            {
                AddLabel(207, 515, 0, "No Filter");
                Teams = ArenaStatsSorter.GetSortedData(ArenaTeam.Teams, SortBy.None);
            }
            else if (m_SortByWins == 1)
            {
                AddLabel(207, 515, 0, "Viewing by Arena Wins");
                Teams = ArenaStatsSorter.GetSortedData(ArenaTeam.Teams, SortBy.SortByArenaWins);
            }
            else if (m_SortByWins == 2)
            {
                AddLabel(207, 515, 0, "Viewing by Tournament Wins");
                Teams = ArenaStatsSorter.GetSortedData(ArenaTeam.Teams, SortBy.SortByTournamentWins);
            }
            else if (m_SortByWins == 3)
            {
                AddLabel(207, 515, 0, "Viewing by Tournament Championships");
                Teams = ArenaStatsSorter.GetSortedData(ArenaTeam.Teams, SortBy.SortByTournamentChampionships);
            }
            else if (m_SortByWins == 4)
            {
                AddLabel(207, 515, 0, "Viewing by Score");
                Teams = ArenaStatsSorter.GetSortedData(ArenaTeam.Teams, SortBy.SortByPoints);
            }
            else
            {
                AddLabel(207, 515, 0, "Vieweing by Damage Ratio");
                Teams = ArenaStatsSorter.GetSortedData(ArenaTeam.Teams, SortBy.SortByDamageRatio);
            }

            AddButton(173, 515, 4011, 4012, 3, GumpButtonType.Reply, 0);

            AddLabel(20, 492, ArenaTeam.InactiveHue, "* - Inactive");

            Teams.Reverse();
            List<ArenaTeam> useList = new List<ArenaTeam>();

            if (m_SortByWins == 0) //No filter preferece, we're now filtering by team type only.
            {
                foreach (ArenaTeam t in ArenaTeam.Teams)
                {
                    if (m_SortByType == 0 || (m_SortByType == 1 && t.TeamType == ArenaTeamType.Single) || (m_SortByType == 2 && t.TeamType == ArenaTeamType.Twosome) || (m_SortByType == 3 && t.TeamType == ArenaTeamType.Foursome))
                        useList.Add(t);
                }
            }
            else  //Filter preference will take non-desired team types out of the pre-sorted list.
            {
                for (int i = 0; i < Teams.Count; i++)
                {
                    if (m_SortByType == 0 || (m_SortByType == 1 && Teams[i].TeamType == ArenaTeamType.Single) || (m_SortByType == 2 && Teams[i].TeamType == ArenaTeamType.Twosome) || (m_SortByType == 3 && Teams[i].TeamType == ArenaTeamType.Foursome))
                        useList.Add(Teams[i]);
                }
            }

            int pages = (int)Math.Ceiling(useList.Count / 12.0);
            int count = 0;
            ArenaTeam team = null;

            for (int i = 1; i <= pages; i++)
            {
                AddPage(i);
                int yOffset = 100;

                for (int j = 0; j < 12; j++)
                {
                    if (count >= useList.Count)
                        break;

                    team = useList[count];
                    count++;
                    if (team == null)
                        continue;

                    string name = team.Name;
                    int color;
                    if (team.Active)
                        color = ArenaHelper.GetTeamTypeColor(team.TeamType);
                    else
                    {
                        color = ArenaTeam.InactiveHue;
                        name = "* " + team.Name;
                    }

                    int realIdx = GetTeamIndex(team);

                    AddLabelCropped(15, yOffset, 25, 20, color, String.Format("{0:##}.", count));
                    AddLabelCropped(40, yOffset, 145, 20, color, String.Format("{0}", name));

                    if (realIdx >= 0)
                    {
                        AddButton(170, yOffset + 3, 1209, 1210, 10 + realIdx, GumpButtonType.Reply, 0);
                        Localizations.GetLocalization(40);
                    }

                    AddLabelCropped(190, yOffset, 55, 20, color, String.Format("{0}", team.TeamType));
                    AddLabelCropped(250, yOffset, 95, 20, color, String.Format("{0}/{1}", team.Wins, team.Losses));
                    AddLabelCropped(350, yOffset, 95, 20, color, String.Format("{0}/{1}", team.TournamentWins, team.TournamentLosses));
                    AddLabelCropped(450, yOffset, 55, 20, color, String.Format("{0}", team.TournamentChampionships));
                    AddLabelCropped(545, yOffset, 55, 20, color, String.Format("{0}", GetDamageRatio(team)));
                    AddLabelCropped(650, yOffset, 135, 20, color, String.Format("{0}", ((int)team.Points).ToString("N0")));
                    yOffset += 30;
                }
                if (i != pages)
                    AddButton(760, 514, 2224, 2224, 0, GumpButtonType.Page, i + 1);
                if (i > 1)
                    AddButton(730, 514, 2223, 2223, 0, GumpButtonType.Page, i - 1);
            }
        }

        public static string GetDamageRatio(ArenaTeam team)
        {
            long given = Simplify(team.DamageGiven);
            long received = Simplify(team.DamageTaken);

            if (given == 0 || received == 0)
            {
                return String.Format("{0}:{1}", given, received);
            }

            long gcd = GCD(given, received);

            return String.Format("{0}:{1}", given / gcd, received / gcd);
        }

        private static long Simplify(long value)
        {
            if (value > 1000)
            {
                double g = (double)value / 1000.0;
                value = (int)g * 1000;
            }
            else if (value > 100)
            {
                double g = (double)value / 100.0;
                value = (int)g * 100;
            }

            return value;
        }

        private static long GCD(long a, long b)
        {
            return b == 0 ? Math.Abs(a) : GCD(b, a % b);
        }

        public static int GetTeamIndex(ArenaTeam team)
        {
            for (int i = 0; i < ArenaTeam.Teams.Count; i++)
            {
                if (team == ArenaTeam.Teams[i])
                    return i;
            }
            return -1;
        }

        public override void OnResponse(RelayInfo button)
        {
            if (button.ButtonID < 10)
            {
                switch (button.ButtonID)
                {
                    default:
                    case 0:
                        break;
                    case 1: //My Team Stats
                        BaseGump.SendGump(new MyTeamStatsGump(User, ArenaTeam.GetTeams(User)));
                        break;
                    case 2: //Sort by Team Types
                        m_SortByType++;
                        Refresh();
                        break;
                    case 3: //Sort by Stats
                        m_SortByWins++;
                        Refresh();
                        break;
                }
            }
            else
            {
                try
                {
                    ArenaTeam team = ArenaTeam.Teams[button.ButtonID - 10];
                    BaseGump.SendGump(new FightersGump(team, User, m_SortByType, m_SortByWins));
                }
                catch
                {
                    Refresh();
                    User.SendMessage("There was an error fetching the players of that team.");
                }
            }
        }
    }

    public class FightersGump : BaseTournamentGump
    {
        private ArenaTeam m_Team;
        private int m_Type;
        private int m_Wins;

        public FightersGump(ArenaTeam team, PlayerMobile from, int type, int wins) : base(from, 20, 20)
        {
            m_Team = team;
            m_Type = type;
            m_Wins = wins;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 300, 200, DarkBackground);
            AddBackground(10, 10, 280, 180, LightBackground);

            AddHtml(0, 15, 300, 20, ColorAndCenter("#FFFFFF", "Players"), false, false);
            int y = 50;
            int x = 20;

            for (int i = 0; i < m_Team.Fighters.Count; ++i)
            {
                Mobile fighter = m_Team.Fighters[i];
                string name;
                Guild guild = m_Team.Fighters[i].Guild as Guild;

                if (guild != null)
                {
                    if (guild.Type == GuildType.Regular)
                        name = fighter.Name + " [" + guild.Abbreviation + "]";
                    else if (guild.Type == GuildType.Order)
                        name = fighter.Name + " [" + guild.Abbreviation + "] (Order)";
                    else
                        name = fighter.Name + " [" + guild.Abbreviation + "] (Chaos)";
                }
                else
                    name = fighter.Name;

                AddLabel(x, y, 0, name);
                y += 30;
            }

            AddButton(12, 168, 4014, 4015, 1, GumpButtonType.Reply, 0);
            AddLabel(45, 168, 0, "Back");

            if (User.AccessLevel >= AccessLevel.GameMaster)
            {
                AddButton(260, 168, 4008, 4010, 2, GumpButtonType.Reply, 0);
                AddLabel(190, 168, 0, "Team Props");
            }
        }

        public override void OnResponse(RelayInfo button)
        {
            switch (button.ButtonID)
            {
                default:
                case 0: break;
                case 1:
                    BaseGump.SendGump(new PlayerStatsGump(User, m_Type, m_Wins));
                    break;
                case 2:
                    User.SendGump(new PropertiesGump(User, m_Team));
                    break;
            }
        }
    }
}