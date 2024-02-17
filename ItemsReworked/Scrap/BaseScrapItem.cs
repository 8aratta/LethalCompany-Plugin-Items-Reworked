#region usisng
using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    public abstract class BaseScrapItem
    {
        public PlayerControllerB HoldingPlayer;
        public GrabbableObject BaseScrap;

        public string ItemName;
        public string ItemDescription = string.Empty;
        public string ItemQuality = string.Empty;
        public bool ItemModified = false;
        public bool ItemPropertiesDiscovered = false;
        public bool HasSecondaryUse = false;
        public bool InSpecialScenario = false;

        protected BaseScrapItem(GrabbableObject baseScrap)
        {
            ItemName = baseScrap.gameObject.GetComponentInChildren<ScanNodeProperties>().headerText;
            HoldingPlayer = baseScrap.playerHeldBy;
            BaseScrap = baseScrap;
        }

        public abstract void UpdateItem();
        public abstract void InspectItem();
        public abstract void UseItem();
        public abstract void SecondaryUseItem();

        protected IEnumerator DelayedActivation(float delayInSeconds, System.Action activationAction)
        {
            yield return new WaitForSeconds(delayInSeconds);
            activationAction.Invoke();
        }
    }
}
