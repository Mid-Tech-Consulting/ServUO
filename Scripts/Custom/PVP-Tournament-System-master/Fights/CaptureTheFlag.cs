using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Mobiles;

namespace Server.TournamentSystem
{
    public class CaptureTheFlagFight : ArenaFight
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public override ArenaFightType ArenaFightType { get { return ArenaFightType.CaptureTheFlag; } }

        private int m_CTFTick;
        private bool m_DoneHalf;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextFlagCheck { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CTFFlag Flag1 { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CTFFlag Flag2 { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CTFFlag Flag3 { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CTFFlag Flag4 { get; set; }

        public CaptureTheFlagFight(PVPTournamentSystem system, Tournament tourney)
            : base(system, tourney)
        {
            AddToForcedRules(FightRules.AllowResurrections);
        }

        public override void OnFightWon(ArenaTeam winner)
        {
            winner.CTFWins++;
        }

        public override void OnBeforeFight()
        {
            base.OnBeforeFight();

            System.SetupFlags(this);
        }

        public override void DoPrefightSetup()
        {
            NextFlagCheck = DateTime.UtcNow + TimeSpan.FromSeconds(60);

            foreach (var team in Teams.Select(x => x.Team))
            {
                foreach (var fighter in team.Fighters)
                {
                    fighter.Frozen = true;
                    fighter.Hidden = true;
                    fighter.SendMessage(ArenaHelper.ParticipantMessageHue, "You cannot move until the game begins!");
                    MoveToStartSpot(fighter, System.RandomStartLocation(team, true), 0);
                }
            }
        }

        public override void OnBeginFight()
        {
            DoStartMessage();

            foreach (var fighter in GetFighters())
            {
                fighter.Frozen = false;
                fighter.Hidden = false;
            }

            if (System.FlagHolder1 == null || System.FlagHolder2 == null)
            {
                CancelFight(CancelReason.SystemError);
                return;
            }
        }

        public override void OnTick()
        {
            bool invalidate = false;

            if (NextFlagCheck < DateTime.UtcNow)
            {
                FlagHolder holder1 = System.FlagHolder1;
                FlagHolder holder2 = System.FlagHolder2;

                if (holder1 != null && holder1.EnemyFlag != null && holder1.FlagPlaced != DateTime.MinValue && holder1.FlagPlaced + TimeSpan.FromMinutes(1) <= DateTime.UtcNow)
                {
                    TeamInfo info = GetTeamInfo(holder1.Owner);
                    ArenaTeam loser = holder2 != null ? holder2.Owner : null;

                    if (info != null)
                    {
                        info.Points++;
                        DoCTFTeamAnnouncement(info.Team, loser, info.Points);
                        invalidate = true;
                    }
                }

                if (holder2 != null && holder2.EnemyFlag != null && holder2.FlagPlaced != DateTime.MinValue && holder2.FlagPlaced + TimeSpan.FromMinutes(1) <= DateTime.UtcNow)
                {
                    TeamInfo info = GetTeamInfo(holder2.Owner);
                    ArenaTeam loser = holder1 != null ? holder1.Owner : null;

                    if (info != null)
                    {
                        info.Points++;
                        DoCTFTeamAnnouncement(info.Team, loser, info.Points);
                        invalidate = true;
                    }
                }

                if (Flag1 != null && Flag1.Holder == null)
                {
                    if (Flag1.LastTaken != DateTime.MinValue && Flag1.LastTaken + TimeSpan.FromMinutes(3) <= DateTime.UtcNow)
                        Flag1.ReturnToLast();
                }

                if (Flag2 != null && Flag2.Holder == null)
                {
                    if (Flag2.LastTaken != DateTime.MinValue && Flag2.LastTaken + TimeSpan.FromMinutes(3) <= DateTime.UtcNow)
                        Flag2.ReturnToLast();
                }

                if (DoCTFRoundAnnouncement(m_CTFTick, m_DoneHalf))
                    m_DoneHalf = true;

                NextFlagCheck = DateTime.UtcNow + TimeSpan.FromSeconds(60);
                m_CTFTick++;
            }

            if (invalidate)
            {
                if (System.Stone != null)
                {
                    System.Stone.InvalidateGumps();
                }

                RefreshGumps(true);
            }
            else
            {
                RefreshGumps();
            }

            if (EndTime < DateTime.UtcNow)
            {
                CheckCTFWinner();
                EndTimer();
                return;
            }
            else if (DateTime.UtcNow + TimeSpan.FromMinutes(1) >= EndTime && !Warning)
            {
                DoWarning();
                Warning = true;
            }

            System.OnTickFight();
        }

        private void CheckCTFWinner()
        {
            if (TeamA.Fighters.Count == 0 && TeamB.Fighters.Count == 0)
            {
                TieBreaker = false;
                EndFight();
            }
            else
            {
                TeamInfo info1 = GetTeamInfo(TeamA);
                TeamInfo info2 = GetTeamInfo(TeamB);

                if (info1 == null && info2 != null)
                {
                    EndFight(info2.Team, null);
                }
                else if (info1 != null && info2 == null)
                {
                    EndFight(info1.Team, null);
                }
                else if (info1 == null && info2 == null)
                {
                    TieBreaker = false;
                    EndFight();
                }
                else
                {
                    if (info1.Points > info2.Points)
                    {
                        EndFight(info1.Team, info2.Team);
                    }
                    else if (info2.Points > info1.Points)
                    {
                        EndFight(info2.Team, info1.Team);
                    }
                    else if (info1.Kills > info2.Kills)
                    {
                        EndFight(info1.Team, info2.Team);
                    }
                    else if (info2.Kills > info1.Kills)
                    {
                        EndFight(info2.Team, info1.Team);
                    }
                    else
                    {
                        TieBreaker = info1.Points > 0 || info2.Points > 0 ||
                                         info1.Kills > 0 || info2.Kills > 0;
                        EndFight();
                    }
                }
            }

            ResetFlags();
        }

        public void ResetFlags()
        {
            if (Flag1 != null)
                Flag1.Delete();

            if (Flag2 != null)
                Flag2.Delete();

            if (Flag3 != null)
                Flag3.Delete();

            if (Flag4 != null)
                Flag4.Delete();

            if (System == null)
                return;

            if (System.FlagHolder1 != null)
                System.FlagHolder1.Reset();

            if (System.FlagHolder2 != null)
                System.FlagHolder2.Reset();

            if (System.FlagHolder3 != null)
                System.FlagHolder3.Reset();

            if (System.FlagHolder4 != null)
                System.FlagHolder4.Reset();
        }

        public override void DoTieBreaker()
        {
            ArenaTeam winningTeam = null;
            ArenaTeam runnerUp = null;

            int highestDamage = 0;
            int highestKills = 0;

            //Kills as tie breaker
            foreach (TeamInfo info in Teams)
            {
                ArenaTeam team = info.Team;

                if (team == null)
                    continue;

                int kills = info.Kills;

                if (kills > highestKills)
                {
                    winningTeam = team;
                    highestKills = kills;
                }
            }

            // Now by damage
            if (winningTeam == null)
            {
                foreach (var team in Teams.Select(x => x.Team))
                {
                    if (team == null)
                        continue;

                    int damage = GetTotalDamageGiven(team);

                    if (damage > highestDamage)
                    {
                        winningTeam = team;
                        highestDamage = damage;
                    }
                }
            }

            //Checks for who has most players in the region
            int aCount = 0;
            int bCount = 0;

            List<Mobile> list = System.FightRegion.GetMobiles();

            foreach (Mobile m in list)
            {
                if (TeamA != null && TeamA.Fighters.Contains(m))
                    aCount++;
                else if (TeamB != null && TeamB.Fighters.Contains(m))
                    bCount++;
            }

            winningTeam = aCount > bCount ? TeamA : bCount > aCount ? TeamB : null;
            ColUtility.Free(list);

            if (winningTeam != null)
            {
                //Randomly chooses runner up
                List<TeamInfo> teams = new List<TeamInfo>(Teams);
                TeamInfo winningInfo = GetTeamInfo(winningTeam);

                if (winningInfo != null && teams.Contains(winningInfo))
                    teams.Remove(winningInfo);

                ArenaTeam runnerup = teams[Utility.Random(teams.Count)].Team;

                EndFight(winningTeam, runnerUp, false);
                ColUtility.Free(teams);
            }
            else
            {
                //Randomly chooses winner and runner up
                List<TeamInfo> teams = new List<TeamInfo>(Teams);
                winningTeam = teams[Utility.Random(teams.Count)].Team;
                TeamInfo winningInfo = GetTeamInfo(winningTeam);

                if (winningInfo != null)
                    teams.Remove(winningInfo);

                ArenaTeam runnerup = teams[Utility.Random(teams.Count)].Team;

                EndFight(winningTeam, runnerup, false);
                ColUtility.Free(teams);
            }
        }

        protected virtual bool DoCTFRoundAnnouncement(int ticks, bool donehalf)
        {
            TeamInfo info1 = GetTeamInfo(TeamA);
            TeamInfo info2 = GetTeamInfo(TeamB);

            if (info1 == null || info2 == null)
                return false;

            TeamInfo winner = null;

            if (info1.Points > info2.Points)
                winner = info1;

            if (FightDuration.TotalMinutes / 2 == ticks && !donehalf)
            {
                string message;

                int winningBy = Math.Abs(info1.Points - info2.Points);

                if (info1.Points > info2.Points)
                    winner = info1;
                else if (info2.Points > info1.Points)
                    winner = info2;

                if (winner != null)
                    message = String.Format("{0} is leading {1} at Capture the Flag by {2} point{3}!", winner.Team.Name, winner == info1 ? info2.Team.Name : info1.Team.Name, winningBy, winningBy == 1 ? "" : "s");
                else
                    message = String.Format("{0} and {1} are currently tied at Capture the Flag!", info1.Team.Name, info2.Team.Name);

                ArenaHelper.DoFightRegionMessage(message, 0, System);
                ArenaHelper.DoAudienceRegionMessage(message, 0, System);

                return true;
            }

            return false;
        }

        protected virtual void DoCTFTeamAnnouncement(ArenaTeam team1, ArenaTeam team2, int points)
        {
            if (team1 == null)
                return;

            foreach (Mobile m in team1.Fighters)
            {
                if (m != null)
                    m.SendMessage(ArenaHelper.ParticipantMessageHue, "Your team has earned a point ({0} total)!", points.ToString());
            }

            if (team2 == null)
                return;

            foreach (Mobile m in team2.Fighters)
            {
                if (m != null)
                    m.SendMessage(ArenaHelper.ParticipantMessageHue, "{0} has earned a point for defending your captured flag!", team1.Name);
            }
        }
    }
}