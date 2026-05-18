using Server;

namespace Server.Items
{
    public class RepondToken : InvasionToken
    {
        [Constructable]
        public RepondToken() : this(1) { }

        [Constructable]
        public RepondToken(int amount) : base(amount)
        {
            Name = "Repond Bone";
            Hue = 0x6BB;
        }

        public RepondToken(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
