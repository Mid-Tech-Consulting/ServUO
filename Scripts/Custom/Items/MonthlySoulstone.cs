namespace Server.Items
{
    public class MonthlySoulstone : SoulStone
    {
        [Constructable]
        public MonthlySoulstone()
            : this(0)
        {
        }

        [Constructable]
        public MonthlySoulstone(int hue)
            : base(null)
        {
            Hue = hue;
            Name = "Monthly Soulstone";
        }

        public MonthlySoulstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
