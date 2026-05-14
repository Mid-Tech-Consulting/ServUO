using System;

using Server;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    // Single-use deed that places a TrashBarrel on a ship the player has at
    // least Crewman access to. The barrel is registered as a ship fixture so
    // it travels with the ship when it sails. Stock UO only lets a player
    // claim a trash barrel through the house sign; this fills the gap for
    // crews who live on a galleon.
    public class ShipTrashBarrelDeed : Item
    {
        public override int LabelNumber { get { return 1041064; } } // a trash barrel

        [Constructable]
        public ShipTrashBarrelDeed()
            : base(0x14F0)
        {
            Name = "a Ship Trash Barrel Deed";
            Hue = 0x47E;
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public ShipTrashBarrelDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            from.SendMessage(0x40, "Target a tile on the deck of your ship.");
            from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly ShipTrashBarrelDeed m_Deed;

            public InternalTarget(ShipTrashBarrelDeed deed)
                : base(8, true, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Deed == null || m_Deed.Deleted)
                    return;

                if (!m_Deed.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    return;
                }

                IPoint3D p = targeted as IPoint3D;

                if (p == null)
                {
                    from.SendMessage(0x22, "Target a deck tile on your ship.");
                    return;
                }

                Point3D loc = new Point3D(p.X, p.Y, p.Z);
                BaseBoat boat = BaseBoat.FindBoatAt(loc, from.Map);

                if (boat == null)
                {
                    from.SendMessage(0x22, "That spot isn't on a ship.");
                    return;
                }

                BaseGalleon galleon = boat as BaseGalleon;

                if (galleon == null)
                {
                    from.SendMessage(0x22, "Trash barrels can only be placed on a galleon.");
                    return;
                }

                if (galleon.GetSecurityLevel(from) < SecurityLevel.Crewman)
                {
                    from.SendLocalizedMessage(1116726); // This is not your ship!
                    return;
                }

                if (!boat.Contains(loc.X, loc.Y))
                {
                    from.SendMessage(0x22, "Place the barrel on the ship's deck.");
                    return;
                }

                TrashBarrel barrel = new TrashBarrel
                {
                    Movable = false,
                };

                barrel.MoveToWorld(loc, from.Map);
                galleon.AddFixture(barrel);

                Effects.PlaySound(loc, from.Map, 0x42);
                from.SendMessage(0x40, "You place the trash barrel on the deck. Three minutes after items are dropped in, they will be deleted.");

                m_Deed.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
