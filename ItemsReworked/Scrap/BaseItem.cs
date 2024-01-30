using GameNetcodeStuff;
using UnityEngine;

namespace ItemsReworked.Scrap
{
    public abstract class BaseItem
    {
        public abstract void UseItem(PlayerControllerB player, GrabbableObject item);

        public abstract void InspectItem(PlayerControllerB player, GrabbableObject item);
    }
}
