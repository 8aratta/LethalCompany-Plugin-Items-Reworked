#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
using System.Collections;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class PillBottle : BaseItem
    {
        private int remainingPills;
        private int pillQuality;

        internal PillBottle(GrabbableObject pillBottle)
        {
            pillQuality = GetPillQuality();
            remainingPills = CalculatePills(pillBottle.scrapValue);
        }

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {
            if (item.itemUsedUp)
            {
                HUDManager.Instance.DisplayTip("An empty pill bottle", "Seems like there are no pills left in this one...");
            }
            else
            {
                HUDManager.Instance.DisplayTip("A pill bottle", "There are some pills in here...");
            }
        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (!item.itemUsedUp && !inSpecialScenario && player.health != 100 && !player.inTerminalMenu)
            {
                inSpecialScenario = true;
                var soundName = "PillPop" + pillQuality + ".mp3";
                AudioHandler.PlaySound(player, "Scrap\\PillBottle\\" + soundName);
                ItemsReworkedPlugin.mls.LogInfo($"playing: {soundName}");
                player.StartCoroutine(DelayedActivation(player, item, 2f, () =>
                {
                    remainingPills = IngestPills(player, remainingPills);
                    item.SetScrapValue(remainingPills);
                    if (remainingPills == 0)
                    {
                        item.SetScrapValue(1);
                        item.itemUsedUp = true;
                    }
                }));
            }
        }

        private int GetPillQuality()
        {
            System.Random random = new System.Random();
            int pillQuality = random.Next(-1, 4);
            if (pillQuality != 0)
                return pillQuality;
            else return 1;
        }

        private int CalculatePills(int scrapValue)
        {
            const int minScrapValue = 16;
            const int maxScrapValue = 40;
            const int minPills = 2; //CONFIG
            const int maxPills = 33; //CONFIG

            if (scrapValue <= minScrapValue)
            {
                return minPills;
            }
            else if (scrapValue >= maxScrapValue)
            {
                return maxPills;
            }
            else
            {
                float percentage = (float)(scrapValue - minScrapValue) / (maxScrapValue - minScrapValue);
                return Mathf.RoundToInt(Mathf.Lerp(minPills, maxPills, percentage));
            }
        }

        private int IngestPills(PlayerControllerB player, int pills)
        {
            int maxHealth = 100;
            int surplus = 0;

            // Calculate healing ammount
            int dose = pills * pillQuality;

            if (player.health + dose > maxHealth)
            {
                //OVERDOSING -- FEATURE TO IMPLEMENT

                surplus = player.health + dose - maxHealth / pillQuality;
                player.StartCoroutine(GradualHealing(player, pills - surplus));
            }
            else
            {
                player.StartCoroutine(GradualHealing(player, pills));
            }

            return surplus;
        }

        private IEnumerator GradualHealing(PlayerControllerB player, int pills)
        {
            ItemsReworkedPlugin.mls.LogInfo("CoroutineStarted");
            int targetHealth = player.health + (pills * pillQuality);
            // Time needed until targetHealth is reached
            float healingDuration;
            switch (pillQuality)
            {
                default:
                    healingDuration = pills * 10f;
                    break;
                case 2:
                    healingDuration = pills * 5f;
                    break;
                case 3:
                    healingDuration = pills * 3f;
                    break;
            }

            // Interval in which the player is healed
            float healingInterval = healingDuration / (float)pills;

            float elapsedTime = 0f;
            float elapsedIntervalTime = 0f;

            while (elapsedTime < healingDuration)
            {
                // check if interval time passed
                if (elapsedIntervalTime > healingInterval)
                {
                    // Apply healing
                    player.health += pillQuality;

                    // Update UI
                    HUDManager.Instance.UpdateHealthUI(player.health, false);

                    //Reset Healing interval
                    elapsedIntervalTime = 0f;
                }

                // Increment elapsed time
                elapsedTime += Time.deltaTime;
                elapsedIntervalTime += Time.deltaTime;

                yield return null; // Wait for the next frame
            }

            // Ensure final health value is correct
            player.health = targetHealth;

            // Update UI one last time
            HUDManager.Instance.UpdateHealthUI(player.health, false);

            // Reset Special Scenario
            inSpecialScenario = false;
        }

        public override void SpecialUseItem(PlayerControllerB player, GrabbableObject item)
        {
            throw new System.NotImplementedException();
        }
    }
}
