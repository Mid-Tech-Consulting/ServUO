using System;

using Server;

namespace Server.Items
{
    public class UmbrascaleGargishBattleDress : BaseClothing
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public UmbrascaleGargishBattleDress()
            : base(0xB2BA, Layer.OuterTorso)
        {
            Name = "Umbrascale Gargish Battle Dress";
            Hue = 0x0ADD;
            Weight = 3.0;
            LootType = LootType.Regular;

            SAAbsorptionAttributes.EaterFire = 15;
            Attributes.BonusStr = 5;
            Attributes.BonusHits = 5;
            Attributes.BonusStam = 8;
            Attributes.AttackChance = 5;
            Attributes.WeaponSpeed = 10;
            Attributes.LowerManaCost = 5;

            SkillBonuses.SetValues(0, LordMorphiusEpaulettes.GetRandomWarriorSkill(), 15.0);

            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public UmbrascaleGargishBattleDress(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
