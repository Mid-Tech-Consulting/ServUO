using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Network;
using Server.Accounting;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;
using Server.Gumps;
using Server.Spells.Necromancy;
using Server.Spells.Fourth;
using Server.Spells.Mysticism;
using Server.Items;
using Server.Engines.NewMagincia;

//#define UsingKnivesChat //Un-Comment out if you prefer knives chat mail messages
#if UsingKnivesChat
using Knives.Chat3;
#endif

namespace Server.TournamentSystem
{
    public static class ArenaHelper
    {
        public static void Initialize()
        {
            CommandSystem.Register("MyTeams", AccessLevel.Player, MyTeams_OnCommand);
            CommandSystem.Register("AllTeams", AccessLevel.Player, ArenaTeams_OnCommand);
            CommandSystem.Register("ClearTournamentStatistics", AccessLevel.Administrator, ClearStats_OnCommand);
            CommandSystem.Register("ViewTeams", AccessLevel.GameMaster, ViewTeams_OnCommand);
        }

        [Usage("MyTeams")]
        [Description("Displays Team Info Gump.")]
        public static void MyTeams_OnCommand(CommandEventArgs e)
        {
            List<ArenaTeam> teams = ArenaTeam.GetTeams(e.Mobile);

            if (teams != null)
            {
                if (e.Mobile.HasGump(typeof(MyTeamStatsGump)))
                    e.Mobile.CloseGump(typeof(MyTeamStatsGump));

                if(e.Mobile is PlayerMobile)
                    BaseGump.SendGump(new MyTeamStatsGump(e.Mobile as PlayerMobile, teams));
                //e.Mobile.SendGump(new MyTeamStatsGump(e.Mobile, teams));
            }
        }

        [Usage("ArenaTeams")]
        [Description("Displays rankings of all arena teams.")]
        public static void ArenaTeams_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile.HasGump(typeof(PlayerStatsGump)))
                e.Mobile.CloseGump(typeof(PlayerStatsGump));

            if(e.Mobile is PlayerMobile)
                e.Mobile.SendGump(new PlayerStatsGump(e.Mobile as PlayerMobile));
        }

        [Usage("ClearTournamentStatistics")]
        [Description("Clears all tourny stats.")]
        public static void ClearStats_OnCommand(CommandEventArgs e)
        {
            PVPTournamentStats.ClearStats();
            e.Mobile.SendMessage("All Tourny Stats Cleared!");
        }

        [Usage("ViewTeams")]
        [Description("Counselor+ can view a players arena teams, kick members, change name, etc.")]
        public static void ViewTeams_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Target the player who you'd like to view the arena teams for.");
            e.Mobile.BeginTarget(12, false, TargetFlags.None, new TargetCallback(ViewTeams_OnTarget));
        }

        public static bool HasArena<TArena>() where TArena : PVPTournamentSystem
        {
            return PVPTournamentSystem.SystemList.Any(sys => sys.GetType() == typeof(TArena));
        }

        public static PVPTournamentSystem GetArena<TArena>()
        {
            return PVPTournamentSystem.SystemList.FirstOrDefault(sys => sys.GetType() == typeof(TArena));
        }

        public static void ViewTeams_OnTarget(Mobile from, object obj)
        {
            if (obj is PlayerMobile)
            {
                Mobile mob = (Mobile)obj;
                List<ArenaTeam> teams = ArenaTeam.GetTeams(mob);

                if (teams != null)
                {
                    foreach (ArenaTeam team in teams)
                        from.SendGump(new PropertiesGump(from, team));

                    if (from.AccessLevel >= AccessLevel.GameMaster)
                        from.SendMessage("Each team has been displayed via props gump. DO NOT change properties if you are unsure of the results!");
                }
                else
                    from.SendMessage("That player does not belong to any arena teams.");

                ColUtility.Free(teams);
            }
            else
                from.SendMessage("You must target a player!");
        }

        public static bool CanRecievePoints(ArenaTeam a, ArenaTeam b)
        {
            foreach (Mobile moba in a.Fighters)
            {
                foreach (Mobile mobb in b.Fighters)
                {
                    if (IsSameAccount(moba, mobb))
                        return false;
                }
            }
            return true;
        }

        public static string GetTourneyStyle(Tournament t)
        {
            switch (t.TourneyStyle)
            {
                default:
                case TourneyStyle.Standard: return "Standard";
                case TourneyStyle.MagesOnly: return "Mages Only";
                case TourneyStyle.DexxersOnly: return "Dexxers Only";
            }
        }

        public static string GetTourneyType(Tournament t)
        {
            if (t == null)
                return "Unknown";

            return GetTourneyType(t.TourneyType);
        }

        public static string GetTourneyType(TourneyType type)
        {
            switch (type)
            {
                default:
                case TourneyType.SingleElim: return "Single Elimination";
                case TourneyType.BestOf3: return "Best of 3";
                case TourneyType.Bracketed: return "Bracketed";
                case TourneyType.DoubleElimination: return "Double Elimination";
            }
        }

        public static string GetRules(FightRules rules)
        {
            switch (rules)
            {
                default: return "None";
                case FightRules.NoPrecasting: return "No Precasting";
                case FightRules.NoPots: return "No Consumables";
                case FightRules.NoSummons: return "No Summons";
                case FightRules.NoSpecials: return "No Specials";
                case FightRules.PureMage: return "Pure Mage";
                case FightRules.PureDexxer: return "Pure Dexxer";
                case FightRules.AllowResurrections: return "Allow Resurrections";
                case FightRules.AllowMounts: return "Allow Mounts";
                case FightRules.NoTies: return "No Ties";
                case FightRules.NoAreaSpells: return "No Area Spells";
            }
        }

        public static readonly int AudienceMessageHue = 1154;
        public static readonly int ParticipantMessageHue = 1159;
        public static readonly int EquipmentMessageHue = 2748;
            
        public static string GetFightType(ArenaFightType type)
        {
            switch (type)
            {
                default:
                case ArenaFightType.SingleElimination: return "Single Elimination";
                case ArenaFightType.BestOf3: return "Best of 3";
                case ArenaFightType.LastManStanding: return "Last Man Standing";
                case ArenaFightType.CaptureTheFlag: return "Capture the Flag";
            }
        }

        public static void DoArenaKeeperMessage(string message, Mobile m)
        {
            if (m.Map != null)
            {
                IPooledEnumerable eable = m.Map.GetMobilesInRange(m.Location, 12);

                foreach (Mobile mob in eable)
                {
                    if (mob is ArenaKeeper)
                        mob.SayTo(m, false, message, m.Name);
                }

                eable.Free();
            }
        }

        public static void DoRegionMessage(string message, int sound, PVPTournamentSystem system)
        {
            DoAudienceRegionMessage(message, sound, system);
            DoFightRegionMessage(message, sound, system);
        }

        public static void DoAudienceRegionMessage(string message, int sound, PVPTournamentSystem system)
        {
            if (system == null || system.AudienceRegion == null)
                return;

            foreach (Mobile m in system.AudienceRegion.GetEnumeratedMobiles().OfType<PlayerMobile>())
            {
                if(message != null)
                    m.SendMessage(AudienceMessageHue, message);

                if (sound > 0)
                    m.SendSound(sound);
            }
        }

        public static void DoFightRegionMessage(string message, int sound, PVPTournamentSystem system, Mobile omit = null)
        {
            if (system == null || system.FightRegion == null)
                return;

            foreach (Mobile m in system.FightRegion.GetEnumeratedMobiles().OfType<PlayerMobile>().Where(m => omit == null || m != omit))
            {
                if (message != null)
                    m.SendMessage(AudienceMessageHue, message);

                if (sound > 0)
                    m.SendSound(sound);
            }
        }

        public static bool IsSameAccount(Mobile a, Mobile b)
        {
            if (a.AccessLevel > AccessLevel.Player && b.AccessLevel > AccessLevel.Player)
                return false;

            Account actA = a.Account as Account;
            Account actB = b.Account as Account;

            if (actB != null && actA != null && actA == actB)
                return true;

            if (Server.Engines.ArenaSystem.PVPArenaSystem.IsSameIP(a, b))
                return true;

            return false;
        }

        public static void DoTeamPM(ArenaTeam team, string subject, string text, Mobile sender)
        {
            DoTeamPM(team, subject, text, sender, Config.DefaultMessageExpire);
        }

        public static void DoTeamPM(ArenaTeam team, string subject, string text, Mobile sender, TimeSpan expires)
        {
            if (team == null)
                return;

            foreach (Mobile mob in team.Fighters)
            {
                DoTeamPM(mob, subject, text, sender, expires);
            }
        }

        public static void DoTeamPM(Mobile to, string subject, string text, Mobile sender)
        {
            DoTeamPM(to, subject, text, sender, Config.DefaultMessageExpire);
        }

        public static void DoTeamPM(Mobile to, string subject, string text, Mobile sender, TimeSpan expires)
        {
#if UsingKnivesChat
            if (sender != null)
            {
                Data data = Data.GetData(to);
                data.AddMessage(new Message(sender, subject, text, MsgType.System));
                General.PmNotify(data.Mobile);
            }
            else if (to.NetState != null)
                to.SendMessage(text);
#else
            MaginciaLottoSystem.SendMessageTo(to, subject, text, expires);
#endif
        }

        public static int GetTeamTypeColor(ArenaTeamType type)
        {
            switch (type)
            {
                default:
                case ArenaTeamType.None: return 0;
                case ArenaTeamType.Single: return 1152;
                case ArenaTeamType.Twosome: return 53;
                case ArenaTeamType.Foursome: return 62;
            }
        }

        public static void DoResurrect(Mobile m)
        {
            if (m == null || m.Alive)
                return;

            m.PlaySound(0x1F2);
            m.FixedEffect(0x376A, 10, 16);
            m.Resurrect();

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), ArenaKeeper.AfterResurrection, m);
        }

        public static void RemoveCurses(Mobile m)
        {
            if (m == null)
                return;

            m.PlaySound(0xF6);
            m.PlaySound(0x1F7);
            m.FixedParticles(0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head);

            IEntity from = new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z - 10), m.Map);
            IEntity to = new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z + 50), m.Map);
            Effects.SendMovingParticles(from, to, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100);

            StatMod mod;

            mod = m.GetStatMod("[Magic] Str Offset");
            if (mod != null && mod.Offset < 0)
                m.RemoveStatMod("[Magic] Str Offset");

            mod = m.GetStatMod("[Magic] Dex Offset");
            if (mod != null && mod.Offset < 0)
                m.RemoveStatMod("[Magic] Dex Offset");

            mod = m.GetStatMod("[Magic] Int Offset");
            if (mod != null && mod.Offset < 0)
                m.RemoveStatMod("[Magic] Int Offset");

            EvilOmenSpell.TryEndEffect(m);
            StrangleSpell.RemoveCurse(m);
            CorpseSkinSpell.RemoveCurse(m);
            CurseSpell.RemoveEffect(m);
            MortalStrike.EndWound(m);
            BloodOathSpell.RemoveCurse(m);
            MindRotSpell.ClearMindRotScalar(m);
            SleepSpell.EndSleep(m);

            BuffInfo.RemoveBuff(m, BuffIcon.Clumsy);
            BuffInfo.RemoveBuff(m, BuffIcon.FeebleMind);
            BuffInfo.RemoveBuff(m, BuffIcon.Weaken);
            BuffInfo.RemoveBuff(m, BuffIcon.Curse);
            BuffInfo.RemoveBuff(m, BuffIcon.MassCurse);
            BuffInfo.RemoveBuff(m, BuffIcon.MortalStrike);
            BuffInfo.RemoveBuff(m, BuffIcon.Mindrot);

            m.SendMessage("You have been spared of all curses!");
        }

        public static void ClearRegion(PVPTournamentSystem system)
        {
            if (system.FightRegion == null || system.NoClearRegion)
                return;

            Region r = system.FightRegion;

            foreach (var m in r.GetEnumeratedMobiles())
            {
                if (m is BaseCreature)
                {
                    if (((BaseCreature)m).Summoned)
                    {
                        Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                        Effects.PlaySound(m, m.Map, 0x201);

                        m.Delete();
                    }
                    else
                    {
                        RemoveFromRegion(m, system);
                    }
                }
            }

            foreach (var i in r.GetEnumeratedItems())
            {
                if (i.Movable || (i is Corpse && ((Corpse)i).Owner is PlayerMobile))
                {
                    RemoveFromRegion(i, system);
                }
            }
        }

        public static void RemoveFromRegion(IEntity e, PVPTournamentSystem system)
        {
            if (e is Item)
                ((Item)e).MoveToWorld(system.GetRandomKickLocation(), system.ArenaMap);
            else if (e is Mobile)
                ((Mobile)e).MoveToWorld(system.GetRandomKickLocation(), system.ArenaMap);
        }

        public static bool CheckWager(ArenaTeam team, int amount)
        {
            if (amount <= 0)
                return true;

            foreach (var pm in team.Fighters.OfType<PlayerMobile>())
            {
                if (pm.AccountGold.TotalCurrency < amount)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool CheckAvailable(ArenaTeam team)
        {
            foreach (var arena in PVPTournamentSystem.SystemList)
            {
                // Already fighting
                if (arena.CurrentFight != null && arena.CurrentFight.Teams.Select(x => x.Team).Any(t => t == team))
                {
                    return false;
                }

                // in queue to fight
                foreach (var fight in arena.Queue)
                {
                    if (fight.Teams.Select(x => x.Team).Any(t => t == team))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public class ActionTimer : Timer
    {
        private static Dictionary<Mobile, ActionTimer> m_WaitingAction = new Dictionary<Mobile, ActionTimer>();
        public static Dictionary<Mobile, ActionTimer> WaitingAction { get { return m_WaitingAction; } }

        private Mobile m_From;
        private Mobile m_To;

        public ActionTimer(Mobile from, Mobile to, TimeSpan wait) : base(wait)
        {
            m_From = from;
            m_To = to;

            m_WaitingAction[m_To] = this;

            Start();
        }

        public static void EndAction(Mobile to)
        {
            if (!m_WaitingAction.ContainsKey(to))
                return;

            if (m_WaitingAction[to] != null)
                m_WaitingAction[to].Stop();

            m_WaitingAction.Remove(to);
        }

        protected override void OnTick()
        {
            m_To.SendMessage("Action timed out.");
            m_From.SendMessage("Action for {0} has timed out.", m_To.Name);

            if (m_To.HasGump(typeof(AddFighterGump)))
                m_To.CloseGump(typeof(AddFighterGump));
            if (m_To.HasGump(typeof(ConfirmFightGump)))
                m_To.CloseGump(typeof(ConfirmFightGump));

            this.Stop();

            if (m_WaitingAction.ContainsKey(m_To))
                m_WaitingAction.Remove(m_To);
        }
    }

    public class LastManStandingActionTimer : Timer
    {
        private static Dictionary<Mobile, LastManStandingActionTimer> m_WaitingAction = new Dictionary<Mobile, LastManStandingActionTimer>();
        public static Dictionary<Mobile, LastManStandingActionTimer> WaitingAction { get { return m_WaitingAction; } }

        private Mobile m_From;
        private List<Mobile> m_ToList;
        private ArenaFight m_Fight;

        public List<Mobile> ToList { get { return m_ToList; } }

        public LastManStandingActionTimer(Mobile from, List<Mobile> toList, TimeSpan wait, ArenaFight fight) : base(wait)
        {
            m_From = from;
            m_ToList = toList;
            m_Fight = fight;

            foreach (Mobile m in toList)
                m_WaitingAction[m] = this;
        }

        public static void OnGumpSelection(Mobile from)
        {
            LastManStandingActionTimer timer = null;

            if (m_WaitingAction.TryGetValue(from, out timer))
            {
                timer.EndAction(from);
            }
        }

        public void EndAction(Mobile to)
        {
            if (m_ToList.Contains(to))
                m_ToList.Remove(to);

            if(m_WaitingAction.ContainsKey(to))
                m_WaitingAction.Remove(to);

            if (m_ToList.Count == 0 && m_Fight != null)
            {
                m_Fight.ActionTimedOut(m_From);

                ColUtility.Free(m_ToList);
                m_WaitingAction.Clear();

                Stop();
            }
        }

        protected override void OnTick()
        {
            foreach (Mobile m in m_ToList)
            {
                if (m_WaitingAction.ContainsKey(m))
                    m_WaitingAction.Remove(m);

                if (m_Fight != null)
                    m_Fight.ActionTimedOut(m_From);
            }

            ColUtility.Free(m_ToList);
            m_WaitingAction.Clear();
        }
    }

    public enum SortBy
    {
        None = 0,
        SortByArenaWins,
        SortByTournamentWins,
        SortByTournamentChampionships,
        SortByPoints,
        SortByDamageRatio
    }

    public class ArenaStatsSorter : IComparer<ArenaTeam>
    {
        private SortBy m_SortBy;

        public ArenaStatsSorter(SortBy sort)
        {
            m_SortBy = sort;
        }

        public int Compare(ArenaTeam x, ArenaTeam y)
        {
            switch (m_SortBy)
            {
                case SortBy.SortByArenaWins:
                    return x.Wins.CompareTo(y.Wins);
                case SortBy.SortByTournamentWins:
                    return x.TournamentWins.CompareTo(y.TournamentWins);
                case SortBy.SortByTournamentChampionships:
                    return x.TournamentChampionships.CompareTo(y.TournamentChampionships);
                case SortBy.SortByPoints:
                    return x.Points.CompareTo(y.Points);
                case SortBy.SortByDamageRatio:
                    return (x.DamageGiven - x.DamageTaken).CompareTo(y.DamageGiven - y.DamageTaken);
            }
            return 0;
        }

        public static List<ArenaTeam> GetSortedData(List<ArenaTeam> teams, SortBy sortBy)
        {
            if (teams == null)
                return null;

            List<ArenaTeam> output = new List<ArenaTeam>(teams);
            output.Sort(new ArenaStatsSorter(sortBy));
            return output;
        }
    }
}