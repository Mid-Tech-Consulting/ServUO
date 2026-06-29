using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
	public class StableListGump : Gump
	{
		private readonly Mobile m_From;
		private readonly List<BaseCreature> m_List;
		private readonly int m_Page;
		private readonly Action<Mobile, BaseCreature> m_OnClaim;
		private readonly string m_TrainerName;

		public StableListGump(string trainerName, Mobile from, List<BaseCreature> list)
			: this(trainerName, from, list, 0, null)
		{
		}

		public StableListGump(string trainerName, Mobile from, List<BaseCreature> list, int page, Action<Mobile, BaseCreature> onClaim)
			: base(50, 50)
		{
			m_TrainerName = trainerName;
			m_From = from;
			m_List = list;
			m_Page = page;
			m_OnClaim = onClaim;

			from.CloseGump(typeof(StableListGump));

			AddPage(0);

			// Styled dark gump background with gold/bronze border
			AddBackground(0, 0, 850, 270, 0x24AE);

			// Title: Taran's Stables
			AddHtml(0, 15, 850, 20, String.Format("<DIV ALIGN=CENTER><BASEFONT COLOR=#5DFFE5 SIZE=5>{0}'s Stables</BASEFONT></DIV>", trainerName), false, false);

			// Thin horizontal divider line
			AddImageTiled(15, 45, 820, 2, 0x23C5);

			int startIndex = page * 6;
			int endIndex = Math.Min(startIndex + 6, list.Count);

			for (int i = startIndex; i < endIndex; i++)
			{
				BaseCreature pet = list[i];

				if (pet == null || pet.Deleted)
				{
					continue;
				}

				int relativeIndex = i - startIndex;
				int col = relativeIndex % 3;
				int row = relativeIndex / 3;

				int x = 25 + col * 275;
				int y = 60 + row * 80;

				int itemID = ShrinkTable.Lookup(pet);
				int hue = pet.Hue;

				// Tile button showing the creature figurine/icon
				AddImageTiledButton(x, y, 0x918, 0x919, i + 1, GumpButtonType.Reply, 0, itemID, hue, 15, 10);

				// Creature name next to the preview box
				AddHtml(x + 70, y + 20, 180, 20, String.Format("<BASEFONT COLOR=#FFFFFF SIZE=4>{0}</BASEFONT>", pet.Name), false, false);
			}

			// Thin horizontal divider line
			AddImageTiled(15, 220, 820, 2, 0x23C5);

			// Stable increases calculations
			double taming = from.Skills[SkillName.AnimalTaming].Value;
			double anlore = from.Skills[SkillName.AnimalLore].Value;
			double vetern = from.Skills[SkillName.Veterinary].Value;
			double sklsum = taming + anlore + vetern;

			int skillCurrent = 0;
			if (sklsum >= 240.0)
			{
				skillCurrent += 5;
			}
			else if (sklsum >= 200.0)
			{
				skillCurrent += 4;
			}
			else if (sklsum >= 160.0)
			{
				skillCurrent += 3;
			}
			else
			{
				skillCurrent += 2;
			}

			if (taming >= 100.0)
			{
				skillCurrent += (int)((taming - 90.0) / 10);
			}

			if (anlore >= 100.0)
			{
				skillCurrent += (int)((anlore - 90.0) / 10);
			}

			if (vetern >= 100.0)
			{
				skillCurrent += (int)((vetern - 90.0) / 10);
			}

			int expansionCurrent = (Core.SA ? 2 : 0) + (Core.TOL ? 2 : 0);
			int masteryCurrent = Server.Spells.SkillMasteries.MasteryInfo.BoardingSlotIncrease(from);
			int tokenCurrent = from is PlayerMobile ? ((PlayerMobile)from).RewardStableSlots : 0;

			// Bottom Left: Stable Count
			AddHtml(25, 235, 200, 20, String.Format("<BASEFONT COLOR=#5DFFE5 SIZE=4>Stable Count: {0}/{1}</BASEFONT>", list.Count, AnimalTrainer.GetMaxStabled(from)), false, false);

			// Bottom Right: Detailed stables capacity breakdown
			string stats = String.Format("<BASEFONT COLOR=#FFF2B2 SIZE=3>Stable Increases - Skill: {0}/14 | Expansion: {1}/4 | Mastery: {2}/3 | UO Store Token: {3}/21</BASEFONT>", skillCurrent, expansionCurrent, masteryCurrent, tokenCurrent);
			AddHtml(230, 235, 530, 20, stats, false, false);

			// Pagination
			if (page > 0)
			{
				AddButton(780, 232, 0x15E5, 0x15E5, 998, GumpButtonType.Reply, 0);
			}

			if ((page + 1) * 6 < list.Count)
			{
				AddButton(810, 232, 0x15E1, 0x15E1, 999, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID == 998) // Previous page
			{
				if (m_Page > 0)
				{
					m_From.SendGump(new StableListGump(m_TrainerName, m_From, m_List, m_Page - 1, m_OnClaim));
				}
			}
			else if (info.ButtonID == 999) // Next page
			{
				if ((m_Page + 1) * 6 < m_List.Count)
				{
					m_From.SendGump(new StableListGump(m_TrainerName, m_From, m_List, m_Page + 1, m_OnClaim));
				}
			}
			else if (info.ButtonID > 0)
			{
				int index = info.ButtonID - 1;

				if (index >= 0 && index < m_List.Count)
				{
					if (m_OnClaim != null)
					{
						m_OnClaim(m_From, m_List[index]);
					}
				}
			}
		}
	}
}
