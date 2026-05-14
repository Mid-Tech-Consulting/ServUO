using Server.Items;

namespace Server.Commands
{
    public static class TestAncientSOSCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("FillAncientChest", AccessLevel.GameMaster, OnCommand);
        }

        [Usage("FillAncientChest")]
        [Description("Spawns a fully-filled Ancient SOS chest at your feet. Admin/GM only.")]
        public static void OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            LockableContainer chest = Utility.RandomBool()
                ? (LockableContainer)new MetalGoldenChest()
                : new WoodenChest();

            chest.Hue = 0x481; // ancient gold tint

            TreasureMapChest.Fill(from, chest, 4, true);

            // Mirror the Ancient SOS path in Fishing.cs: fabled net + 2
            // guaranteed legendaries on top of whatever Fill rolled.
            chest.DropItem(new FabledFishingNet());

            for (int n = 0; n < 2; n++)
            {
                Item legendary = Custom.Loot.LegendaryRoller.TryRollLegendary();
                if (legendary != null)
                    chest.DropItem(legendary);
            }

            // Same guaranteed gem stash every real SOS chest drops -- 9
            // pre-stacked stacks of 25 each.
            chest.DropItem(new Amber(25));
            chest.DropItem(new Amethyst(25));
            chest.DropItem(new Citrine(25));
            chest.DropItem(new Diamond(25));
            chest.DropItem(new Emerald(25));
            chest.DropItem(new Ruby(25));
            chest.DropItem(new Sapphire(25));
            chest.DropItem(new StarSapphire(25));
            chest.DropItem(new Tourmaline(25));

            chest.Movable = true;
            chest.Locked = false;
            chest.TrapType = TrapType.None;
            chest.TrapPower = 0;
            chest.TrapLevel = 0;
            chest.IsShipwreckedItem = true;

            chest.MoveToWorld(from.Location, from.Map);

            from.SendMessage("Filled Ancient SOS chest dropped at your feet.");
        }
    }
}
