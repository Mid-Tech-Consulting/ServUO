using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.CannedEvil;
using Server.Commands;

namespace Server.Items
{
    public class BigBeastChampSpawner : Item
    {
        [Constructable]
        public BigBeastChampSpawner() : base( 40123 )
        {
            Name = "Big Beast Champion Spawner";
            Hue = 1964;
            Movable = true;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);
            list.Add("([Event Item - Placeable])");
            list.Add("Double click as GM to spawn altar");
        }

        public override void OnDoubleClick( Mobile from )
        {
            if ( from.AccessLevel < AccessLevel.GameMaster )
            {
                from.SendMessage( "You must be a Game Master to use this." );
                return;
            }

            from.FixedParticles( 0x373A, 10, 15, 5036, EffectLayer.Head ); 
            from.PlaySound( 521 );

            ChampionSpawn spawn = new ChampionSpawn();
            var theme = Server.Custom.Events.CustomChampThemes.Find("Big Beast");
            if (theme != null)
                spawn.Type = theme.SpawnType;
            
            // ChampionSpawn platform is at Z-20, altar is at Z-15.
            // Placing the spawner at Z+20 ensures the platform and altar sit exactly on the ground level.
            Point3D spawnLoc = new Point3D( from.X, from.Y, from.Z + 20 );
            spawn.MoveToWorld( spawnLoc, from.Map );
            ChampionSystem.AllSpawns.Add( spawn );

            CommandLogging.WriteLine( from, "{0} {1} spawning a Big Beast Champion Spawn Altar at {2} on map {3}", from.AccessLevel, from.Name, from.Location, from.Map );
            from.SendMessage( "BEWARE! THE BIG BEAST CHAMPION SPAWN ALTAAR HAS APPEARED!" );
            this.Delete();
        }

        public BigBeastChampSpawner( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}
