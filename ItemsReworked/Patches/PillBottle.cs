namespace ItemsReworked.Patches
{
    internal class PillBottlePatches
    {
        bool itemUsedUp = false;

        public void ItemActivate(int scrapValue)
        {
            if (!itemUsedUp)
            {
                ItemsReworkedPlugin.mls.LogInfo($"Scrap Value: {scrapValue}");
                HealPlayer(scrapValue);
                scrapValue = 1;
                itemUsedUp = true;
            }
        }

        private static void HealPlayer(int hp)
        {
            ItemsReworkedPlugin.mls.LogInfo("Entering healing of player");
            StartOfRound.Instance.localPlayerController.health += hp;
        }
    }
}
