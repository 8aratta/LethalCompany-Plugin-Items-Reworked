#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
using System.Collections;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class RedSodaCan : BaseItem
    {
        float effectDuration;

        internal RedSodaCan(GrabbableObject redSodaCan)
        {
            effectDuration = CalculateEffectDuration(redSodaCan.scrapValue);
        }

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {
            effectDuration = CalculateEffectDuration(item.scrapValue);
            ItemsReworkedPlugin.mls.LogInfo($"Effect duration: {effectDuration}");

            if (!item.itemUsedUp)
                HUDManager.Instance.DisplayTip("Some kind of energy drink", "What did they say in those advertisements when I was younger... something about a red bull... and wings? I dunno...");
            else
                HUDManager.Instance.DisplayTip("An empty energy drink", "Wow... the ads were not lying");

        }

        public override void SpecialUseItem(PlayerControllerB player, GrabbableObject item)
        {
            throw new System.NotImplementedException();
        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (!item.itemUsedUp)
            {
                item.itemUsedUp = true;
                System.Random random = new System.Random();
                var soundName = "Soda" + random.Next(1, 3) + ".mp3";
                AudioHandler.PlaySound(player, "Scrap\\RedSodaCan\\" + soundName);
                ItemsReworkedPlugin.mls.LogInfo($"playing: {soundName}");
                player.StartCoroutine(DelayedActivation(player, item, 3f, () =>
                {
                    player.StartCoroutine(Energize(player));
                    item.SetScrapValue(item.scrapValue / 2);
                }));
            }
        }

        private float CalculateEffectDuration(int scrapValue)
        {
            const int minValue = 19;
            const int maxValue = 90;
            const float minDuration = 5f; // CONFIG
            const float maxDuration = 60f;    // CONFIG

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
            ItemsReworkedPlugin.mls.LogInfo($"Original force: {originalForce}");
            player.jumpForce = originalForce;
        }
    }
}
