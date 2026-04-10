using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using System.Collections.Generic;
 
namespace Server.TournamentSystem
{
    public class CTFArena1 : PVPTournamentSystem
    {
        public override string DefaultName { get { return "Capture the Flag Arena 1"; } }
        public override int WallItemID { get { return 0; } }
        public override Map ArenaMap { get { return Map.Felucca; } }
		
		public override Point3D FlagHolder1Loc { get { return new Point3D(5131, 1290, 0); } }
		public override Point3D FlagHolder2Loc { get { return new Point3D(5187, 1292, 0); } }
		
		public override bool ForceFightType { get { return true; } }
		public override ArenaFightType ForcedFightType { get { return ArenaFightType.CaptureTheFlag; } }

        private ArenaDefinition _Definition;

        public override ArenaDefinition Definition
        {
            get
            {
                if (_Definition == null)
                {
                    _Definition = new ArenaDefinition(new Point3D(5220, 1287, 0), new Point3D(5131, 1290, 0), new Point3D(5187, 1292, 0),
                                                     new Point3D(5223, 1296, 2), new Point3D(5220, 1290, 9), new Point3D(5220, 1291, 9),
                                                     new Point3D(5220, 1286, 2), new Point3D(5222, 1286, 2), Point3D.Zero,
                                                     new Point3D(5224, 1286, 2), new Point3D(5224, 1286, 11),
                                                     new Rectangle2D(5220, 1287, 5, 9), new Rectangle2D(0, 0, 0, 0),
                                                     new Rectangle2D[] { new Rectangle2D( 5124, 1285, 95, 103 ) },
                                                     new Rectangle2D[] { new Rectangle2D( 5220, 1287, 5, 9 ) },
                                                     new Rectangle2D(0, 0, 0, 0));
                }

                return _Definition;
            }
        }

        public CTFArena1(TournamentStone stone) : base(stone)
        {
            Active = true;
        }

        public override void InitializeSystem()
        {
            base.InitializeSystem();

            StatsBoard.ItemID = 7774;
            TournamentBoard.ItemID = 7774;

            for (int i = 0; i < 13; i++)
            {
                Point3D p = new Point3D(5237 + i, 1495, 0);

                var st = new Static(2083);
                st.MoveToWorld(p, ArenaMap);

                var los = new LOSBlocker();
                los.MoveToWorld(p, ArenaMap);
            }
        }

        public static void Setup(Mobile from)
        {
            if (ArenaHelper.HasArena<CTFArena1>())
            {
                from.SendMessage(22, "CTF Arena 1 already exists!");
                return;
            }

            CTFArena1Stone stone = new CTFArena1Stone();
            CTFArena1 arena = new CTFArena1(stone);
            stone.MoveToWorld(arena.StoneLocation, arena.ArenaMap);

            from.SendMessage(1154, "CTF Arena 1 Setup!");
        }

        public static void Delete(Mobile from)
        {
            var arena = ArenaHelper.GetArena<CTFArena1>();

            if (arena == null)
                return;

            var map = arena.ArenaMap;

            if (arena.Stone != null)
            {
                arena.Stone.Delete();
                from.SendMessage(22, "CTF Arena 1 removed!");
            }
            else
            {
                from.SendMessage(22, "Error removaing CTF Arena 1.");
            }

            if (map != null)
            {
                for (int i = 0; i < 13; i++)
                {
                    Point3D p = new Point3D(5237 + i, 1495, 0);

                    IPooledEnumerable eable = map.GetItemsInRange(p, 0);

                    foreach (Item item in eable)
                    {
                        if (item is Static || item is LOSBlocker)
                        {
                            item.Delete();
                        }
                    }

                    eable.Free();
                }
            }
        }
 
        public override void SetLinkedSystem()
        {
            foreach (PVPTournamentSystem system in PVPTournamentSystem.SystemList)
            {
                if (system is CTFArena2)
                {
                    LinkedSystem = system;
 
                    if (system.LinkedSystem != this)
                        system.LinkedSystem = this;
                }
            }
        }
 
        public CTFArena1(GenericReader reader, TournamentStone stone) : base(reader, stone)
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
            /*int version = */reader.ReadInt();
        }
    }

    [DeleteConfirm("Are you sure you want to delete this? Deleting this stone will remove any upcoming tournaments and any prize items and all of the arena.")]
    public class CTFArena1Stone : TournamentStone
    {
        public CTFArena1Stone()
        {
        }
 
        public CTFArena1Stone(Serial serial) : base (serial)
        {
        }
 
        public override void LoadSystem(GenericReader reader)
        {
            System = new CTFArena1(reader, this);
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

    public class CTFArena2 : PVPTournamentSystem
    {
        public override string DefaultName { get { return "Capture the Flag Arena 2"; } }
        public override int WallItemID { get { return 0; } }
        public override Map ArenaMap { get { return Map.Felucca; } }

        public override Point3D FlagHolder1Loc { get { return new Point3D(5299, 1483, 0); } }
        public override Point3D FlagHolder2Loc { get { return new Point3D(5156, 1508, 0); } }

        private ArenaDefinition _Defintion;

        public override ArenaDefinition Definition
        {
            get
            {
                if (_Defintion == null)
                {
                    _Defintion = new ArenaDefinition(new Point3D(5226, 1498, 0), new Point3D(5299, 1483, 0), new Point3D(5156, 1508, 0),
                                                     new Point3D(5235, 1500, 0), new Point3D(5238, 1496, 9), new Point3D(5248, 1496, 9),
                                                     new Point3D(5231, 1496, 0), new Point3D(5234, 1496, 0), Point3D.Zero,
                                                     new Point3D(5223, 1495, 2), new Point3D(5228, 1496, 7),
                                                     new Rectangle2D(5220, 1496, 40, 12), new Rectangle2D(0, 0, 0, 0),
                                                     new Rectangle2D[] { new Rectangle2D(5125, 1412, 248, 84), new Rectangle2D(5125, 1495, 69, 21) },
                                                     new Rectangle2D[] { new Rectangle2D( 5220, 1496, 40, 12 ) },
                                                     new Rectangle2D(0, 0, 0, 0));
                }

                return _Defintion;
            }
        }

        public override bool ForceFightType { get { return true; } }
        public override ArenaFightType ForcedFightType { get { return ArenaFightType.CaptureTheFlag; } }

        public CTFArena2(TournamentStone stone)
            : base(stone)
        {
            Active = true;
        }

        public override void InitializeSystem()
        {
            base.InitializeSystem();

            StatsBoard.ItemID = 7774;
            TournamentBoard.ItemID = 7774;

            for (int i = 0; i < 10; i++)
            {
                Point3D p = new Point3D(5219, 1287 + i, 0);

                var st = new Static(2081);
                st.MoveToWorld(p, ArenaMap);

                var los = new LOSBlocker();
                los.MoveToWorld(p, ArenaMap);
            }
        }

        public static void Setup(Mobile from)
        {
            if (ArenaHelper.HasArena<CTFArena2>())
            {
                from.SendMessage(22, "CTF Arena 2 already exists!");
                return;
            }

            CTFArena2Stone stone = new CTFArena2Stone();
            CTFArena2 arena = new CTFArena2(stone);
            stone.MoveToWorld(arena.StoneLocation, arena.ArenaMap);

            from.SendMessage(1154, "CTF Arena 2 Setup!");
        }

        public static void Delete(Mobile from)
        {
            var arena = ArenaHelper.GetArena<CTFArena2>();

            if (arena == null)
                return;

            var map = arena.ArenaMap;

            if (arena.Stone != null)
            {
                arena.Stone.Delete();
                from.SendMessage(22, "CTF Arena 2 removed!");
            }
            else
            {
                from.SendMessage(22, "Error removing CTF Arena 2.");
            }

            if (map != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    Point3D p = new Point3D(5219, 1287 + i, 0);

                    IPooledEnumerable eable = map.GetItemsInRange(p, 0);

                    foreach (Item item in eable)
                    {
                        if (item is Static || item is LOSBlocker)
                        {
                            item.Delete();
                        }
                    }

                    eable.Free();
                }
            }
        }

        public override void SetLinkedSystem()
        {
            foreach (PVPTournamentSystem system in PVPTournamentSystem.SystemList)
            {
                if (system is CTFArena1)
                {
                    LinkedSystem = system;
 
                    if (system.LinkedSystem != this)
                        system.LinkedSystem = this;
                }
            }
        }

        public CTFArena2(GenericReader reader, TournamentStone stone)
            : base(reader, stone)
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
            /*int version = */reader.ReadInt();
        }
    }

    [DeleteConfirm("Are you sure you want to delete this? Deleting this stone will remove any upcoming tournaments and any prize items and all of the arena.")]
    public class CTFArena2Stone : TournamentStone
    {
        public CTFArena2Stone()
        {
        }

        public CTFArena2Stone(Serial serial)
            : base(serial)
        {
        }

        public override void LoadSystem(GenericReader reader)
        {
            System = new CTFArena2(reader, this);
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