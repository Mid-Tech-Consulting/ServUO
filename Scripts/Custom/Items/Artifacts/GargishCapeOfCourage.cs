using System;
using Server;

namespace Server.Items
{
    public class GargishCapeOfCourage : GargishClothWingArmor
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishCapeOfCourage()
        {
            Name = "Cape of Courage";
            Hue = 1281;
            LootType = LootType.Regular;

            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 2;
            Attributes.SpellDamage = 15;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 5;
            Attributes.BonusHits = 5;
            Attributes.LowerRegCost = 10;
            Attributes.RegenMana = 3;
            Attributes.RegenHits = 3;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 10.0);
        }

        public GargishCapeOfCourage(Serial serial) : base(serial)
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
