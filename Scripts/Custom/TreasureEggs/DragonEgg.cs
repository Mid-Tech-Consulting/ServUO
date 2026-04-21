namespace Server.Items
{
    public class DragonEgg : Item
    {
        public override double DefaultWeight { get { return 1.0; } }

        [Constructable]
        public DragonEgg() : this(1)
        {
        }

        [Constructable]
        public DragonEgg(int amount) : base(0x41BD)
        {
            Name = "a dragon egg";
            Hue = 1152;
            Stackable = true;
            Amount = amount;
        }

        public DragonEgg(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Turn in at a Dragon Egg Stone for pet statuettes.");
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
