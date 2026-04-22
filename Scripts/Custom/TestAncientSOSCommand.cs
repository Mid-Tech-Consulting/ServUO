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

            chest.DropItem(new FabledFishingNet());

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
