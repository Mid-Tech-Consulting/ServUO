using Server;

namespace Server.Items
{
    // BG's Fishing Pole -- 2nd-year veteran reward variant of the classic
    // fishing pole. Blessed, channels spells, never consumes charges.
    //
    // Charge handling on Siege: HarvestSystem.cs:248-255 decrements
    // IUsesRemaining tools every harvest and deletes them at zero (active
    // on Siege regardless of the tool's BaseHarvestTool ancestry). To make
    // this pole truly chargeless on Siege we re-implement IUsesRemaining
    // explicitly, so interface dispatch from HarvestSystem lands on no-op
    // accessors that report int.MaxValue and refuse to be set lower or to
    // flip the "show uses remaining" tooltip on. Bait and hook charges
    // still tick because those are separate fields owned by the base class.
    public class BGFishingPole : FishingPole, IUsesRemaining
    {
        public override int LabelNumber { get { return 1041447; } } // fishing pole

        [Constructable]
        public BGFishingPole()
        {
            Name = "BG's Fishing Pole";
            ItemID = 0x0DBF;
            Hue = 0;
            Weight = 8.0;
            LootType = LootType.Blessed;
            Attributes.SpellChanneling = 1;
        }

        public BGFishingPole(Serial serial)
            : base(serial)
        {
        }

        // Explicit interface impls -- silently absorb the Siege harvest
        // system's decrement and ShowUsesRemaining=true write.
        int IUsesRemaining.UsesRemaining
        {
            get { return int.MaxValue; }
            set { /* no-op: pole is unbreakable */ }
        }

        bool IUsesRemaining.ShowUsesRemaining
        {
            get { return false; }
            set { /* no-op: never display a charge counter */ }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1076218); // 2nd Year Veteran Reward (formula: 1076216 + year)
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
