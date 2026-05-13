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

namespace Server.Gumps
{
    public class VirtualStorageGump : Gump
    {
        private VirtualStorageChest m_Chest;
        private int m_Page;
        
        private const int ItemsPerPage = 16;
        private const int Width = 350;
        private const int Height = 450;
        private const int MaxInteractionRange = 25;

        public VirtualStorageGump(Mobile from, VirtualStorageChest chest, int page = 0) : base(50, 50)
        {
            m_Chest = chest;
            m_Page = page;

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
            AddHtml(0, 20, Width, 20, string.Format("<center><BASEFONT COLOR=#FFFF00 size=7>{0}</BASEFONT></center>", m_Chest.ChestTitle), false, false);
            AddButton(25, 20, 4005, 4007, 9999, GumpButtonType.Reply, 0); 
            AddLabel(60, 20, 1152, "ADD");

            int y = 55;
            AddLabel(70, y, 1152, "Item Name");
            AddLabel(240, y, 1152, "Count");
            AddImageTiled(20, y + 22, Width - 40, 2, 96);

            List<Type> keys = new List<Type>(m_Chest.Content.Keys);
            keys.Sort((a, b) => 
            {
                int pA = GetSortPriority(a);
                int pB = GetSortPriority(b);
                if (pA != pB) return pA.CompareTo(pB);
                return string.Compare(a.Name, b.Name);
            });

            int start = page * ItemsPerPage;
            int end = start + ItemsPerPage;
            int count = keys.Count;
            y += 30;

            for (int i = start; i < end && i < count; i++)
            {
                Type t = keys[i];
                AddButton(30, y + 3, 0x837, 0x838, 1000 + i, GumpButtonType.Reply, 0);
                AddLabel(50, y, 0x481, t.Name);
                AddLabel(240, y, 0x481, m_Chest.Content[t].ToString());
                y += 20;
            }

            if (end < count) AddButton(Width - 40, 20, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0); 
            if (m_Page > 0) AddButton(Width - 75, 20, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0); 
        }

        private int GetSortPriority(Type t)
        {
            string name = t.Name;

            if (name.Contains("Board")) return 1;
            if (name.Contains("Log")) return 2;
            if (name.Contains("Ingot")) return 10;
            if (name.Contains("Granite")) return 11;
            if (name.Contains("Scale")) return 12;
            if (name.Contains("Leather") || name.Contains("Hide")) return 20;
            if (name.Contains("Cloth") || name.Contains("BoltOfCloth")) return 21;

            string[] gems = { "Amber", "Amethyst", "Citrine", "Diamond", "Emerald", "Ruby", "Sapphire", "Tourmaline", "StarSapphire" };
            if (gems.Any(g => name.Equals(g, StringComparison.OrdinalIgnoreCase))) return 30;

            switch (name)
            {
                // Magery Circle 1
                case "ClumsyScroll": return 101; case "CreateFoodScroll": return 102; case "FeeblemindScroll": return 103;
                case "HealScroll": return 104; case "MagicArrowScroll": return 105; case "NightSightScroll": return 106;
                case "ReactiveArmorScroll": return 107; case "WeakenScroll": return 108;
                // Magery Circle 2
                case "AgilityScroll": return 109; case "CunningScroll": return 110; case "CureScroll": return 111;
                case "HarmScroll": return 112; case "MagicTrapScroll": return 113; case "MagicUntrapScroll": return 114;
                case "ProtectionScroll": return 115; case "StrengthScroll": return 116;
                // Magery Circle 3
                case "BlessScroll": return 117; case "FireballScroll": return 118; case "MagicLockScroll": return 119;
                case "PoisonScroll": return 120; case "TelekinesisScroll": return 121; case "TeleportScroll": return 122;
                case "UnlockScroll": return 123; case "WallOfStoneScroll": return 124;
                // Magery Circle 4
                case "ArchCureScroll": return 125; case "ArchProtectionScroll": return 126; case "CurseScroll": return 127;
                case "FireFieldScroll": return 128; case "GreaterHealScroll": return 129; case "LightningScroll": return 130;
                case "ManaDrainScroll": return 131; case "RecallScroll": return 132;
                // Magery Circle 5
                case "BladeSpiritsScroll": return 133; case "DispelFieldScroll": return 134; case "IncognitoScroll": return 135;
                case "MagicReflectionScroll": return 136; case "MindBlastScroll": return 137; case "ParalyzeScroll": return 138;
                case "PoisonFieldScroll": return 139; case "SummonCreatureScroll": return 140;
                // Magery Circle 6
                case "DispelScroll": return 141; case "EnergyBoltScroll": return 142; case "ExplosionScroll": return 143;
                case "InvisibilityScroll": return 144; case "MarkScroll": return 145; case "MassCurseScroll": return 146;
                case "ParalyzeFieldScroll": return 147; case "RevealScroll": return 148;
                // Magery Circle 7
                case "ChainLightningScroll": return 149; case "EnergyFieldScroll": return 150; case "FlamestrikeScroll": return 151;
                case "GateTravelScroll": return 152; case "ManaVampireScroll": return 153; case "MassDispelScroll": return 154;
                case "MeteorSwarmScroll": return 155; case "PolymorphScroll": return 156;
                // Magery Circle 8
                case "EarthquakeScroll": return 157; case "EnergyVortexScroll": return 158; case "ResurrectionScroll": return 159;
                case "SummonAirElementalScroll": return 160; case "SummonDaemonScroll": return 161; case "SummonEarthElementalScroll": return 162;
                case "SummonFireElementalScroll": return 163; case "SummonWaterElementalScroll": return 164;

                // Necromancy
                case "CurseWeaponScroll": return 201; case "BloodOathScroll": return 202; case "CorpseSkinScroll": return 203;
                case "EvilOmenScroll": return 204; case "PainSpikeScroll": return 205; case "WraithFormScroll": return 206;
                case "MindRotScroll": return 207; case "SummonFamiliarScroll": return 208; case "AnimateDeadScroll": return 209;
                case "HorrificBeastScroll": return 210; case "PoisonStrikeScroll": return 211; case "WitherScroll": return 212;
                case "StrangleScroll": return 213; case "LichFormScroll": return 214; case "ExorcismScroll": return 215;
                case "VengefulSpiritScroll": return 216; case "VampiricEmbraceScroll": return 217;

                // Mysticism
                case "HealingStoneScroll": return 301; case "NetherBoltScroll": return 302; case "EnchantScroll": return 303;
                case "PurgeMagicScroll": return 304; case "EagleStrikeScroll": return 305; case "SleepScroll": return 306;
                case "AnimatedWeaponScroll": return 307; case "StoneFormScroll": return 308; case "MassSleepScroll": return 309;
                case "SpellTriggerScroll": return 310; case "BombardScroll": return 311; case "CleansingWindsScroll": return 312;
                case "HailStormScroll": return 313; case "SpellPlagueScroll": return 314; case "NetherCycloneScroll": return 315;
                case "RisingColossusScroll": return 316;

                // Spellweaving
                case "ArcaneCircleScroll": return 401; case "AttuneWeaponScroll": return 402; case "GiftOfRenewalScroll": return 403;
                case "NaturesFuryScroll": return 404; case "ImmolatingWeaponScroll": return 405; case "ThunderstormScroll": return 406;
                case "ArcaneEmpowermentScroll": return 407; case "EtherealVoyageScroll": return 408; case "ReaperFormScroll": return 409;
                case "GiftOfLifeScroll": return 410; case "SummonFeyScroll": return 411; case "SummonFiendScroll": return 412;
                case "DryadAllureScroll": return 413; case "EssenceOfWindScroll": return 414; case "WildfireScroll": return 415;
                case "WordOfDeathScroll": return 416;
            }

            if (t == typeof(Bottle)) return 999;
            return 500; 
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            if (info.ButtonID == 0 || m_Chest.Deleted || !from.InRange(m_Chest.GetWorldLocation(), MaxInteractionRange)) return;

            if (info.ButtonID == 9999) { from.SendMessage("Target an item to add."); from.Target = new AddTarget(m_Chest); return; }
            if (info.ButtonID == 1) { from.SendGump(new VirtualStorageGump(from, m_Chest, m_Page - 1)); return; }
            if (info.ButtonID == 2) { from.SendGump(new VirtualStorageGump(from, m_Chest, m_Page + 1)); return; }

            if (info.ButtonID >= 1000)
            {
                int index = info.ButtonID - 1000;
                List<Type> keys = new List<Type>(m_Chest.Content.Keys);
                keys.Sort((a, b) => {
                    int pA = GetSortPriority(a); int pB = GetSortPriority(b);
                    if (pA != pB) return pA.CompareTo(pB);
                    return string.Compare(a.Name, b.Name);
                });

                if (index >= 0 && index < keys.Count)
                {
                    Type typeToWithdraw = keys[index];
                    int amount = Math.Min(m_Chest.Content[typeToWithdraw], 100);
                    try {
                        Item item = (Item)Activator.CreateInstance(typeToWithdraw);
                        item.Amount = amount;
                        if (from.Backpack != null && from.Backpack.CheckHold(from, item, true)) {
                            from.AddToBackpack(item);
                            m_Chest.Content[typeToWithdraw] -= amount;
                            if (m_Chest.Content[typeToWithdraw] <= 0) m_Chest.Content.Remove(typeToWithdraw);
                        } else { item.Delete(); from.SendMessage("Backpack full."); }
                    } catch { from.SendMessage("Error."); }
                }
                from.SendGump(new VirtualStorageGump(from, m_Chest, m_Page));
            }
        }

        private class AddTarget : Target
        {
            private VirtualStorageChest m_Chest;
            public AddTarget(VirtualStorageChest chest) : base(12, false, TargetFlags.None) { m_Chest = chest; }
            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.InRange(m_Chest.GetWorldLocation(), MaxInteractionRange)) return;
                int added = 0;
                if (targeted is Container c) {
                    foreach (Item i in c.FindItemsByType(m_Chest.AllowedTypes, true)) if (m_Chest.TryAdd(i) > 0) added++;
                } else if (targeted is Item i && m_Chest.TryAdd(i) > 0) added++;
                if (added > 0) { from.PlaySound(0x42); from.SendMessage("Added {0} items.", added); }
                from.SendGump(new VirtualStorageGump(from, m_Chest));
            }
        }
    }
}
