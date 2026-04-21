using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands
{
    public static class TransferCharCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("TransferChar", AccessLevel.Administrator, OnCommand);
        }

        [Usage("TransferChar [<sourceAccount> <charName> <destAccount>]")]
        [Description("Transfer a character between accounts. No args opens a gump; with args performs the transfer directly. Admin only.")]
        public static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length == 0)
            {
                if (from is PlayerMobile)
                {
                    BaseGump.SendGump(new TransferCharGump((PlayerMobile)from));
                }
                return;
            }

            if (e.Length != 3)
            {
                from.SendMessage("Usage: TransferChar <sourceAccount> <charName> <destAccount>");
                from.SendMessage("Or run [TransferChar with no args to open the gump.");
                return;
            }

            string srcName = e.GetString(0);
            string charName = e.GetString(1);
            string destName = e.GetString(2);

            string result = PerformTransfer(srcName, charName, destName);
            from.SendMessage(result);
        }

        public static string PerformTransfer(string srcName, string charName, string destName)
        {
            Account src = Accounts.GetAccount(srcName) as Account;
            Account dest = Accounts.GetAccount(destName) as Account;

            if (src == null)
                return string.Format("Source account '{0}' not found.", srcName);

            if (dest == null)
                return string.Format("Destination account '{0}' not found.", destName);

            if (src == dest)
                return "Source and destination are the same account.";

            int srcSlot = -1;
            Mobile m = null;
            for (int i = 0; i < src.Length; i++)
            {
                Mobile candidate = src[i];
                if (candidate != null && Insensitive.Equals(candidate.Name, charName))
                {
                    srcSlot = i;
                    m = candidate;
                    break;
                }
            }

            if (m == null)
                return string.Format("Character '{0}' not found on account {1}.", charName, src.Username);

            int freeSlot = -1;
            for (int i = 0; i < dest.Length; i++)
            {
                if (dest[i] == null) { freeSlot = i; break; }
            }

            if (freeSlot == -1)
                return string.Format("Destination account {0} has no free character slots.", dest.Username);

            if (m.NetState != null)
            {
                m.SendMessage("You are being transferred to another account. Please log back in.");
                m.NetState.Dispose();
            }

            src[srcSlot] = null;
            dest[freeSlot] = m;

            return string.Format("Transferred {0} from {1} (slot {2}) -> {3} (slot {4}).",
                m.Name, src.Username, srcSlot, dest.Username, freeSlot);
        }
    }

    public class TransferCharGump : BaseGump
    {
        private const int SourceEntryId = 100;
        private const int DestEntryId = 101;
        private const int LoadButtonId = 1;
        private const int CharButtonBase = 1000;

        private string _SourceName;
        private string _DestName;
        private Account _Source;

        public TransferCharGump(PlayerMobile user, string sourceName = "", string destName = "")
            : base(user, 100, 100)
        {
            _SourceName = sourceName ?? "";
            _DestName = destName ?? "";

            if (!string.IsNullOrWhiteSpace(_SourceName))
            {
                _Source = Accounts.GetAccount(_SourceName) as Account;
            }
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 420, 440, 9200);
            AddBackground(10, 10, 400, 420, 3500);

            AddLabel(140, 22, 53, "Character Transfer");

            // Source row
            AddLabel(25, 60, 0, "Source Account:");
            AddBackground(160, 57, 180, 24, 9300);
            AddTextEntry(165, 60, 170, 20, 0, SourceEntryId, _SourceName);
            AddButton(345, 60, 4005, 4007, LoadButtonId, GumpButtonType.Reply, 0);
            AddLabel(380, 60, 0, "Load");

            // Dest row
            AddLabel(25, 90, 0, "Dest Account:");
            AddBackground(160, 87, 180, 24, 9300);
            AddTextEntry(165, 90, 170, 20, 0, DestEntryId, _DestName);

            AddLabel(25, 120, 53, "Characters on source:");

            if (_Source == null)
            {
                if (!string.IsNullOrWhiteSpace(_SourceName))
                    AddLabel(25, 145, 33, string.Format("Account '{0}' not found.", _SourceName));
                else
                    AddLabel(25, 145, 0, "Enter a source account name and click Load.");
            }
            else
            {
                int y = 145;
                bool anyChars = false;
                for (int i = 0; i < _Source.Length; i++)
                {
                    Mobile m = _Source[i];
                    if (m == null) continue;
                    anyChars = true;

                    AddButton(30, y + 2, 4005, 4007, CharButtonBase + i, GumpButtonType.Reply, 0);
                    AddLabel(65, y, 0, string.Format("Slot {0}: {1}", i, m.Name));
                    y += 24;
                }

                if (!anyChars)
                    AddLabel(25, 145, 33, "No characters on this account.");

                AddLabel(25, 400, 53, "Click a character to transfer to Dest Account.");
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            TextRelay srcEntry = info.GetTextEntry(SourceEntryId);
            TextRelay destEntry = info.GetTextEntry(DestEntryId);

            string srcName = srcEntry != null ? srcEntry.Text.Trim() : "";
            string destName = destEntry != null ? destEntry.Text.Trim() : "";

            if (info.ButtonID == LoadButtonId)
            {
                BaseGump.SendGump(new TransferCharGump(User, srcName, destName));
                return;
            }

            if (info.ButtonID >= CharButtonBase && info.ButtonID < CharButtonBase + 10)
            {
                int slot = info.ButtonID - CharButtonBase;
                Account src = Accounts.GetAccount(srcName) as Account;

                if (src == null || slot < 0 || slot >= src.Length)
                {
                    User.SendMessage("Source account invalid.");
                    return;
                }

                Mobile m = src[slot];
                if (m == null)
                {
                    User.SendMessage("That slot is empty.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(destName))
                {
                    User.SendMessage("Enter a destination account first.");
                    BaseGump.SendGump(new TransferCharGump(User, srcName, destName));
                    return;
                }

                string result = TransferCharCommand.PerformTransfer(srcName, m.Name, destName);
                User.SendMessage(result);

                BaseGump.SendGump(new TransferCharGump(User, srcName, destName));
            }
        }
    }
}
