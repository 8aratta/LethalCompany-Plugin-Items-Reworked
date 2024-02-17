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
            ItemName = "Mysterious Flask";
            // Update Visible Scrap Name
            BaseScrap.gameObject.GetComponentInChildren<ScanNodeProperties>().headerText = ItemName;

            ItemDescription = "Should I really risk drinking the content?";
            RandomizeEffect();
        }

        public override void UpdateItem()
        {
            if (ItemPropertiesDiscovered)
            {
                // Update Visible Scrap Name
                BaseScrap.gameObject.GetComponentInChildren<ScanNodeProperties>().headerText = ItemName;
            }

            // Reset modified state
            ItemModified = false;
        }

        public override void InspectItem()
        {
            if (!BaseScrap.itemUsedUp)
            {
                HUDManager.Instance.DisplayTip($"{ItemName}", $"{ItemDescription}");
            }
            else
                HUDManager.Instance.DisplayTip($"{ItemName}", "... the taste, was not that great.");
        }

        public override void UseItem()
        {
            if (!BaseScrap.itemUsedUp)
            {
                BaseScrap.itemUsedUp = true;

                HoldingPlayer.StartCoroutine(DelayedActivation(3f, () =>
                    {
                        switch (flaskEffect)
                        {
                            default:
                            case "None":
                                NoEffect();
                                break;
                            case "Intoxication":
                                ApplyDrunkEffect(HoldingPlayer);
                                break;
                            case "Poisoning":
                                HoldingPlayer.StartCoroutine(ApplyPoisonEffect(HoldingPlayer));
                                break;
                            case "Healing":
                                HoldingPlayer.StartCoroutine(ApplyHealEffect(HoldingPlayer));
                                break;
                        }
                    }));
                BaseScrap.SetScrapValue(3);
            }
        }

        public override void SecondaryUseItem()
        {
            throw new System.NotImplementedException();
        }

        private void RandomizeEffect()
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
                return;

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
                            ItemName = "Pointless Flask";
                            ItemDescription = "It does nothing! Or does it...";
                            break;
                        case 1:
                            flaskEffect = "Intoxication";
                            ItemName = "Flask of Intoxication";
                            ItemDescription = "The flask oozes a sweet aroma... reminding you of certain beverages your father used to drink.";
                            break;
                        case 2:
                            flaskEffect = "Poisoning";
                            ItemName = "Flask of Poisoning";
                            ItemDescription = "Filled with a deadly toxin... Only the most depraved would dare to consume this.";
                            break;
                        case 3:
                            flaskEffect = "Healing";
                            ItemName = "Flask of Healing";
                            ItemDescription = "What a rare find!";
                            break;
                    }
                    break;
                }
            }
        }

        private void NoEffect()
        {
            if (ItemPropertiesDiscovered)
                HUDManager.Instance.DisplayTip("Nothing", "Well now we are certain that it does nothing...");
            else
                HUDManager.Instance.DisplayTip("Nothing", "Nothing happened...");
        }

        private void ApplyDrunkEffect(PlayerControllerB player)
        {
            if (ItemPropertiesDiscovered)
                HUDManager.Instance.DisplayTip("Intoxication", "Don't drink and drive!");
            else
                HUDManager.Instance.DisplayTip("Intoxication", "You feel a bit dizzy.");

            // Make the HoldingPlayer drunk
            AudioHandler.PlaySound(player, "Scrap\\Flask\\Intoxication.mp3");
            player.drunkness = 1f;
        }

        private IEnumerator ApplyPoisonEffect(PlayerControllerB player)
        {
            if (ItemPropertiesDiscovered)
                HUDManager.Instance.DisplayTip("You've been poisoned", "I don't know what you expected to happen...", true);
            else
                HUDManager.Instance.DisplayTip("Poisoning Effect", "Well..");

            float elapsedTime = 0f;

            while (player.health > ItemsReworkedPlugin.MaxPoison.Value)
            {
                // Decrement HoldingPlayer's health gradually
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
            if (ItemPropertiesDiscovered)
                HUDManager.Instance.DisplayTip("Healing", "You feel rejuvenated... this is starting to feel like a some kind of JRPG...");
            else
                HUDManager.Instance.DisplayTip("Healing", "You are being healed.");

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
                // Increment HoldingPlayer's health gradually
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
