#region usings
using GameNetcodeStuff;
#endregion

namespace ItemsReworked.Scrap
{
    internal class Candy : BaseScrapItem
    {
        // Give to little girl to make her disappear
        internal Candy(GrabbableObject candy): base(candy)
        {
            ItemDescription = "A sweet little treat!";
        }
        public override void UpdateItem()
        {
            if (ItemPropertiesDiscovered)
                ItemDescription += string.Empty + $"Give it to the little girl if you're being haunted.";
            
            // Reset modified state
            ItemModified = false;
        }

        public override void InspectItem()
        {
            HUDManager.Instance.DisplayTip($"{ItemName}", $"{ItemDescription}");
        }

        public override void SecondaryUseItem()
        {
            throw new System.NotImplementedException();
        }

        public override void UseItem()
        {
            if (!BaseScrap.itemUsedUp && HoldingPlayer.insanityLevel > 1f)
            {
                HoldingPlayer.insanityLevel = 0f;
                BaseScrap.itemUsedUp = true;
                BaseScrap.DiscardItemOnClient();
            }
        }
    }
}
