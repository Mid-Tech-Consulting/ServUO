using Server.Network;

namespace Server.Items
{
    public class FieldWardingStone : Item
    {
        public override string DefaultName => "Field Warding Stone";

        [Constructable]
        public FieldWardingStone() : base(0xED4)
        {
            Movable = false;
        }

        public FieldWardingStone(Serial serial) : base(serial) { }

        public override bool OnMoveOver(Mobile m)
        {
            return true;
        }

        public override void SendInfoTo(NetState state, bool sendOplPacket)
        {
            if (state.Mobile != null && state.Mobile.AccessLevel >= AccessLevel.GameMaster)
                base.SendInfoTo(state, sendOplPacket);
        }

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
