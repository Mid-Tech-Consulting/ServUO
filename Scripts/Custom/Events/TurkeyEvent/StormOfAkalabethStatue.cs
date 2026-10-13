using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class StormOfAkalabethStatue : Item
    {
        private bool m_IsUberRare;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsUberRare
        {
            get => m_IsUberRare;
            set { m_IsUberRare = value; InvalidateProperties(); }
        }

        [Constructable]
        public StormOfAkalabethStatue() : this(0)
        {
        }

        [Constructable]
        public StormOfAkalabethStatue(int specificHue) : base(0xB1FE)
        {
            Name = "Storm Of Akalabeth";
            Weight = 1.0;
            LootType = LootType.Blessed;

            if (specificHue > 0)
            {
                Hue = specificHue;
                m_IsUberRare = StormOfAkalabeth.IsUberRareHue(specificHue);
            }
            else
            {
                bool uber;
                Hue = StormOfAkalabeth.RollRandomHue(out uber);
                m_IsUberRare = uber;
            }
        }

        public StormOfAkalabethStatue(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsUberRare)
            {
                list.Add("<BASEFONT COLOR=#FFD700><b>[Uber Rare Variant]</b></BASEFONT>");
            }

            list.Add(1158954); // *Redeemable for a pet*<br>*Requires Grandmaster Taming to Claim Pet*
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); // This must be on your person to use.
                return;
            }

            if (from.Skills[SkillName.AnimalTaming].Value < 100.0)
            {
                from.SendLocalizedMessage(1158959, "100"); // ~1_SKILL~ Animal Taming skill is required to redeem this pet.
                return;
            }

            from.CloseGump(typeof(ConfirmAkalabethClaimGump));
            from.SendGump(new ConfirmAkalabethClaimGump(from, this));
        }

        public void Claim(Mobile from)
        {
            if (Deleted || !IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); // This must be on your person to use.
                return;
            }

            if (from.Skills[SkillName.AnimalTaming].Value < 100.0)
            {
                from.SendLocalizedMessage(1158959, "100");
                return;
            }

            if ((from.Followers + 2) > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049611); // You have too many followers to summon that creature.
                return;
            }

            StormOfAkalabeth pet = new StormOfAkalabeth();
            pet.Hue = this.Hue;
            pet.Controlled = true;
            pet.ControlMaster = from;
            pet.IsBonded = true;

            pet.MoveToWorld(from.Location, from.Map);
            from.PlaySound(0x5A);

            from.SendMessage(0x35, "You have redeemed the Storm of Akalabeth! The magnificent beast bonds with you immediately.");
            if (m_IsUberRare)
            {
                from.SendMessage(0x22, "Congratulations! You are riding an UBER RARE variant of the Storm of Akalabeth!");
            }

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(m_IsUberRare);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_IsUberRare = reader.ReadBool();
        }
    }

    public class ConfirmAkalabethClaimGump : Gump
    {
        private readonly Mobile m_From;
        private readonly StormOfAkalabethStatue m_Statue;

        public ConfirmAkalabethClaimGump(Mobile from, StormOfAkalabethStatue statue) : base(200, 200)
        {
            m_From = from;
            m_Statue = statue;

            AddPage(0);
            AddBackground(0, 0, 360, 210, 0x2422);

            AddHtml(20, 20, 320, 25, "<BASEFONT COLOR=#FFA500 size=5><CENTER><b>Claim Storm of Akalabeth</b></CENTER></BASEFONT>", false, false);

            string bodyText = statue.IsUberRare
                ? "<BASEFONT COLOR=#FFFFFF>Are you sure you wish to claim this <BASEFONT COLOR=#FFD700><b>UBER RARE</b></BASEFONT> Storm of Akalabeth?<br><br>The pet will be automatically bonded to you upon claiming (requires 2 follower slots).</BASEFONT>"
                : "<BASEFONT COLOR=#FFFFFF>Are you sure you wish to claim this Storm of Akalabeth?<br><br>The pet will be automatically bonded to you upon claiming (requires 2 follower slots).</BASEFONT>";

            AddHtml(25, 55, 310, 90, bodyText, false, false);

            // Accept / OK
            AddButton(60, 160, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            AddHtml(95, 162, 80, 20, "<BASEFONT COLOR=#FFFFFF><b>Claim</b></BASEFONT>", false, false);

            // Cancel
            AddButton(200, 160, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddHtml(235, 162, 80, 20, "<BASEFONT COLOR=#C0C0C0>Cancel</BASEFONT>", false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_Statue == null || m_Statue.Deleted)
                return;

            if (info.ButtonID == 1)
            {
                m_Statue.Claim(m_From);
            }
        }
    }
}
