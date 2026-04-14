using System;
using Server;

namespace Server.Items
{
	public class GoldShirt : FormalShirt
	{

		public override int BaseFireResistance{ get{ return 5; } }
		public override int BaseColdResistance{ get{ return 5; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 5; } }

		[Constructable]
		public GoldShirt()
		{

			Name = "Gold Panners Shirt";
			Hue = 18;

			Attributes.BonusStr = 5;
			Attributes.RegenHits = 2;
			Attributes.BonusHits = 5;
			//Attributes.DefendChance = Utility.RandomMinMax(5, 15);
			//Attributes.LowerManaCost = 10;
			//Attributes.WeaponDamage = 20;
			//Attributes.WeaponSpeed = 15;
		}

		public override bool Dye( Mobile from, DyeTub sender )
		{
			from.SendLocalizedMessage( sender.FailMessage );
			return false;
		}

		public GoldShirt( Serial serial ) : base( serial )
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
