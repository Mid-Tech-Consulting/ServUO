using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    // Single-use deed: double-click, target a weapon in your pack, deed is
    // consumed and the weapon gains Spell Channeling (with the standard -1
    // FCR penalty so it matches imbuing / runic behavior).
    public class SpellChannelingDeed : Item
    {
        public override int LabelNumber { get { return 1079766; } } // Spell Channeling

        [Constructable]
        public SpellChannelingDeed()
            : base(0x14F0)
        {
            Name = "a Spell Channeling Deed";
            Hue = 0x47E;
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public SpellChannelingDeed(Serial serial)
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

            from.SendMessage(0x40, "Target the weapon you wish to enchant with Spell Channeling.");
            from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly SpellChannelingDeed m_Deed;

            public InternalTarget(SpellChannelingDeed deed)
                : base(2, false, TargetFlags.None)
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

                if (!(targeted is BaseWeapon weapon))
                {
                    from.SendMessage(0x22, "You can only enchant a weapon.");
                    return;
                }

                if (!weapon.IsChildOf(from.Backpack))
                {
                    from.SendMessage(0x22, "The weapon must be in your backpack.");
                    return;
                }

                if (weapon.Attributes.SpellChanneling != 0)
                {
                    from.SendMessage(0x22, "That weapon already has Spell Channeling.");
                    return;
                }

                // Mirror Imbuing.cs:543 -- SC adds -1 FCR penalty if the weapon
                // isn't already at a negative cast speed.
                weapon.Attributes.SpellChanneling = 1;

                if (weapon.Attributes.CastSpeed >= 0)
                    weapon.Attributes.CastSpeed -= 1;

                weapon.InvalidateProperties();

                from.SendMessage(0x40, "The weapon now channels spells.");
                from.PlaySound(0x1F5);
                Effects.SendLocationEffect(from.Location, from.Map, 0x376A, 16, 1);

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
