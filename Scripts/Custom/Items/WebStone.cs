using Server;

namespace Server.Items
{
    public class WebStone : Item
    {
        private string m_Url;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        [Constructable]
        public WebStone() : this("Web Stone")
        {
        }

        [Constructable]
        public WebStone(string name) : base(0xED4)
        {
            Name = name;
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (string.IsNullOrEmpty(m_Url))
            {
                from.SendMessage("This Web Stone has no URL configured.");
                return;
            }

            from.LaunchBrowser(m_Url);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!string.IsNullOrEmpty(m_Url))
                list.Add(m_Url);
        }

        public WebStone(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write(m_Url);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Url = reader.ReadString();
        }
    }
}
