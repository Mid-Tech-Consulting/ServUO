using Server;

namespace Server.Items
{
    public class DragonscaleToken : InvasionToken
    {
        [Constructable]
        public DragonscaleToken() : this(1) { }

        [Constructable]
        public DragonscaleToken(int amount) : base(amount)
        {
            Name = "Dragonscale Bone";
            Hue = 0x42;
        }

        public DragonscaleToken(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
