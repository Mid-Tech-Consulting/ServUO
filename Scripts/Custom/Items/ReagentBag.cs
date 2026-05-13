using System;

using Server;
using Server.Network;

namespace Server.Items
{
    // Weightless reagent satchel: holds up to 60,000 of each Magery,
    // Necromancy and Mysticism reagent and nothing else. Because it lives in
    // the backpack, the crafting system (Alchemy, Inscription, etc.) draws
    // reagents straight out of it -- CraftItem.ConsumeTotalGrouped already
    // recurses into sub-containers. Blessed so players don't lose a full
    // stock on death.
    public class ReagentBag : Container
    {
        public const int MaxPerReagent = 60000;

        public override int DefaultMaxItems { get { return 125; } }
        public override int DefaultMaxWeight { get { return 1000000; } } // never the limiting factor; weight is zeroed below

        // The 17 reagent types this bag accepts -- same set the Reagent
        // Stockpile chest uses (8 Magery, 5 Necromancy, 4 Mysticism, with
        // Daemon Bone shared). Bottles / empty kegs are intentionally not
        // included; this is a reagent bag, not an alchemy bag.
        private static readonly Type[] _Allowed =
        {
            // Magery
            typeof(BlackPearl), typeof(Bloodmoss), typeof(Garlic), typeof(Ginseng),
            typeof(MandrakeRoot), typeof(Nightshade), typeof(SulfurousAsh), typeof(SpidersSilk),
            // Necromancy
            typeof(BatWing), typeof(GraveDust), typeof(DaemonBlood), typeof(NoxCrystal), typeof(PigIron),
            // Mysticism
            typeof(Bone), typeof(DragonBlood), typeof(FertileDirt), typeof(DaemonBone),
        };

        [Constructable]
        public ReagentBag()
            : base(0xE76)
        {
            Name = "a reagent bag";
            Hue = 0x48D;
            Weight = 2.0;
            LootType = LootType.Blessed;
        }

        public ReagentBag(Serial serial)
            : base(serial)
        {
        }

        // Weightless to the carrier. Individual reagent stacks keep their own
        // weight, but it never propagates up past this container.
        public override int GetTotal(TotalType type)
        {
            if (type == TotalType.Weight)
                return 0;

            return base.GetTotal(type);
        }

        private static bool IsAllowed(Item item)
        {
            Type t = item.GetType();

            for (int i = 0; i < _Allowed.Length; i++)
            {
                if (_Allowed[i] == t)
                    return true;
            }

            return false;
        }

        private int CountOf(Type t)
        {
            int total = 0;

            foreach (Item i in Items)
            {
                if (i != null && !i.Deleted && i.GetType() == t)
                    total += i.Amount;
            }

            return total;
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (item == null)
                return false;

            if (!IsAllowed(item))
            {
                if (message && m != null)
                    m.SendMessage("Only Magery, Necromancy and Mysticism reagents can go in this bag.");
                return false;
            }

            if (CountOf(item.GetType()) + item.Amount > MaxPerReagent)
            {
                if (message && m != null)
                    m.SendMessage("This bag holds at most {0:N0} of each reagent. Split your stack.", MaxPerReagent);
                return false;
            }

            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!CheckHold(from, dropped, true, true, 0, 0))
                return false;

            return base.OnDragDrop(from, dropped);
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!CheckHold(from, item, true, true, 0, 0))
                return false;

            return base.OnDragDropInto(from, item, p);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
