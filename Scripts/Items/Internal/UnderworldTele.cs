using System;
using Server.Mobiles;

namespace Server.Items
{ 
    public class UnderworldTele : Teleporter
    { 
        [Constructable]
        public UnderworldTele()
        {
        }

        public UnderworldTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            return base.OnMoveOver(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}