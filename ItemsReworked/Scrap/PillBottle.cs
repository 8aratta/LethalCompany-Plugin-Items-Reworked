#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
using System.Collections;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class PillBottle : BaseScrapItem
    {
        private int remainingPills;
        private int pillQuality;

        internal PillBottle(GrabbableObject pillBottle) : base(pillBottle)
        {
            pillQuality = GetPillQuality();
            remainingPills = CalculatePills(BaseScrap.scrapValue);
        }

        public override void InspectItem()
        {
            if (BaseScrap.itemUsedUp)
            {
                HUDManager.Instance.DisplayTip("An empty pill bottle", "Seems like there are no pills left in this one...");
            }
            else
            {
                HUDManager.Instance.DisplayTip("A pill bottle", "There are some pills in here...");
            }
        }

        public override void UseItem()
        {
            if (LocalPlayer != null && !BaseScrap.itemUsedUp && !inSecondaryMode && LocalPlayer.health != 100 && !LocalPlayer.inTerminalMenu)
            {
                inSecondaryMode = true;
                var soundName = "PillPop" + pillQuality + ".mp3";
                AudioHandler.PlaySound(LocalPlayer, "Scrap\\PillBottle\\" + soundName);
                ItemsReworkedPlugin.mls?.LogInfo($"playing: {soundName}");
                LocalPlayer.StartCoroutine(DelayedActivation(2f, () =>
                {
                    remainingPills = IngestPills( remainingPills);
                    BaseScrap.SetScrapValue(remainingPills);
                    if (remainingPills == 0)
                    {
                        BaseScrap.SetScrapValue(1);
                        BaseScrap.itemUsedUp = true;
                    }
                }));
            }
        }

        public override void SpecialUseItem()
        {
            throw new System.NotImplementedException();
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
            int dose = pills * pillQuality;

            if (LocalPlayer.health + dose > maxHealth)
            {
                //OVERDOSING -- FEATURE TO IMPLEMENT

                surplus = LocalPlayer.health + dose - maxHealth / pillQuality;
                LocalPlayer.StartCoroutine(GradualHealing( pills - surplus));
            }
            else
            {
                LocalPlayer?.StartCoroutine(GradualHealing(pills));
            }

            return surplus;
        }

        private IEnumerator GradualHealing(int pills)
        {
            ItemsReworkedPlugin.mls?.LogInfo("CoroutineStarted");
            int targetHealth = LocalPlayer.health + (pills * pillQuality);
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

            // Interval in which the LocalPlayer is healed
            float healingInterval = healingDuration / (float)pills;

            float elapsedTime = 0f;
            float elapsedIntervalTime = 0f;

            while (elapsedTime < healingDuration)
            {
                // check if interval time passed
                if (elapsedIntervalTime > healingInterval)
                {
                    // Apply healing
                    LocalPlayer.health += pillQuality;

                    // Update UI
                    HUDManager.Instance.UpdateHealthUI(LocalPlayer.health, false);

                    //Reset Healing interval
                    elapsedIntervalTime = 0f;
                }

                // Increment elapsed time
                elapsedTime += Time.deltaTime;
                elapsedIntervalTime += Time.deltaTime;

                yield return null; // Wait for the next frame
            }

            // Ensure final health value is correct
            LocalPlayer.health = targetHealth;

            // Update UI one last time
            HUDManager.Instance.UpdateHealthUI(LocalPlayer.health, false);

            // Reset Special Scenario
            inSecondaryMode = false;
        }
    }
}
