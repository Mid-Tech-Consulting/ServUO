using System;
using System.Collections.Generic;

using Server;

namespace Server.TournamentSystem
{
    public static class Localizations
    {
        public static Dictionary<int, string> LocalizationTable { get; set; }

        public static void Configure()
        {
            LocalizationTable = new Dictionary<int, string>();

            LocalizationTable[1] = "View Statistics for previously completed tournaments";
            LocalizationTable[2] = "View a detailed 'how to' on utilizing this system";
            LocalizationTable[3] = "View detailed statistics of each arena team";
            LocalizationTable[4] = "View detailed statistics of your arena teams";
            LocalizationTable[5] = "View details on any future tournaments";
            LocalizationTable[6] = "Register a single, twosome or foursome arena team";
            LocalizationTable[7] = "Furture tournament registration";
            LocalizationTable[8] = "Sign up for a quick, 1 v 1 match. Target your opponent...";
            LocalizationTable[9] = "Register a new arena fight";
            LocalizationTable[10] = "Select your team size: Single, Twosome, or Foursome";
            LocalizationTable[11] = "Choose an appropriate team name. Must be unique and no more than 16 characters";
            LocalizationTable[12] = "Choose a player for your team";
            LocalizationTable[13] = "Like a wager, entry fees are collected from each team member, once that team joins the tournament";
            LocalizationTable[14] = "Tournament Start Time - Month";
            LocalizationTable[15] = "Tournament Start Time - Day";
            LocalizationTable[16] = "Tournament Start Time - Hour";
            LocalizationTable[17] = "Tournament Type - this determines the way players are eliminated";
            LocalizationTable[18] = "Choose the team size that can enter the tournament";
            LocalizationTable[19] = "Choose a tournament Style - this determines the type of player that can join";
            LocalizationTable[20] = "Choose a fight duration, in minutes";
            LocalizationTable[21] = "Use this arena's tram/fel counterpart to speed up the tournament";
            LocalizationTable[22] = "Magery spells only - no other spells schools allowed";
            LocalizationTable[23] = "Allow/Disallow all spells schools";
            LocalizationTable[24] = "Allow/Disallow consuming buff items, ie potions";
            LocalizationTable[25] = "Allow/Disallow casting spells prior to the wall dropping";
            LocalizationTable[26] = "Allow/Disallow summoning spells, ie Energy Vortex or Summon Daemon";
            LocalizationTable[27] = "Allow/Disallow weapon special moves";
            LocalizationTable[28] = "Allow/Disallow resurrecting - only pertains to Twosome or Foursome fights";
            LocalizationTable[29] = "Allow/Disallow mounts";
            LocalizationTable[30] = "Allow/Disallow ties. No ties will result in a tie breaker in the event time runs out";
            LocalizationTable[31] = "Allow/Disallow area spells, ie wither and earthquake";
            LocalizationTable[32] = "Choose the reward item for the tournament champion";
            LocalizationTable[33] = "Choose the reward item for the tournament runner up";
            LocalizationTable[34] = "Fight ends when all players from one team are dead";
            LocalizationTable[35] = "Best out of 3 fights determine the winner";
            LocalizationTable[36] = "Free for all, everybody vs everybody, last person alive wins!";
            LocalizationTable[37] = "Select your opponent";
            LocalizationTable[38] = "The challenging team can choose a wager in gold. This wager applies to <b>each</b> fighter in each team ";
            LocalizationTable[39] = "and will be automatically deducted from their account once the fight is agreed upon";
            LocalizationTable[40] = "View Fighters";
            LocalizationTable[41] = "Any type of character may join the tournament";
            LocalizationTable[42] = "Only magery based characters can join the tournament";
            LocalizationTable[43] = "Only dexxer based characters can join the tournament";
            LocalizationTable[44] = "Once a team loses, they are out of the tournament";
            LocalizationTable[45] = "On a teams first defeat, they continue on. Once they are defeated for the second time,";
            LocalizationTable[46] = "they are eliminated from the tournament";
            LocalizationTable[47] = "Same as single elimination, however the two teams will have a best of 3 duel to determine who";
            LocalizationTable[48] = "moves on in the tournament";
            LocalizationTable[49] = "Designed to give a pass to those with pre-fight jitters. Losers of round one will all fight in";
            LocalizationTable[50] = "round two, where only those winners can continue on";

            LocalizationTable[51] = "- Once a team loses, they are out of the tournament";
            LocalizationTable[52] = "- Once a team loses for the second time, they are out of the tournament";
            LocalizationTable[53] = "- Once a team loses a best of 3 duel, they are out of the tournament";
            LocalizationTable[54] = "- Round one losers move to a losers bracket, where next loss they are out of the tournament";

            LocalizationTable[55] = "- Any type of character may join the tournament";
            LocalizationTable[56] = "- Only magery based characters can join the tournament";
            LocalizationTable[57] = "- Only dexxer based characters can join the tournament";

            LocalizationTable[58] = "If not using your own gear, each player will utilize a standardized robe.";
            LocalizationTable[59] = "Your current gear will be moved to your bankbox once the duel begins.";
        }

        public static string GetLocalization(int cliloc)
        {
            if (LocalizationTable.ContainsKey(cliloc))
            {
                return LocalizationTable[cliloc];
            }

            return String.Empty;
        }

        public static string GetStyleTooltip(Tournament tournament)
        {
            switch (tournament.TourneyStyle)
            {
                default:
                case TourneyStyle.Standard: return Localizations.GetLocalization(55);
                case TourneyStyle.MagesOnly: return Localizations.GetLocalization(56);
                case TourneyStyle.DexxersOnly: return Localizations.GetLocalization(57);
            }
        }

        public static string GetTournyTypeTooltip(Tournament tournament)
        {
            switch (tournament.TourneyType)
            {
                default:
                case TourneyType.SingleElim: return Localizations.GetLocalization(51);
                case TourneyType.DoubleElimination: return Localizations.GetLocalization(52);
                case TourneyType.BestOf3: return Localizations.GetLocalization(53);
                case TourneyType.Bracketed: return Localizations.GetLocalization(54);
            }
        }
    }
}
