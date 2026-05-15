/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Targeting;
using System.Collections;
using Server.Multis;

namespace Server.Items
{
    // 0xB2E7 is a King's Collection cabinet sprite with no documented flip
    // partner in the codebase. 0xB2E8 is the most likely sibling -- UO art
    // typically packs facing pairs into adjacent tile IDs. Verify visually
    // in-game after restart; if the rotated sprite looks wrong, swap to
    // 0xB2E6 instead.
    [Flipable(0xB2E7, 0xB2E8)]
    public class RefinementCabinet : Item, ISecurable
    {
        private List<Item> m_Stored = new List<Item>();
        public List<Item> Stored => m_Stored;
        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
        [Constructable]
        public RefinementCabinet() : base(0xB2E7)
        {
            Name = "Refinement Cabinet";
            Weight = 25.0;
            m_Level = SecureLevel.CoOwners;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // Too far away.
                return;
            }

            if (!IsLockedDown && !IsSecure)
            {
                from.SendMessage("This must be secured in a house to function.");
                return;
            }

            from.SendGump(new RefinementCabinetGump(from, this));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!IsLockedDown && !IsSecure)
            {
                from.SendMessage("This must be secured in a house to function.");
                return false;
            }

            if (TryAdd(dropped))
            {
                from.PlaySound(0x42);
                RefreshGump(from);
                return true;
            }

            from.SendMessage("That is not a valid refinement or material.");
            return false;
        }

        public bool TryAdd(Item item)
        {
            if (item is RefinementComponent || item is RefinementItem ||
                item is MalleableAlloy || item is LeatherBraid || item is SolventFlask)
            {
                m_Stored.Add(item);
                item.Internalize();
                return true;
            }
            return false;
        }

        public void RefreshGump(Mobile from)
        {
            if (from.HasGump(typeof(RefinementCabinetGump)))
            {
                from.CloseGump(typeof(RefinementCabinetGump));
                from.SendGump(new RefinementCabinetGump(from, this));
            }
        }

        public RefinementCabinet(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.WriteItemList(m_Stored);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
            ArrayList list = reader.ReadItemList();
            m_Stored = new List<Item>();
            foreach (Item item in list) m_Stored.Add(item);
        }
    }
}

namespace Server.Gumps
{
    public class RefinementCabinetGump : Gump
    {
        private RefinementCabinet m_Chest;
        private List<StorageEntry> m_Entries;
        private int m_Page;

        private const int Width = 440;
        private const int Height = 450;
        private const int ItemsPerPage = 16;

        public RefinementCabinetGump(Mobile from, RefinementCabinet chest, int page = 0) : base(50, 50)
        {
            m_Chest = chest;
            m_Page = page;
            m_Entries = AggregateItems(chest.Stored);

            this.TypeID = (int)chest.Serial;
            if (from.NetState != null)
            {
                List<Gump> gumps = from.NetState.Gumps;
                for (int i = 0; i < gumps.Count; i++)
                {
                    if (gumps[i].TypeID == this.TypeID)
                    {
                        from.NetState.Send(new CloseGump(gumps[i].TypeID, 0));
                        gumps.RemoveAt(i);
                        break;
                    }
                }
            }

            AddPage(0);
            AddBackground(0, 0, Width, Height, 9270);

            // Centered Yellow Title
            AddHtml(0, 20, Width, 20, "<center><BASEFONT COLOR=#FFFF00 size=7>Refinement Stockpile</BASEFONT></center>", false, false);

            // ADD Button — top-left, matches VirtualStorageGump
            AddButton(25, 20, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddLabel(60, 20, 1152, "ADD");

            // Page Controls — top-right, matching VirtualStorageGump button IDs and positions
            if ((page + 1) * ItemsPerPage < m_Entries.Count)
                AddButton(Width - 40, 20, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0);

            if (page > 0)
                AddButton(Width - 75, 20, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0);

            // Column Headers
            int y = 55;
            AddLabel(70, y, 1152, "Item Name");
            AddLabel(330, y, 1152, "Count");
            AddImageTiled(20, y + 22, Width - 40, 2, 96);

            y += 30;
            for (int i = page * ItemsPerPage; i < m_Entries.Count && i < (page + 1) * ItemsPerPage; i++)
            {
                StorageEntry entry = m_Entries[i];

                AddButton(30, y + 3, 0x837, 0x838, i + 10, GumpButtonType.Reply, 0);
                AddLabel(50, y, 0x481, entry.DisplayName);
                AddLabel(330, y, 0x481, entry.TotalAmount.ToString());

                y += 20;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 1: // Prev
                    from.SendGump(new RefinementCabinetGump(from, m_Chest, m_Page - 1));
                    break;
                case 2: // Next
                    from.SendGump(new RefinementCabinetGump(from, m_Chest, m_Page + 1));
                    break;
                case 3: // Add
                    from.SendMessage("Target an item or container to add to the stockpile.");
                    from.Target = new InternalAddTarget(m_Chest);
                    break;
                default:
                    if (info.ButtonID >= 10)
                    {
                        int index = info.ButtonID - 10;
                        ProcessWithdrawal(from, index);
                    }
                    break;
            }
        }

        private void ProcessWithdrawal(Mobile from, int index)
        {
            if (index >= m_Entries.Count) return;

            StorageEntry entry = m_Entries[index];
            Item toWithdraw = entry.Instances[0];

            // Withdrawal is strictly one unit at a time as requested
            if (toWithdraw.Stackable && toWithdraw.Amount > 1)
            {
                Item split = Activator.CreateInstance(toWithdraw.GetType()) as Item;
                if (split != null)
                {
                    split.Amount = 1;
                    if (from.Backpack.CheckHold(from, split, true))
                    {
                        from.AddToBackpack(split);
                        toWithdraw.Amount -= 1;
                    }
                    else split.Delete();
                }
            }
            else
            {
                if (from.Backpack.CheckHold(from, toWithdraw, true))
                {
                    from.AddToBackpack(toWithdraw);
                    m_Chest.Stored.Remove(toWithdraw);
                }
            }
            from.SendGump(new RefinementCabinetGump(from, m_Chest, m_Page));
        }

        private List<StorageEntry> AggregateItems(List<Item> items)
        {
            var entries = new Dictionary<string, StorageEntry>();
            foreach (Item item in items)
            {
                string key = GetKey(item);
                if (!entries.ContainsKey(key)) entries[key] = new StorageEntry(item);
                entries[key].Instances.Add(item);
                entries[key].TotalAmount += (item.Stackable ? item.Amount : 1);
            }

            return entries.Values
                .OrderBy(e => (int)e.CraftType)
                .ThenBy(e => (int)e.SubCraft)
                .ThenByDescending(e => (int)e.RefinementType) // Deflecting before Reinforcing
                .ThenBy(e => (int)e.Mod)
                .ToList();
        }

        private string GetKey(Item item)
        {
            if (item is RefinementComponent c) return $"C_{c.CraftType}_{c.SubCraftType}_{c.RefinementType}_{c.ModType}";
            if (item is RefinementItem r) return $"I_{r.CraftType}_{r.SubCraftType}_{r.RefinementType}_{r.ModType}";
            return item.GetType().Name;
        }

        private class StorageEntry
        {
            public string DisplayName;
            public List<Item> Instances = new List<Item>();
            public int TotalAmount;

            public RefinementCraftType CraftType = (RefinementCraftType)99;
            public RefinementSubCraftType SubCraft = (RefinementSubCraftType)99;
            public RefinementType RefinementType = (RefinementType)0;
            public ModType Mod = (ModType)99;

            public StorageEntry(Item item)
            {
                if (item is RefinementComponent c)
                {
                    DisplayName = $"{c.SubCraftType} {(c.RefinementType == RefinementType.Deflecting ? "Deflect" : "Reinf")} {c.ModType} Comp";
                    CraftType = c.CraftType; SubCraft = c.SubCraftType; RefinementType = c.RefinementType; Mod = c.ModType;
                }
                else if (item is RefinementItem r)
                {
                    DisplayName = $"{r.SubCraftType} {(r.RefinementType == RefinementType.Deflecting ? "Deflect" : "Reinf")} {r.ModType} Refine";
                    CraftType = r.CraftType; SubCraft = r.SubCraftType; RefinementType = r.RefinementType; Mod = r.ModType;
                }
                else
                {
                    // Resolved Material Names
                    if (item.LabelNumber == 1154003) DisplayName = "Leather Braid";
                    else if (item.LabelNumber == 1154004) DisplayName = "Solvent Flask";
                    else if (item.LabelNumber == 1154005) DisplayName = "Malleable Alloy";
                    else DisplayName = item.Name ?? (item.LabelNumber > 0 ? $"#{item.LabelNumber}" : item.GetType().Name);
                }
            }
        }

        private class InternalAddTarget : Target
        {
            private RefinementCabinet m_Chest;
            public InternalAddTarget(RefinementCabinet chest) : base(12, false, TargetFlags.None) { m_Chest = chest; }

            protected override void OnTarget(Mobile from, object targeted)
            {
                int added = 0;
                if (targeted is Container c)
                {
                    List<Item> toAdd = new List<Item>();
                    CheckContainer(c, toAdd);
                    foreach (Item i in toAdd) if (m_Chest.TryAdd(i)) added++;
                }
                else if (targeted is Item i && m_Chest.TryAdd(i)) added++;

                if (added > 0) { from.PlaySound(0x42); from.SendMessage($"Added {added} items."); }
                from.SendGump(new RefinementCabinetGump(from, m_Chest));
            }

            private void CheckContainer(Container cont, List<Item> toAdd)
            {
                foreach (Item item in cont.Items)
                {
                    if (item is RefinementComponent || item is RefinementItem ||
                        item is MalleableAlloy || item is LeatherBraid || item is SolventFlask)
                        toAdd.Add(item);
                    else if (item is Container sub)
                        CheckContainer(sub, toAdd);
                }
            }
        }
    }
}
