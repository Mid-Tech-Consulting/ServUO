/*Script Modded by: Leonel Strouse A.K.A. AlphaDragon
 * 11/09/2019 18:00 HRS
 * Version 0.002
 */
using Server;
using Server.Gumps;
using System.Linq;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Targeting;
using System;

namespace Server.Items
{
    public enum AdvancedDecorateCommand
    {
        None,
        Secure,
        Lockdown,
        Release,
        Turn,
        Up,
        Down,
        North,
        East,
        South,
        West,
        GetHue,
        Close
    }

    public class AdvancedInteriorDecorator : Item
    {
//        public override int LabelNumber { get { return 1041280; } } // an interior decorator

        private AdvancedDecorateCommand m_Command;

        [Constructable]
        public AdvancedInteriorDecorator()
            : base(0xFC1)
        {
            Name = " An Advance Interior Decorator";
            Weight = 1.0;
            LootType = LootType.Regular;
        }

        public AdvancedInteriorDecorator(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AdvancedDecorateCommand Command
        {
            get { return m_Command; }
            set
            {
                m_Command = value;
                InvalidateProperties();
            }
        }

        public static bool InHouse(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            return (house != null && house.IsFriend(from));
        }

        public static bool CheckUse(AdvancedInteriorDecorator tool, Mobile from)
        {
            if (!InHouse(from))
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            else
                return true;

            return false;
        }

        //public override void GetProperties(ObjectPropertyList list)
        //{
        //    base.GetProperties(list);

        //    if (m_Command != AdvancedDecorateCommand.None)
        //        list.Add(1018322 + (int)m_Command); // Turn/Up/Down
        //}

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

        public override void OnDoubleClick(Mobile from)
        {
            //if (!CheckUse(this, from))
            //    return;

            m_Command = AdvancedDecorateCommand.None;

            if (from.FindGump(typeof(InternalGump)) == null)
                from.SendGump(new InternalGump(from, this));
        }

        private class InternalGump : Gump
        {
            private readonly AdvancedInteriorDecorator m_Decorator;

            public InternalGump(Mobile from, AdvancedInteriorDecorator decorator)
                : base(150, 50)
            {
                m_Decorator = decorator;

                AddPage(0);
                AddBackground(0, 0, 400, 310, 5054);

                // Title
                AddHtml(0, 15, 400, 20, "<center><BASEFONT COLOR=#FFFF00 size=7>Deco Tool</BASEFONT></center>", false, false);
                AddImageTiled(20, 40, 360, 2, 96);

                int leftX = 30;
                int rightX = 210;
                int labelOffsetX = 35;
                int startY = 55;
                int spacing = 35;

                // Left column - first 6 options
                AddButton(leftX, startY, (decorator.Command == AdvancedDecorateCommand.Secure ? 2154 : 2152), 2154, 1, GumpButtonType.Reply, 0);
                AddLabel(leftX + labelOffsetX, startY + 2, 0x481, "Secure");

                AddButton(leftX, startY + spacing, (decorator.Command == AdvancedDecorateCommand.Lockdown ? 2154 : 2152), 2154, 2, GumpButtonType.Reply, 0);
                AddLabel(leftX + labelOffsetX, startY + spacing + 2, 0x481, "Lockdown");

                AddButton(leftX, startY + spacing * 2, (decorator.Command == AdvancedDecorateCommand.Release ? 2154 : 2152), 2154, 3, GumpButtonType.Reply, 0);
                AddLabel(leftX + labelOffsetX, startY + spacing * 2 + 2, 0x481, "Release");

                AddButton(leftX, startY + spacing * 3, (decorator.Command == AdvancedDecorateCommand.Turn ? 2154 : 2152), 2154, 4, GumpButtonType.Reply, 0);
                AddLabel(leftX + labelOffsetX, startY + spacing * 3 + 2, 0x481, "Turn");

                AddButton(leftX, startY + spacing * 4, (decorator.Command == AdvancedDecorateCommand.Up ? 2154 : 2152), 2154, 5, GumpButtonType.Reply, 0);
                AddLabel(leftX + labelOffsetX, startY + spacing * 4 + 2, 0x481, "Up");

                AddButton(leftX, startY + spacing * 5, (decorator.Command == AdvancedDecorateCommand.Down ? 2154 : 2152), 2154, 6, GumpButtonType.Reply, 0);
                AddLabel(leftX + labelOffsetX, startY + spacing * 5 + 2, 0x481, "Down");

                // Right column - remaining 6 options
                AddButton(rightX, startY, (decorator.Command == AdvancedDecorateCommand.North ? 2154 : 2152), 2154, 7, GumpButtonType.Reply, 0);
                AddLabel(rightX + labelOffsetX, startY + 2, 0x481, "North");

                AddButton(rightX, startY + spacing, (decorator.Command == AdvancedDecorateCommand.East ? 2154 : 2152), 2154, 8, GumpButtonType.Reply, 0);
                AddLabel(rightX + labelOffsetX, startY + spacing + 2, 0x481, "East");

                AddButton(rightX, startY + spacing * 2, (decorator.Command == AdvancedDecorateCommand.South ? 2154 : 2152), 2154, 9, GumpButtonType.Reply, 0);
                AddLabel(rightX + labelOffsetX, startY + spacing * 2 + 2, 0x481, "South");

                AddButton(rightX, startY + spacing * 3, (decorator.Command == AdvancedDecorateCommand.West ? 2154 : 2152), 2154, 10, GumpButtonType.Reply, 0);
                AddLabel(rightX + labelOffsetX, startY + spacing * 3 + 2, 0x481, "West");

                AddButton(rightX, startY + spacing * 4, (decorator.Command == AdvancedDecorateCommand.GetHue ? 2154 : 2152), 2154, 11, GumpButtonType.Reply, 0);
                AddLabel(rightX + labelOffsetX, startY + spacing * 4 + 2, 0x481, "Get Hue");

                AddButton(rightX, startY + spacing * 5, (decorator.Command == AdvancedDecorateCommand.Close ? 2154 : 2152), 2154, 13, GumpButtonType.Reply, 0);
                AddLabel(rightX + labelOffsetX, startY + spacing * 5 + 2, 0x481, "Close");
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                AdvancedDecorateCommand command = AdvancedDecorateCommand.None;
                Mobile m = sender.Mobile;

                int cliloc = 0;
                string c_String = null;

                switch (info.ButtonID)
                {
                    case 1://secure
                        c_String = "Select an object to secure."; // Select an object to secure.
                        command = AdvancedDecorateCommand.Secure;
                        break;
                    case 2://lockdown
                        c_String = "Select an object to lock down."; // Select an object to lock down.
                        command = AdvancedDecorateCommand.Lockdown;
                        break;
                    case 3://release
                        c_String = "Select an object to release."; // Select an object to release.
                        command = AdvancedDecorateCommand.Release;
                        break;
                    case 4://turn
                        cliloc = 1073404; // Select an object to turn.
                        command = AdvancedDecorateCommand.Turn;
                        break;
                    case 5://up
                        cliloc = 1073405; // Select an object to increase its height.
                        command = AdvancedDecorateCommand.Up;
                        break;
                    case 6://down
                        cliloc = 1073406; // Select an object to lower its height.
                        command = AdvancedDecorateCommand.Down;
                        break;
                    case 7://north
                        c_String = "Select an object to move north."; // Select an object to move north.
                        command = AdvancedDecorateCommand.North;
                        break;
                    case 8://east
                        c_String = "Select an object to move east."; // Select an object to move east.
                        command = AdvancedDecorateCommand.East;
                        break;
                    case 9://south
                        c_String = "Select an object to move south."; // Select an object to move south.
                        command = AdvancedDecorateCommand.South;
                        break;
                    case 10://west
                        c_String = "Select an object to move west."; // Select an object to move west.
                        command = AdvancedDecorateCommand.West;
                        break;
                    case 11://get hue
                        cliloc = 1158864; // Select an object to get the hue.
                        command = AdvancedDecorateCommand.GetHue;
                        break;
                    case 12://Close
                        c_String = "Close"; // Close
                        command = AdvancedDecorateCommand.Close;
                        break;
                }

                if (command != AdvancedDecorateCommand.None & command != AdvancedDecorateCommand.Close)
                {
                    m_Decorator.Command = command;
                    m.SendGump(new InternalGump(m, m_Decorator));

                    if (cliloc != 0)
                        m.SendLocalizedMessage(cliloc);
                    if (c_String != null)
                        m.SendMessage(c_String);

                    m.Target = new InternalTarget(m_Decorator);
                }
                else
                {
                    Target.Cancel(m);
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly AdvancedInteriorDecorator m_Decorator;

            public InternalTarget(AdvancedInteriorDecorator decorator)
                : base(-1, false, TargetFlags.None)
            {
                CheckLOS = false;

                m_Decorator = decorator;
            }

            protected override void OnTargetNotAccessible(Mobile from, object targeted)
            {
                OnTarget(from, targeted);
            }

            private static Type[] m_KingsCollectionTypes = new Type[]
            {
                typeof(BirdLamp),    typeof(DragonLantern),
                typeof(KoiLamp),   typeof(TallLamp)
            };

            private static bool IsKingsCollection(Item item)
            {
                return m_KingsCollectionTypes.Any(t => t == item.GetType());
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Decorator.Command == AdvancedDecorateCommand.GetHue)
                {
                    int hue = 0;

                    if (targeted is Item)
                        hue = ((Item)targeted).Hue;
                    else if (targeted is Mobile)
                        hue = ((Mobile)targeted).Hue;
                    else
                    {
                        from.Target = new InternalTarget(m_Decorator);
                        return;
                    }

                    from.SendLocalizedMessage(1158862, String.Format("{0}", hue)); // That object is hue ~1_HUE~
                }
                else if (targeted is Item && CheckUse(m_Decorator, from))
                {
                    BaseHouse house = BaseHouse.FindHouseAt(from);
                    Item item = (Item)targeted;

                    bool isDecorableComponent = false;

                    if (m_Decorator.Command == AdvancedDecorateCommand.Turn && IsKingsCollection(item))
                    {
                        isDecorableComponent = true;
                    }
                    else if (item is AddonComponent || item is AddonContainerComponent || item is BaseAddonContainer)
                    {
                        object addon = null;
                        int count = 0;

                        if (item is AddonComponent)
                        {
                            AddonComponent component = (AddonComponent)item;
                            count = component.Addon.Components.Count;
                            addon = component.Addon;
                        }
                        else if (item is AddonContainerComponent)
                        {
                            AddonContainerComponent component = (AddonContainerComponent)item;
                            count = component.Addon.Components.Count;
                            addon = component.Addon;
                        }
                        else if (item is BaseAddonContainer)
                        {
                            BaseAddonContainer container = (BaseAddonContainer)item;
                            count = container.Components.Count;
                            addon = container;
                        }

                        if (count == 1 && Core.SE)
                            isDecorableComponent = true;

                        if (item is EnormousVenusFlytrapAddon)
                            isDecorableComponent = true;

                        if (m_Decorator.Command == AdvancedDecorateCommand.Turn)
                        {
                            FlipableAddonAttribute[] attributes = (FlipableAddonAttribute[])addon.GetType().GetCustomAttributes(typeof(FlipableAddonAttribute), false);

                            if (attributes.Length > 0)
                                isDecorableComponent = true;
                        }
                    }
                    else if (item is Banner && m_Decorator.Command != AdvancedDecorateCommand.Turn)
                    {
                        isDecorableComponent = true;
                    }

                    if (house == null || /*!house.IsCoOwner(from)*/ !house.IsFriend(from))
                    {
                        from.SendLocalizedMessage(502092); // You must be in your house to do
                    }
                    else if (item.Parent != null || !house.IsInside(item))
                    {
                        from.SendLocalizedMessage(1042270); // That is not in your house.
                    }
                    else if (!house.IsLockedDown(item) && !house.IsSecure(item) && !isDecorableComponent)
                    {
                        if (item is AddonComponent && m_Decorator.Command == AdvancedDecorateCommand.Turn)
                            from.SendLocalizedMessage(1042273); // You cannot turn that.
                        else if (item is AddonComponent && m_Decorator.Command == AdvancedDecorateCommand.Up)
                            from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
                        else if (item is AddonComponent && m_Decorator.Command == AdvancedDecorateCommand.Down)
                            from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
                        else if (m_Decorator.Command == AdvancedDecorateCommand.Secure)
                            Secure(item, from);
                        else if (m_Decorator.Command == AdvancedDecorateCommand.Lockdown)
                            Lockdown(item, from);
                        else
                            from.SendMessage("That is not locked down or secured.");
                    }
                    else if (item is VendorRentalContract)
                    {
                        from.SendLocalizedMessage(1062491); // You cannot use the house decorator on that object.
                    }
                    /*else if (item.TotalWeight + item.PileWeight > 100)
                    {
                        from.SendLocalizedMessage(1042272); // That is too heavy.
                    }*/
                    else
                    {
                        switch (m_Decorator.Command)
                        {
                            case AdvancedDecorateCommand.None:
                                None(item, from);
                                break;
                            case AdvancedDecorateCommand.Secure:
                                Secure(item, from);
                                break;
                            case AdvancedDecorateCommand.Lockdown:
                                Lockdown(item, from);
                                break;
                            case AdvancedDecorateCommand.Release:
                                Release(item, from);
                                break;
                            case AdvancedDecorateCommand.Turn:
                                Turn(item, from);
                                break;
                            case AdvancedDecorateCommand.Up:
                                Up(item, from);
                                break;
                            case AdvancedDecorateCommand.Down:
                                Down(item, from);
                                break;
                            case AdvancedDecorateCommand.North:
                                North(item, from);
                                break;
                            case AdvancedDecorateCommand.East:
                                East(item, from);
                                break;
                            case AdvancedDecorateCommand.South:
                                South(item, from);
                                break;
                            case AdvancedDecorateCommand.West:
                                West(item, from);
                                break;
                            case AdvancedDecorateCommand.GetHue:
                                GetHue(item, from);
                                break;
                            case AdvancedDecorateCommand.Close:
                                Close(item, from);
                                break;
                        }
                    }
                }

                from.Target = new InternalTarget(m_Decorator);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (cancelType == TargetCancelType.Canceled)
                    from.CloseGump(typeof(AdvancedInteriorDecorator.InternalGump));
            }

            private static void None(Item item, Mobile from)
            {
            }

            private static void Secure(Item item, Mobile from)
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);
                if (house.IsLockedDown(item))
                    from.SendMessage("That is already locked down.");
                else if (house.IsSecure(item))
                    from.SendMessage("That is already secured.");
                else if (house.IsFriend(from)&&!house.IsCoOwner(from))
                { from.SendMessage("Only Owners and CoOwners are allowed to secure things in a house.");
                    return;
                }
                else
                    house.AddSecure(from, item);
            }

            private static void Lockdown(Item item, Mobile from)
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);
                if (house.IsLockedDown(item))
                    from.SendMessage("That is already locked down.");
                else if (house.IsSecure(item))
                    from.SendMessage("That is already secured.");
                else
                    house.LockDown(from, item, true);
            }

            private static void Release(Item item, Mobile from)
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);
                if (!house.IsLockedDown(item) && !house.IsSecure(item) && (item.Movable))
                    from.SendMessage("That is not locked down or secured.");
                else
                    house.Release(from, item);
            }

            private static void Turn(Item item, Mobile from)
            {
                    if (item is IFlipable)
                    {
                        ((IFlipable)item).OnFlip(from);
                        return;
                    }

                    if (item is AddonComponent || item is AddonContainerComponent || item is BaseAddonContainer)
                    {
                        object addon = null;

                        if (item is AddonComponent)
                            addon = ((AddonComponent)item).Addon;
                        else if (item is AddonContainerComponent)
                            addon = ((AddonContainerComponent)item).Addon;
                        else if (item is BaseAddonContainer)
                            addon = (BaseAddonContainer)item;

                        FlipableAddonAttribute[] aAttributes = (FlipableAddonAttribute[])addon.GetType().GetCustomAttributes(typeof(FlipableAddonAttribute), false);

                        if (aAttributes.Length > 0)
                        {
                            aAttributes[0].Flip(from, (Item)addon);
                            return;
                        }
                    }

                    FlipableAttribute[] attributes = (FlipableAttribute[])item.GetType().GetCustomAttributes(typeof(FlipableAttribute), false);

                    if (attributes.Length > 0)
                        attributes[0].Flip(item);
                    else
                        from.SendLocalizedMessage(1042273); // You cannot turn that.                
            }

            private static void Up(Item item, Mobile from)
            {
                int floorZ = GetFloorZ(item);

                if (floorZ > int.MinValue && item.Z < (floorZ + 14)) // Confirmed : no height checks here
                    item.Location = new Point3D(item.Location, item.Z + 1);
                else
                    from.SendLocalizedMessage(1042274); // You cannot raise it up any higher.
            }

            private static void Down(Item item, Mobile from)
            {
                int floorZ = GetFloorZ(item);

                if (floorZ > int.MinValue && item.Z > GetFloorZ(item))
                    item.Location = new Point3D(item.Location, item.Z - 1);
                else
                    from.SendLocalizedMessage(1042275); // You cannot lower it down any further.
            }

            private static void North(Item item, Mobile from)
            {
                BaseHouse house = BaseHouse.FindHouseAt(item);

                Point3D ourLoc = item.GetWorldLocation();
                Point3D goingLoc = new Point3D(ourLoc.X, ourLoc.Y -2, ourLoc.Z);

                if (house.IsInside(goingLoc, ourLoc.Z))
                    item.Y = (item.Y -1);
                else
                    from.SendMessage("You cannot move it to the north any further.");

            }

            private static void East(Item item, Mobile from)
            {
                BaseHouse house = BaseHouse.FindHouseAt(item);

                Point3D ourLoc = item.GetWorldLocation();
                Point3D goingLoc = new Point3D(ourLoc.X +1, ourLoc.Y, ourLoc.Z);

                if (house.IsInside(goingLoc, ourLoc.Z))
                    item.X = (item.X +1);
                else
                    from.SendMessage("You cannot move it to the east any further.");

            }

            private static void South(Item item, Mobile from)
            {
                BaseHouse house = BaseHouse.FindHouseAt(item);

                Point3D ourLoc = item.GetWorldLocation();
                Point3D goingLoc = new Point3D(ourLoc.X, ourLoc.Y +1, ourLoc.Z);

                if (house.IsInside(goingLoc, ourLoc.Z))
                     item.Y = (item.Y +1); 
                else
                    from.SendMessage("You cannot move it to the south any further.");
            }

            private static void West(Item item, Mobile from)
            {
                BaseHouse house = BaseHouse.FindHouseAt(item);

                Point3D ourLoc = item.GetWorldLocation();
                Point3D goingLoc = new Point3D(ourLoc.X -2, ourLoc.Y, ourLoc.Z);

                if (house.IsInside(goingLoc, ourLoc.Z))
                    item.X = (item.X -1);
                else
                    from.SendMessage("You cannot move it to the west any further.");
            }

            private static void GetHue(Item item, Mobile from)
            {
            }

            private static void Close(Item item, Mobile from)
            {
                from.CloseGump(typeof(AdvancedInteriorDecorator.InternalGump));
                Target.Cancel(from);
            }
            private static void Command(Item item, Mobile from)
            {
            }
            private static int GetFloorZ(Item item)
            {
                Map map = item.Map;

                if (map == null)
                    return int.MinValue;

                StaticTile[] tiles = map.Tiles.GetStaticTiles(item.X, item.Y, true);

                int z = int.MinValue;

                for (int i = 0; i < tiles.Length; ++i)
                {
                    StaticTile tile = tiles[i];
                    ItemData id = TileData.ItemTable[tile.ID & 0x3FFF];

                    int top = tile.Z; // Confirmed : no height checks here

                    if (id.Surface && !id.Impassable && top > z && top <= item.Z)
                        z = top;
                }

                if (z == int.MinValue)
                    z = map.Tiles.GetLandTile(item.X, item.Y).Z;

                return z;
            }
        }
    }
}
