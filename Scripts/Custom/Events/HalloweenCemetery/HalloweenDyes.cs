using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    #region Base Halloween Hair Dye
    public class HalloweenHairDye : Item
    {
        private string m_ColorName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string ColorName
        {
            get { return m_ColorName; }
            set { m_ColorName = value; InvalidateProperties(); }
        }

        [Constructable]
        public HalloweenHairDye() : this(1161, "Candy Corn Orange")
        {
        }

        [Constructable]
        public HalloweenHairDye(int hue, string colorName) : base(0xEFE)
        {
            Weight = 1.0;
            Hue = hue;
            m_ColorName = colorName;
            Name = String.Format("{0} Hair Dye", colorName);
            LootType = LootType.Blessed;
        }

        public HalloweenHairDye(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FFA500>Special Halloween Hair Dye</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Permanent Color: {0}</BASEFONT>", m_ColorName ?? "Spooky Hue");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null)
                {
                    from.CloseGump(typeof(HalloweenHairDyeConfirmGump));
                    from.SendGump(new HalloweenHairDyeConfirmGump(pm, this));
                }
            }
            else
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_ColorName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_ColorName = reader.ReadString();
        }
    }
    #endregion

    #region Base Halloween Beard Dye
    public class HalloweenBeardDye : Item
    {
        private string m_ColorName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string ColorName
        {
            get { return m_ColorName; }
            set { m_ColorName = value; InvalidateProperties(); }
        }

        [Constructable]
        public HalloweenBeardDye() : this(1161, "Candy Corn Orange")
        {
        }

        [Constructable]
        public HalloweenBeardDye(int hue, string colorName) : base(0xEFE)
        {
            Weight = 1.0;
            Hue = hue;
            m_ColorName = colorName;
            Name = String.Format("{0} Beard Dye", colorName);
            LootType = LootType.Blessed;
        }

        public HalloweenBeardDye(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FFA500>Special Halloween Beard Dye</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Permanent Color: {0}</BASEFONT>", m_ColorName ?? "Spooky Hue");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null)
                {
                    from.CloseGump(typeof(HalloweenBeardDyeConfirmGump));
                    from.SendGump(new HalloweenBeardDyeConfirmGump(pm, this));
                }
            }
            else
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_ColorName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_ColorName = reader.ReadString();
        }
    }
    #endregion

    #region The 5 Special Hair Dyes (Exact Hues from Old Server)
    // 1. Candy Corn Orange (1161)
    public class CandyCornOrangeHairDye : HalloweenHairDye
    {
        [Constructable]
        public CandyCornOrangeHairDye() : base(1161, "Candy Corn Orange") { }
        public CandyCornOrangeHairDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // 2. Spider Silk White (1153)
    public class SpiderSilkWhiteHairDye : HalloweenHairDye
    {
        [Constructable]
        public SpiderSilkWhiteHairDye() : base(1153, "Spider Silk White") { }
        public SpiderSilkWhiteHairDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // 3. Vibrant Crimson (1964)
    public class VibrantCrimsonHairDye : HalloweenHairDye
    {
        [Constructable]
        public VibrantCrimsonHairDye() : base(1964, "Vibrant Crimson") { }
        public VibrantCrimsonHairDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // 4. Spectral Venom (1655)
    public class SpectralVenomHairDye : HalloweenHairDye
    {
        [Constructable]
        public SpectralVenomHairDye() : base(1655, "Spectral Venom") { }
        public SpectralVenomHairDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // 5. Spectral Amethyst (1296)
    public class SpectralAmethystHairDye : HalloweenHairDye
    {
        [Constructable]
        public SpectralAmethystHairDye() : base(1296, "Spectral Amethyst") { }
        public SpectralAmethystHairDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // Legacy Aliases for backwards-compatibility
    public class JackOLanternBlazeHairDye : CandyCornOrangeHairDye { [Constructable] public JackOLanternBlazeHairDye() { } public JackOLanternBlazeHairDye(Serial s) : base(s) { } }
    public class SpectralPhantomHairDye : SpiderSilkWhiteHairDye { [Constructable] public SpectralPhantomHairDye() { } public SpectralPhantomHairDye(Serial s) : base(s) { } }
    public class VampireBloodHairDye : VibrantCrimsonHairDye { [Constructable] public VampireBloodHairDye() { } public VampireBloodHairDye(Serial s) : base(s) { } }
    public class WitchesBrewHairDye : SpectralVenomHairDye { [Constructable] public WitchesBrewHairDye() { } public WitchesBrewHairDye(Serial s) : base(s) { } }
    public class NightmareVoidHairDye : SpectralAmethystHairDye { [Constructable] public NightmareVoidHairDye() { } public NightmareVoidHairDye(Serial s) : base(s) { } }
    #endregion

    #region The 5 Special Beard Dyes (Exact Hues from Old Server)
    // 1. Candy Corn Orange Beard Dye (1161)
    public class CandyCornOrangeBeardDye : HalloweenBeardDye
    {
        [Constructable]
        public CandyCornOrangeBeardDye() : base(1161, "Candy Corn Orange") { }
        public CandyCornOrangeBeardDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // 2. Spider Silk White Beard Dye (1153)
    public class SpiderSilkWhiteBeardDye : HalloweenBeardDye
    {
        [Constructable]
        public SpiderSilkWhiteBeardDye() : base(1153, "Spider Silk White") { }
        public SpiderSilkWhiteBeardDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // 3. Vibrant Crimson Beard Dye (1964)
    public class VibrantCrimsonBeardDye : HalloweenBeardDye
    {
        [Constructable]
        public VibrantCrimsonBeardDye() : base(1964, "Vibrant Crimson") { }
        public VibrantCrimsonBeardDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // 4. Spectral Venom Beard Dye (1655)
    public class SpectralVenomBeardDye : HalloweenBeardDye
    {
        [Constructable]
        public SpectralVenomBeardDye() : base(1655, "Spectral Venom") { }
        public SpectralVenomBeardDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // 5. Spectral Amethyst Beard Dye (1296)
    public class SpectralAmethystBeardDye : HalloweenBeardDye
    {
        [Constructable]
        public SpectralAmethystBeardDye() : base(1296, "Spectral Amethyst") { }
        public SpectralAmethystBeardDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }

    // Legacy Aliases for backwards-compatibility
    public class JackOLanternBlazeBeardDye : CandyCornOrangeBeardDye { [Constructable] public JackOLanternBlazeBeardDye() { } public JackOLanternBlazeBeardDye(Serial s) : base(s) { } }
    public class SpectralPhantomBeardDye : SpiderSilkWhiteBeardDye { [Constructable] public SpectralPhantomBeardDye() { } public SpectralPhantomBeardDye(Serial s) : base(s) { } }
    public class VampireBloodBeardDye : VibrantCrimsonBeardDye { [Constructable] public VampireBloodBeardDye() { } public VampireBloodBeardDye(Serial s) : base(s) { } }
    public class WitchesBrewBeardDye : SpectralVenomBeardDye { [Constructable] public WitchesBrewBeardDye() { } public WitchesBrewBeardDye(Serial s) : base(s) { } }
    public class NightmareVoidBeardDye : SpectralAmethystBeardDye { [Constructable] public NightmareVoidBeardDye() { } public NightmareVoidBeardDye(Serial s) : base(s) { } }
    #endregion

    #region Confirmation Gumps
    public class HalloweenHairDyeConfirmGump : Gump
    {
        private readonly PlayerMobile m_User;
        private readonly HalloweenHairDye m_Dye;

        public HalloweenHairDyeConfirmGump(PlayerMobile user, HalloweenHairDye dye) : base(200, 200)
        {
            m_User = user;
            m_Dye = dye;

            user.CloseGump(typeof(HalloweenHairDyeConfirmGump));

            AddPage(0);
            AddBackground(0, 0, 360, 210, 9270);
            AddBackground(15, 15, 75, 75, 9390);

            // Item art preview
            AddItem(25, 25, dye.ItemID, dye.Hue);

            AddHtml(105, 20, 240, 45, String.Format("<BASEFONT COLOR=#FFA500 size=5><b>{0}</b></BASEFONT>", dye.Name), false, false);
            AddHtml(105, 65, 240, 75, "<BASEFONT COLOR=#FFFFFF>This special Halloween potion will permanently alter your hair color until another dye is applied.</BASEFONT>", false, false);

            AddButton(95, 150, 0x992, 0x993, 1, GumpButtonType.Reply, 0); // Okay
            AddButton(205, 150, 0x995, 0x996, 0, GumpButtonType.Reply, 0); // Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (m_Dye == null || m_Dye.Deleted || !m_Dye.IsChildOf(m_User.Backpack))
                {
                    m_User.SendLocalizedMessage(1042010);
                    return;
                }

                if (m_User.HairItemID == 0)
                {
                    m_User.SendLocalizedMessage(502623);
                    return;
                }

                m_User.HairHue = m_Dye.Hue;
                m_User.PlaySound(0x4E);
                m_User.FixedParticles(0x376A, 9, 32, 5030, m_Dye.Hue, 0, EffectLayer.Waist);
                m_User.SendMessage(0x35, "You apply the {0} to your hair!", m_Dye.Name);

                m_Dye.Delete();
            }
        }
    }

    public class HalloweenBeardDyeConfirmGump : Gump
    {
        private readonly PlayerMobile m_User;
        private readonly HalloweenBeardDye m_Dye;

        public HalloweenBeardDyeConfirmGump(PlayerMobile user, HalloweenBeardDye dye) : base(200, 200)
        {
            m_User = user;
            m_Dye = dye;

            user.CloseGump(typeof(HalloweenBeardDyeConfirmGump));

            AddPage(0);
            AddBackground(0, 0, 360, 210, 9270);
            AddBackground(15, 15, 75, 75, 9390);

            // Item art preview
            AddItem(25, 25, dye.ItemID, dye.Hue);

            AddHtml(105, 20, 240, 45, String.Format("<BASEFONT COLOR=#FFA500 size=5><b>{0}</b></BASEFONT>", dye.Name), false, false);
            AddHtml(105, 65, 240, 75, "<BASEFONT COLOR=#FFFFFF>This special Halloween potion will permanently alter your facial hair color until another dye is applied.</BASEFONT>", false, false);

            AddButton(95, 150, 0x992, 0x993, 1, GumpButtonType.Reply, 0); // Okay
            AddButton(205, 150, 0x995, 0x996, 0, GumpButtonType.Reply, 0); // Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (m_Dye == null || m_Dye.Deleted || !m_Dye.IsChildOf(m_User.Backpack))
                {
                    m_User.SendLocalizedMessage(1042010);
                    return;
                }

                if (m_User.FacialHairItemID == 0)
                {
                    m_User.SendMessage(0x22, "You have no facial hair to dye!");
                    return;
                }

                m_User.FacialHairHue = m_Dye.Hue;
                m_User.PlaySound(0x4E);
                m_User.FixedParticles(0x376A, 9, 32, 5030, m_Dye.Hue, 0, EffectLayer.Waist);
                m_User.SendMessage(0x35, "You apply the {0} to your facial hair!", m_Dye.Name);

                m_Dye.Delete();
            }
        }
    }
    #endregion
}
