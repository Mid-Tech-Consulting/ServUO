using System;
using Server;
using Server.Accounting;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
    public class OneTimeRewardStone : Item
    {
        [Constructable]
        public OneTimeRewardStone() : base(0xED4) // stone graphic
        {
            Name = "a reward stone";
            Movable = false;
            Hue = 1153;
        }

        public OneTimeRewardStone(Serial serial) : base(serial) { }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile player))
                return;

            // Check if already claimed
            if (!(player.Account is Account acct) || acct.GetTag("RewardStoneUsed") != null)
            {
                from.SendMessage("You have already claimed this reward.");
                return;
            }

            // Give reward (EDIT THIS PART)
            from.AddToBackpack(new Gold(10000)); // example reward
            from.SendMessage("You receive your reward!");

            // Mark as used
            acct.SetTag("RewardStoneUsed", "true");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}