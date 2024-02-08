using GameNetcodeStuff;
using System.Collections;
using UnityEngine;

namespace ItemsReworked.Scrap
{
    public abstract class BaseItem
    {
        public bool inSpecialScenario = false;
        public bool hasSpecialUse = false;
        public abstract void UseItem(PlayerControllerB player, GrabbableObject item);
        public abstract void SpecialUseItem(PlayerControllerB player, GrabbableObject item);

        public abstract void InspectItem(PlayerControllerB player, GrabbableObject item);

        protected IEnumerator DelayedActivation(PlayerControllerB player, GrabbableObject item, float delayInSeconds, System.Action activationAction)
        {
            yield return new WaitForSeconds(delayInSeconds);
            activationAction.Invoke();
        }
    }
}
