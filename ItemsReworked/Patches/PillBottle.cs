using GameNetcodeStuff;

namespace ItemsReworked.Patches
{
    internal class PillBottle : BaseItem
    {
        public PillBottle()
        {
            ItemName = "PillBottle";
        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            ItemsReworkedPlugin.mls.LogWarning($"{ItemName} detected");

            if (!item.itemUsedUp)
            {
                ItemsReworkedPlugin.mls.LogInfo($"Scrap Value: {item.scrapValue}");
                HealPlayer(player, item.scrapValue);
                item.SetScrapValue(1);
                item.itemUsedUp = true;
            }
        }

        private void HealPlayer(PlayerControllerB player, int hp)
        {
            player.health += hp;
            HUDManager.Instance.UpdateHealthUI(player.health);
        }
    }
}
