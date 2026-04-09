using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.TournamentSystem
{
	public abstract class BaseArenaAddon : BaseAddon
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active 
		{ 
			get 
			{ 
				return Stone != null && Stone.System != null && Stone.System.Active; 
			}
			set
			{
				if(Stone != null && Stone.System != null)
				{
					Stone.System.Active = value;
				}
			}
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public TournamentStone Stone { get; set; }

        public abstract ArenaDefinition Definition { get; }

        public override bool ShareHue { get { return false; } }
        public override BaseAddonDeed Deed { get { return null; } }

        public abstract int[,] ComponentList { get; }

		[Constructable]
		public BaseArenaAddon()
		{
            for (int i = 0; i < ComponentList.Length / 4; i++)
            {
                SetComponent(ComponentList[i, 0], ComponentList[i, 1], ComponentList[i, 2], ComponentList[i, 3]); 
            }
		}

        public virtual void SetComponent(int id, int x, int y, int z)
        {
            var component = new AddonComponent(id);

            AddComponent(component, x, y, z);
        }
		
		private void ConfigureSystem()
		{
			Stone = new AddonArenaStone();
			var sys = new AddonArenaSystem(this, Stone);
			
			Stone.Location = sys.StoneLocation;

            sys.Active = true;
		}

        public virtual void OnSystemConfigured()
        {
        }
		
		public override void OnLocationChange(Point3D oldLoc)
		{
			base.OnLocationChange(oldLoc);

            if (Stone == null)
            {
                ConfigureSystem();
            }

            if (Stone != null)
            {
                var sys = Stone.System;
                Stone.Location = sys.StoneLocation;

                if (sys != null)
                {
                    if (sys.StatsBoard != null)
                        sys.StatsBoard.Location = sys.StatsBoardLocation;

                    if (sys.TournamentBoard != null)
                        sys.TournamentBoard.Location = sys.TournamentInfoBoardLocation;

                    if (sys.TeamsBoard != null)
                        sys.TeamsBoard.Location = sys.TeamsBoardLocation;

                    if (sys.Chest != null)
                        sys.Chest.Location = sys.ChestLocation;

                    int xOffset = X - oldLoc.X;
                    int yOffset = Y - oldLoc.Y;
                    int zOffset = Z - oldLoc.Z;

                    if (sys.ArenaKeeper != null)
                        sys.ArenaKeeper.Location = sys.ArenaKeeperLocation;

                    if (sys.Wagers != null && sys.Wagers.Count > 0)
                    {
                        foreach (var wage in sys.Wagers.Values)
                        {
                            wage.Location = new Point3D(wage.X + xOffset, wage.Y + yOffset, wage.Z + zOffset);
                        }
                    }

                    var list = new List<Mobile>();

                    if (sys.FightRegion != null)
                    {
                        foreach (var m in sys.FightRegion.GetEnumeratedMobiles().Where(mob => mob != sys.ArenaKeeper))
                        {
                            list.Add(m);
                        }

                        sys.FightRegion.Unregister();
                        sys.FightRegion = sys.GetFightRegion;
                    }

                    if (sys.AudienceRegion != null)
                    {
                        foreach (var m in sys.AudienceRegion.GetEnumeratedMobiles().Where(mob => mob != sys.ArenaKeeper && !list.Contains(mob)))
                        {
                            list.Add(m);
                        }

                        sys.AudienceRegion.Unregister();
                        sys.AudienceRegion = sys.GetAudienceRegion;
                    }

                    foreach (var m in list)
                    {
                        m.Location = new Point3D(m.X + xOffset, m.Y + yOffset, m.Z + zOffset);
                    }

                    ColUtility.Free(list);
                }
            }
		}
		
		public override void OnMapChange()
		{
			base.OnMapChange();
			
			if(Map == null || Map == Map.Internal)
			{
				if(Stone != null)
				{
					Stone.System.Active = false;
				}
			}
			else if (Stone != null)
			{
				AddonArenaSystem sys = Stone.System as AddonArenaSystem;
				Stone.Map = Map;

                if (sys != null)
                {
                    sys.SetMap(Map);

                    if (sys.StatsBoard != null)
                        sys.StatsBoard.Map = Map;

                    if (sys.TournamentBoard != null)
                        sys.TournamentBoard.Map = Map;

                    if (sys.TeamsBoard != null)
                        sys.TeamsBoard.Map = Map;

                    if (sys.Chest != null)
                        sys.Chest.Map = Map;

                    if (sys.ArenaKeeper != null)
                        sys.ArenaKeeper.Map = Map;

                    if (sys.Wagers != null && sys.Wagers.Count > 0)
                    {
                        foreach (var wage in sys.Wagers.Values)
                        {
                            wage.Map = Map;
                        }
                    }

                    if (sys.FightRegion != null)
                    {
                        foreach (var m in sys.FightRegion.GetEnumeratedMobiles())
                        {
                            m.Map = Map;
                        }

                        sys.FightRegion.Unregister();
                        sys.FightRegion = sys.GetFightRegion;
                    }

                    if (sys.AudienceRegion != null)
                    {
                        sys.AudienceRegion.Unregister();
                        sys.AudienceRegion = sys.GetAudienceRegion;
                    }
                }
                else
                {
                    Stone.Delete();
                }
			}
		}
		
		public override void Delete()
		{
			base.Delete();

            if (Stone != null)
            {
                Stone.Delete();
            }
		}
		
		public BaseArenaAddon(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
			writer.Write(Stone);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			reader.ReadInt();
			
			Stone = reader.ReadItem() as TournamentStone;
		}
	}
	
	public class AddonArenaSystem : PVPTournamentSystem
	{
		public override Rectangle2D KickZone { get { return ConvertOffset(Definition.KickZone); } }
		public override Rectangle2D WallArea { get { return ConvertOffset(Definition.WallArea); } }
		
		public override Rectangle2D[] FightingRegionBounds { get { return ConvertOffset(Definition.FightingRegionBounds); } }
		public override Rectangle2D[] AudienceRegionBounds { get { return ConvertOffset(Definition.AudienceRegionBounds); } }
		
		public override Point3D StoneLocation { get { return ConvertOffset(Definition.StoneLocation); } }
		public override Point3D TeamAStartLocation { get { return ConvertOffset(Definition.TeamAStartLocation); } }
		public override Point3D TeamBStartLocation { get { return ConvertOffset(Definition.TeamBStartLocation); } }
		public override Point3D ArenaKeeperLocation { get { return ConvertOffset(Definition.ArenaKeeperLocation); } }
		public override Point3D TeamAWageDisplay { get { return ConvertOffset(Definition.TeamAWageDisplay); } }
		public override Point3D TeamBWageDisplay { get { return ConvertOffset(Definition.TeamBWageDisplay); } }
		public override Point3D StatsBoardLocation { get { return ConvertOffset(Definition.StatsBoardLocation); } }
		public override Point3D TournamentInfoBoardLocation { get { return ConvertOffset(Definition.TournamentInfoBoardLocation); } }
		public override Point3D TeamsBoardLocation { get { return ConvertOffset(Definition.TeamsBoardLocation); } }
		public override Point3D ChestLocation { get { return ConvertOffset(Definition.ChestLocation); } }

        private Map _Map;
		private BaseArenaAddon _Addon;

        public virtual string DefaultArenaName { get { return "PVP Arena"; } }

        public override string DefaultName
        {
            get
            {
                string name = String.Empty;
                int i = 0;

                do
                {
                    name = String.Format("{0} #{1}", DefaultArenaName, ++i);
                }
                while (!ValidateName(name));

                return name;
            }
        }

		[CommandProperty(AccessLevel.GameMaster)]
		public BaseArenaAddon Addon 
		{
			get { return _Addon; }
			set
			{
				if(_Addon != null && value == null)
				{
                    if (Stone != null)
                    {
                        Stone.Delete();
                    }
				}
				
				_Addon = value;
			}
		}

        public override Map ArenaMap { get { return _Map; } }
		public override ArenaDefinition Definition { get { return Addon.Definition; } }
		
		public AddonArenaSystem(BaseArenaAddon addon, TournamentStone stone)
			: base(stone)
		{
			Addon = addon;

            UseLinked = false;
		}

        public void SetMap(Map map)
        {
            _Map = map;
        }
		
		private Point3D ConvertOffset(Point3D offset)
		{
            return new Point3D(Addon.X + offset.X, Addon.Y + offset.Y, Addon.Z + offset.Z);
		}
		
		private Rectangle2D ConvertOffset(Rectangle2D rec)
		{
            return new Rectangle2D(Addon.X + rec.X, Addon.Y + rec.Y, rec.Width, rec.Height);
		}
		
		private Rectangle2D[] ConvertOffset(Rectangle2D[] recs)
		{
            var newRec = new Rectangle2D[recs.Length];
			
			for(int i = 0; i < recs.Length; i++)
			{
				newRec[i] = ConvertOffset(recs[i]);
			}
			
			return newRec;
		}

        public override void OnSystemConfigured()
        {
            if (Addon != null)
            {
                Addon.OnSystemConfigured();
            }
        }

		public AddonArenaSystem(GenericReader reader, TournamentStone stone) : base(reader, stone)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
            writer.Write(Addon);
            writer.Write(_Map);

			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
            Addon = reader.ReadItem() as BaseArenaAddon;
            _Map = reader.ReadMap();

			base.Deserialize(reader);
			reader.ReadInt();
			
            if (Addon == null)
            {
                if (Stone != null)
                {
                    Timer.DelayCall(() => Stone.Delete());
                }
            }

            Timer.DelayCall(() =>
                {
                    if (FightRegion != null)
                    {
                        FightRegion.Unregister();
                        FightRegion = null;
                    }

                    if (AudienceRegion != null)
                    {
                        AudienceRegion.Unregister();
                        AudienceRegion = null;
                    }

                    FightRegion = GetFightRegion;
                    AudienceRegion = GetAudienceRegion;
                });
		}
	}
	
	public class AddonArenaStone : TournamentStone
	{
		public AddonArenaStone()
		{
		}
		
		public AddonArenaStone(Serial serial)
            : base(serial)
		{
		}

        public override void Delete()
        {
            if (System != null && System is AddonArenaSystem && ((AddonArenaSystem)System).Addon != null && !((AddonArenaSystem)System).Addon.Deleted)
            {
                ((AddonArenaSystem)System).Addon.Delete();
            }

            base.Delete();
        }

		public override void LoadSystem(GenericReader reader)
		{
			System = new AddonArenaSystem(reader, this);
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			reader.ReadInt();
		}
	}
}