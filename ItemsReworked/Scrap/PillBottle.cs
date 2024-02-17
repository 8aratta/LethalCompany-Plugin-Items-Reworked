#region usings
using ItemsReworked.Handlers;
using System;
using System.Collections;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class PillBottle : BaseScrapItem
    {
        private int remainingPills;
        private int pillMultiplier = -2;

        internal PillBottle(GrabbableObject pillBottle) : base(pillBottle)
        {
            ItemDescription = "Filled with a few pills";
            pillMultiplier = GetPillQuality();
            remainingPills = CalculatePills(BaseScrap.scrapValue);
        }

        public override void UpdateItem()
        {
            pillMultiplier = GetPillQuality();
            remainingPills = CalculatePills(BaseScrap.scrapValue);

            if (ItemPropertiesDiscovered)
            {
                ItemName = ItemQuality + " " + ItemName;
                ItemDescription += string.Empty + $"Looks like there are {remainingPills} pills left.";
                
                // Update Visible Scrap Name
                BaseScrap.gameObject.GetComponentInChildren<ScanNodeProperties>().headerText = ItemName;
            }

            // Reset modified state
            ItemModified = false;
        }

        public override void InspectItem()
        {
            if (!BaseScrap.itemUsedUp)
                HUDManager.Instance.DisplayTip($"{ItemName}", $"{ItemDescription}");
            else
                HUDManager.Instance.DisplayTip($"Empty {ItemName}", "Nothing in here...");
        }

        public override void UseItem()
        {
            if (HoldingPlayer != null && !BaseScrap.itemUsedUp && !InSpecialScenario && HoldingPlayer.health != 100 && !HoldingPlayer.inTerminalMenu)
            {
                InSpecialScenario = true;
                var soundName = "PillPop" + pillMultiplier + ".mp3";
                AudioHandler.PlaySound(HoldingPlayer, "Scrap\\PillBottle\\" + soundName);
                ItemsReworkedPlugin.mls?.LogInfo($"playing: {soundName}");
                HoldingPlayer.StartCoroutine(DelayedActivation(2f, () =>
                {
                    remainingPills = IngestPills(remainingPills);
                    BaseScrap.SetScrapValue(remainingPills);
                    if (remainingPills == 0)
                    {
                        BaseScrap.SetScrapValue(1);
                        BaseScrap.itemUsedUp = true;
                    }
                }));
            }
        }

        public override void SecondaryUseItem()
        {
            throw new System.NotImplementedException();
        }

        private int GetPillQuality()
        {
            // Initial pillMultiplier initialization
            if (pillMultiplier == -2)
            {
                System.Random random = new System.Random();
                pillMultiplier = random.Next(-1, 4);
            }

            switch (pillMultiplier)
            {
                default:
                    ItemQuality = "Basic";
                    if (ItemPropertiesDiscovered)
                        ItemDescription = " Makes the pain go away.";
                    return 1;
                case -1:
                    ItemQuality = "Bad";
                    if (ItemPropertiesDiscovered)
                        ItemDescription = " Probably not a good idea to take any...";
                    break;
                case 2:
                    ItemQuality = "Great";
                    if (ItemPropertiesDiscovered)
                        ItemDescription = " These are pretty strong ones.";
                    break;
                case 3:
                    ItemQuality = "Supreme";
                    if (ItemPropertiesDiscovered)
                        ItemDescription = " Big Pharma tried to keep these a secret!";
                    break;
            }

            return pillMultiplier;
        }

        private int CalculatePills(int scrapValue)
        {
            const int minScrapValue = 16;
            const int maxScrapValue = 40;
            int minPills = ItemsReworkedPlugin.MinPills.Value;
            int maxPills = ItemsReworkedPlugin.MaxPills.Value;

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

        private int IngestPills(int pills)
        {
            int maxHealth = 100;
            int surplus = 0;

            // Calculate healing ammount
            int dose = pills * pillMultiplier;

            if (HoldingPlayer.health + dose > maxHealth)
            {
                //TODO: OVERDOSING
                surplus = HoldingPlayer.health + dose - maxHealth / pillMultiplier;
                HoldingPlayer.StartCoroutine(GradualHealing(pills - surplus));
            }
            else
            {
                HoldingPlayer?.StartCoroutine(GradualHealing(pills));
            }

            return surplus;
        }

        private IEnumerator GradualHealing(int pills)
        {
            ItemsReworkedPlugin.mls?.LogInfo("CoroutineStarted");
            int targetHealth = HoldingPlayer.health + (pills * pillMultiplier);
            // Time needed until targetHealth is reached
            float healingDuration;
            switch (pillMultiplier)
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

            // Interval in which the HoldingPlayer is healed
            float healingInterval = healingDuration / (float)pills;

            float elapsedTime = 0f;
            float elapsedIntervalTime = 0f;

            while (elapsedTime < healingDuration)
            {
                // check if interval time passed
                if (elapsedIntervalTime > healingInterval)
                {
                    // Apply healing
                    HoldingPlayer.health += pillMultiplier;

                    // Update UI
                    HUDManager.Instance.UpdateHealthUI(HoldingPlayer.health, false);

                    //Reset Healing interval
                    elapsedIntervalTime = 0f;
                }

                // Increment elapsed time
                elapsedTime += Time.deltaTime;
                elapsedIntervalTime += Time.deltaTime;

                yield return null; // Wait for the next frame
            }

            // Ensure final health value is correct
            HoldingPlayer.health = targetHealth;

            // Update UI one last time
            HUDManager.Instance.UpdateHealthUI(HoldingPlayer.health, false);

            // Reset Special Scenario
            InSpecialScenario = false;
        }
    }
}
