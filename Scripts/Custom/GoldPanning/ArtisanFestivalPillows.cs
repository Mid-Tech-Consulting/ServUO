using System;

namespace Server.Items
{
	//2016
	[FlipableAttribute(40477, 40478)]
	public class CompassionPillow : Item
	{
		public override string DefaultName{ get { return "A Compassion Throw Pillow"; } }
		public override double DefaultWeight{ get { return 1.0; } }

		[Constructable]
		public CompassionPillow ()
			: base(40477)
		{
		}

		public CompassionPillow (Serial serial)
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

	[FlipableAttribute(40479, 40480)]
	public class HonestyPillow : Item
	{
		public override string DefaultName{ get { return "A Honesty Throw Pillow"; } }
		public override double DefaultWeight{ get { return 1.0; } }

		[Constructable]
		public HonestyPillow ()
			: base(40479)
		{
		}

		public HonestyPillow (Serial serial)
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
	[FlipableAttribute(41113, 41114)]
	public class MasksPillow : Item
	{
		public override string DefaultName{ get { return "A Masks Throw Pillow"; } }
		public override double DefaultWeight{ get { return 1.0; } }

		[Constructable]
		public MasksPillow ()
			: base(41113)
		{
		}

		public MasksPillow (Serial serial)
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

	[FlipableAttribute(41115, 41116)]
	public class AxePillow : Item
	{
		public override string DefaultName{ get { return "A Axe Throw Pillow"; } }
		public override double DefaultWeight{ get { return 1.0; } }

		[Constructable]
		public AxePillow ()
			: base(41115)
		{
		}

		public AxePillow (Serial serial)
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

	[FlipableAttribute(41117, 41118)]
	public class SwordPillow : Item
	{
		public override string DefaultName{ get { return "A Sword Throw Pillow"; } }
		public override double DefaultWeight{ get { return 1.0; } }

		[Constructable]
		public SwordPillow ()
			: base(41117)
		{
		}

		public SwordPillow (Serial serial)
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

	[FlipableAttribute(41119, 41120)]
	public class AnkhPillow : Item
	{
		public override string DefaultName{ get { return "A Ankh Throw Pillow"; } }
		public override double DefaultWeight{ get { return 1.0; } }

		[Constructable]
		public AnkhPillow ()
			: base(41119)
		{
		}

		public AnkhPillow (Serial serial)
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
