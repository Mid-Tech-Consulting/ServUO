using System;

using Server;

namespace Server.Items
{
    public class RiftwardensPendant : BaseTalisman
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public RiftwardensPendant()
            : base(0x9E2B)
        {
            Name = "Riftwarden's Pendant";
            Hue = 0x0AE1;
            Weight = 1.0;
            LootType = LootType.Regular;

            Attributes.BonusInt = 8;
            Attributes.BonusMana = 12;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 16;
            Attributes.LowerManaCost = 12;
            Attributes.LowerRegCost = 15;
            Attributes.CastRecovery = 2;
            Attributes.CastSpeed = 0;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 10.0);

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public RiftwardensPendant(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write(0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); reader.ReadInt(); }
    }
}
