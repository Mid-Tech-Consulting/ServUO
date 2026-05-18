using Server;

namespace Server.Items
{
    public class BoneToken : InvasionToken
    {
        [Constructable]
        public BoneToken() : this(1) { }

        [Constructable]
        public BoneToken(int amount) : base(amount)
        {
            Name = "Bone Token";
            Hue = 0x47E;
        }

        public BoneToken(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
