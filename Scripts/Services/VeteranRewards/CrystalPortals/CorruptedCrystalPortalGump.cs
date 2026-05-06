using System;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class CorruptedCrystalPortalGump : Gump
	{
		private const int EntriesPerPage = 12;

		private readonly Mobile m_From;
		private readonly CorruptedCrystalPortal m_Portal;
		private readonly int m_PageIndex;

		public CorruptedCrystalPortalGump(Mobile from, CorruptedCrystalPortal portal)
			: this(from, portal, 0) { }

		public CorruptedCrystalPortalGump(Mobile from, CorruptedCrystalPortal portal, int pageIndex)
			: base(50, 50)
		{
			m_From = from;
			m_Portal = portal;
			m_PageIndex = pageIndex;

			from.CloseGump(typeof(CorruptedCrystalPortalGump));

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddBackground(0, 0, 380, 460, 9270);

			AddHtml(0, 12, 380, 22, "<center><basefont color=#FFFFFF>Corrupted Crystal Portal</basefont></center>", false, false);
			AddHtml(0, 38, 380, 18, "<center><basefont color=#CCCCCC>Select a dungeon</basefont></center>", false, false);
			AddHtml(0, 56, 380, 18, "<center><basefont color=#999999>(or SAY the destination name)</basefont></center>", false, false);

			CorruptedCrystalPortal.Destination[] dests = CorruptedCrystalPortal.AllDestinations;
			int totalPages = Math.Max(1, (dests.Length + EntriesPerPage - 1) / EntriesPerPage);
			int page = Math.Max(0, Math.Min(pageIndex, totalPages - 1));

			int start = page * EntriesPerPage;
			int end = Math.Min(start + EntriesPerPage, dests.Length);

			int y = 86;
			for (int i = start; i < end; i++)
			{
				CorruptedCrystalPortal.Destination d = dests[i];
				AddButton(40, y + 1, 0x4B9, 0x4BA, 1000 + i, GumpButtonType.Reply, 0);
				AddHtml(70, y, 280, 22, "<basefont color=#FFFFFF>" + FormatLabel(d) + "</basefont>", false, false);
				y += 26;
			}

			// Footer
			AddHtml(40, 420, 100, 22, String.Format("<basefont color=#FFFFFF>Page {0} / {1}</basefont>", page + 1, totalPages), false, false);

			AddButton(150, 420, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0); // Close
			AddHtml(185, 420, 60, 22, "<basefont color=#FFFFFF>Close</basefont>", false, false);

			if (page > 0)
				AddButton(240, 420, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0); // Prev

			if (page + 1 < totalPages)
				AddButton(340, 420, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0); // Next
		}

		private static string FormatLabel(CorruptedCrystalPortal.Destination d)
		{
			// Tag the facet so duplicates between Tram/Fel are distinguishable.
			return String.Format("{0} ({1})", d.Label, d.Map.Name);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (m_From == null || m_From.Deleted || sender.Mobile != m_From)
				return;

			if (m_Portal == null || m_Portal.Deleted)
				return;

			int id = info.ButtonID;

			if (id == 0)
				return;

			if (id == 1)
			{
				m_From.SendGump(new CorruptedCrystalPortalGump(m_From, m_Portal, m_PageIndex - 1));
				return;
			}

			if (id == 2)
			{
				m_From.SendGump(new CorruptedCrystalPortalGump(m_From, m_Portal, m_PageIndex + 1));
				return;
			}

			int destIndex = id - 1000;
			CorruptedCrystalPortal.Destination[] dests = CorruptedCrystalPortal.AllDestinations;

			if (destIndex < 0 || destIndex >= dests.Length)
				return;

			if (!m_From.InRange(m_Portal.Location, 3))
			{
				m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return;
			}

			CorruptedCrystalPortal.Destination d = dests[destIndex];

			if (d.RequireAbyssEntry)
			{
				PlayerMobile pm = m_From as PlayerMobile;
				if (pm == null || !pm.AbyssEntry)
				{
					m_From.SendLocalizedMessage(1112226);
					return;
				}
			}

			m_Portal.TryTeleport(m_From, d.Location, d.Map);
		}
	}
}
