using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x1451, 0x1456 )]
	public class OldBoneHelm : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 70; } }
		public override int BaseFireResistance{ get{ return -15; } }
		public override int BaseColdResistance{ get{ return -15; } }
		public override int BasePoisonResistance{ get{ return -15; } }
		public override int BaseEnergyResistance{ get{ return -15; } }

		public override int InitMinHits{ get{ return 100; } }
		public override int InitMaxHits{ get{ return 100; } }

		public override int AosStrReq{ get{ return 20; } }
		public override int OldStrReq{ get{ return 40; } }

		public override int ArmorBase{ get{ return 30; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public OldBoneHelm() : base( 0x1451 )
		{
			Name = " Old Bone Helm [RARE]";
			Hue = 1126;
			Weight = 3.0;
			Attributes.WeaponSpeed = 20;
			Attributes.LowerManaCost = 40;
		}

		public OldBoneHelm( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			if ( Weight == 1.0 )
				Weight = 3.0;
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}