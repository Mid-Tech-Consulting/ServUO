using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.TournamentSystem
{
    /// <summary>
    /// Default values for various properties within the system
    /// </summary>
    public static class Config
    {
        public static string Version = "1.3.0.0";

        [CallPriority(int.MaxValue)]
        public static void Initialize()
        {
            Notoriety.Handler = new NotorietyHandler(PVPTournamentSystem.MobileNotoriety);
        }

        #region Persistence
        public static void Configure()
        {
            EventSink.WorldSave += OnSave;
            EventSink.WorldLoad += OnLoad;
        }

        public static string FilePath = Path.Combine("Saves/Customs", "PvPTournamentSystem.bin");

        public static bool Configured { get { return PVPTournamentSystem.SystemList.Count > 0; } }

        public static void OnSave(WorldSaveEventArgs e)
        {
            Persistence.Serialize(
                FilePath,
                writer =>
                {
                    writer.Write(1);

                    ForcedGear.Serialize(writer);

                    writer.Write(true);

                    writer.Write(PVPTournamentStats.TournamentStats.Count);
                    for (int i = 0; i < PVPTournamentStats.TournamentStats.Count; ++i)
                        PVPTournamentStats.TournamentStats[i].Serialize(writer);

                    writer.Write(ArenaTeam.Teams.Count);
                    for (int i = 0; i < ArenaTeam.Teams.Count; ++i)
                        ArenaTeam.Teams[i].Serialize(writer);

                    Timer.DelayCall(TimeSpan.FromSeconds(5), ArenaTeam.DefragTeams);
                });
        }

        public static void OnLoad()
        {
            Utility.WriteConsoleColor(ConsoleColor.Cyan, "*** Loading PVP Tournament System, Version {0}...", Version);
            var loaded = true;

            Persistence.Deserialize(
                FilePath,
                reader =>
                {
                    int version = reader.ReadInt();

                    switch (version)
                    {
                        case 1:
                            ForcedGear.Deserialize(reader);
                            goto case 0;
                        case 0:
                            loaded = reader.ReadBool();

                            int count = reader.ReadInt();

                            for (int i = 0; i < count; i++)
                                new PVPTournamentStats(reader);

                            count = reader.ReadInt();

                            for (int i = 0; i < count; i++)
                                new ArenaTeam(reader);

                            Timer.DelayCall(TimeSpan.FromSeconds(5), ArenaTeam.DefragTeams);
                            break;
                    }

                    if (PVPTournamentSystem.SystemList.Any(a => a.Tournaments.Count > 0))
                    {
                        Timer.DelayCall(TournamentBoard.UpdateBoards);
                    }
                });

            if (!loaded)
            {
                Utility.WriteConsoleColor(ConsoleColor.Cyan, "Running system for first time. Be sure to use [SetupPVPTournamentSystem to configure system.");
            }
            else
            {
                Utility.WriteConsoleColor(ConsoleColor.Cyan, "Complete! {0} arenas setup with {1} active arena teams.", PVPTournamentSystem.SystemList.Count.ToString(), ArenaTeam.Teams.Where(t => t.Active).Count().ToString());
            }
        }
        #endregion

        #region System
        /// <summary>
        /// Expire time for messages
        /// </summary>
        public static TimeSpan DefaultMessageExpire = TimeSpan.FromDays(7);

        /// <summary>
        /// Clear reagion after fight of mobiles, movable items, etc.
        /// </summary>
        public static bool NoClearRegion { get { return false; } }

        /// <summary>
        /// Uses pretty hues of fel/tram stones. If you want to use custom hues, mark this false and manually change the hue.
        /// </summary>
        public static bool UseStoneDefaultHue { get { return true; } }
        #endregion

        #region Arena Fights

        /// <summary>
        /// Do we want to nullify points/restrict fights when fighters are same IP? This could help combat point farming
        /// </summary>
        public static readonly bool RestrictSameIP = true;

        /// <summary>
        /// Can players be attacked in audience region?
        /// </summary>
        public static readonly bool AudienceRegionNoAttack = true;

        /// <summary>
        /// Can spells be cast in audience region?
        /// </summary>
        public static readonly bool AudienceRegionNoSpells = true;

        /// <summary>
        /// Randomized or static start locations for standard duels
        /// </summary>
        public static readonly bool RandomizeStartLocations = true;

        /// <summary>
        /// Team A Hue, mainly for Capture the Flag 
        /// </summary>
        public static readonly int TeamAHue = 37; // Red

        /// <summary>
        /// Team B Hue, mainly for Capture the Flag 
        /// </summary>
        public static readonly int TeamBHue = 5;  // Blue

        /// <summary>
        /// Randomized or static start locations for CTF 
        /// </summary>
        public static readonly bool CTFRandomStart = true;

        /// <summary>
        /// Delay from the moment a duel is agreed upon, to when you are teleported into the arena
        /// </summary>
        public static readonly TimeSpan StartDelay = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Delay in which you are removed from the arena at the conclusion of the duel
        /// </summary>
        public static readonly TimeSpan EjectDelay = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Delay from when you are teleported into the arena, to when the duel actuall begins. This is also known as the wall period
        /// </summary>
        public static readonly TimeSpan PreFightDelay = TimeSpan.FromSeconds(15);

        /// <summary>
        /// Default time the duel will last
        /// </summary>
        public static readonly TimeSpan DefaultDuration = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Minimum teams you need to start a last man standing fight
        /// </summary>
        public static readonly int MinLastManStanding = 3;

        /// <summary>
        /// Default team type (single, twosome, foursome) for a standard duel
        /// </summary>
        public static readonly ArenaTeamType DefaultTeamType = ArenaTeamType.Single;

        /// <summary>
        /// Gives players teh option of using own gear, or standardized gear for an even playing field.
        /// </summary>
        public static readonly bool AllowStandardizedGear = true;

        public static readonly Type[] NonConsumableList =
        {
            typeof(BasePotion), typeof(SpellScroll), typeof(OrangePetals), typeof(RoseOfTrinsicPetal), typeof(BaseBalmOrLotion), typeof(BaseMagicalFood),
            typeof(Food), typeof(BaseBeverage), typeof(BaseFishPie)
        };

        #endregion

        #region Tournaments
        /// <summary>
        /// The time prior to the actual tournament start time where the arena will be come un-available for other duels
        /// </summary>
        public static readonly TimeSpan TournamentSetupTime = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Time between duels during a tournament
        /// </summary>
        public static readonly TimeSpan FightWait = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Time between each round in a tournamnet
        /// </summary>
        public static readonly TimeSpan RoundWait = TimeSpan.FromMinutes(2);

        /// <summary>
        /// Minimum time (from current time) you may schedule a tournament. For example, if its the 1st of the month, you have to schedule the tournament 
        /// out to at least the sixth of the month.
        /// </summary>
        public static readonly TimeSpan TournamentWait = TimeSpan.FromDays(5);

        /// <summary>
        /// In the event the server restarts during a tournament, this will be the wait time for players to get ready once the server is
        /// rebooted.
        /// </summary>
        public static readonly TimeSpan TournamentResumeTime = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Minimum participants a tounrmanet must have to resume. If minimum is not met, the tournament will be canceled
        /// </summary>
        public static readonly int MinEntries = 4;

        /// <summary>
        /// Maximum participants in a tournament
        /// </summary>
        public static readonly int MaxEntries = 40;

        /// <summary>
        /// Max Tournament Entry Fee. Set to -1 if you don't want a max entry fee
        /// </summary>
        public static readonly int MaxEntryFee = 500000;

        /// <summary>
        /// Maximum tournaments each arena can have pending at one time
        /// </summary>
        public static readonly int MaxTournaments = 10;

        /// <summary>
        /// Use alternate arena default value. If the arena has a fel/tram counterpart, it will use that arena for the arena duels, 
        /// as well as the actual arena. This is recommended for large tournaments and to speed the tournament up.
        /// </summary>
        public static readonly bool UseAlternateArena = false;
        #endregion
    }
}
