using System;

using Server;

namespace Server.Items
{
    // Gargoyle counterpart to VoidwovenStrand. Same stats, gargoyle
    // middle-torso slot, race-locked.
    public class GargishVoidwovenStrand : GargishSash
    {
        public override bool IsArtifact { get { return true; } }
        public override Race RequiredRace { get { return Race.Gargoyle; } }
        public override bool CanBeWornByGargoyles { get { return true; } }

        [Constructable]
        public GargishVoidwovenStrand()
        {
            Name = "Voidwoven Strand";
            Hue = 0x0AD7;
            Weight = 3.0;
            LootType = LootType.Regular;

            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.BonusStam = 8;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 5;
            Attributes.WeaponDamage = 25;
            Attributes.WeaponSpeed = 10;

            SkillBonuses.SetValues(0, SkillName.Stealth, 5.0);
            SkillBonuses.SetValues(1, SkillName.Hiding, 5.0);
            SkillBonuses.SetValues(2, SkillName.Snooping, 5.0);
            SkillBonuses.SetValues(3, SkillName.Stealing, 5.0);


            StrRequirement = 10;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public GargishVoidwovenStrand(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
