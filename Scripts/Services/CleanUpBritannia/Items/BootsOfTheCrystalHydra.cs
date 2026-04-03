using System;
using Server;

namespace Server.Items
{
	public class BootsOfTheCrystalHydra : Boots
	{
		public override int LabelNumber { get { return 1151209; } } // Boots of the Crystal Hydra
		public override bool IsArtifact { get { return true; } }

		[Constructable]
		public BootsOfTheCrystalHydra()
		{
			Hue = 0x47E;
			Resistances.Energy = 2;
			SAAbsorptionAttributes.EaterEnergy = 5;
			Attributes.CastRecovery = 1;
			Attributes.RegenHits = 2;
		}

		public BootsOfTheCrystalHydra( Serial serial )
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

			int version = reader.ReadInt();
		}
	}
}