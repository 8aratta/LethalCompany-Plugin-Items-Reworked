using GameNetcodeStuff;

namespace ItemsReworked.Patches
{
    public abstract class BaseItem
    {
        public string ItemName { get; protected set; }

        public abstract void UseItem(PlayerControllerB player, GrabbableObject item);
    }
}
