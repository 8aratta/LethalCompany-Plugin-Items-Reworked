#region usings
using GameNetcodeStuff;
#endregion

namespace ItemsReworked.Scrap
{
    internal class Candy : BaseItem
    {
        // Give to little girl to make her disappear

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {
            throw new System.NotImplementedException();
        }

        public override void SpecialUseItem(PlayerControllerB player, GrabbableObject item)
        {
            throw new System.NotImplementedException();
        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (!item.itemUsedUp && player.insanityLevel > 1f)
            {
                player.insanityLevel = 0f;
                item.itemUsedUp = true;
                player.currentlyHeldObject.DiscardItemOnClient();
            }
        }
    }
}
