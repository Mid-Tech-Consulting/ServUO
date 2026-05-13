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
using Server.Targeting;
using Server.Multis;
using Server.Engines.Plants;

namespace Server.Items
{
    // Same metal chest sprite pair as the abstract VirtualStorageChest base
    // so the Advanced Interior Decorator's Turn button rotates this chest
    // between the south-facing (0xE7C) and east-facing (0x9AB) views.
    [Flipable(0xE7C, 0x9AB)]
    public class DyeStorageChest : Item
    {
        // Tracks by "TypeFullName|HueData" to preserve colors
        public Dictionary<string, int> Content = new Dictionary<string, int>();

        [Constructable]
        public DyeStorageChest() : base(0xE7C) // Metal chest graphic
        {
            Name = "Dye Storage Chest";
            //Hue = 1173;
            Movable = true;
            Weight = 10.0;
        }

        public DyeStorageChest(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // Too far away.
                return;
            }

            if (!IsLockedDown && !IsSecure)
            {
                from.SendMessage("This storage chest must be secured in a house to function.");
                return;
            }

            from.SendGump(new DyeStorageGump(from, this));
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!IsLockedDown && !IsSecure)
            {
                from.SendMessage("This storage chest must be secured in a house to function.");
                return false;
            }

            if (TryAdd(dropped) > 0)
            {
                from.PlaySound(0x42);
                from.SendGump(new DyeStorageGump(from, this));
                return true;
            }

            from.SendMessage("That item does not belong in this stockpile.");
            return false;
        }

        private bool IsDye(Item item)
        {
            if (item is NaturalDye || item is NaturalHairDye || item is PlantPigment) return true;
            // BaseCubStoreDye / BasePetCubDye are UO Wildlands custom types not present on this shard.
            if (item is DyeTub || item is Dyes) return true;

            string n = item.GetType().Name;
            return n.Contains("Pigment") || n.Contains("Dye") || n.Contains("HairDye");
        }

        public static string GenerateKey(Item item)
        {
            string typeName = item.GetType().FullName;

            // Handle internal Enums specifically
            if (item is NaturalDye nd) return typeName + "|" + (int)nd.PigmentHue;
            if (item is PlantPigment pp) return typeName + "|" + (int)pp.PigmentHue;
            if (item is NaturalHairDye nhd) return typeName + "|" + (int)nhd.Type;

            // For Tokuno Pigments, Haochis, Abyssal Hair Dyes, etc.
            if (item.Hue != 0) return typeName + "|" + item.Hue;

            return typeName;
        }

        public int TryAdd(Item item)
        {
            if (!IsDye(item)) return 0;

            string key = GenerateKey(item);
            int amountToAdd = item.Amount;
                        
            if (Content.ContainsKey(key)) Content[key] += amountToAdd;
            else Content[key] = amountToAdd;

            item.Delete();
            return amountToAdd;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
            writer.Write(Content.Count);
            foreach (var kvp in Content)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                int amt = reader.ReadInt();
                Content[key] = amt;
            }
        }
    }

    public class DyeStorageGump : Gump
    {
        private DyeStorageChest m_Chest;
        private int m_Page;

        private const int ItemsPerPage = 16;
        private const int Width = 350;
        private const int Height = 450;
        private const int MaxInteractionRange = 25;

        public DyeStorageGump(Mobile from, DyeStorageChest chest, int page = 0) : base(50, 50)
        {
            m_Chest = chest;
            m_Page = page;

            this.TypeID = (int)chest.Serial;
            if (from.NetState != null)
            {
                foreach (var gump in from.NetState.Gumps.ToList())
                {
                    if (gump.TypeID == this.TypeID)
                    {
                        from.NetState.Send(new CloseGump(gump.TypeID, 0));
                        from.NetState.Gumps.Remove(gump);
                    }
                }
            }

            AddPage(0);
            AddBackground(0, 0, Width, Height, 9270);
            AddHtml(0, 20, Width, 20, "<center><BASEFONT COLOR=#FFFF00 size=7>Dye Stockpile</BASEFONT></center>", false, false);
            AddButton(25, 20, 4005, 4007, 9999, GumpButtonType.Reply, 0);
            AddLabel(60, 20, 1152, "ADD");

            int y = 55;
            AddLabel(70, y, 1152, "Dye Type");
            AddLabel(240, y, 1152, "Count");
            AddImageTiled(20, y + 22, Width - 40, 2, 96);

            List<string> keys = new List<string>(m_Chest.Content.Keys);
            keys.Sort(); // Alphabetical sorts by Type then Hue natively

            int start = page * ItemsPerPage;
            int end = start + ItemsPerPage;
            int count = keys.Count;
            y += 30;

            for (int i = start; i < end && i < count; i++)
            {
                string key = keys[i];
                AddButton(30, y + 3, 0x837, 0x838, 1000 + i, GumpButtonType.Reply, 0);
                AddLabel(50, y, 1153, GetLabelName(key));
                AddLabel(240, y, 0x481, m_Chest.Content[key].ToString());
                y += 20;
            }

            if (end < count) AddButton(Width - 40, 20, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0);
            if (m_Page > 0) AddButton(Width - 75, 20, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0);
        }



        private string GetLabelName(string key)
        {
            string[] parts = key.Split('|');
            string typeName = parts[0].Split('.').Last();

            if (parts.Length > 1)
            {
                int data = int.Parse(parts[1]);

                if (typeName == "NaturalDye")
                    return "Natural Dye: " + ((PlantPigmentHue)data).ToString();

                if (typeName == "PlantPigment")
                    return "Plant Pigment: " + ((PlantPigmentHue)data).ToString();

                if (typeName == "NaturalHairDye")
                    return "Hair Dye: " + ((HairDyeType)data).ToString();

                if (typeName.StartsWith("Cub")) return typeName.Replace("Cub", "") + " (Cub)";
                if (typeName.StartsWith("PetCub")) return typeName.Replace("PetCub", "") + " (Pet)";
                if (typeName.StartsWith("SamplePetCub")) return typeName.Replace("SamplePetCub", "") + " (Sample)";

                // Catch-all for items that track color via standard Hue integers
                return typeName + " (Hue: " + data + ")";
            }

            return typeName;
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (info.ButtonID == 0 || m_Chest.Deleted || !from.InRange(m_Chest.GetWorldLocation(), MaxInteractionRange)) return;

            if (info.ButtonID == 9999)
            {
                from.SendMessage("Target a dye, tub, or pigment to add.");
                from.Target = new AddTarget(m_Chest);
                return;
            }
            if (info.ButtonID == 1) { from.SendGump(new DyeStorageGump(from, m_Chest, m_Page - 1)); return; }
            if (info.ButtonID == 2) { from.SendGump(new DyeStorageGump(from, m_Chest, m_Page + 1)); return; }

            if (info.ButtonID >= 1000)
            {
                int index = info.ButtonID - 1000;
                List<string> keys = new List<string>(m_Chest.Content.Keys);
                keys.Sort();

                if (index >= 0 && index < keys.Count)
                {
                    string keyToWithdraw = keys[index];
                    int available = m_Chest.Content[keyToWithdraw];

                    try
                    {
                        int deductedAmt = 0;
                        Item item = RecreateItem(keyToWithdraw, available, out deductedAmt);

                        if (item != null)
                        {
                            if (from.Backpack != null && from.Backpack.CheckHold(from, item, true))
                            {
                                from.AddToBackpack(item);
                                m_Chest.Content[keyToWithdraw] -= deductedAmt;
                                if (m_Chest.Content[keyToWithdraw] <= 0) m_Chest.Content.Remove(keyToWithdraw);
                            }
                            else
                            {
                                item.Delete();
                                from.SendMessage("Your backpack is full.");
                            }
                        }
                    }
                    catch { from.SendMessage("Error recreating dye item."); }
                }
                from.SendGump(new DyeStorageGump(from, m_Chest, m_Page));
            }
        }

        private Item RecreateItem(string key, int available, out int deducted)
        {
            deducted = available;
            string[] parts = key.Split('|');
            string typeName = parts[0];

            Type typeToWithdraw = ScriptCompiler.FindTypeByFullName(typeName);
            if (typeToWithdraw == null) return null;

            Item item = (Item)Activator.CreateInstance(typeToWithdraw);

            // Reapply saved Hue or Enums
            if (parts.Length > 1)
            {
                int data = int.Parse(parts[1]);
                if (item is NaturalDye nd) nd.PigmentHue = (PlantPigmentHue)data;
                else if (item is PlantPigment pp) pp.PigmentHue = (PlantPigmentHue)data;
                else if (item is NaturalHairDye nhd) nhd.Type = (HairDyeType)data;
                else item.Hue = data;
            }

            if (item is NaturalDye d)
            {
                deducted = 1;
                // d.UsesRemaining stays at its default (full uses)
            }
            // BaseCubStoreDye branch removed -- UO Wildlands custom type, not present on this shard.
            else
            {
                deducted = Math.Min(available, 60000);
                item.Amount = deducted;
            }

            return item;
        }

        private class AddTarget : Target
        {
            private DyeStorageChest m_Chest;
            public AddTarget(DyeStorageChest chest) : base(12, false, TargetFlags.None) { m_Chest = chest; }
            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.InRange(m_Chest.GetWorldLocation(), MaxInteractionRange)) return;
                int added = 0;

                if (targeted is Container c)
                {
                    // Look through backpack for anything dye-related
                    foreach (Item i in c.FindItemsByType(typeof(Item), true))
                        if (m_Chest.TryAdd(i) > 0) added++;
                }
                else if (targeted is Item i && m_Chest.TryAdd(i) > 0) added++;

                if (added > 0)
                {
                    from.PlaySound(0x42);
                    from.SendMessage("Added {0} charges/items to the stockpile.", added);
                }
                from.SendGump(new DyeStorageGump(from, m_Chest));
            }
        }
    }
}
