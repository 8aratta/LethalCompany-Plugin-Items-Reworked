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
        }

        public override void InspectItem()
        {
            ItemsReworkedPlugin.mls.LogInfo($"Caffeine effect duration: {effectDuration}");

            if (!BaseScrap.itemUsedUp)
                HUDManager.Instance.DisplayTip("A cup of coffee", "I wonder who left it here... it's still warm.");
            else
                HUDManager.Instance.DisplayTip("A cup of coffee", "WHO DRANK IT?");

        }

        public override void UseItem()
        {
            if (!BaseScrap.itemUsedUp && !inSecondaryMode)
            {
                inSecondaryMode = true;
                var soundName = "Mug.mp3";
                AudioHandler.PlaySound(LocalPlayer, "Scrap\\Mug\\" + soundName);
                ItemsReworkedPlugin.mls.LogInfo($"playing: {soundName}");
                LocalPlayer.StartCoroutine(DelayedActivation(1.5f, () =>
                {
                    LocalPlayer.StartCoroutine(Caffeinated());
                    BaseScrap.itemUsedUp = true;
                    BaseScrap.SetScrapValue(BaseScrap.scrapValue / 2);
                }));
            }
        }

        public override void SpecialUseItem()
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
                LocalPlayer.sprintMeter = 1f;
                LocalPlayer.isSprinting = true;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            inSecondaryMode = false;
            ItemsReworkedPlugin.mls.LogInfo($"Effect has worn off");
        }


    }
}
