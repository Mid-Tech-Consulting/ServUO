using System;

using Server.Accounting;
using Server.Multis;

namespace Server.Engines.VeteranRewards
{
    public class HouseStorageIncreaseDeed : Item, IRewardItem
    {
        private bool m_IsRewardItem;

        [Constructable]
        public HouseStorageIncreaseDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
            Hue = 0x47E;
        }

        public HouseStorageIncreaseDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber { get { return 1049643; } } // a house storage increase deed

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; InvalidateProperties(); }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("House Storage Increase Deed");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
            {
                list.Add(1076216); // 1st Year Veteran Reward
            }

            list.Add(1049645, "80"); // Increases maximum lockdowns and secure storage by ~1_VAL~%.
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            Account acct = from.Account as Account;

            if (acct != null && from.IsPlayer())
            {
                TimeSpan time = RewardSystem.RewardInterval - (DateTime.UtcNow - acct.Created);

                if (time > TimeSpan.Zero)
                {
                    from.SendLocalizedMessage(1008126, true, Math.Ceiling(time.TotalDays / RewardSystem.RewardInterval.TotalDays).ToString()); // Your account is not old enough to use this item.
                    return;
                }
            }

            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house == null)
            {
                from.SendMessage("You must be standing inside your house to use this deed.");
                return;
            }

            if (!house.IsOwner(from))
            {
                from.SendLocalizedMessage(502094); // You must be in your house to do this.
                return;
            }

            if (house.HasHouseStorageIncrease)
            {
                from.SendMessage("This house already has an increased storage bonus applied.");
                return;
            }

            house.HasHouseStorageIncrease = true;
            from.SendMessage("Your house storage and lockdown capacity have been increased.");

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }
}
