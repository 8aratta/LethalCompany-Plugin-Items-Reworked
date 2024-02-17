#region usings
using ItemsReworked.Handlers;
using System.Collections;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class RedSodaCan : BaseScrapItem
    {
        float effectDuration;

        internal RedSodaCan(GrabbableObject redSodaCan) : base(redSodaCan)
        {
            ItemDescription = "BokeCola – Elevate your jump game";
            effectDuration = CalculateEffectDuration(redSodaCan.scrapValue);
        }

        public override void UpdateItem()
        {
            if (ItemPropertiesDiscovered)
                ItemDescription += $" for {effectDuration} seconds!";

            // Reset modified state
            ItemModified = false;
        }

        public override void InspectItem()
        {
            if (!BaseScrap.itemUsedUp)
                HUDManager.Instance.DisplayTip($"{ItemName}", $"{ItemDescription}!");
            else
                HUDManager.Instance.DisplayTip($"Empty {ItemName}", "Wow... the ads were not lying");

        }

        public override void SecondaryUseItem()
        {
            throw new System.NotImplementedException();
        }

        public override void UseItem()
        {
            if (HoldingPlayer != null && !BaseScrap.itemUsedUp)
            {
                BaseScrap.itemUsedUp = true;
                System.Random random = new System.Random();
                var soundName = "Soda" + random.Next(1, 3) + ".mp3";
                AudioHandler.PlaySound(HoldingPlayer, "Scrap\\RedSodaCan\\" + soundName);
                ItemsReworkedPlugin.mls?.LogInfo($"playing: {soundName}");
                HoldingPlayer.StartCoroutine(DelayedActivation(3f, () =>
                {
                    HoldingPlayer.StartCoroutine(Energize());
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
            float originalForce = HoldingPlayer.jumpForce;

            while (elapsedTime < effectDuration)
            {
                HoldingPlayer.jumpForce = originalForce * 1.5f;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            ItemsReworkedPlugin.mls?.LogInfo($"Effect worn off");
            ItemsReworkedPlugin.mls?.LogInfo($"Original force: {originalForce}");
            HoldingPlayer.jumpForce = originalForce;
        }
    }
}
