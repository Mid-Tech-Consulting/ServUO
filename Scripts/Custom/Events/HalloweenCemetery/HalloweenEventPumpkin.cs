using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class HalloweenEventPumpkin : Item
    {
        [Constructable]
        public HalloweenEventPumpkin() : this(1)
        {
        }

        [Constructable]
        public HalloweenEventPumpkin(int amount) : base(0x4694)
        {
            Stackable = true;
            Weight = 0.01;
            Amount = amount;
            Hue = 0;
            Name = "Halloween Pumpkin";
            LootType = LootType.Regular;
        }

        public HalloweenEventPumpkin(Serial serial) : base(serial)
        {
        }

        public override double DefaultWeight => 0.01;

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add("<BASEFONT COLOR=#FFA500>Halloween Event Currency</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Redeem at Cemetery Reward Stones or Jack the Pumpkin Carver</BASEFONT>");
            if (Amount > 1)
            {
                list.Add("<BASEFONT COLOR=#A5D6A7>Double-click to split stack</BASEFONT>");
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from != null && from.Alive && Amount > 1 && IsAccessibleTo(from) && from.InRange(GetWorldLocation(), 3) && from.CanSee(this))
            {
                list.Add(new SplitPumpkinEntry(this));
            }
        }

        private class SplitPumpkinEntry : ContextMenuEntry
        {
            private readonly HalloweenEventPumpkin m_Pumpkin;

            public SplitPumpkinEntry(HalloweenEventPumpkin pumpkin) : base(6142) // Separate
            {
                m_Pumpkin = pumpkin;
            }

            public override bool NonLocalUse => true;

            public override void OnClick()
            {
                Mobile from = Owner?.From;

                if (from == null || m_Pumpkin == null || m_Pumpkin.Deleted)
                    return;

                m_Pumpkin.BeginSplit(from);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || Deleted)
                return;

            if (!IsAccessibleTo(from) || !from.InRange(GetWorldLocation(), 3) || !from.CanSee(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (Amount > 1)
            {
                BeginSplit(from);
            }
            else
            {
                int packTotal = from.Backpack != null ? from.Backpack.GetAmount(typeof(HalloweenEventPumpkin)) : 0;
                from.SendMessage(0x35, "You have {0:N0} Halloween Pumpkin{1} in your backpack. Slay monsters in cemeteries to collect more, then redeem them at a Cemetery Reward Stone or Jack the Pumpkin Carver!", 
                    packTotal,
                    packTotal == 1 ? "" : "s");
            }
        }

        public void BeginSplit(Mobile from)
        {
            if (from == null || Deleted)
                return;

            if (!IsAccessibleTo(from) || !from.InRange(GetWorldLocation(), 3) || !from.CanSee(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.CloseGump(typeof(HalloweenPumpkinSplitGump));
            from.SendGump(new HalloweenPumpkinSplitGump(from, this));
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from != null && Amount > 1)
            {
                if ((Weight * Amount) > (from.MaxWeight - from.TotalWeight))
                {
                    from.SendMessage(0x35, "Tip: Double-click or right-click this stack of pumpkins to split it into a lighter amount!");
                }
            }

            return base.OnDragLift(from);
        }

        public bool Split(Mobile from, int amount, bool toBackpack)
        {
            if (from == null || !from.Alive || Deleted || Amount <= 1)
                return false;

            if (!IsAccessibleTo(from) || !from.InRange(GetWorldLocation(), 3) || !from.CanSee(this))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return false;
            }

            if (amount <= 0)
            {
                from.SendMessage(0x22, "Please enter an amount greater than zero.");
                return false;
            }

            if (amount > Amount)
            {
                from.SendMessage(0x22, "You cannot split more pumpkins than exist in the stack ({0:N0}).", Amount);
                return false;
            }

            if (amount == Amount)
            {
                if (toBackpack)
                {
                    if (IsChildOf(from.Backpack))
                    {
                        from.SendMessage(0x35, "The entire stack is already in your backpack.");
                        return true;
                    }

                    if (from.Backpack == null)
                    {
                        from.SendMessage(0x22, "You do not have a backpack.");
                        return false;
                    }

                    if (!from.Backpack.CheckHold(from, this, true, true))
                    {
                        from.SendMessage(0x22, "Your backpack cannot hold the entire stack.");
                        return false;
                    }

                    from.Backpack.TryDropItem(from, this, false);
                    from.PlaySound(0x2E6);
                    from.SendMessage(0x35, "You have moved all {0:N0} Halloween Pumpkins into your backpack.", amount);
                    return true;
                }
                else
                {
                    from.SendMessage(0x22, "To split, specify an amount less than the full stack ({0:N0}).", Amount);
                    return false;
                }
            }

            // Normal split: 1 <= amount < Amount
            if (toBackpack)
            {
                if (from.Backpack == null)
                {
                    from.SendMessage(0x22, "You do not have a backpack.");
                    return false;
                }

                HalloweenEventPumpkin splitItem = new HalloweenEventPumpkin(amount);

                if (!from.Backpack.CheckHold(from, splitItem, true, true))
                {
                    splitItem.Delete();
                    from.SendMessage(0x22, "Your backpack cannot hold that much weight or item count.");
                    return false;
                }

                Amount -= amount;

                if (Parent == from.Backpack)
                {
                    from.Backpack.DropItem(splitItem);
                }
                else
                {
                    from.Backpack.TryDropItem(from, splitItem, false);
                }

                from.PlaySound(0x2E6);
                from.SendMessage(0x35, "You have split {0:N0} Halloween Pumpkins into your backpack. ({1:N0} remaining in the stack)", amount, Amount);
                return true;
            }
            else
            {
                Container parentContainer = Parent as Container;

                if (parentContainer == null)
                {
                    from.SendMessage(0x22, "This stack must be inside a container to split into it.");
                    return false;
                }

                HalloweenEventPumpkin splitItem = new HalloweenEventPumpkin(amount);

                if (!parentContainer.CheckHold(from, splitItem, true, true))
                {
                    splitItem.Delete();
                    from.SendMessage(0x22, "The container cannot hold that much weight or item count.");
                    return false;
                }

                Amount -= amount;
                parentContainer.DropItem(splitItem);

                from.PlaySound(0x2E6);
                from.SendMessage(0x35, "You have split {0:N0} Halloween Pumpkins into the container. ({1:N0} remaining in the stack)", amount, Amount);
                return true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Weight > 0.01)
            {
                Weight = 0.01;
            }
        }
    }
}
