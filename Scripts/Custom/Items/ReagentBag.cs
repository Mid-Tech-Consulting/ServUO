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
        public override int DefaultMaxWeight { get { return 1000000; } } // never the limiting factor; weight is hidden via IsVirtualItem

        // Weightless to the carrier and to any container we live in. This is
        // the same trick BankBox uses: IsVirtualItem=true makes both
        // Item.UpdateTotal stop propagating weight to the parent AND
        // Container.UpdateTotals skip us in the full recompute, so the two
        // weight-tracking paths can never disagree. (The earlier
        // GetTotal(Weight)=>0 override only fixed one path and caused the
        // cached parent totals to drift when reagents were moved in/out.)
        public override bool IsVirtualItem { get { return true; } }

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

            // Item-count guard against pathological state; with stack merging the
            // bag never realistically holds more than the 17 reagent types.
            if (!m.IsStaff() && checkItems && MaxItems != 0 &&
                (TotalItems + plusItems + item.TotalItems + (item.IsVirtualItem ? 0 : 1)) > MaxItems)
            {
                if (message)
                    SendFullItemsMessage(m, item);
                return false;
            }

            // Deliberately NOT calling base.CheckHold -- the base walks up the
            // parent chain to ask the carrier (backpack) whether it can hold
            // the new weight, which rejects 60k bloodmoss every time even
            // though the bag is virtual and that weight will never actually
            // land on the pack. The bag's own weight cap is meaningless
            // (DefaultMaxWeight is artificially huge) so there's nothing more
            // to check at the bag level either.
            return true;
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
