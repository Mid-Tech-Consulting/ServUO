using System;

using Server;

namespace Server.Items
{
    public class HexweaversIdol : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public HexweaversIdol()
            : base(0x2F59)
        {
            Name = "Hexweaver's Idol";
            Hue = 0x0AE1;
            Weight = 1.0;
            LootType = LootType.Regular;

            Removal = TalismanRemoval.Ward;

            Attributes.BonusInt = 8;
            Attributes.BonusHits = 5;
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 3;
            Attributes.AttackChance = 5;
            Attributes.DefendChance = 10;
            Attributes.SpellDamage = 16;
            Attributes.CastRecovery = 1;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 15;

            SkillBonuses.SetValues(0, SkillName.Necromancy, 10.0);

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public HexweaversIdol(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
