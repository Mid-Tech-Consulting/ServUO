using System;
using System.Collections.Generic;

using Server;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Prompts;

namespace Server.TournamentSystem
{
    public class MyTeamStatsGump : BaseTournamentGump
    {
        private List<ArenaTeam> Teams { get; set; }

        public MyTeamStatsGump(PlayerMobile from, List<ArenaTeam> teams)
            : base(from, 20, 20)
        {
            Teams = teams;
        }

        public override void AddGumpLayout()
        {
            AddPage(0);
            AddBackground(0, 0, 500, 400, DarkBackground);
            AddBackground(10, 55, 480, 180, LightBackground);
            AddBackground(10, 245, 480, 145, LightBackground);
            AddBackground(167, 10, 167, 30, 9350);


            AddLabel(210, 15, LabelHue, "My Team Info");

            AddLabel(200, 60, LabelHue, "Team Information");
            AddLabel(20, 80, LabelHue, "Team:");
            AddLabel(20, 105, LabelHue, "Leader:");
            AddLabel(20, 130, LabelHue, "Fighters:");

            AddLabel(215, 250, LabelHue, "Team Stats");
            AddLabel(40, 275, LabelHue, "Wins/Losses");
            AddLabel(20, 290, LabelHue, "Arena");
            AddLabel(85, 290, LabelHue, "Tournament");
            AddLabel(190, 290, LabelHue, "Tourny Champs");
            AddLabel(320, 290, LabelHue, "Damage Ratio");
            AddLabel(425, 290, LabelHue, "Points");

            if (Teams == null || Teams.Count == 0)
            {
                AddLabel(90, 80, 0, "You do not belong to any registered teams");
                return;
            }

            int idx = 0;
            int pages = Teams.Count;

            for (int i = 1; i <= pages; ++i)
            {
                if (idx >= Teams.Count)
                    break;

                AddPage(i);
                ArenaTeam team = Teams[idx];

                if (team.CanRename(User))
                {
                    AddButton(460, 58, 4005, 4007, idx + 3000, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(356, 58, 100, 20, AlignRightLoc, "Rename Team", 0, false, false);
                }

                AddLabel(90, 80, !team.Active ? 32 : 0, String.Format("{0}{1}", team.Name, !team.Active ? " [Inactive]" : ""));
                AddLabel(90, 105, 0, team.TeamLeader != null ? team.TeamLeader.Name : "Nobody");

                for (int j = 0; j < team.Fighters.Count; j++)
                {
                    AddLabel(90, 130 + (j * 25), 0, team.Fighters[j] != null ? team.Fighters[j].Name : "Open");

                    if (User == team.Fighters[j])
                    {
                        AddButton(460, 82, 4002, 4003, idx + 100, GumpButtonType.Reply, 0); //Resign Button
                        AddHtmlLocalized(356, 82, 100, 20, AlignRightLoc, "Resign", 0, false, false);
                    }

                    if (User == team.TeamLeader && User != team.Fighters[j])
                    {
                        AddButton(460, 130 + (25 * j), 4020, 4022, (idx * 10) + j, GumpButtonType.Reply, 0); //Kick Button
                        AddHtmlLocalized(356, 130 + (25 * j), 100, 20, AlignRightLoc, "Kick", 0, false, false);
                    }
                }

                if (!team.Active && team.Fighters.Count != (int)team.TeamType && User == team.TeamLeader)
                {
                    AddHtmlLocalized(356, 105, 100, 20, AlignRightLoc, "Add Fighter", 0, false, false);
                    AddButton(460, 105, 4023, 4025, idx + 1000, GumpButtonType.Reply, 0); //Add Button
                }

                AddLabel(20, 315, 0, String.Format("{0}/{1}", team.Wins, team.Losses));
                AddLabel(85, 315, 0, String.Format("{0}/{1}", team.TournamentWins, team.TournamentWins));
                AddLabel(190, 315, 0, String.Format("{0}", team.TournamentChampionships));
                AddLabel(320, 315, 0, String.Format("{0}", PlayerStatsGump.GetDamageRatio(team)));
                AddLabel(425, 315, 0, String.Format("{0}", (int)team.Points));

                if (i != pages)
                    AddButton(252, 368, 4005, 4007, 0, GumpButtonType.Page, i + 1);
                if (i > 1)
                    AddButton(218, 368, 4014, 4016, 0, GumpButtonType.Page, i - 1);
                idx++;
            }
        }

        public override void OnResponse(RelayInfo button)
        {
            int id = button.ButtonID;

            if (id == 0)
                return;

            ArenaTeam team = null;

            if (id >= 100 && id < 1000) //Resign
            {
                try
                {
                    team = Teams[id - 100];
                    BaseGump.SendGump(new ConfirmKickGump(User, team, User, true));
                }
                catch
                {
                }
            }
            else if (id < 10) //Kick Single
            {
                try
                {
                    int kickidx = id;
                    team = Teams[0];

                    if (team.TeamLeader == User)
                        BaseGump.SendGump(new ConfirmKickGump(User, team, team.Fighters[kickidx], false));
                }
                catch
                {
                    User.SendGump(new MyTeamStatsGump(User, Teams));
                }
            }
            else if (id < 20) //Kick Twosome
            {
                try
                {
                    int kickidx = id - 10;
                    team = Teams[1];

                    if (team.TeamLeader == User)
                        BaseGump.SendGump(new ConfirmKickGump(User, team, team.Fighters[kickidx], false));
                }
                catch
                {
                    Refresh();
                }
            }
            else if (id < 30) //Kick Foursome
            {
                try
                {
                    int kickidx = id - 20;
                    team = Teams[2];

                    if (team.TeamLeader == User)
                        BaseGump.SendGump(new ConfirmKickGump(User, team, team.Fighters[kickidx], false));
                }
                catch
                {
                    Refresh();
                }
            }
            else if (id >= 1000 && id < 2500) //Add Player
            {
                try
                {
                    int addidx = id - 1000;
                    team = Teams[addidx];

                    if (team.Fighters.Count != (int)team.TeamType)
                    {
                        User.Target = new InternalTarget(User, team, Teams);
                        User.SendMessage("Target the player you'd like to have in your arena team.");
                    }
                    else
                        Refresh();
                }
                catch
                {
                }
            }
            else if (id >= 3000)
            {
                int idx = id - 3000;

                if (idx >= 0 && idx < Teams.Count)
                {
                    team = Teams[idx];

                    if (team != null && team.CanRename(User))
                    {
                        User.SendMessage("What would you like to rename this team to?");
                        User.Prompt = new InternalPrompt(team);
                    }
                }
            }
        }

        private class InternalPrompt : Prompt
        {
            private ArenaTeam m_Team;

            public InternalPrompt(ArenaTeam team)
            {
                m_Team = team;
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (m_Team != null && text != null && text.Length > 0)
                    m_Team.TryRename(from, text.Trim());
            }
        }

        public class InternalTarget : Target
        {
            private Mobile m_From;
            private ArenaTeam m_Team;
            private List<ArenaTeam> Teams;

            public InternalTarget(Mobile from, ArenaTeam team, List<ArenaTeam> teams)
                : base(12, false, TargetFlags.None)
            {
                m_From = from;
                m_Team = team;
                Teams = teams;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (target is Mobile)
                {
                    Mobile mob = (Mobile)target;

                    if (mob is PlayerMobile && mob.Alive)
                    {
                        if (ArenaTeam.HasTeam(mob, m_Team.TeamType))
                            m_From.SendMessage("They already belong to a team with that many fighters.");
                        else if (mob == m_From)
                            m_From.SendMessage("You are already on the team!");
                        else if (m_Team.Fighters.Contains(mob))
                            m_From.SendMessage("They are already on the team!");
                        else if (mob.HasGump(typeof(AddFighterGump)))
                            m_From.SendMessage("They are already deciding on joining an arena team. Try again later.");
                        else if(from is PlayerMobile)
                        {
                            BaseGump.SendGump(new AddFighterGump(m_From as PlayerMobile, mob as PlayerMobile, m_Team, true, null));
                            m_From.SendMessage("Please wait while they contemplate joining your team.");
                        }
                    }
                    else
                        m_From.SendMessage("You cannot add them as a member of the team.");
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType type)
            {
                if(m_From is PlayerMobile)
                    BaseGump.SendGump(new MyTeamStatsGump(m_From as PlayerMobile, Teams));
            }
        }
    }

    public class ConfirmKickGump : BaseTournamentGump
    {
        private ArenaTeam m_Team;
        private Mobile m_ToKick;
        private bool m_Resign;

        public ConfirmKickGump(PlayerMobile user, ArenaTeam team, Mobile kick, bool resign) : base(user, 20, 20)
        {
            m_Team = team;
            m_ToKick = kick;
            m_Resign = resign;
        }

        public override void AddGumpLayout()
        {
            AddPage(0);
            AddBackground(0, 0, 250, 200, 5120);
            AddBackground(10, 10, 230, 180, 5100);

            AddButton(30, 155, 2074, 2075, 1, GumpButtonType.Reply, 0); //Okay
            AddButton(175, 155, 2071, 2072, 2, GumpButtonType.Reply, 0);  //Cancel

            if (m_Team == null || m_ToKick == null)
                return;

            string text;
            if (m_Resign)
                text = String.Format("You have selected the option to resign from your Arena Team, {0}. To continue, click Okay. To cancel resignation, select cancel.", m_Team.Name);
            else
                text = String.Format("You have selected to kick {0} from your Arena Team, {0}. To continue, click Okay.  To Cancel kicking {0}, select cancel.", m_ToKick.Name, m_Team.Name);

            AddHtml(25, 20, 200, 120, text, true, true);
        }

        public override void OnResponse(RelayInfo button)
        {
            bool resign = User == m_ToKick;

            switch (button.ButtonID)
            {
                default:
                case 0: break;
                case 1:
                    {
                        if (resign)
                            m_Team.Resign(m_ToKick);
                        else
                        {
                            m_Team.Kick(m_ToKick);
                            User.SendMessage("You kick them from your arena team.");
                        }
                        break;
                    }
                case 2:
                    {
                        string text;
                        if (resign)
                            text = "You decide not to resign from your Arena Team.";
                        else
                            text = "You decide not to kick them from your Arena Team.";

                        User.SendMessage(text);
                        break;
                    }

            }
        }
    }
}