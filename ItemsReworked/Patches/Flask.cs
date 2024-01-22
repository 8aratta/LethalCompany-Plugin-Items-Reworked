using GameNetcodeStuff;

namespace ItemsReworked.Patches
{
    internal class Flask : BaseItem
    {
        public Flask()
        {
            ItemName = "Flask";
        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            ItemsReworkedPlugin.mls.LogWarning($"{ItemName} detected");

            if (!item.itemUsedUp)
            {
                // Logic Here
                item.itemUsedUp = true;
            }
        }
    }
}
