#region usings
using GameNetcodeStuff;
using System.Collections;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class RedSodaCan : BaseItem
    {
        float effectDuration;

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {
            effectDuration = CalculateEffectDuration(item.scrapValue);
            ItemsReworkedPlugin.mls.LogInfo($"Effect duration: {effectDuration}");

            if (!item.itemUsedUp)
                HUDManager.Instance.DisplayTip("Some kind of energy drink", "What did they say in those advertisements when I was younger... something about a red bull... and wings? I dunno...");
            else
                HUDManager.Instance.DisplayTip("An empty energy drink", "Wow... the ads were not lying");

        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (!item.itemUsedUp)
            {
                effectDuration = CalculateEffectDuration(item.scrapValue);
                player.StartCoroutine(Energize(player));
                item.itemUsedUp = true;
                item.SetScrapValue(item.scrapValue / 2);
            }
        }

        private float CalculateEffectDuration(int scrapValue)
        {
            const int minValue = 19;
            const int maxValue = 90;
            const float minDuration = 60f; // CONFIG
            const float maxDuration = 180f;    // CONFIG

            // Calculate the percentage of scrapValue between minValue and maxValue
            float percentage = (float)(scrapValue - minValue) / (maxValue - minValue);

            // Use lerp to calculate the interpolated caffeine duration time
            float effectDuration = Mathf.Lerp(minDuration, maxDuration, percentage);

            return effectDuration;
        }

        private IEnumerator Energize(PlayerControllerB player)
        {
            float elapsedTime = 0f;
            float originalForce = player.jumpForce;
          
            while (elapsedTime < effectDuration)
            {
                player.jumpForce = originalForce * 1.5f;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ItemsReworkedPlugin.mls.LogInfo($"Effect worn off");
            player.jumpForce = originalForce;

        }
    }
}
