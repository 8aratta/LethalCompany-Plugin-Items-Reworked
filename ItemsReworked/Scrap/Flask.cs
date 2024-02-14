#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
using System.Collections;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class Flask : BaseScrapItem
    {
        /// <summary>
        // TODO:
        // - Add custom text for every item after use and inspect
        // - Add possible drawbacks after use
        // EFFECT IDEAS:
        // SCARED / STRONG / BRAVE / SLOWED / INVERTED CONTROLS / FROZEN / INSTANT DEATH 
        /// </summary>

        private System.Random random = new System.Random();
        private string flaskEffect = "None";

        internal Flask(GrabbableObject flask) : base(flask)
        {
            int totalProbability = 0;
            int[] probabilities = new int[4];

            // Assign probabilities based on configuration
            probabilities[0] = ItemsReworkedPlugin.NoEffectProbability.Value;
            totalProbability += probabilities[0];

            probabilities[1] = ItemsReworkedPlugin.IntoxicationEffectProbability.Value;
            totalProbability += probabilities[1];

            probabilities[2] = ItemsReworkedPlugin.PoisoningEffectProbability.Value;
            totalProbability += probabilities[2];

            probabilities[3] = ItemsReworkedPlugin.HealingEffectProbability.Value;
            totalProbability += probabilities[3];

            // If no effects are enabled, set NoEffect as default
            if (totalProbability == 0)
            {
                flaskEffect = "None";
                return;
            }

            // Generate a random number within the total probability space
            int randomNum = random.Next(totalProbability);

            // Determine the selected effect based on the random number
            int cumulativeProbability = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulativeProbability += probabilities[i];
                if (randomNum < cumulativeProbability)
                {
                    // Effect i is selected
                    switch (i)
                    {
                        case 0:
                            flaskEffect = "NoEffect";
                            break;
                        case 1:
                            flaskEffect = "Intoxication";
                            break;
                        case 2:
                            flaskEffect = "Poisoning";
                            break;
                        case 3:
                            flaskEffect = "Healing";
                            break;
                    }
                    break;
                }
            }
        }

        public override void InspectItem()
        {
            if (!BaseScrap.itemUsedUp)
                HUDManager.Instance.DisplayTip("A random flask", "Should I really risk drinking the content?");
            else
                HUDManager.Instance.DisplayTip("A random flask", "... the taste, was not great.");
        }

        public override void UseItem()
        {
            if (!BaseScrap.itemUsedUp)
            {
                BaseScrap.itemUsedUp = true;

                LocalPlayer.StartCoroutine(DelayedActivation(3f, () =>
                    {
                        switch (flaskEffect)
                        {
                            default:
                            case "None":
                                NoEffect();
                                break;
                            case "Intoxication":
                                ApplyDrunkEffect(LocalPlayer);
                                break;
                            case "Poisoning":
                                LocalPlayer.StartCoroutine(ApplyPoisonEffect(LocalPlayer));
                                break;
                            case "Healing":
                                LocalPlayer.StartCoroutine(ApplyHealEffect(LocalPlayer));
                                break;
                        }
                    }));
                BaseScrap.SetScrapValue(3);
            }
        }

        public override void SpecialUseItem()
        {
            throw new System.NotImplementedException();
        }

        private void NoEffect()
        {
            HUDManager.Instance.DisplayTip("Nothing", "Nothing happened...");
        }

        private void ApplyDrunkEffect(PlayerControllerB player)
        {
            // Make the LocalPlayer drunk
            AudioHandler.PlaySound(player, "Scrap\\Flask\\Intoxication.mp3");
            player.drunkness = 1f;
            HUDManager.Instance.DisplayTip("Intoxication", "You feel a bit dizzy.");
        }

        private IEnumerator ApplyPoisonEffect(PlayerControllerB player)
        {
            HUDManager.Instance.DisplayTip("Poisoning Effect", "You feel a burning sensation.");

            float elapsedTime = 0f;

            while (player.health > ItemsReworkedPlugin.MaxPoison.Value)
            {
                // Decrement LocalPlayer's health gradually
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 3f) // Adjust the duration as needed
                {
                    player.DamagePlayer(1, false);
                    elapsedTime = 0f;

                    // Update UI
                    HUDManager.Instance.UpdateHealthUI(player.health, true);
                }

                yield return null; // Wait for the next frame
            }
        }


        private IEnumerator ApplyHealEffect(PlayerControllerB player)
        {
            HUDManager.Instance.DisplayTip("Healing Effect", "You feel rejuvenated.");

            const int minScrapValue = 16;
            const int maxScrapValue = 44;
            int minHealing = 1;
            int maxHealing = ItemsReworkedPlugin.MaxHealing.Value;
            int healing;

            if (BaseScrap.scrapValue <= minScrapValue)
                healing = minHealing;
            else if (BaseScrap.scrapValue >= maxScrapValue)
                healing = maxHealing;
            else
            {
                float percentage = (float)(BaseScrap.scrapValue - minScrapValue) / (maxScrapValue - minScrapValue);
                healing = Mathf.RoundToInt(Mathf.Lerp(minHealing, maxHealing, percentage));
            }

            int targetHealth = player.health + healing;

            float elapsedTime = 0f;

            while (player.health < targetHealth && player.health < 100)
            {
                // Increment LocalPlayer's health gradually
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= 3f)
                {
                    player.health++;
                    elapsedTime = 0f;

                    // Ensure health does not exceed the target health
                    player.health = Mathf.Min(player.health, targetHealth);

                    // Update UI
                    HUDManager.Instance.UpdateHealthUI(player.health, false);
                }
                yield return null; // Wait for the next frame
            }
        }
    }
}
