using System;

namespace Server.Items
{
    public class GargishGlenda : DiscMace
    {
        public override int LabelNumber { get { return 1157346; } } // glenda
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishGlenda()
        {
            ExtendedWeaponAttributes.BoneBreaker = 1;
            WeaponAttributes.HitLeechMana = 20;
            WeaponAttributes.HitLeechHits = 20;
            WeaponAttributes.HitLowerDefend = 70;
            WeaponAttributes.HitHarm = 30;
            Attributes.BonusStr = 16;
            Attributes.BonusStam = 10;
            Attributes.WeaponDamage = 100;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 100;
            fire = cold = nrgy = chaos = direct = pois = 0;
        }

        public GargishGlenda(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

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
