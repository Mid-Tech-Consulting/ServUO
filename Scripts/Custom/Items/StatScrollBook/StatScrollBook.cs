/*
Author: Cork
Creation Date: 3/22/2026
Description:
    Extends BaseSpecialScrollBook and accepts only StatCapScroll items
    Uses the same flipable book art as the Power Scroll Book (0x9A95/0x9AA7) with hue 0x481 (matching stat scrolls)
    Overrides OnDragDrop with stat-scroll-specific string messages
    Provides ConstructStatScroll to retrieve scrolls by value
    Has GetScrollCount to count scrolls at each bonus tier
    Reads the stat cap from config to correctly match scroll values
*/
using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    [Flipable(0x9A95, 0x9AA7)]
    public class StatScrollBook : BaseSpecialScrollBook
    {
        public static int StatCap { get { return Config.Get("PlayerCaps.TotalStatCap", 225); } }

        public override Type ScrollType { get { return typeof(StatCapScroll); } }
        public override int LabelNumber { get { return 0; } }
        public override string DefaultName { get { return "Stat Scroll Book"; } }
        public override int BadDropMessage { get { return 0; } }
        public override int DropMessage { get { return 0; } }
        public override int RemoveMessage { get { return 0; } }
        public override int GumpTitle { get { return 0; } }

        public override Dictionary<SkillCat, List<SkillName>> SkillInfo { get { return null; } }
        public override Dictionary<int, double> ValueInfo { get { return null; } }

        public static readonly int[] ScrollValues = new int[] { 5, 10, 15, 20, 25 };

        [Constructable]
        public StatScrollBook()
            : base(0x9A95)
        {
            Hue = 37;
        }

        public StatScrollBook(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(GetWorldLocation(), 2))
            {
                BaseGump.SendGump(new StatScrollBookGump((PlayerMobile)m, this));
            }
            else if (m.AccessLevel > AccessLevel.Player)
            {
                base.OnDoubleClick(m);
            }
        }

        public override bool OnDragDrop(Mobile m, Item dropped)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (dropped.GetType() != ScrollType)
                {
                    m.SendMessage("This book only holds Stat Scrolls.");
                }
                else if (house == null || !IsLockedDown)
                {
                    m.SendLocalizedMessage(1151765); // You must lock this book down in a house to add scrolls to it.
                }
                else if (!house.CheckAccessibility(this, m))
                {
                    m.SendLocalizedMessage(1155693); // This item is impermissible and can not be added to the book.
                }
                else if (Items.Count < Capacity)
                {
                    DropItem(dropped);

                    m.SendMessage("You add the scroll to your Stat Scroll Book.");

                    dropped.Movable = false;

                    m.CloseGump(typeof(StatScrollBookGump));

                    return true;
                }
            }

            return false;
        }

        public void ConstructStatScroll(Mobile m, double value)
        {
            var scroll = Items.OfType<StatCapScroll>().FirstOrDefault(s => s.Value == value);

            if (scroll != null)
            {
                if (m.Backpack == null || !m.Backpack.TryDropItem(m, scroll, false))
                {
                    m.SendLocalizedMessage(502868); // Your backpack is too full.
                }
                else
                {
                    BaseHouse house = BaseHouse.FindHouseAt(this);

                    if (house != null && house.LockDowns.ContainsKey(scroll))
                    {
                        house.LockDowns.Remove(scroll);
                    }

                    if (!scroll.Movable)
                    {
                        scroll.Movable = true;
                    }

                    if (scroll.IsLockedDown)
                    {
                        scroll.IsLockedDown = false;
                    }

                    m.SendMessage("You remove a Stat Scroll and put it in your pack.");
                }
            }
        }

        public int GetScrollCount(int bonus)
        {
            double value = StatCap + bonus;

            return Items.OfType<StatCapScroll>().Count(s => s.Value == value);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Hue = 37;
        }
    }
}
