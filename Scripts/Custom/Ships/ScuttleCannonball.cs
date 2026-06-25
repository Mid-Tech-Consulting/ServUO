// ScuttleCannonball.cs
// ServUO (High Seas) - Destroyer Cannonball + Scuttle/Distress system
//
// This file now includes a COMPATIBILITY ScuttleCannonball type to fix:
//  - World load "Type 'Server.Custom.Ships.ScuttleCannonball' was not found"
//  - Other scripts referencing ScuttleCannonball (e.g. MythicChest.cs)
//
// AND keeps your current working loading behavior by providing:
//  - LightDestroyerCannonball : LightCannonball
//  - HeavyDestroyerCannonball : HeavyCannonball
//
// IMPORTANT:
// - Cannons decide loadability by TYPE (LoadTypes/TryLoadAmmo / FindItemByType), not ItemID.
// - We do NOT use Item.WorldLocation anywhere.
//

using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Custom.Ships
{
    // =========================================================
    // COMPATIBILITY AMMO ITEM (fixes old saves + other scripts)
    // =========================================================
    //
    // If your world save has items stored as:
    //   Server.Custom.Ships.ScuttleCannonball
    // ServUO MUST be able to find this exact type at load time.
    //
    // Also if you previously had it in other namespaces, TypeAlias helps
    // map those saved names to this class.

    [TypeAlias(
        "Server.Custom.Ships.ScuttleCannonball",
        "Server.Items.ScuttleCannonball"
    )]
    public class ScuttleCannonball : Cannonball
    {
        [Constructable]
        public ScuttleCannonball() : this(1)
        {
        }

        [Constructable]
        public ScuttleCannonball(int amount)
            : base(amount)
        {
            Stackable = true;
            Amount = Math.Max(1, amount);

            // Classic single cannonball look (NOT the 3-ball cluster)
            ItemID = 0xE73;

            Name = "Destroyer Cannonball";
            Hue = 0x0481;
            Weight = 0.1;
        }

        public ScuttleCannonball(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            _ = reader.ReadInt();

            // Safety: enforce appearance after world load
            ItemID = 0xE73;
            Hue = 0x0481;
        }
    }

    // =========================================================
    // AMMO ITEMS (what players load for Light/Heavy cannons)
    // =========================================================

    // LIGHT version (loads into LightShipCannon because it's a LightCannonball type)
    public class LightDestroyerCannonball : LightCannonball
    {
        [Constructable]
        public LightDestroyerCannonball() : this(1)
        {
        }

        [Constructable]
        public LightDestroyerCannonball(int amount)
            : base(amount)
        {
            // Force classic single cannonball look
            ItemID = 0xE73;

            Name = "Destroyer Cannonball (Light)";
            Hue = 0x0481;
            Weight = 0.1;
            Stackable = true;
            Amount = Math.Max(1, amount);
        }

        public LightDestroyerCannonball(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            _ = reader.ReadInt();

            // Keep the look consistent after loads
            ItemID = 0xE73;
            Hue = 0x0481;
        }
    }

    // HEAVY version (loads into HeavyShipCannon because it's a HeavyCannonball type)
    public class HeavyDestroyerCannonball : HeavyCannonball
    {
        [Constructable]
        public HeavyDestroyerCannonball() : this(1)
        {
        }

        [Constructable]
        public HeavyDestroyerCannonball(int amount)
            : base(amount)
        {
            // Force classic single cannonball look
            ItemID = 0xE73;

            Name = "Destroyer Cannonball (Heavy)";
            Hue = 0x0481;
            Weight = 0.1;
            Stackable = true;
            Amount = Math.Max(1, amount);
        }

        public HeavyDestroyerCannonball(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            _ = reader.ReadInt();

            // Keep the look consistent after loads
            ItemID = 0xE73;
            Hue = 0x0481;
        }
    }

    // =========================================================
    // SYSTEM: called by your cannon file
    // =========================================================
    public static class ScuttleHitSystem
    {
        // 30 minutes
        public static readonly TimeSpan ScuttleDuration = TimeSpan.FromMinutes(30.0);

        // Fireworks tick (gated by nearby players)
        public static readonly TimeSpan FireworkInterval = TimeSpan.FromSeconds(8.0);

        // Only show FX/messages if a player is within this range
        public const int NearbyRange = 18;

        // Distress hue (gray)
        public const int DistressHue = 0x0481;

        private class ScuttleContext
        {
            public BaseBoat Boat;
            public Mobile Shooter;
            public DateTime EndAt;

            public int OriginalBoatHue;
            public Dictionary<Serial, int> OriginalItemHues = new Dictionary<Serial, int>();

            public Timer FireworkTimer;
            public Timer EndTimer;

            public bool Active;
        }

        private static readonly Dictionary<Serial, ScuttleContext> _byBoatSerial = new Dictionary<Serial, ScuttleContext>();

        // Optional: lets your cannon pass context pre-damage (kept for compatibility)
        public static void SetShotContext(BaseBoat target, Mobile shooter)
        {
            // no-op for now (your cannon will call TryRegisterScuttleHit after damage)
        }

        public static void TryRegisterScuttleHit(BaseBoat target, Mobile shooter)
        {
            if (target == null || target.Deleted)
                return;

            // Already running? do nothing.
            if (_byBoatSerial.TryGetValue(target.Serial, out var existing) && existing != null && existing.Active)
                return;

            StartScuttle(target, shooter);
        }

        // Optional (if you ever add repair logic later)
        public static bool CancelScuttle(BaseBoat target)
        {
            if (target == null)
                return false;

            if (!_byBoatSerial.TryGetValue(target.Serial, out var ctx) || ctx == null || !ctx.Active)
                return false;

            EndContext(ctx, restoreHue: true, deleteBoat: false);
            return true;
        }

        private static void StartScuttle(BaseBoat boat, Mobile shooter)
        {
            var ctx = new ScuttleContext
            {
                Boat = boat,
                Shooter = shooter,
                EndAt = DateTime.UtcNow + ScuttleDuration,
                Active = true,
                OriginalBoatHue = boat.Hue
            };

            _byBoatSerial[boat.Serial] = ctx;

            // Tell nearby players and anyone on-board
            NotifyNearby(boat, 0x22, "A Destroyer strike marks this ship! Repair it within 30 minutes or it will be lost!");
            AnnounceToBoat(boat, 0x22, "Your ship is in distress! The hull begins to fail...");

            // Distress hue (best-effort)
            ApplyDistressHue(ctx);

            // Fireworks timer (gated by NearbyRange)
            ctx.FireworkTimer = Timer.DelayCall(TimeSpan.FromSeconds(1.0), FireworkInterval, () =>
            {
                if (!ctx.Active || ctx.Boat == null || ctx.Boat.Deleted)
                {
                    EndContext(ctx, restoreHue: false, deleteBoat: false);
                    return;
                }

                // GATE: only play FX if a player is within 18 tiles
                if (!HasNearbyPlayers(ctx.Boat, NearbyRange))
                    return;

                DoDistressFireworks(ctx.Boat);

                // Under 10 minutes warning (only when players are nearby)
                TimeSpan remaining = ctx.EndAt - DateTime.UtcNow;
                if (remaining > TimeSpan.Zero && remaining <= TimeSpan.FromMinutes(10.0))
                {
                    NotifyNearby(ctx.Boat, 33, "WARNING: Ship will be lost in " + FormatRemaining(remaining) + " if not repaired!");
                }
            });

            // End timer (delete ship)
            ctx.EndTimer = Timer.DelayCall(ScuttleDuration, () =>
            {
                if (!ctx.Active)
                    return;

                if (ctx.Boat == null || ctx.Boat.Deleted)
                {
                    EndContext(ctx, restoreHue: false, deleteBoat: false);
                    return;
                }

                // Final blast (only if players are nearby; deletion happens regardless)
                if (HasNearbyPlayers(ctx.Boat, NearbyRange))
                    DoFinalExplosion(ctx.Boat);

                NotifyNearby(ctx.Boat, 33, "The ship was not repaired in time and collapses into the sea!");
                EndContext(ctx, restoreHue: false, deleteBoat: true);
            });
        }

        private static bool HasNearbyPlayers(BaseBoat boat, int range)
        {
            if (boat == null || boat.Deleted || boat.Map == null || boat.Map == Map.Internal)
                return false;

            foreach (NetState ns in NetState.Instances)
            {
                Mobile m = ns.Mobile;

                if (m == null || m.Deleted || m.Map != boat.Map)
                    continue;

                if (!m.Player)
                    continue;

                if (m.InRange(boat.Location, range))
                    return true;
            }

            return false;
        }

        private static void ApplyDistressHue(ScuttleContext ctx)
        {
            BaseBoat boat = ctx.Boat;

            if (boat == null || boat.Deleted)
                return;

            try
            {
                ctx.OriginalBoatHue = boat.Hue;
                boat.Hue = DistressHue;
            }
            catch { }

            try
            {
                Map map = boat.Map;
                if (map == null || map == Map.Internal)
                    return;

                IPooledEnumerable eable = map.GetItemsInRange(boat.Location, 12);

                foreach (Item item in eable)
                {
                    if (item == null || item.Deleted)
                        continue;

                    string t = item.GetType().Name;

                    bool likelyShipPart =
                        t.IndexOf("Tiller", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        t.IndexOf("Hold", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        t.IndexOf("Plank", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        t.IndexOf("Ship", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        t.IndexOf("Galleon", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        item is IShipCannon;

                    if (!likelyShipPart)
                        continue;

                    if (!ctx.OriginalItemHues.ContainsKey(item.Serial))
                        ctx.OriginalItemHues[item.Serial] = item.Hue;

                    item.Hue = DistressHue;
                }

                eable.Free();
            }
            catch { }
        }

        private static void RestoreHue(ScuttleContext ctx)
        {
            BaseBoat boat = ctx.Boat;

            if (boat != null && !boat.Deleted)
            {
                try { boat.Hue = ctx.OriginalBoatHue; } catch { }
            }

            try
            {
                foreach (var kvp in ctx.OriginalItemHues)
                {
                    Item item = World.FindItem(kvp.Key);

                    if (item != null && !item.Deleted)
                    {
                        try { item.Hue = kvp.Value; } catch { }
                    }
                }
            }
            catch { }
        }

        private static void DoDistressFireworks(BaseBoat boat)
        {
            if (boat == null || boat.Deleted || boat.Map == null || boat.Map == Map.Internal)
                return;

            Map map = boat.Map;

            for (int i = 0; i < 3; i++)
            {
                Point3D p = new Point3D(
                    boat.X + Utility.RandomMinMax(-3, 3),
                    boat.Y + Utility.RandomMinMax(-3, 3),
                    boat.Z + Utility.RandomMinMax(0, 5));

                int[] effects = { 0x36BD, 0x36BE, 0x36BF, 0x3728, 0x373A };
                int eff = effects[Utility.Random(effects.Length)];

                Effects.SendLocationEffect(p, map, eff, 20, 10);
            }

            Effects.PlaySound(boat.Location, map, 0x307);
        }

        private static void DoFinalExplosion(BaseBoat boat)
        {
            if (boat == null || boat.Deleted || boat.Map == null || boat.Map == Map.Internal)
                return;

            Map map = boat.Map;

            for (int i = 0; i < 12; i++)
            {
                Point3D p = new Point3D(
                    boat.X + Utility.RandomMinMax(-4, 4),
                    boat.Y + Utility.RandomMinMax(-4, 4),
                    boat.Z + Utility.RandomMinMax(0, 8));

                Effects.SendLocationEffect(p, map, 0x36BD, 30, 10);
            }

            Effects.PlaySound(boat.Location, map, 0x11D);
        }

        private static void EndContext(ScuttleContext ctx, bool restoreHue, bool deleteBoat)
        {
            if (ctx == null)
                return;

            ctx.Active = false;

            try { ctx.FireworkTimer?.Stop(); } catch { }
            try { ctx.EndTimer?.Stop(); } catch { }

            if (restoreHue)
                RestoreHue(ctx);

            if (deleteBoat && ctx.Boat != null && !ctx.Boat.Deleted)
            {
                try { SafeDisembark(ctx.Boat); } catch { }
                try { ctx.Boat.Delete(); } catch { }
            }

            if (ctx.Boat != null)
                _byBoatSerial.Remove(ctx.Boat.Serial);
        }

        private static void SafeDisembark(BaseBoat boat)
        {
            if (boat == null || boat.Deleted || boat.Map == null || boat.Map == Map.Internal)
                return;

            List<Mobile> toMove = new List<Mobile>();

            try
            {
                if (boat is BaseGalleon g)
                    toMove.AddRange(g.GetEntitiesOnBoard().OfType<Mobile>());
            }
            catch { }

            if (toMove.Count == 0)
            {
                IPooledEnumerable eable = boat.Map.GetMobilesInRange(boat.Location, 12);

                foreach (Mobile m in eable)
                {
                    if (m != null && !m.Deleted && m.Player)
                        toMove.Add(m);
                }

                eable.Free();
            }

            foreach (Mobile m in toMove)
            {
                if (m == null || m.Deleted)
                    continue;

                Point3D p = new Point3D(
                    boat.X + Utility.RandomMinMax(-6, 6),
                    boat.Y + Utility.RandomMinMax(-6, 6),
                    boat.Z);

                try { m.MoveToWorld(p, boat.Map); } catch { }
            }
        }

        private static void AnnounceToBoat(BaseBoat boat, int hue, string msg)
        {
            if (boat == null || boat.Deleted || boat.Map == null || boat.Map == Map.Internal)
                return;

            try
            {
                if (boat is BaseGalleon g)
                {
                    foreach (PlayerMobile pm in g.GetEntitiesOnBoard().OfType<PlayerMobile>())
                        pm.SendMessage(hue, msg);

                    return;
                }
            }
            catch { }

            NotifyNearby(boat, hue, msg);
        }

        private static void NotifyNearby(BaseBoat boat, int hue, string msg)
        {
            if (boat == null || boat.Deleted)
                return;

            foreach (NetState ns in NetState.Instances)
            {
                Mobile m = ns.Mobile;

                if (m == null || m.Deleted || m.Map != boat.Map)
                    continue;

                if (m.InRange(boat.Location, NearbyRange))
                    m.SendMessage(hue, msg);
            }
        }

        private static string FormatRemaining(TimeSpan ts)
        {
            if (ts <= TimeSpan.Zero)
                return "0s";

            int hours = (int)ts.TotalHours;
            int mins = ts.Minutes;
            int secs = ts.Seconds;

            if (hours > 0)
                return string.Format("{0}h {1}m", hours, mins);

            if (mins > 0)
                return string.Format("{0}m {1}s", mins, secs);

            return string.Format("{0}s", (int)ts.TotalSeconds);
        }
    }
}
