using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class TransmogrificationPotion : Item
    {
        [Constructable]
        public TransmogrificationPotion() : base(0xF0E)
        {
            Weight = 1.0;
            Hue = 1161;
            Name = "Transmogrification Potion";
            LootType = LootType.Blessed;
        }

        public TransmogrificationPotion(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return;
            }

            from.CloseGump(typeof(TransmogGump));
            from.SendGump(new TransmogGump(this, from, null, null, TransmogMessage.SelectEquipment));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public enum TransmogMessage
    {
        SelectEquipment,
        MustBeInBackpack,
        InvalidItemType,
        SelectBothItems,
        ItemsAreIdentical,
        LayerMismatch
    }

    public class TransmogGump : Gump
    {
        private readonly TransmogrificationPotion m_Potion;
        private readonly Mobile m_From;
        private readonly Item m_Target;
        private readonly Item m_Sample;
        private readonly TransmogMessage m_Message;

        public TransmogGump(TransmogrificationPotion potion, Mobile from, Item target, Item sample, TransmogMessage message)
            : base(100, 80)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            m_Potion = potion;
            m_From = from;
            m_Target = target;
            m_Sample = sample;
            m_Message = message;

            BuildLayout();
        }

        private static string GetMessageText(TransmogMessage message)
        {
            switch (message)
            {
                case TransmogMessage.MustBeInBackpack: return "That item must be on your character.";
                case TransmogMessage.InvalidItemType: return "You can only transmog clothing, armor, shields, weapons, and jewelry.";
                case TransmogMessage.SelectBothItems: return "You have not selected both items yet.";
                case TransmogMessage.ItemsAreIdentical: return "These items already look the same.";
                case TransmogMessage.LayerMismatch: return "Both items must be of the same equipment type.";
                case TransmogMessage.SelectEquipment:
                default: return "Select the item to transmog and an appearance sample.";
            }
        }

        private void BuildLayout()
        {
            AddPage(0);
            AddBackground(33, 40, 446, 351, 5054);
            AddImage(6, 52, 13);
            AddImage(46, 12, 201);
            AddImage(7, 50, 50970, 1);
            AddImage(46, 372, 233);
            AddImage(2, 56, 202);
            AddImage(473, 56, 203);
            AddImage(2, 372, 204);
            AddImage(473, 372, 205);
            AddImage(2, 12, 206);
            AddImage(473, 12, 207);

            AddLabel(180, 46, 2414, "Transmogrification Potion");

            AddBackground(171, 109, 60, 60, 3000);
            if (m_Target != null)
                AddItem(178, 117, m_Target.ItemID, m_Target.Hue);

            AddBackground(171, 174, 60, 60, 3000);
            if (m_Sample != null)
                AddItem(178, 183, m_Sample.ItemID, m_Sample.Hue);

            AddButton(246, 115, 4031, 4030, 1, GumpButtonType.Reply, 0);
            AddLabel(288, 115, 2424, "Select the item to change.");

            AddButton(246, 181, 4031, 4030, 2, GumpButtonType.Reply, 0);
            AddLabel(288, 181, 2424, "Select the appearance sample.");

            AddLabel(240, 205, 937, "<- This equipment will be");
            AddLabel(390, 205, 332, "destroyed.");

            AddButton(380, 368, 239, 240, 3, GumpButtonType.Reply, 0); // Apply

            AddBackground(60, 284, 400, 32, 3000);
            AddLabel(70, 289, 1265, GetMessageText(m_Message));
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0: // close
                    return;

                case 1: // pick target
                    from.Target = new PickItemTarget(m_Potion, m_From, m_Target, m_Sample, true);
                    return;

                case 2: // pick sample
                    from.Target = new PickItemTarget(m_Potion, m_From, m_Target, m_Sample, false);
                    return;

                case 3: // apply
                    ApplyTransmog(from);
                    return;
            }
        }

        private void ApplyTransmog(Mobile from)
        {
            if (m_Target == null || m_Sample == null)
            {
                Reopen(TransmogMessage.SelectBothItems);
                return;
            }

            if (!m_Target.IsChildOf(from.Backpack) || !m_Sample.IsChildOf(from.Backpack))
            {
                from.SendMessage("Both items must be in your backpack.");
                return;
            }

            if (m_Target.Layer != m_Sample.Layer)
            {
                Reopen(TransmogMessage.LayerMismatch);
                return;
            }

            if (m_Target.ItemID == m_Sample.ItemID)
            {
                Reopen(TransmogMessage.ItemsAreIdentical);
                return;
            }

            m_Target.ItemID = m_Sample.ItemID;
            m_Sample.Delete();

            from.SendMessage("You transmogrify the appearance of your equipment.");
            from.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
            from.PlaySound(0x1EA);

            m_Potion.Delete();
        }

        private void Reopen(TransmogMessage message)
        {
            m_From.CloseGump(typeof(TransmogGump));
            m_From.SendGump(new TransmogGump(m_Potion, m_From, m_Target, m_Sample, message));
        }

        private class PickItemTarget : Target
        {
            private readonly TransmogrificationPotion m_Potion;
            private readonly Mobile m_From;
            private readonly Item m_CurrentTarget;
            private readonly Item m_CurrentSample;
            private readonly bool m_SettingTarget;

            public PickItemTarget(TransmogrificationPotion potion, Mobile from, Item target, Item sample, bool settingTarget)
                : base(2, false, TargetFlags.None)
            {
                m_Potion = potion;
                m_From = from;
                m_CurrentTarget = target;
                m_CurrentSample = sample;
                m_SettingTarget = settingTarget;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                Item newTarget = m_CurrentTarget;
                Item newSample = m_CurrentSample;
                TransmogMessage message = TransmogMessage.SelectEquipment;

                if (!(target is BaseWeapon || target is BaseArmor || target is BaseClothing
                      || target is BaseJewel || target is BaseShield))
                {
                    message = TransmogMessage.InvalidItemType;
                }
                else
                {
                    Item picked = (Item)target;

                    if (picked.RootParent != m_From)
                    {
                        message = TransmogMessage.MustBeInBackpack;
                    }
                    else if (m_SettingTarget)
                    {
                        newTarget = picked;
                    }
                    else
                    {
                        newSample = picked;
                    }
                }

                m_From.CloseGump(typeof(TransmogGump));
                m_From.SendGump(new TransmogGump(m_Potion, m_From, newTarget, newSample, message));
            }
        }
    }
}
