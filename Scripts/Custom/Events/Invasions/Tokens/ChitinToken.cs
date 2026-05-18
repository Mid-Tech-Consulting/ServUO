using Server;

namespace Server.Items
{
    public class ChitinToken : InvasionToken
    {
        [Constructable]
        public ChitinToken() : this(1) { }

        [Constructable]
        public ChitinToken(int amount) : base(amount)
        {
            Name = "Chitin Bone";
            Hue = 0x497;
        }

        public ChitinToken(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
