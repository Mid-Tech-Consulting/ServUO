using System;

using Server;

namespace Server.Items
{
    // Abstract base for every invasion event's currency item. Concrete
    // subclasses (BoneToken / BrimstoneToken / etc.) just override Name and
    // pick a hue so each theme stacks separately even though all use the
    // bone-pile art.
    public abstract class InvasionToken : Item
    {
        public InvasionToken() : this(1)
        {
        }

        public InvasionToken(int amount) : base(0x1B11)
        {
            Stackable = true;
            Weight = 0.0;
            Amount = amount;
            LootType = LootType.Regular;
        }

        public InvasionToken(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
