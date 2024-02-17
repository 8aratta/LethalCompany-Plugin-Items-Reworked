#region usings
using ItemsReworked.Handlers;
using System;
using System.Collections;
using UnityEngine;
#endregion
namespace ItemsReworked.Scrap
{
    /// <summary>
    /// TODO: Change inf stamina to actual boost
    /// Add balancing --> Stamina depletion/diarreha
    /// </summary>
    internal class Mug : BaseScrapItem
    {
        float effectDuration;

        internal Mug(GrabbableObject mug) : base(mug)
        {
            effectDuration = CalculateEffectDuration();
            ItemDescription = "A warm cup of coffee... wait why is still warm?";
        }

        public override void UpdateItem()
        {
            if (ItemPropertiesDiscovered)
               ItemDescription = $"A warm cup of coffee... Boosts stamina for {effectDuration} seconds!";

            // Reset modified state
            ItemModified = false;
        }

        public override void InspectItem()
        {
            if (!BaseScrap.itemUsedUp)
                HUDManager.Instance.DisplayTip($"{ItemName}", $"{ItemDescription}");
            else
                HUDManager.Instance.DisplayTip($"{ItemName}", "... I need to buy an espresso machine");
        }

        public override void UseItem()
        {
            if (HoldingPlayer != null && !BaseScrap.itemUsedUp && !InSpecialScenario)
            {
                InSpecialScenario = true;
                var soundName = "Mug.mp3";
                AudioHandler.PlaySound(HoldingPlayer, "Scrap\\Mug\\" + soundName);
                ItemsReworkedPlugin.mls?.LogInfo($"playing: {soundName}");
                HoldingPlayer.StartCoroutine(DelayedActivation(1.5f, () =>
                {
                    HoldingPlayer.StartCoroutine(Caffeinated());
                    BaseScrap.itemUsedUp = true;
                    BaseScrap.SetScrapValue(BaseScrap.scrapValue / 2);
                }));
            }
        }

        public override void SecondaryUseItem()
        {
            throw new NotImplementedException();
        }


        private float CalculateEffectDuration()
        {
            const int minValue = 24;
            const int maxValue = 68;
            float minDuration = ItemsReworkedPlugin.MinDurationStaminaBoost.Value;
            float maxDuration = ItemsReworkedPlugin.MaxDurationStaminaBoost.Value;

            // Calculate the percentage of scrapValue between minValue and maxValue
            float percentage = (float)(BaseScrap.scrapValue - minValue) / (maxValue - minValue);

            // Use lerp to calculate the interpolated caffeine duration time
            float effectDuration = Mathf.Lerp(minDuration, maxDuration, percentage);

            return effectDuration;
        }

        private IEnumerator Caffeinated()
        {
            float elapsedTime = 0f;

            while (elapsedTime < effectDuration)
            {
                HoldingPlayer.sprintMeter = 1f;
                HoldingPlayer.isSprinting = true;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            InSpecialScenario = false;
            ItemsReworkedPlugin.mls?.LogInfo($"Effect has worn off");
        }


    }
}
