/*
Author: Cork
Creation Date: 3/22/2026
Description: 
    Holds SkillMasteryPrimer items, up to 300 of them.
    Uses the Power Scroll Book graphic with hue 1266 (purple)
    Lockdown-required, blessed, securable
    Add in-game with [add MasteryPrimerBook
*/
using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;
using System.Linq;
using Server.ContextMenus;

namespace Server.Items
{
    [Flipable(0x9A95, 0x9AA7)]
    public class MasteryPrimerBook : Container, ISecurable
    {
        public const int MaxPrimers = 300;

        private int _Capacity;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Capacity
        {
            get { return _Capacity <= 0 ? MaxPrimers : _Capacity; }
            set
            {
                _Capacity = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public override bool DisplaysContent { get { return false; } }
        public override double DefaultWeight { get { return 1.0; } }
        public override string DefaultName { get { return "Mastery Primer Book"; } }

        public static readonly string[] VolumeNames = new string[] { "Volume I", "Volume II", "Volume III" };
        public static readonly int[] Volumes = new int[] { 1, 2, 3 };

        public static Dictionary<SkillCat, List<SkillName>> _SkillInfo;

        public static void Initialize()
        {
            _SkillInfo = new Dictionary<SkillCat, List<SkillName>>();

            _SkillInfo[SkillCat.Combat] = new List<SkillName>() { SkillName.Fencing, SkillName.Macing, SkillName.Swords, SkillName.Throwing, SkillName.Parry, SkillName.Poisoning, SkillName.Wrestling, SkillName.Archery };
            _SkillInfo[SkillCat.Magic] = new List<SkillName>() { SkillName.Bushido, SkillName.Chivalry, SkillName.Magery, SkillName.Mysticism, SkillName.Necromancy, SkillName.Ninjitsu, SkillName.Spellweaving };
            _SkillInfo[SkillCat.Wilderness] = new List<SkillName>() { SkillName.AnimalTaming };
            _SkillInfo[SkillCat.Bard] = new List<SkillName>() { SkillName.Discordance, SkillName.Peacemaking, SkillName.Provocation };
        }

        [Constructable]
        public MasteryPrimerBook()
            : base(0x9A95)
        {
            Hue = 1266;
            LootType = LootType.Blessed;
        }

        public MasteryPrimerBook(Serial serial)
            : base(serial)
        {
        }

        public override int GetTotal(TotalType type)
        {
            return 0;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(GetWorldLocation(), 2))
            {
                BaseGump.SendGump(new MasteryPrimerBookGump((PlayerMobile)m, this));
            }
            else if (m.AccessLevel > AccessLevel.Player)
            {
                base.OnDoubleClick(m);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1151797, String.Format("{0}\t{1}", Items.Count, Capacity)); // Scrolls in book: ~1_val~/~2_val~
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override bool OnDragDrop(Mobile m, Item dropped)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (!(dropped is SkillMasteryPrimer))
                {
                    m.SendMessage("This book only holds Mastery Primers.");
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

                    m.SendMessage("You add the primer to your Mastery Primer Book.");

                    dropped.Movable = false;

                    m.CloseGump(typeof(MasteryPrimerBookGump));

                    return true;
                }
            }

            return false;
        }

        public void ConstructPrimer(Mobile m, SkillName skill, int volume)
        {
            var primer = Items.OfType<SkillMasteryPrimer>().FirstOrDefault(p => p.Skill == skill && p.Volume == volume);

            if (primer != null)
            {
                if (m.Backpack == null || !m.Backpack.TryDropItem(m, primer, false))
                {
                    m.SendLocalizedMessage(502868); // Your backpack is too full.
                }
                else
                {
                    BaseHouse house = BaseHouse.FindHouseAt(this);

                    if (house != null && house.LockDowns.ContainsKey(primer))
                    {
                        house.LockDowns.Remove(primer);
                    }

                    if (!primer.Movable)
                    {
                        primer.Movable = true;
                    }

                    if (primer.IsLockedDown)
                    {
                        primer.IsLockedDown = false;
                    }

                    m.SendMessage("You remove a Mastery Primer and put it in your pack.");
                }
            }
        }

        public int GetPrimerCount(SkillName skill, int volume)
        {
            return Items.OfType<SkillMasteryPrimer>().Count(p => p.Skill == skill && p.Volume == volume);
        }

        public bool HasPrimerForSkill(SkillName skill)
        {
            return Items.OfType<SkillMasteryPrimer>().Any(p => p.Skill == skill);
        }

        public bool HasPrimerForCategory(List<SkillName> skills)
        {
            return Items.OfType<SkillMasteryPrimer>().Any(p => skills.Contains(p.Skill));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)Level);
            writer.Write(_Capacity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();
            _Capacity = reader.ReadInt();

            Timer.DelayCall(
                () =>
                {
                    foreach (var item in Items.Where(i => i.Movable))
                        item.Movable = false;
                });
        }
    }
}
