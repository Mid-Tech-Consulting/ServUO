using System;
using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    #region Reaper's Harvest Scythe
    public class ReapersHarvestScythe : Scythe
    {
        [Constructable]
        public ReapersHarvestScythe()
        {
            Name = "Reaper's Harvest Scythe";
            Hue = 1157; // Vampire Blood Crimson

            Slayer = SlayerName.Silver; // Undead Slayer
            Attributes.SpellChanneling = 1;
            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 30;

            WeaponAttributes.HitLeechHits = 50;
            WeaponAttributes.HitLeechMana = 50;
            WeaponAttributes.HitLightning = 50;

            SkillBonuses.SetValues(0, SkillName.Necromancy, 10.0);
            SkillBonuses.SetValues(1, SkillName.Swords, 5.0);
        }

        public ReapersHarvestScythe(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive</BASEFONT>");
            list.Add("<BASEFONT COLOR=#FF3333>Soul Harvest: 20% chance to siphon 10 Mana & Stamina on hit</BASEFONT>");
        }

        public override void OnHit(Mobile attacker, IDamageable defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (attacker != null && defender != null && Utility.RandomDouble() < 0.20)
            {
                attacker.PlaySound(0x1FB); // Haunting scream
                attacker.FixedParticles(0x374A, 10, 15, 5038, 1157, 2, EffectLayer.Waist);
                attacker.Mana = Math.Min(attacker.ManaMax, attacker.Mana + 10);
                attacker.Stam = Math.Min(attacker.StamMax, attacker.Stam + 10);
                attacker.SendMessage(0x25, "The scythe harvests the soul essence of your victim!");
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
        }
    }
    #endregion

    #region Jack's Soul Lantern
    public class JacksSoulLantern : BaseTalisman
    {
        [Constructable]
        public JacksSoulLantern() : base(0x2F5B)
        {
            Name = "Jack's Soul Lantern";
            Hue = 1161; // Pumpkin Blaze Orange
            LootType = LootType.Blessed;

            Slayer = TalismanSlayerName.Undead;
            Attributes.NightSight = 1;
            Attributes.DefendChance = 10;
            Attributes.SpellDamage = 12;
            Attributes.LowerManaCost = 8;
            Attributes.CastRecovery = 1;
        }

        public JacksSoulLantern(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive</BASEFONT>");
            list.Add("<BASEFONT COLOR=#FFA500>Contains a restless Halloween spirit</BASEFONT>");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack) || Parent == from)
            {
                from.PlaySound(0x47B); // Ghostly cackle
                from.FixedParticles(0x3709, 10, 30, 5052, 1161, 0, EffectLayer.Head);
                from.SendMessage(0x35, "The soul lantern flickers warmly, chuckling with otherworldly mischief!");
            }
            else
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
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
        }
    }
    #endregion

    #region Mantle of the Crypt Lord
    public class MantleOfTheCryptLord : BaseCloak
    {
        [Constructable]
        public MantleOfTheCryptLord() : base(0x1515, 1170) // Nightmare Void Purple
        {
            Name = "Mantle of the Crypt Lord";
            Weight = 3.0;
            LootType = LootType.Blessed;

            Attributes.BonusHits = 8;
            Attributes.BonusMana = 8;
            Attributes.BonusStam = 8;
            Attributes.RegenMana = 2;
            Attributes.LowerRegCost = 15;

            Resistances.Physical = 5;
            Resistances.Fire = 5;
            Resistances.Cold = 5;
            Resistances.Poison = 5;
            Resistances.Energy = 5;
        }

        public MantleOfTheCryptLord(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Infused with the chilling shadows of the crypts</BASEFONT>");
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
    #endregion

    #region Mask of the Jack-o'-Lantern
    public class MaskOfTheJackOLantern : BaseArmor
    {
        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Cloth; } }

        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }

        public override int InitMinHits { get { return 0; } }
        public override int InitMaxHits { get { return 0; } }
        public override int AosStrReq { get { return 0; } }

        [Constructable]
        public MaskOfTheJackOLantern() : base(0xA3EA)
        {
            Name = "Mask of the Jack-o'-Lantern";
            Hue = 1161; // Pumpkin Blaze
            Weight = 1.0;
            Layer = Layer.Helm;
            LootType = LootType.Blessed;
        }

        public MaskOfTheJackOLantern(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive</BASEFONT>");
            list.Add("<BASEFONT COLOR=#FFA500>Cosmetic Transmog Mask</BASEFONT>");
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
    #endregion

    #region Spooky Spiderweb Sash
    public class SpookySpiderwebSash : BodySash
    {
        [Constructable]
        public SpookySpiderwebSash() : base(1150) // Spectral Ghost White
        {
            Name = "Spooky Spiderweb Sash";
            LootType = LootType.Blessed;

            Attributes.AttackChance = 5;
            Attributes.DefendChance = 10;
            Attributes.BonusStam = 10;
            Attributes.CastSpeed = 1;
            Attributes.LowerManaCost = 4;
        }

        public SpookySpiderwebSash(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Woven from the enchanted silk of cemetery spiders</BASEFONT>");
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
    #endregion

    #region Grave Robber's Haversack
    public class GraveRobbersHaversack : Backpack
    {
        private int m_WeightReduction = 50;

        [CommandProperty(AccessLevel.GameMaster)]
        public int WeightReduction
        {
            get { return m_WeightReduction; }
            set { m_WeightReduction = value; InvalidateProperties(); }
        }

        [Constructable]
        public GraveRobbersHaversack()
        {
            Name = "Grave Robber's Haversack";
            Hue = 1168; // Witches' Brew Green
            m_WeightReduction = 50;
            LootType = LootType.Blessed;
        }

        public GraveRobbersHaversack(Serial serial) : base(serial)
        {
        }

        public override int GetTotal(TotalType type)
        {
            int total = base.GetTotal(type);

            if (type == TotalType.Weight && m_WeightReduction > 0)
                total -= (int)(total * ((double)m_WeightReduction / 100.0));

            return total;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive</BASEFONT>");
            list.Add("<BASEFONT COLOR=#00FF00>50% Weight Reduction</BASEFONT>");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_WeightReduction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_WeightReduction = reader.ReadInt();
        }
    }
    #endregion

    #region Witches' Brew Cauldron Addon & Deed
    public class WitchesCauldronAddon : BaseAddon
    {
        private DateTime m_NextTreat;

        public override BaseAddonDeed Deed { get { return new WitchesCauldronDeed(); } }

        [Constructable]
        public WitchesCauldronAddon()
        {
            AddonComponent pot = new AddonComponent(2420);
            pot.Hue = 1168; // Toxic Green Cauldron
            AddComponent(pot, 0, 0, 0);

            AddonComponent fire = new AddonComponent(4012);
            fire.Light = LightType.Circle150;
            AddComponent(fire, 0, 0, 0);

            m_NextTreat = DateTime.UtcNow;
        }

        public WitchesCauldronAddon(Serial serial) : base(serial)
        {
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.PlaySound(0x21); // Bubbling cauldron sound
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 1168, 0, 5023, 0);

            if (DateTime.UtcNow >= m_NextTreat)
            {
                Item treat = null;
                switch (Utility.Random(5))
                {
                    case 0: treat = new PumpkinPie(); break;
                    case 1: treat = new Taffy(); break;
                    case 2: treat = new JellyBeans(); break;
                    case 3: treat = new NougatSwirl(); break;
                    default: treat = new WrappedCandy(); break;
                }

                from.AddToBackpack(treat);
                from.SendMessage(0x35, "You reach into the bubbling cauldron and pull out a Halloween treat!");
                m_NextTreat = DateTime.UtcNow + TimeSpan.FromHours(1.0);
            }
            else
            {
                TimeSpan remaining = m_NextTreat - DateTime.UtcNow;
                from.SendMessage(0x22, "The cauldron bubbles mysteriously... It will brew another treat in {0} minutes.", (int)remaining.TotalMinutes + 1);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_NextTreat);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_NextTreat = reader.ReadDateTime();
        }
    }

    public class WitchesCauldronDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new WitchesCauldronAddon(); } }

        [Constructable]
        public WitchesCauldronDeed()
        {
            Name = "Witches' Brew Cauldron Deed";
            Hue = 1168;
            LootType = LootType.Blessed;
        }

        public WitchesCauldronDeed(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive House Addon</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Dispenses Halloween sweets once per hour</BASEFONT>");
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
    #endregion

    #region Tombstone of the Awakened Dead Addon & Deed
    public class TombstoneOfTheAwakenedDeadAddon : BaseAddon
    {
        private static readonly string[] m_Epitaphs = new string[]
        {
            "\"Here lies an adventurer who forgot to watch their hit points...\"",
            "\"Restless is the earth upon which you tread...\"",
            "\"They sought pumpkins, but found only their doom!\"",
            "\"Beware the orange arachnid that crawls in the dark...\"",
            "\"Listen closely... you can hear the crypts weeping...\""
        };

        public override BaseAddonDeed Deed { get { return new TombstoneOfTheAwakenedDeadDeed(); } }

        [Constructable]
        public TombstoneOfTheAwakenedDeadAddon()
        {
            AddonComponent tombstone = new AddonComponent(0x116E);
            tombstone.Hue = 1150; // Spectral White
            AddComponent(tombstone, 0, 0, 0);
        }

        public TombstoneOfTheAwakenedDeadAddon(Serial serial) : base(serial)
        {
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446);
                return;
            }

            from.PlaySound(0x19C); // Undead moan
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 20, 1150, 0, 5029, 0);

            string epitaph = m_Epitaphs[Utility.Random(m_Epitaphs.Length)];
            from.SendMessage(0x35, epitaph);
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

    public class TombstoneOfTheAwakenedDeadDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new TombstoneOfTheAwakenedDeadAddon(); } }

        [Constructable]
        public TombstoneOfTheAwakenedDeadDeed()
        {
            Name = "Tombstone of the Awakened Dead Deed";
            Hue = 1150;
            LootType = LootType.Blessed;
        }

        public TombstoneOfTheAwakenedDeadDeed(Serial serial) : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("<BASEFONT COLOR=#FF7700>Halloween Cemetery Event 2026 Exclusive House Addon</BASEFONT>");
            list.Add("<BASEFONT COLOR=#C0C0C0>Interactive gravestone with spectral apparitions</BASEFONT>");
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
    #endregion
}
