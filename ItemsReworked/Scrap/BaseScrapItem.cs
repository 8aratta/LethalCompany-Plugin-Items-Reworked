using GameNetcodeStuff;
using System.Collections;
using UnityEngine;

namespace ItemsReworked.Scrap
{
    public abstract class BaseScrapItem
    {
#nullable enable
        public static PlayerControllerB? LocalPlayer => HUDManager.Instance != null ? HUDManager.Instance.localPlayer : null;
#nullable disable

        public GrabbableObject BaseScrap;

        public bool hasSecondaryUse = false;
        public bool wasChanged = false;
        public bool inSecondaryMode = false;

        protected BaseScrapItem(GrabbableObject baseScrap)
        {
            this.BaseScrap = baseScrap;
        }

        public abstract void UseItem();
        public abstract void SpecialUseItem();
        public abstract void InspectItem();

        protected IEnumerator DelayedActivation(float delayInSeconds, System.Action activationAction)
        {
            yield return new WaitForSeconds(delayInSeconds);
            activationAction.Invoke();
        }
    }
}
