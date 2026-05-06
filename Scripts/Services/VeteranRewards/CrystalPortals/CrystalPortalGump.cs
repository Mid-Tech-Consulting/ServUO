using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
	public class CrystalPortalGump : Gump
	{
		// Display order for the facet picker page.
		private static readonly Map[] _Facets = new[]
		{
			Map.Trammel,
			Map.Felucca,
			Map.Ilshenar,
			Map.Malas,
			Map.Tokuno,
			Map.TerMur
		};

		private const int EntriesPerPage = 10;

		private readonly Mobile m_From;
		private readonly CrystalPortal m_Portal;
		private readonly Map m_Facet;          // null = facet picker
		private readonly int m_PageIndex;

		public CrystalPortalGump(Mobile from, CrystalPortal portal)
			: this(from, portal, null, 0) { }

		public CrystalPortalGump(Mobile from, CrystalPortal portal, Map facet, int pageIndex)
			: base(50, 50)
		{
			m_From = from;
			m_Portal = portal;
			m_Facet = facet;
			m_PageIndex = pageIndex;

			from.CloseGump(typeof(CrystalPortalGump));

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddBackground(0, 0, 380, 420, 9270);

			AddHtml(0, 12, 380, 22, Center("<basefont color=#FFFFFF>Crystal Portal" + (facet == null ? "" : " (" + facet.Name + ")") + "</basefont>"), false, false);

			if (facet == null)
				BuildFacetPicker();
			else
				BuildDestinationList();
		}

		private static string Center(string text)
		{
			return "<center>" + text + "</center>";
		}

		private void BuildFacetPicker()
		{
			AddHtml(0, 38, 380, 18, Center("<basefont color=#CCCCCC>Select a facet</basefont>"), false, false);

			int y = 70;
			for (int i = 0; i < _Facets.Length; i++)
			{
				Map facet = _Facets[i];

				// Skip facets with no destinations defined.
				if (!CrystalPortal.GetDestinations(facet).Any())
					continue;

				AddButton(40, y + 1, 0x4B9, 0x4BA, 100 + i, GumpButtonType.Reply, 0);
				AddHtml(70, y, 280, 22, "<basefont color=#FFFFFF>" + facet.Name + "</basefont>", false, false);
				y += 28;
			}

			AddButton(170, 380, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0); // Close
			AddHtml(205, 380, 100, 22, "<basefont color=#FFFFFF>Close</basefont>", false, false);
		}

		private void BuildDestinationList()
		{
			CrystalPortal.Destination[] dests = CrystalPortal.GetDestinations(m_Facet).ToArray();
			int totalPages = Math.Max(1, (dests.Length + EntriesPerPage - 1) / EntriesPerPage);
			int page = Math.Max(0, Math.Min(m_PageIndex, totalPages - 1));

			AddHtml(0, 38, 380, 18, Center("<basefont color=#CCCCCC>Select a destination</basefont>"), false, false);
			AddHtml(0, 56, 380, 18, Center("<basefont color=#999999>(or SAY the destination name)</basefont>"), false, false);

			int start = page * EntriesPerPage;
			int end = Math.Min(start + EntriesPerPage, dests.Length);

			int y = 86;
			for (int i = start; i < end; i++)
			{
				CrystalPortal.Destination d = dests[i];
				AddButton(40, y + 1, 0x4B9, 0x4BA, 1000 + i, GumpButtonType.Reply, 0);
				AddHtml(70, y, 280, 22, "<basefont color=#FFFFFF>" + d.Label + "</basefont>", false, false);
				y += 26;
			}

			// Footer
			AddHtml(40, 380, 100, 22, String.Format("<basefont color=#FFFFFF>Page {0} / {1}</basefont>", page + 1, totalPages), false, false);

			AddButton(150, 380, 0xFB1, 0xFB3, 1, GumpButtonType.Reply, 0); // Back to facet picker
			AddHtml(185, 380, 60, 22, "<basefont color=#FFFFFF>Back</basefont>", false, false);

			if (page > 0)
				AddButton(240, 380, 0x15E3, 0x15E7, 2, GumpButtonType.Reply, 0); // Prev

			if (page + 1 < totalPages)
				AddButton(340, 380, 0x15E1, 0x15E5, 3, GumpButtonType.Reply, 0); // Next
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

			// Facet picker page: pick a facet
			if (m_Facet == null)
			{
				int facetIndex = id - 100;
				if (facetIndex >= 0 && facetIndex < _Facets.Length)
				{
					Map facet = _Facets[facetIndex];
					m_From.SendGump(new CrystalPortalGump(m_From, m_Portal, facet, 0));
				}
				return;
			}

			// Destination page navigation
			if (id == 1)
			{
				m_From.SendGump(new CrystalPortalGump(m_From, m_Portal));
				return;
			}

			if (id == 2)
			{
				m_From.SendGump(new CrystalPortalGump(m_From, m_Portal, m_Facet, m_PageIndex - 1));
				return;
			}

			if (id == 3)
			{
				m_From.SendGump(new CrystalPortalGump(m_From, m_Portal, m_Facet, m_PageIndex + 1));
				return;
			}

			int destIndex = id - 1000;
			CrystalPortal.Destination[] dests = CrystalPortal.GetDestinations(m_Facet).ToArray();

			if (destIndex < 0 || destIndex >= dests.Length)
				return;

			if (!m_From.InRange(m_Portal.Location, 3))
			{
				m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return;
			}

			CrystalPortal.Destination d = dests[destIndex];
			m_Portal.TryTeleport(m_From, d.Location, d.Map);
		}
	}
}
