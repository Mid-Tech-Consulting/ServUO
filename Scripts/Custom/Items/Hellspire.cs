using System;

namespace Server.Items
{
    public class Hellspire : HeavyCrossbow
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public Hellspire()
        {
            Name = "Hellspire";
            Hue = 1358;

            WeaponAttributes.HitLightning = 70;
            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.HitLowerAttack = 60;
            WeaponAttributes.HitLowerDefend = 60;
            Velocity = 60;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 1;
            Attributes.WeaponSpeed = 40;
            Attributes.WeaponDamage = 60;
            AosElementDamages.Fire = 100;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public Hellspire(Serial serial)
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
            int version = reader.ReadInt();
        }
    }

    public class GargishHellspire : Cyclone
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishHellspire()
        {
            Name = "Hellspire";
            Hue = 1358;

            WeaponAttributes.HitLightning = 70;
            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.HitLowerAttack = 60;
            WeaponAttributes.HitLowerDefend = 60;
            Velocity = 60;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 1;
            Attributes.WeaponSpeed = 40;
            Attributes.WeaponDamage = 60;
            AosElementDamages.Fire = 100;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public GargishHellspire(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}
