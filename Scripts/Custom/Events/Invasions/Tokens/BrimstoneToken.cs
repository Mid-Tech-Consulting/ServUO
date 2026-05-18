using Server;

namespace Server.Items
{
    public class BrimstoneToken : InvasionToken
    {
        [Constructable]
        public BrimstoneToken() : this(1) { }

        [Constructable]
        public BrimstoneToken(int amount) : base(amount)
        {
            Name = "Brimstone Bone";
            Hue = 0x21;
        }

        public BrimstoneToken(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
