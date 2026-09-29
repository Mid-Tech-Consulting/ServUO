using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class HalloweenRewardStone : Item
    {
        [Constructable]
        public HalloweenRewardStone() : base(0xEDC)
        {
            Movable = false;
            Hue = 1161; // Glowing Blaze Orange
            Name = "Halloween Cemetery Event Rewards";
        }

        public HalloweenRewardStone(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Turn in Halloween Pumpkins for Exclusive Rewards</BASEFONT>");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.CloseGump(typeof(HalloweenRewardGump));
            from.SendGump(new HalloweenRewardGump(from));
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

namespace Server.Mobiles
{
    public class JackThePumpkinCarver : BaseCreature
    {
        private static readonly string[] m_Greetings = new string[]
        {
            "Hehehe... welcome, mortal! Have you purged the cemeteries of the unquiet dead?",
            "Bring me the pumpkins the graveyard fiends stole, and I shall bestow upon you dark riches!",
            "I possess dyes of phantom white, vampire blood, and witch green... none of which can be found in any common shop!",
            "Beware the orange spiders lurking in the crypts... but if you slay them, they make magnificent mounts!",
            "Trade with Old Jack! These cemetery treasures cannot be found on champions or common beasts!"
        };

        [Constructable]
        public JackThePumpkinCarver() : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            Name = "Jack";
            Title = "the Pumpkin Carver";
            Body = 0x190;
            CantWalk = true;
            Frozen = true;
            Blessed = true;

            InitOutfit();
        }

        public JackThePumpkinCarver(Serial serial) : base(serial)
        {
        }

        public void InitOutfit()
        {
            AddItem(new JackOLanternHelm() { Hue = 1161, Movable = false });
            AddItem(new FancyShirt(1161) { Movable = false });
            AddItem(new FullApron(1157) { Movable = false });
            AddItem(new LongPants(1170) { Movable = false });
            AddItem(new Boots(1) { Movable = false });
            AddItem(new ButcherKnife() { Movable = false, Hue = 1157 });
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(Location, 4))
            {
                from.SendLocalizedMessage(500446);
                return;
            }

            Say(m_Greetings[Utility.Random(m_Greetings.Length)]);
            PlaySound(0x47B); // Ghostly cackle

            from.CloseGump(typeof(HalloweenRewardGump));
            from.SendGump(new HalloweenRewardGump(from));
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
