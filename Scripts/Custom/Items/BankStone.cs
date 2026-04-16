using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;

namespace Server.Items
{
    public class BankStone : Item, ISecurable
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public BankStone() : base(3796)
        {
            Movable = true;
            Hue = 0x485;
            Name = "Bank Stone";
            Level = SecureLevel.Friends;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
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

            if (!house.HasSecureAccess(from, Level))
            {
                from.SendLocalizedMessage(1061637); // You are not allowed to access this.
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

            writer.Write((int)1); // version

            writer.Write((int)Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version >= 1)
                Level = (SecureLevel)reader.ReadInt();
            else
                Level = SecureLevel.CoOwners;
        }
    }
}
