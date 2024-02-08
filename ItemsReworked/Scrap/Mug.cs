#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
using System;
using System.Collections;
using UnityEngine;
#endregion
namespace ItemsReworked.Scrap
{
    namespace ItemsReworked.Scrap
    {
        internal class Mug : BaseItem
        {
            float effectDuration;

            internal Mug(GrabbableObject item)
            {
                effectDuration = CalculateEffectDuration(item.scrapValue);
            }

            public override void InspectItem(PlayerControllerB player, GrabbableObject item)
            {
                ItemsReworkedPlugin.mls.LogInfo($"Caffeine effect duration: {effectDuration}");

                if (!item.itemUsedUp)
                    HUDManager.Instance.DisplayTip("A cup of coffee", "I wonder who left it here... it's still warm.");
                else
                    HUDManager.Instance.DisplayTip("A cup of coffee", "WHO DRANK IT?");

            }

            public override void UseItem(PlayerControllerB player, GrabbableObject item)
            {
                if (!item.itemUsedUp && !inSpecialScenario)
                {
                    inSpecialScenario = true;
                    var soundName = "Mug.mp3";
                    AudioHandler.PlaySound(player, "Scrap\\Mug\\" + soundName);
                    ItemsReworkedPlugin.mls.LogInfo($"playing: {soundName}");
                    player.StartCoroutine(DelayedActivation(player, item, 1.5f, () =>
                    {
                        player.StartCoroutine(Caffeinated(player));
                        item.itemUsedUp = true;
                        item.SetScrapValue(item.scrapValue / 2);
                    }));
                }
            }

            private float CalculateEffectDuration(int scrapValue)
            {
                const int minValue = 24;
                const int maxValue = 68;
                const float minDuration = 10f; // CONFIG
                const float maxDuration = 30f;    // CONFIG

                // Calculate the percentage of scrapValue between minValue and maxValue
                float percentage = (float)(scrapValue - minValue) / (maxValue - minValue);

                // Use lerp to calculate the interpolated caffeine duration time
                float effectDuration = Mathf.Lerp(minDuration, maxDuration, percentage);

                return effectDuration;
            }

            private IEnumerator Caffeinated(PlayerControllerB player)
            {
                float elapsedTime = 0f;

                while (elapsedTime < effectDuration)
                {
                    player.sprintMeter = 1f;
                    player.isSprinting = true;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                inSpecialScenario = false;
                ItemsReworkedPlugin.mls.LogInfo($"Effect has worn off");
            }
        }
    }
}
