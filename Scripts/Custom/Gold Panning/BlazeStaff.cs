using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class BlazeStaff : BlackStaff
	{
		//TODO: Pre-AoS stuff
		public override int LabelNumber{ get{ return 1017413; } } // Glacial Staff

		[Constructable]
		public BlazeStaff()
		{
			Name = "Blaze Staff";
			Hue = 1174;
			WeaponAttributes.HitFireball = Utility.RandomMinMax( 30, 70 );
			WeaponAttributes.MageWeapon = Utility.RandomMinMax( 5, 20 );
			Attributes.CastRecovery = 1 * Utility.RandomMinMax( 1, 4 );
			this.Attributes.CastSpeed = 1;
			this.Attributes.SpellDamage = Utility.RandomMinMax(5, 20);
			//Attributes.LowerManaCost = 15;
			//Attributes.LowerRegCost = 20;

			AosElementDamages[AosElementAttribute.Fire] = 30 + (5 * Utility.RandomMinMax( 0, 6 ));

		}

		public BlazeStaff( Serial serial ) : base( serial )
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
