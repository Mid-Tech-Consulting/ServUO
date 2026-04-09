using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Network;
using Server.Mobiles;
using Server.TournamentSystem;
 
namespace Server.Gumps
{
    public class TournamentStoneGump : BaseTournamentGump
    {
        private PVPTournamentSystem System { get; set; }
        private List<ArenaTeam> Teams { get; set; }
 
        public TournamentStoneGump(PVPTournamentSystem system, PlayerMobile from) : base(from, 20, 20)
        {
            System = system;
            Teams = ArenaTeam.GetTeams(User);
        }

        public override void AddGumpLayout()
        {
            AddPage(0);
            AddBackground(0, 0, 350, 475, DarkBackground);
            AddBackground(10, 10, 330, 125, LightBackground);
            AddBackground(10, 145, 330, 125, LightBackground);
            AddBackground(10, 280, 330, 185, LightBackground);

            AddHtml(0, 15, 350, 16, ColorAndCenter(LabelColor, System.Name), false, false);

            AddLabel(55, 45, 0, "Tournament Stats");
            AddTooltip(Localizations.GetLocalization(1));
            AddButton(15, 45, 4029, 4031, 1, GumpButtonType.Reply, 0);
            AddTooltip(Localizations.GetLocalization(1));

            AddLabel(230, 45, 0, "System Info");
            AddTooltip(Localizations.GetLocalization(2));
            AddButton(190, 45, 4029, 4031, 8, GumpButtonType.Reply, 0);
            AddTooltip(Localizations.GetLocalization(2));

            AddLabel(55, 75, 0, "Team Stats");
            AddTooltip(Localizations.GetLocalization(3));
            AddButton(15, 75, 4029, 4031, 2, GumpButtonType.Reply, 0);
            AddTooltip(Localizations.GetLocalization(3));

            if (Teams != null)
            {
                AddButton(190, 75, 4029, 4031, 9, GumpButtonType.Reply, 0);
                AddTooltip(Localizations.GetLocalization(4));
                AddLabel(230, 75, 0, "My Teams Info");
                AddTooltip(Localizations.GetLocalization(4));
            }

            AddButton(15, 105, 4029, 4031, 6, GumpButtonType.Reply, 0);
            AddTooltip(Localizations.GetLocalization(5));
            AddLabel(55, 105, 0, "Future Tournaments");
            AddTooltip(Localizations.GetLocalization(5));

            AddHtml(0, 150, 350, 20, ColorAndCenter(LabelColor, "Fight/Register"), false, false);

            if (System.Active)
            {
                if ((User is PlayerMobile && !((PlayerMobile)User).Young))
                {
                    AddLabel(55, 180, 0, "Register Team");
                    AddTooltip(Localizations.GetLocalization(6));
                    AddButton(15, 180, 4023, 4025, 3, GumpButtonType.Reply, 0);
                    AddTooltip(Localizations.GetLocalization(6));
                }

                if (System.CanRegisterTournament(User))
                {
                    AddLabel(55, 210, 0, "Register Tournament");
                    AddTooltip(Localizations.GetLocalization(7));
                    AddButton(15, 210, 4023, 4025, 4, GumpButtonType.Reply, 0);
                    AddTooltip(Localizations.GetLocalization(7));
                }

                if (!ActionTimer.WaitingAction.ContainsKey(User))
                {
                    if (System.CanRegisterFight(User) && (!System.ForceFightType || System.ForcedFightType == ArenaFightType.SingleElimination))
                    {
                        AddButton(190, 210, 4023, 4025, 10, GumpButtonType.Reply, 0);
                        AddTooltip(Localizations.GetLocalization(8));
                        AddLabel(230, 210, 0, "Quick Fight");
                        AddTooltip(Localizations.GetLocalization(8));
                    }

                    if (System.CanRegisterFight(User))
                    {
                        string fightType = "";
                        if (System.ForceFightType)
                            fightType = String.Format(" ({0})", PVPTournamentSystem.GetFightType(System.ForcedFightType));

                        AddButton(15, 240, 4023, 4025, 11, GumpButtonType.Reply, 0);
                        AddTooltip(Localizations.GetLocalization(9));
                        AddLabel(56, 240, 0, "New Arena Fight" + fightType);
                        AddTooltip(Localizations.GetLocalization(9));
                    }
                }
            }

            string status = "";
            string start = "";

            if (!System.Active)
                status = "Arena Inactive";
            else if (!System.InUse)
                status = "Arena Available";

            else if (System.CurrentFight != null && !System.CurrentFight.IsTournament)
            {
                ArenaFight currentfight = System.CurrentFight;
                status = String.Format("In Use - {0}", ArenaHelper.GetFightType(System.CurrentFight.ArenaFightType));

                ArenaTeam teamA = currentfight.TeamA;
                ArenaTeam teamB = currentfight.TeamB;

                if (teamA != null && teamB != null)
                {
                    if (currentfight.ArenaFightType == ArenaFightType.LastManStanding)
                    {
                        if (currentfight.PreFight)
                        {
                            status = status + String.Format("<br><br>{0} teams will be fighting shortly.", currentfight.Teams.Count);
                        }
                        else
                        {
                            status = status + String.Format("<br><br>{0}teams are fighting eachother!", currentfight.Teams.Count);
                        }
                    }
                    else
                    {
                        if (currentfight.PreFight)
                        {
                            status = status + "<br><br>" + String.Format("{0} will be fighting {1} shortly.", teamA.Name, teamB.Name);
                        }
                        else
                        {
                            status = status + "<br><br>" + String.Format("{0} is currently fighting {1} in a {2} match!", teamA.Name, teamB.Name, ArenaHelper.GetFightType(currentfight.ArenaFightType));
                        }
                    }

                    switch (currentfight.ArenaFightType)
                    {
                        case ArenaFightType.CaptureTheFlag:
                            TeamInfo info1 = currentfight.GetTeamInfo(currentfight.TeamA);
                            TeamInfo info2 = currentfight.GetTeamInfo(currentfight.TeamB);

                            status += "<br>";
                            status += String.Format("<br>{0} Points: {1}", currentfight.TeamA.Name, info1.Points);
                            status += String.Format("<br>{0} Kills: {1}", currentfight.TeamA.Name, info1.Kills);
                            status += String.Format("<br>{0} Count: {1}", currentfight.TeamA.Name, info1.Team.Fighters.Count);
                            status += "<br>";
                            status += String.Format("<br>{0} Points: {1}", currentfight.TeamB.Name, info2.Points);
                            status += String.Format("<br>{0} Kills: {1}", currentfight.TeamB.Name, info2.Kills);
                            status += String.Format("<br>{0} Count: {1}", currentfight.TeamB.Name, info2.Team.Fighters.Count);
                            break;
                        case ArenaFightType.BestOf3:
                            int winA = currentfight is BestOf3Fight ? ((BestOf3Fight)currentfight).TeamAWins : 0;
                            int winB = currentfight is BestOf3Fight ? ((BestOf3Fight)currentfight).TeamBWins : 0;

                            status += "<br>";

                            if (winA + winB == 0)
                            {
                                status += String.Format("<br>This is the first fight between {0} and {1}.", currentfight.TeamA.Name, currentfight.TeamB.Name);
                            }
                            else if (winA + winB == 1)
                            {
                                status += String.Format("<br>{0} has won round one!", winA == 1 ? currentfight.TeamA.Name : currentfight.TeamB.Name);
                            }
                            else
                            {
                                status += String.Format("<br>{0} and {1} have each won a round, next kill wins!", currentfight.TeamA.Name, currentfight.TeamB.Name);
                            }
                            break;
                        case ArenaFightType.LastManStanding:
                            status += "<br><br>Still Alive:<br>";
                            foreach (var team in currentfight.Teams.Select(x => x.Team).Where(t => !t.AllDead()))
                            {
                                status += String.Format("<br>{0}", team.Name);
                            }
                            break;
                    }
                }

                if (System.Queue.Count > 0)
                {
                    ArenaFight fight = System.Queue[0];

                    if (fight.ArenaFightType == ArenaFightType.LastManStanding)
                        status = status + "<br>" + String.Format("On Deck: Last Man Standing with {0} participants.", fight.Teams.Count.ToString());
                    else
                        status = status + "<br>" + String.Format("On Deck: {0} vs {1}", fight.TeamA != null ? fight.TeamA.Name : "Unknown", fight.TeamB != null ? fight.TeamB.Name : "Unknown");

                    if (System.Queue.Count > 1)
                    {
                        fight = System.Queue[1];
                        status = status + "<br>" + String.Format("In The Hole: {0} vs {1}", fight.TeamA != null ? fight.TeamA.Name : "Unknown", fight.TeamB != null ? fight.TeamB.Name : "Unknown");
                    }
                }
            }
            else if (System.CurrentTournament != null)
            {
                Tournament tourney = System.CurrentTournament;

                if (DateTime.Now < tourney.StartTime)
                {
                    TimeSpan ts = tourney.StartTime - DateTime.Now;

                    if (ts.TotalMinutes == 1)
                        start = "1 minute";
                    else if (ts.TotalMinutes < 1)
                        start = String.Format("{0} seconds", (int)ts.TotalSeconds);
                    else
                        start = String.Format("{0} minutes", (int)ts.TotalMinutes);

                    //status = String.Format("The {0} Tournament will begin in {1}.", tourney.Name, time);
                }
                else
                    start = "Tournament in Progress";

                start = start + "<br>";

                if (tourney.Participants != null && tourney.Participants.Count > 0 && tourney.Round > 0)
                {
                    if (tourney.RoundMatches.Count == 0)
                        status = String.Format("Round {0} is ending, waiting for round {1}", tourney.Round - 1, tourney.Round);
                    else
                    {
                        try
                        {
                            ArenaTeam a = null;
                            ArenaTeam b = null;
                            ArenaTeam c = null;
                            ArenaTeam d = null;

                            if (tourney.ArenaB != null && tourney.UseAlternateArena && tourney.OriginalArena.Count > 0)
                            {
                                a = tourney.OriginalArena[0];
                                if (tourney.OriginalArena.Count > 1)
                                    b = tourney.OriginalArena[1];
                                if (tourney.AlternateArena.Count > 0)
                                    c = tourney.AlternateArena[0];
                                if (tourney.AlternateArena.Count > 1)
                                    d = tourney.OriginalArena[1];

                                if (a != null && b != null && c != null && d != null)
                                    status = String.Format(tourney.Name + "   Round " + tourney.Round + "<br>Main Arena: {0} Vs {1}<br>Alternate Arena: {2} Vs {3}<br>Teams Left in current round: " + tourney.RoundMatches.Count.ToString() + "<br>Teams left in tournament: " + tourney.Participants.Count.ToString(), a.Name, b.Name, c.Name, d.Name);
                                else if (a != null && b != null)
                                    status = String.Format(tourney.Name + "   Round " + tourney.Round + "<br>Main Arena: {0} Vs {1}<br>Teams Left in the current round: " + tourney.RoundMatches.Count.ToString() + "<br>Teams left in tournament: " + tourney.Participants.Count.ToString(), a.Name, b.Name);
                                else
                                    status = String.Format(tourney.Name + "   Round " + tourney.Round + "<br>Teams Left in the current round: " + tourney.RoundMatches.Count.ToString() + "<br>Teams left in tournament: " + tourney.Participants.Count.ToString());
                            }
                            else if (tourney.RoundMatches.Count > 0)
                            {
                                a = tourney.RoundMatches[0];
                                if (tourney.RoundMatches.Count > 1)
                                    b = tourney.RoundMatches[1];
                                if (tourney.RoundMatches.Count > 2)
                                    c = tourney.RoundMatches[2];
                                if (tourney.RoundMatches.Count > 3)
                                    d = tourney.RoundMatches[3];

                                if (a != null && b != null && c != null && d != null)
                                    status = String.Format(tourney.Name + "   Round " + tourney.Round + "<br>Current Fight: {0} Vs {1}<br>Next Fight: {2} Vs {3}<br>Teams Left in current round: " + tourney.RoundMatches.Count.ToString() + "<br>Teams left in tournament: " + tourney.Participants.Count.ToString(), a.Name, b.Name, c.Name, d.Name);
                                else if (a != null && b != null)
                                    status = String.Format(tourney.Name + "   Round " + tourney.Round + "<br>Current Fight: {0} Vs {1}<br>Teams Left in the current round: " + tourney.RoundMatches.Count.ToString() + "<br>Teams left in tournament: " + tourney.Participants.Count.ToString(), a.Name, b.Name);
                                else
                                    status = String.Format(tourney.Name + "   Round " + tourney.Round + "<br>Teams Left in the current round: " + tourney.RoundMatches.Count.ToString() + "<br>Teams left in tournament: " + tourney.Participants.Count.ToString());
                            }
                        }
                        catch { }
                    }
                }

            }

            AddHtml(0, 284, 350, 20, ColorAndCenter(LabelColor, "Arena Status"), false, false);
            AddHtml(26, 309, 297, 140, start + status, true, true);
        }
 
        public override void OnResponse(RelayInfo button)
        {
            switch (button.ButtonID)
            {
                default:
                case 0: break;
                case 1: //Tourney Stats
                    BaseGump.SendGump(new TournamentStatsGump(User, System));
                    break;
                case 2: //Team Stats
                    BaseGump.SendGump(new PlayerStatsGump(User));
                    break;
                case 3: //Register Team
                    if (System.Active && User is PlayerMobile && !((PlayerMobile)User).Young)
                        BaseGump.SendGump(new RegisterTeamGump(User, new ArenaTeam(User), System));
                    break;
                case 4: //Register Tourney
                    if (System.Active && System.CanRegisterTournament(User))
                        BaseGump.SendGump(new RegisterTournamentGump(System, new Tournament(System), User));
                    break;
                case 5: //New Fight
                    {
                        /*if (System.Active && !ActionTimer.WaitingAction.ContainsKey(User) && System.CanRegisterFight(User))
                        {
                            if (!ArenaTeam.HasTeam(User))
                                User.SendMessage("You must belong to a registered arena team before starting a new fight!");
                            else
                                User.SendGump(new RegisterFightGump(System, new ArenaFight(System, false), User));
                        }*/
                    }
                    break;
                case 6: //Upcoming Tourneys
                    BaseGump.SendGump(new TournamentsGump(System, User));
                    break;
                case 7: //My Teams Info
                    {
                        break;
                    }
                case 8: //System Info
                    {
                        if (!User.HasGump(typeof(SystemInfoGump)))
                            BaseGump.SendGump(new SystemInfoGump(User, System));
 
                        break;
                    }
                case 9: //My Teams Stats
                    {
                        if (!User.HasGump(typeof(MyTeamStatsGump)))
                            BaseGump.SendGump(new MyTeamStatsGump(User, ArenaTeam.GetTeams(User)));
                        break;
                    }
                case 10:
                    {
                        if (!ActionTimer.WaitingAction.ContainsKey(User) && System.CanRegisterFight(User, false))
                        {
                            ArenaFight fight = new SingleEliminationFight(System, null);
							fight.FightType = ArenaTeamType.Single;
 
                            User.Target = new RegisterFightGump.InternalTarget(System, fight, User, true, true);
                            User.SendMessage("Target the player you'd like to fight.");
                        }
                        break;
                    }
                case 11: // Register Fight
                    {
                        if (System.Active && !ActionTimer.WaitingAction.ContainsKey(User) && System.CanRegisterFight(User))
                        {
                            if (!ArenaTeam.HasTeam(User))
                                User.SendMessage("You must belong to a registered arena team before starting a new fight!");
                            else
                            {
                                if (System.ForceFightType)
                                {
                                    BaseGump.SendGump(new RegisterFightGump(System, System.ConstructArenaFight(System.ForcedFightType), User));
                                }
                                else
                                {
                                    BaseGump.SendGump(new ChooseFightTypeGump(User, System));
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}