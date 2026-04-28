using Server.Engines.VvV;
using Server.Items;

namespace Server.Commands
{
    public static class RestockVvVTradersCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("RestockVvVTraders", AccessLevel.Administrator, OnCommand);
        }

        [Usage("RestockVvVTraders")]
        [Description("Wipes every SilverTrader's backpack and re-stocks fresh reward items so the vendor preview reflects current item stats.")]
        public static void OnCommand(CommandEventArgs e)
        {
            int restocked = 0;

            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is SilverTrader trader)
                {
                    if (trader.Backpack != null)
                        ColUtility.SafeDelete<Item>(trader.Backpack.Items, null);

                    trader.StockInventory();
                    restocked++;
                }
            }

            e.Mobile.SendMessage("Restocked {0} SilverTrader(s).", restocked);
        }
    }
}
