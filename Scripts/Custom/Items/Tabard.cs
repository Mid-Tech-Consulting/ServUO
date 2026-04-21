namespace Server.Items
{
    public class Tabard : BaseOuterTorso
    {
        [Constructable]
        public Tabard()
            : base(0xA412)
        {
            Weight = 3.0;
            Name = "tabard";
            LootType = LootType.Blessed;
        }

        public Tabard(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
