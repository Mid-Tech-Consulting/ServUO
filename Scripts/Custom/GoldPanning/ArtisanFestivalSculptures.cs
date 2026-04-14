using System;
using Server;

namespace Server.Items
{
	//2016
	//Sword Sculpture
	public class ValorSculpture : BaseLight
	{
		public override int LitItemID { get { return 40576; } }
		public override int UnlitItemID { get { return 40430; } }

		[Constructable]
		public ValorSculpture()
			: base( 40430 )
		{
			Name = "Wooden Valor Sculpture";
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;
			Duration = TimeSpan.Zero;
		}

		public ValorSculpture( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}

	//AXE Sculpture
	public class DespiseSculpture : BaseLight
	{
		public override int LitItemID { get { return 40577; } }
		public override int UnlitItemID { get { return 40434; } }

		[Constructable]
		public DespiseSculpture()
			: base( 40434 )
		{
			Name = "Wooden Despise Sculpture";
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;
			Duration = TimeSpan.Zero;
		}

		public DespiseSculpture( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}

	//Ankh Sculpture
	public class SpiritualitySculpture : BaseLight
	{
		public override int LitItemID { get { return 40578; } }
		public override int UnlitItemID { get { return 40438; } }

		[Constructable]
		public SpiritualitySculpture()
			: base( 40438 )
		{
			Name = "Wooden Spirituality Sculpture";
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;
			Duration = TimeSpan.Zero;
		}

		public SpiritualitySculpture( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}

	//Mask Sculpture
	public class DeceitSculpture : BaseLight
	{
		public override int LitItemID { get { return 40579; } }
		public override int UnlitItemID { get { return 40442; } }

		[Constructable]
		public DeceitSculpture()
			: base( 40442 )
		{
			Name = "Wooden Deceit Sculpture";
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;
			Duration = TimeSpan.Zero;
		}

		public DeceitSculpture( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}


	//2017
	//Moon Sculpture
	public class MoonSculpture : BaseLight
	{
		public override int LitItemID { get { return 41057; } }
		public override int UnlitItemID { get { return 41056; } }

		[Constructable]
		public MoonSculpture()
			: base( 41056 )
		{
			Name = "Wooden Moon Sculpture";
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;
			Duration = TimeSpan.Zero;
		}

		public MoonSculpture( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}

	//Goblet Sculpture
	public class ChaliceSculpture : BaseLight
	{
		public override int LitItemID { get { return 41062; } }
		public override int UnlitItemID { get { return 41061; } }

		[Constructable]
		public ChaliceSculpture()
			: base( 41061 )
		{
			Name = "Wooden Chalice Sculpture";
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;
			Duration = TimeSpan.Zero;
		}

		public ChaliceSculpture( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}

	//Skull Sculpture
	public class SkullSculpture : BaseLight
	{
		public override int LitItemID { get { return 41067; } }
		public override int UnlitItemID { get { return 41066; } }

		[Constructable]
		public SkullSculpture()
			: base( 41066 )
		{
			Name = "Wooden Skull Sculpture";
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;
			Duration = TimeSpan.Zero;
		}

		public SkullSculpture( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}

	//Seed Sculpture
	public class TearDropSculpture : BaseLight
	{
		public override int LitItemID { get { return 41072; } }
		public override int UnlitItemID { get { return 41071; } }

		[Constructable]
		public TearDropSculpture()
			: base( 41071 )
		{
			Name = "Wooden Tear Drop Sculpture";
			Weight = 10.0;
			LootType = LootType.Blessed;

			Light = LightType.Circle300;
			Duration = TimeSpan.Zero;
		}

		public TearDropSculpture( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();
		}
	}
}
