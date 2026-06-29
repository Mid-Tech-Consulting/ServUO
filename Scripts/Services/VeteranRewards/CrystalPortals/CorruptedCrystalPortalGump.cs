using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class CorruptedCrystalPortalGump : Gump
	{
		private static readonly Map[] _Facets = new[]
		{
			Map.Felucca,
			Map.Malas,
			Map.Tokuno,
			Map.TerMur
		};

		private readonly Mobile m_From;
		private readonly CorruptedCrystalPortal m_Portal;
		private readonly Map m_Facet;

		public CorruptedCrystalPortalGump(Mobile from, CorruptedCrystalPortal portal)
			: this(from, portal, GetDefaultFacet(from)) { }

		public CorruptedCrystalPortalGump(Mobile from, CorruptedCrystalPortal portal, Map facet)
			: base(50, 50)
		{
			m_From = from;
			m_Portal = portal;
			m_Facet = facet;

			from.CloseGump(typeof(CorruptedCrystalPortalGump));

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			// Gold-bordered dark background, width 480, height 600
			AddBackground(0, 0, 480, 600, 9270);

			// Title: Corrupted Crystal Portal with green crystal balls on both sides
			AddItem(80, 10, 0x468B, 2601);
			AddHtml(0, 15, 480, 22, "<DIV ALIGN=CENTER><BASEFONT COLOR=#5DFFE5 SIZE=5><b>Corrupted Crystal Portal</b></BASEFONT></DIV>", false, false);
			AddItem(360, 10, 0x468B, 2601);

			// Facet tabs at the top (symmetrical 2x2 layout)
			int yTab = 48;
			for (int i = 0; i < _Facets.Length; i++)
			{
				Map f = _Facets[i];
				int col = i % 2;
				int row = i / 2;

				int x = 90 + col * 150;
				int y = yTab + row * 38;

				string name = f.Name;
				if (f == Map.Tokuno)
				{
					name = "Tokuno Islands";
				}
				else if (f == Map.TerMur)
				{
					name = "Ter Mur & Eodon";
				}

				if (f == m_Facet)
				{
					AddImage(x, y, 0x637);
					AddHtml(x, y + 5, 140, 20, String.Format("<DIV ALIGN=CENTER><BASEFONT COLOR=#000000 SIZE=3><b>{0}</b></BASEFONT></DIV>", name), false, false);
				}
				else
				{
					AddButton(x, y, 0x636, 0x637, 100 + i, GumpButtonType.Reply, 0);
					AddHtml(x, y + 5, 140, 20, String.Format("<DIV ALIGN=CENTER><BASEFONT COLOR=#000000 SIZE=3>{0}</BASEFONT></DIV>", name), false, false);
				}
			}

			// Destination list title
			AddHtml(20, 142, 440, 20, "<DIV ALIGN=CENTER><BASEFONT COLOR=#5DFFE5 SIZE=4><b>Choose Your Destination</b></BASEFONT></DIV>", false, false);

			// Get all destinations for selected facet
			var dests = new List<CorruptedCrystalPortal.Destination>();
			var destIndices = new List<int>();

			for (int i = 0; i < CorruptedCrystalPortal.AllDestinations.Length; i++)
			{
				var d = CorruptedCrystalPortal.AllDestinations[i];
				if (d.Map == m_Facet)
				{
					dests.Add(d);
					destIndices.Add(i);
				}
			}

			int half = (dests.Count + 1) / 2;

			for (int i = 0; i < dests.Count; i++)
			{
				var d = dests[i];
				int origIndex = destIndices[i];

				int x, y;
				if (i < half)
				{
					x = 20;
					y = 172 + i * 24;
				}
				else
				{
					x = 250;
					y = 172 + (i - half) * 24;
				}

				AddButton(x, y + 2, 0x4B9, 0x4BA, 1000 + origIndex, GumpButtonType.Reply, 0);
				AddHtml(x + 25, y, 180, 20, String.Format("<BASEFONT COLOR=#FFFFFF SIZE=4>{0}</BASEFONT>", d.Label), false, false);
			}
		}

		private static Map GetDefaultFacet(Mobile from)
		{
			Map map = from.Map;
			if (map == Map.Felucca || map == Map.Malas || map == Map.Tokuno || map == Map.TerMur)
			{
				return map;
			}
			return Map.Felucca;
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (m_From == null || m_From.Deleted || sender.Mobile != m_From)
			{
				return;
			}

			if (m_Portal == null || m_Portal.Deleted)
			{
				return;
			}

			int id = info.ButtonID;

			if (id == 0)
			{
				return;
			}

			if (id >= 100 && id < 100 + _Facets.Length)
			{
				Map facet = _Facets[id - 100];
				m_From.SendGump(new CorruptedCrystalPortalGump(m_From, m_Portal, facet));
				return;
			}

			int destIndex = id - 1000;
			if (destIndex >= 0 && destIndex < CorruptedCrystalPortal.AllDestinations.Length)
			{
				if (!m_From.InRange(m_Portal.Location, 3))
				{
					m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
					return;
				}

				CorruptedCrystalPortal.Destination d = CorruptedCrystalPortal.AllDestinations[destIndex];

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
}
