using System;
using System.Collections.Generic;

using Server;
using Server.Custom.Events;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    // Generic redemption stone. Concrete subclasses pin a specific theme.
    // GMs spawn one of the per-theme subclasses (UndeadInvasionRewardStone
    // / DemonInvasionRewardStone / ...) in their event hub town and players
    // double-click to redeem the matching token type for that theme's
    // rewards.
    public abstract class InvasionRewardStone : Item
    {
        public abstract InvasionTheme Theme { get; }

        protected InvasionRewardStone() : base(0xEDC)
        {
            ItemID = Theme.StoneItemID;
            Hue = Theme.StoneHue;
            Name = Theme.Name + " Reward Stone";
            Movable = false;
            Weight = 50.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // Too far away.
                return;
            }

            from.CloseGump(typeof(InvasionRewardStoneGump));
            from.SendGump(new InvasionRewardStoneGump(from, Theme, 0));
        }

        protected InvasionRewardStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class UndeadInvasionRewardStone : InvasionRewardStone
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Undead; } }
        [Constructable] public UndeadInvasionRewardStone() { }
        public UndeadInvasionRewardStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class DemonInvasionRewardStone : InvasionRewardStone
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Demon; } }
        [Constructable] public DemonInvasionRewardStone() { }
        public DemonInvasionRewardStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ReptileInvasionRewardStone : InvasionRewardStone
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Reptile; } }
        [Constructable] public ReptileInvasionRewardStone() { }
        public ReptileInvasionRewardStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class ArachnidInvasionRewardStone : InvasionRewardStone
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Arachnid; } }
        [Constructable] public ArachnidInvasionRewardStone() { }
        public ArachnidInvasionRewardStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class FeyInvasionRewardStone : InvasionRewardStone
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Fey; } }
        [Constructable] public FeyInvasionRewardStone() { }
        public FeyInvasionRewardStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    public class RepondInvasionRewardStone : InvasionRewardStone
    {
        public override InvasionTheme Theme { get { return InvasionThemes.Repond; } }
        [Constructable] public RepondInvasionRewardStone() { }
        public RepondInvasionRewardStone(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}

namespace Server.Gumps
{
    // OSI Spoils-of-War style grid: 3 columns x 2 rows = 6 cells per page.
    // Each cell is a gold-bordered parchment frame with the item art centered
    // and the cost in red below. Hovering the art shows the real item OPL.
    public class InvasionRewardStoneGump : Gump
    {
        private readonly Mobile m_From;
        private readonly InvasionTheme m_Theme;
        private readonly int m_Page;
        private readonly List<Item> m_Previews = new List<Item>();

        private const int Width = 540;
        private const int Height = 460;
        private const int EntriesPerPage = 6;
        private const int Cols = 3;
        private const int CellW = 160;
        private const int CellH = 140;
        private const int CellGapX = 10;
        private const int CellGapY = 10;
        private const int GridStartX = 20;
        private const int GridStartY = 80;

        public InvasionRewardStoneGump(Mobile from, InvasionTheme theme, int page) : base(50, 50)
        {
            m_From = from;
            m_Theme = theme;
            m_Page = page;

            int tokens = CountTokens(from, theme);

            AddBackground(0, 0, Width, Height, 9270);

            // Title banner
            AddHtml(0, 14, Width, 22,
                String.Format("<center><BASEFONT COLOR=#FFFF00 size=7>{0} Rewards</BASEFONT></center>", theme.Name),
                false, false);

            // Token balance line
            AddHtml(0, 46, Width, 20,
                String.Format("<center><BASEFONT COLOR=#FFD700>{0}: <b>{1}</b></BASEFONT></center>", theme.TokenName + "s", tokens),
                false, false);

            List<Server.Custom.Events.InvasionRewardEntry> entries = theme.Rewards;

            if (entries.Count == 0)
            {
                AddHtml(0, Height / 2 - 10, Width, 40,
                    "<center><BASEFONT COLOR=#C0C0C0>No rewards available for this event yet.</BASEFONT></center>",
                    false, false);
            }
            else
            {
                int start = page * EntriesPerPage;
                int end = Math.Min(start + EntriesPerPage, entries.Count);

                for (int i = start; i < end; i++)
                {
                    int slot = i - start;
                    int col = slot % Cols;
                    int row = slot / Cols;

                    int cx = GridStartX + col * (CellW + CellGapX);
                    int cy = GridStartY + row * (CellH + CellGapY);

                    Server.Custom.Events.InvasionRewardEntry entry = entries[i];
                    bool canAfford = tokens >= entry.Cost;

                    // Cell frame (parchment / stone background)
                    AddBackground(cx, cy, CellW, CellH, 0x2421);

                    // Spawn a preview item and push its OPL packet so the client
                    // knows what stats to show on hover. Without the explicit
                    // send the client falls back to a stale serial lookup
                    // (e.g. "Peculiar Seeds").
                    Item preview = null;
                    try { preview = Activator.CreateInstance(entry.Type) as Item; }
                    catch { }

                    if (preview != null)
                    {
                        preview.Internalize();
                        m_Previews.Add(preview);

                        if (from.NetState != null)
                            from.NetState.Send(preview.PropertyList);

                        // Centered item art. Most equipment art is ~44x44 so
                        // half-offset is 22; nudged up a bit to leave room for
                        // the cost label + buy button at the bottom.
                        AddItem(cx + CellW / 2 - 22, cy + 18, preview.ItemID, preview.Hue);
                        AddItemProperty(preview.Serial);
                    }

                    // Cost label, centered, color-coded.
                    AddHtml(cx, cy + CellH - 50, CellW, 20,
                        String.Format("<center><BASEFONT COLOR={0}><b>{1}</b></BASEFONT></center>",
                            canAfford ? "#33CC33" : "#FF3333", entry.Cost),
                        false, false);

                    // Visible Buy button centered at the bottom of the cell.
                    // Green check = "purchase" -- universal positive signal,
                    // and the icon stands on its own without a label overlay.
                    AddButton(cx + CellW / 2 - 11, cy + CellH - 26, 0xFB7, 0xFB9,
                        100 + i, GumpButtonType.Reply, 0);
                }

                int totalPages = (entries.Count + EntriesPerPage - 1) / EntriesPerPage;
                if (page > 0)
                    AddButton(30, Height - 40, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0);
                if (page < totalPages - 1)
                    AddButton(Width - 50, Height - 40, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0);

                AddHtml(0, Height - 36, Width, 20,
                    String.Format("<center><BASEFONT COLOR=#C0C0C0>Page {0} of {1}</BASEFONT></center>", page + 1, totalPages),
                    false, false);
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            DeletePreviews();

            if (sender == null || sender.Mobile == null)
                return;

            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0: return;
                case 1: from.SendGump(new InvasionRewardStoneGump(from, m_Theme, m_Page - 1)); return;
                case 2: from.SendGump(new InvasionRewardStoneGump(from, m_Theme, m_Page + 1)); return;
            }

            int index = info.ButtonID - 100;
            List<Server.Custom.Events.InvasionRewardEntry> entries = m_Theme.Rewards;
            if (index < 0 || index >= entries.Count)
                return;

            Server.Custom.Events.InvasionRewardEntry entry = entries[index];

            if (!Redeem(from, m_Theme, entry))
                from.SendMessage(0x21, String.Format("You do not have enough {0}s.", m_Theme.TokenName));

            from.SendGump(new InvasionRewardStoneGump(from, m_Theme, m_Page));
        }

        public override void OnServerClose(NetState owner)
        {
            DeletePreviews();
            base.OnServerClose(owner);
        }

        private void DeletePreviews()
        {
            foreach (Item p in m_Previews)
            {
                if (p != null && !p.Deleted)
                    p.Delete();
            }
            m_Previews.Clear();
        }

        private static int CountTokens(Mobile from, InvasionTheme theme)
        {
            if (from == null || from.Backpack == null)
                return 0;
            return from.Backpack.GetAmount(theme.TokenType);
        }

        private static bool Redeem(Mobile from, InvasionTheme theme, Server.Custom.Events.InvasionRewardEntry entry)
        {
            if (from == null || from.Backpack == null)
                return false;
            if (!from.Backpack.ConsumeTotal(theme.TokenType, entry.Cost))
                return false;

            Item reward = Activator.CreateInstance(entry.Type) as Item;
            if (reward == null)
            {
                Item refund = Activator.CreateInstance(theme.TokenType, new object[] { entry.Cost }) as Item;
                if (refund != null)
                    from.AddToBackpack(refund);
                from.SendMessage(0x21, "Internal error -- tokens refunded.");
                return true;
            }

            from.AddToBackpack(reward);
            from.SendMessage(theme.StoneHue == 0 ? 0x47E : theme.StoneHue, String.Format("You redeem your tokens for {0}.", entry.DisplayName));
            return true;
        }
    }
}
