using Server;

namespace Server.Items
{
    public class FeyToken : InvasionToken
    {
        [Constructable]
        public FeyToken() : this(1) { }

        [Constructable]
        public FeyToken(int amount) : base(amount)
        {
            Name = "Fey Bone";
            Hue = 0x1BF;
        }

        public FeyToken(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
