using System;

namespace Server.Items
{
	//2016
	[FlipableAttribute(40493, 40494)]
	public class CovetousTapestry : Item
	{
		public override string DefaultName{ get { return "A Covetous Tapestry"; } }
		public override double DefaultWeight{ get { return 5.0; } }

		[Constructable]
		public CovetousTapestry ()
			: base(40493)
		{
		}

		public CovetousTapestry (Serial serial)
			: base(serial)
		{
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

	[FlipableAttribute(40495, 40496)]
	public class PrideTapestry : Item
	{
		public override string DefaultName{ get { return "A Pride Tapestry"; } }
		public override double DefaultWeight{ get { return 5.0; } }

		[Constructable]
		public PrideTapestry ()
			: base(40495)
		{
		}

		public PrideTapestry(Serial serial)
			: base(serial)
		{
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

	[FlipableAttribute(40497, 40498)]
	public class HumilityTapestry : Item
	{
		public override string DefaultName{ get { return "A Humility Tapestry"; } }
		public override double DefaultWeight{ get { return 5.0; } }

		[Constructable]
		public HumilityTapestry ()
			: base(40497)
		{
		}

		public HumilityTapestry(Serial serial)
			: base(serial)
		{
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

	[FlipableAttribute(40499, 40500)]
	public class JusticeTapestry : Item
	{
		public override string DefaultName{ get { return "A Justice Tapestry"; } }
		public override double DefaultWeight{ get { return 5.0; } }

		[Constructable]
		public JusticeTapestry ()
			: base(40499)
		{
		}

		public JusticeTapestry(Serial serial)
			: base(serial)
		{
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


	//2017
	[FlipableAttribute(41123, 41124)]
	public class HeartTapestry : Item
	{
		public override string DefaultName{ get { return "A Heart Tapestry"; } }
		public override double DefaultWeight{ get { return 5.0; } }

		[Constructable]
		public HeartTapestry ()
			: base(41123)
		{
		}

		public HeartTapestry(Serial serial)
			: base(serial)
		{
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

	[FlipableAttribute(41125, 41126)]
	public class ShieldTapestry : Item
	{
		public override string DefaultName{ get { return "A Broken-Shield Tapestry"; } }
		public override double DefaultWeight{ get { return 5.0; } }

		[Constructable]
		public ShieldTapestry ()
			: base(41125)
		{
		}

		public ShieldTapestry(Serial serial)
			: base(serial)
		{
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

	[FlipableAttribute(41127, 41128)]
	public class HandTapestry : Item
	{
		public override string DefaultName{ get { return "A Hand Tapestry"; } }
		public override double DefaultWeight{ get { return 5.0; } }

		[Constructable]
		public HandTapestry ()
			: base(41127)
		{
		}

		public HandTapestry(Serial serial)
			: base(serial)
		{
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

	[FlipableAttribute(41129, 41130)]
	public class ScalesTapestry : Item
	{
		public override string DefaultName{ get { return "A Scales Tapestry"; } }
		public override double DefaultWeight{ get { return 5.0; } }

		[Constructable]
		public ScalesTapestry ()
			: base(41129)
		{
		}

		public ScalesTapestry(Serial serial)
			: base(serial)
		{
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
