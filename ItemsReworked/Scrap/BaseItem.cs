using GameNetcodeStuff;

namespace ItemsReworked.Scrap
{
    public abstract class BaseItem
    {
        public abstract void UseItem(PlayerControllerB player, GrabbableObject item);
    }
}
