using Server.Multis;

namespace Server.Items
{
    public class BankStone : Item
    {
        [Constructable]
        public BankStone() : base(3796)
        {
            Movable = true;
            Hue = 0x485;
            Name = "Bank Stone";
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
            {
                from.SendMessage("You are too far away to use the Bank Stone.");
                return;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null || (!IsLockedDown && !IsSecure))
            {
                from.SendMessage("This bank stone must be secured or locked down in a house.");
                return;
            }

            if (!house.IsOwner(from) && !house.IsCoOwner(from) && !house.IsFriend(from))
            {
                from.SendMessage("You must be a friend, co-owner, or owner of the house to use this bank stone.");
                return;
            }

            from.BankBox.Open();
        }

        public BankStone(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}
