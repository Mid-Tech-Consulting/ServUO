using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Custom.Events.TurkeyEvent
{
    public class TurkeyRewardStone : Item
    {
        [Constructable]
        public TurkeyRewardStone() : base(0xEDC)
        {
            Movable = true; // Movable so GM can carry and place it!
            Hue = 1;        // Pure Pitch Black for Black Friday
            Name = "Black Friday Turkey Rewards";
        }

        public TurkeyRewardStone(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FFA500><b>Black Friday Turkey Rewards</b></BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Turn in Turkey Feathers for Exclusive Pets & Artifacts</BASEFONT>");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (from.AccessLevel >= AccessLevel.Counselor)
                {
                    from.SendMessage(0x35, "Target the location on the ground where you want to place the Black Friday Turkey Reward Stone:");
                    from.Target = new PlaceStoneTarget(this);
                    return;
                }
            }

            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.CloseGump(typeof(TurkeyRewardGump));
            from.SendGump(new TurkeyRewardGump(from));
        }

        private class PlaceStoneTarget : Target
        {
            private readonly TurkeyRewardStone m_Stone;

            public PlaceStoneTarget(TurkeyRewardStone stone) : base(12, true, TargetFlags.None)
            {
                m_Stone = stone;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Stone == null || m_Stone.Deleted)
                    return;

                IPoint3D p = targeted as IPoint3D;
                if (p != null)
                {
                    Point3D loc = new Point3D(p);
                    m_Stone.MoveToWorld(loc, from.Map);
                    m_Stone.Movable = false; // Lock down in place once placed in the world!
                    from.SendMessage(0x35, "The Black Friday Turkey Reward Stone has been placed and locked down.");
                    Effects.SendLocationParticles(EffectItem.Create(loc, from.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 1, 0, 5030, 0);
                    from.PlaySound(0x214);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
