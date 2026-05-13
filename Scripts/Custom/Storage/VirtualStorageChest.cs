/*
 * UO Wildlands Custom Script
 * Derived from ServUO Core
 * Compiled & Modified by: [Feng / UO Wildlands Team]
 * * Licensed under the GNU General Public License v3.0 (GPL-3.0)
 */
using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Network;
using Server.Targeting;
using Server.Multis;

namespace Server.Items
{
    // Modifications from upstream Feng/UO Wildlands release (still GPL-3.0):
    //   - Deserialize uses ScriptCompiler.FindTypeByFullName so stockpile
    //     contents survive a restart even when the stored type lives in
    //     Scripts.dll. The original Type.GetType only resolved types in the
    //     calling assembly and silently dropped the rest.
    //   - Bumped to serial version 1; persists a SecureLevel.
    //   - Implements ISecurable so the chest gets the standard Set Security
    //     Level context menu (Owner / Co-Owner / Friend / Anyone), matching
    //     the rest of the shard's locked-down household items.
    //   - [Flipable] attribute is on each concrete subclass instead of here,
    //     because the AdvancedInteriorDecorator calls GetCustomAttributes
    //     with inherit:false -- attributes on abstract base classes are
    //     invisible to it. Upstream chests had no flip support at all and
    //     couldn't be rotated.
    public abstract class VirtualStorageChest : Item, ISecurable
    {
        // The Virtual List: Stores Item Type and Quantity
        public Dictionary<Type, int> Content = new Dictionary<Type, int>();

        public abstract Type[] AllowedTypes { get; }
        public abstract string ChestTitle { get; }

        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public VirtualStorageChest(int itemID) : base(itemID)
        {
            Movable = true;
            Weight = 10.0;
            m_Level = SecureLevel.CoOwners;
        }

        public VirtualStorageChest(Serial serial) : base(serial) { }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        // Centralised lockdown + per-level access gate. Both open and
        // deposit paths funnel through this so a Friend-only chest stays
        // friend-only for drag-drop too.
        protected bool CheckAccess(Mobile from, bool message)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (!IsLockedDown && !IsSecure)
            {
                if (message)
                    from.SendMessage("This storage chest must be secured in a house to function.");
                return false;
            }

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && !house.HasSecureAccess(from, m_Level))
            {
                if (message)
                    from.SendLocalizedMessage(503301, "", 0x22); // You don't have permission to do that.
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // Too far away.
                return;
            }

            if (!CheckAccess(from, true))
                return;

            from.SendGump(new VirtualStorageGump(from, this));
        }

        // Defensive overrides so the chest always lifts cleanly when it
        // isn't yet locked down. Some clients/regions can short-circuit
        // the default Movable check; being explicit avoids the "you
        // cannot pick that up" rejection when the chest is fresh from
        // [add or just dropped on the ground.
        public override bool VerifyMove(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (IsLockedDown || IsSecure)
                return false;

            return Movable;
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (IsLockedDown || IsSecure)
            {
                reject = LRReason.CannotLift;
                return false;
            }

            return base.CheckLift(from, item, ref reject);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!CheckAccess(from, true))
                return false;

            // Using the new int return type
            if (TryAdd(dropped) > 0)
            {
                from.PlaySound(0x42);
                from.SendGump(new VirtualStorageGump(from, this));
                return true;
            }

            from.SendMessage("That item does not belong in this stockpile.");
            return false;
        }

        // Returns the amount added, or 0 if failed
        public int TryAdd(Item item)
        {
            Type t = item.GetType();
            bool isAllowed = false;

            foreach (Type a in AllowedTypes)
            {
                if (t == a || t.IsSubclassOf(a))
                {
                    isAllowed = true;
                    break;
                }
            }

            if (isAllowed)
            {
                int amountToAdd = item.Amount;
                if (Content.ContainsKey(t)) Content[t] += amountToAdd;
                else Content[t] = amountToAdd;

                item.Delete();
                return amountToAdd;
            }
            return 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
            writer.WriteEncodedInt((int)m_Level);
            writer.Write(Content.Count);
            foreach (var kvp in Content) { writer.Write(kvp.Key.FullName); writer.Write(kvp.Value); }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 1)
                m_Level = (SecureLevel)reader.ReadEncodedInt();
            else
                m_Level = SecureLevel.CoOwners;

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string typeName = reader.ReadString();
                int amt = reader.ReadInt();
                Type t = ScriptCompiler.FindTypeByFullName(typeName);
                if (t != null) Content[t] = amt;
            }
        }
    }
}
