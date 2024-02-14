#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
using System.Collections;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class RedSodaCan : BaseScrapItem
    {
        float effectDuration;

        internal RedSodaCan(GrabbableObject redSodaCan):base(redSodaCan)
        {
            effectDuration = CalculateEffectDuration(redSodaCan.scrapValue);
        }

        public override void InspectItem()
        {
            effectDuration = CalculateEffectDuration(BaseScrap.scrapValue);
            ItemsReworkedPlugin.mls.LogInfo($"Effect duration: {effectDuration}");

            if (!BaseScrap.itemUsedUp)
                HUDManager.Instance.DisplayTip("Some kind of energy drink", "What did they say in those advertisements when I was younger... something about a red bull... and wings? I dunno...");
            else
                HUDManager.Instance.DisplayTip("An empty energy drink", "Wow... the ads were not lying");

        }

        public override void SpecialUseItem()
        {
            throw new System.NotImplementedException();
        }

        public override void UseItem()
        {
            if (!BaseScrap.itemUsedUp)
            {
                BaseScrap.itemUsedUp = true;
                System.Random random = new System.Random();
                var soundName = "Soda" + random.Next(1, 3) + ".mp3";
                AudioHandler.PlaySound(LocalPlayer, "Scrap\\RedSodaCan\\" + soundName);
                ItemsReworkedPlugin.mls.LogInfo($"playing: {soundName}");
                LocalPlayer.StartCoroutine(DelayedActivation(3f, () =>
                {
                    LocalPlayer.StartCoroutine(Energize());
                    BaseScrap.SetScrapValue(BaseScrap.scrapValue / 2);
                }));
            }
        }

        private float CalculateEffectDuration(int scrapValue)
        {
            const int minValue = 19;
            const int maxValue = 90;
            float minDuration = ItemsReworkedPlugin.MinDurationJumpBoost.Value;
            float maxDuration = ItemsReworkedPlugin.MaxDurationJumpBoost.Value;

            // Calculate the percentage of scrapValue between minValue and maxValue
            float percentage = (float)(scrapValue - minValue) / (maxValue - minValue);

            // Use lerp to calculate the interpolated caffeine duration time
            float effectDuration = Mathf.Lerp(minDuration, maxDuration, percentage);

            return effectDuration;
        }

        private IEnumerator Energize()
        {
            float elapsedTime = 0f;
            float originalForce = LocalPlayer.jumpForce;

            while (elapsedTime < effectDuration)
            {
                LocalPlayer.jumpForce = originalForce * 1.5f;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ItemsReworkedPlugin.mls.LogInfo($"Effect worn off");
            ItemsReworkedPlugin.mls.LogInfo($"Original force: {originalForce}");
            LocalPlayer.jumpForce = originalForce;
        }
    }
}
