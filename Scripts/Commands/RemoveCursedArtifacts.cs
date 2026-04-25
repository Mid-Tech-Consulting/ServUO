using System.Linq;
using Server.Items;

namespace Server.Commands
{
    public static class RemoveCursedArtifacts
    {
        public static void Initialize()
        {
            CommandSystem.Register("RemoveCursedArtifacts", AccessLevel.Administrator,
                new CommandEventHandler(OnCommand));
        }

        [Usage("RemoveCursedArtifacts")]
        [Description("Scans every item in the world and clears LootType.Cursed from Major/Legendary Artifact tier items. One-shot cleanup for items that rolled under the old logic.")]
        public static void OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;
            int count = 0;

            foreach (var item in World.Items.Values.ToList())
            {
                if (item == null || item.Deleted)
                    continue;

                if (item.LootType != LootType.Cursed)
                    continue;

                if (!(item is ICombatEquipment combat))
                    continue;

                if (combat.ItemPower >= ItemPower.MajorArtifact)
                {
                    item.LootType = LootType.Regular;
                    item.InvalidateProperties();
                    count++;
                }
            }

            m.SendMessage("Cleaned Cursed from {0} Major/Legendary artifact item(s).", count);
        }
    }
}
