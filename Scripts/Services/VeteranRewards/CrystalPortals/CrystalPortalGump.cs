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
		private static readonly Map[] _Facets = new[]
		{
			Map.Felucca,
			Map.Ilshenar,
			Map.Malas,
			Map.Tokuno,
			Map.TerMur
		};

		private readonly Mobile m_From;
		private readonly CrystalPortal m_Portal;
		private readonly Map m_Facet;

		public CrystalPortalGump(Mobile from, CrystalPortal portal)
			: this(from, portal, GetDefaultFacet(from)) { }

		public CrystalPortalGump(Mobile from, CrystalPortal portal, Map facet)
			: base(50, 50)
		{
			m_From = from;
			m_Portal = portal;
			m_Facet = facet;

			from.CloseGump(typeof(CrystalPortalGump));

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			// Gold-bordered dark background, width 480, height 600
			AddBackground(0, 0, 480, 600, 9270);

			// Title: Crystal Portal with crystal balls on both sides
			AddItem(100, 10, 0x468B);
			AddHtml(0, 15, 480, 22, "<DIV ALIGN=CENTER><BASEFONT COLOR=#5DFFE5 SIZE=5><b>Crystal Portal</b></BASEFONT></DIV>", false, false);
			AddItem(340, 10, 0x468B);

			// Facet tabs at the top (using wider parchment banners)
			int yTab = 48;
			for (int i = 0; i < _Facets.Length; i++)
			{
				Map f = _Facets[i];
				int col = i % 3;
				int row = i / 3;

				int x = 15 + col * 150;
				int y = yTab + row * 38;

				if (i >= 3)
				{
					x = 90 + (i - 3) * 150;
				}

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

			// Column Headers: Moongates and Banks
			AddHtml(20, 142, 200, 20, "<BASEFONT COLOR=#5DFFE5 SIZE=4><b>Moongates</b></BASEFONT>", false, false);
			AddHtml(250, 142, 200, 20, "<BASEFONT COLOR=#5DFFE5 SIZE=4><b>Banks</b></BASEFONT>", false, false);

			int mIndex = 0;
			int bIndex = 0;

			for (int i = 0; i < CrystalPortal.AllDestinations.Length; i++)
			{
				CrystalPortal.Destination d = CrystalPortal.AllDestinations[i];
				if (d.Map != m_Facet)
				{
					continue;
				}

				string cleanLabel = d.Label
					.Replace(" Moongate", "")
					.Replace(" moongate", "")
					.Replace(" Mint", "")
					.Replace(" mint", "")
					.Replace(" Bank", "")
					.Replace(" bank", "");

				if (d.SpeechKey.IndexOf("moongate") >= 0)
				{
					int x = 20;
					int y = 172 + mIndex * 24;
					mIndex++;

					AddButton(x, y + 2, 0x4B9, 0x4BA, 1000 + i, GumpButtonType.Reply, 0);
					AddHtml(x + 25, y, 180, 20, String.Format("<BASEFONT COLOR=#FFFFFF SIZE=4>{0}</BASEFONT>", cleanLabel), false, false);
				}
				else
				{
					int x = 250;
					int y = 172 + bIndex * 24;
					bIndex++;

					AddButton(x, y + 2, 0x4B9, 0x4BA, 1000 + i, GumpButtonType.Reply, 0);
					AddHtml(x + 25, y, 180, 20, String.Format("<BASEFONT COLOR=#FFFFFF SIZE=4>{0}</BASEFONT>", cleanLabel), false, false);
				}
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
				m_From.SendGump(new CrystalPortalGump(m_From, m_Portal, facet));
				return;
			}

			int destIndex = id - 1000;
			if (destIndex >= 0 && destIndex < CrystalPortal.AllDestinations.Length)
			{
				if (!m_From.InRange(m_Portal.Location, 3))
				{
					m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
					return;
				}

				CrystalPortal.Destination d = CrystalPortal.AllDestinations[destIndex];
				m_Portal.TryTeleport(m_From, d.Location, d.Map);
			}
		}
	}
}
