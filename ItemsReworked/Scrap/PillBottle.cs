using GameNetcodeStuff;

namespace ItemsReworked.Scrap
{
    internal class PillBottle : BaseItem
    {
        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (!item.itemUsedUp)
            {
                int remainingPills = HealPlayer(player, item.scrapValue);
                item.SetScrapValue(remainingPills);
                if (remainingPills == 0)
                {
                    item.SetScrapValue(1);
                    item.itemUsedUp = true;
                }
            }
        }


        private int HealPlayer(PlayerControllerB player, int hp)
        {
            int currentHealth = player.health;
            int maxHealth = 100;
            int surplus = 0;

            if (currentHealth + hp > maxHealth)
            {
                surplus = currentHealth + hp - maxHealth;
                player.health = maxHealth;
            }
            else
            {
                player.health += hp;
            }

            HUDManager.Instance.UpdateHealthUI(player.health, false);
            return surplus;
        }
    }
}
