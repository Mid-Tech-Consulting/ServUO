namespace Server.Items
{
    public abstract class BaseDragonEggStatuette : BaseImprisonedMobile
    {
        public const double RequiredLore = 100.0;
        public const double RequiredTaming = 100.0;

        public BaseDragonEggStatuette(int itemID) : base(itemID)
        {
        }

        public BaseDragonEggStatuette(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            if (from.Skills[SkillName.AnimalLore].Value < RequiredLore ||
                from.Skills[SkillName.AnimalTaming].Value < RequiredTaming)
            {
                from.SendMessage(
                    string.Format("You need at least {0:F0} Animal Lore and {1:F0} Animal Taming to summon this creature.",
                        RequiredLore, RequiredTaming));
                return;
            }

            base.OnDoubleClick(from);
        }

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
    }
}
