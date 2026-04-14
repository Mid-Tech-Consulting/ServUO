using System;
using Server.Items;


namespace Server.Items
{
    public class SaltyBoots : BaseArmor
    {
        //public override int ArtifactRarity { get { return 20; } }  
        //public override int InitMinHits{ get{ return 255; } }
		//public override int InitMaxHits{ get{ return 255; } }
        public override int ColdResistance { get { return 15; } } 

	public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
                
		[Constructable]
		public SaltyBoots() : base( 0x1711 )
		{
			Name = "<BASEFONT COLOR=#A335EE>Salty Boots</font>";
			Weight = 0.2;
            Hue = 18;
            //Name = "Salty Boots";

            Attributes.SpellDamage = Utility.RandomMinMax(5, 20);
		}
		public override void AddNameProperty(ObjectPropertyList list)
        {
            base.AddNameProperty(list);
            list.Add("<BASEFONT COLOR=#A335EE>(Gamma)</font>");
        }

		public SaltyBoots( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}