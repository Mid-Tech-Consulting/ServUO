using System;

using Server.Engines.Craft;

namespace Server.Items
{
    // Alter points at GargishDetectiveBoots rather than inheriting Boots'
    // generic LeatherTalons target -- otherwise altering to gargoyle drops
    // the Replica status (CanFortify => false) and players can powder it.
    [Alterable(typeof(DefTailoring), typeof(GargishDetectiveBoots))]
    public class DetectiveBoots : Boots
	{
		public override bool IsArtifact { get { return true; } }
        private int m_Level;
        [Constructable]
        public DetectiveBoots()
        {
            Hue = 0x455;
            Level = Utility.RandomMinMax(0, 2);
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 2;
            SkillBonuses.SetValues(1, SkillName.MagicResist, 10.0);
        }

        public DetectiveBoots(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094894 + this.m_Level;
            }
        }// [Quality] Detective of the Royal Guard [Replica]
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = Math.Max(Math.Min(2, value), 0);
                this.Attributes.BonusInt = 2 + this.m_Level;
                this.InvalidateProperties();
            }
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

            this.Level = this.Attributes.BonusInt - 2;
        }
    }

    // Gargoyle counterpart -- what DetectiveBoots alters into. Mirrors the
    // Replica behavior (CanFortify => false) so the no-powder rule survives
    // the alter. Level is derived from BonusInt rather than stored, so it
    // stays correct after the alter dupe copies Attributes across.
    public class GargishDetectiveBoots : LeatherTalons
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishDetectiveBoots()
        {
            Hue = 0x455;
            Attributes.BonusInt = 2 + Utility.RandomMinMax(0, 2);
            Attributes.BonusStam = 8;
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 2;
            SkillBonuses.SetValues(1, SkillName.MagicResist, 10.0);
        }

        public GargishDetectiveBoots(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get { return Math.Max(0, Math.Min(2, Attributes.BonusInt - 2)); }
            set
            {
                Attributes.BonusInt = 2 + Math.Max(0, Math.Min(2, value));
                InvalidateProperties();
            }
        }

        public override int LabelNumber { get { return 1094894 + Level; } }
        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }
        public override bool CanFortify { get { return false; } }

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
