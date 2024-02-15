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

        }

        public override void InspectItem()
        {
            throw new System.NotImplementedException();
        }

        public override void SpecialUseItem()
        {
            throw new System.NotImplementedException();
        }

        public override void UseItem()
        {
            if (LocalPlayer != null && !BaseScrap.itemUsedUp && LocalPlayer.insanityLevel > 1f)
            {
                LocalPlayer.insanityLevel = 0f;
                BaseScrap.itemUsedUp = true;
                LocalPlayer.currentlyHeldObject.DiscardItemOnClient();
            }
        }
    }
}
