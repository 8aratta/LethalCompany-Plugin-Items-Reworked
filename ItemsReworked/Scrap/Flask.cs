#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
#endregion

namespace ItemsReworked.Scrap
{
    internal class Flask : BaseItem
    {
        private System.Random random = new System.Random();
        private string flaskEffect = "None";
        internal Flask()
        {
            int randomEffect = random.Next(3);
            switch (randomEffect)
            {
                case 0:
                    flaskEffect = "Intoxication";
                    break;
                case 1:
                    flaskEffect = "Poisoning";
                    break;
                case 2:
                    flaskEffect = "Healing";
                    break;
            }
        }

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {
            if (!item.itemUsedUp)
                HUDManager.Instance.DisplayTip("A random flask", "Should I really risk drinking the content?");
            else
                HUDManager.Instance.DisplayTip("A random flask", "... the taste, was not great.");
        }
        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (!item.itemUsedUp)
            {
                item.itemUsedUp = true;

                player.StartCoroutine(DelayedActivation(player, item, 3f, () =>
                    {
                        switch (flaskEffect)
                        {
                            //EFFECT IDEAS:
                            // SCARED / STRONG / BRAVE / SLOWED / INVERTED CONTROLS / FROZEN / INSTANT DEATH 

                            case "Intoxication":
                                ApplyDrunkEffect(player);
                                break;
                            case "Poisoning":
                                ApplyPoisonEffect(player);
                                break;
                            case "Healing":
                                ApplyHealEffect(player, item.scrapValue);
                                break;
                        }
                    }));
                item.SetScrapValue(3);
            }
        }

        private void ApplyDrunkEffect(PlayerControllerB player)
        {
            // Make the player drunk
            AudioHandler.PlaySound(player, "Scrap\\Flask\\Intoxication.mp3");
            player.drunkness = 1f;
            HUDManager.Instance.DisplayTip("Intoxication", "You feel a bit dizzy.");
        }

        private void ApplyPoisonEffect(PlayerControllerB player)
        {
            HUDManager.Instance.DisplayTip("Poison Effect", "You feel a burning sensation.");
            //Damage Player
            player.health = 1;
            // Update UI
            HUDManager.Instance.UpdateHealthUI(player.health, true);
        }

        private void ApplyHealEffect(PlayerControllerB player, int hp)
        {
            HUDManager.Instance.DisplayTip("Healing Effect", "You feel rejuvenated.");

            int currentHealth = player.health;
            int maxHealth = 100;

            if (currentHealth + hp > maxHealth)
            {
                player.health = maxHealth;
            }
            else
            {
                player.health += hp;
            }

            HUDManager.Instance.UpdateHealthUI(player.health, false);
        }
    }
}
