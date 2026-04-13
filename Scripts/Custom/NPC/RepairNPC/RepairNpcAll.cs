using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class RepairNPCAll : BaseCreature
    {
        private bool m_FreeRepairs;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool FreeRepairs
        {
            get { return m_FreeRepairs; }
            set { m_FreeRepairs = value; }
        }

        [Constructable]
        public RepairNPCAll() : base(AIType.AI_Vendor, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Equipment Repairman";
            Body = Utility.RandomList(400, 401); // random male/female human body
            Hue = Utility.RandomSkinHue();
            CanMove = false;

            AddItem(new Server.Items.Bandana());
            AddItem(new Server.Items.ShortPants());
            AddItem(new Server.Items.Shirt());
            AddItem(new Server.Items.Shoes());
            AddItem(new Server.Items.FullApron());
            AddItem(new Server.Items.SmithHammer(50));
        }

        public RepairNPCAll(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.Alive)
                return;

            if (from.InRange(this.Location, 3))
            {
                from.SendGump(new RepairAllGump(from, this));
            }
            else
            {
                from.SendLocalizedMessage(500446);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write(m_FreeRepairs);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_FreeRepairs = reader.ReadBool();
        }
    }
}
