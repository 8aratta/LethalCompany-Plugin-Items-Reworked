using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace ItemsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatches
    {
        [HarmonyPatch("ActivateItem_performed")]
        [HarmonyPrefix]
        private static void UseScrapItem(PlayerControllerB __instance, InputAction.CallbackContext context)
        {
            ItemsReworkedPlugin.mls.LogWarning("Activate Performed");

            var usedItem = __instance.currentlyHeldObjectServer.gameObject.GetComponent<GrabbableObject>();
            ItemsReworkedPlugin.mls.LogWarning($"Holding item: {usedItem.name}");

            switch (usedItem.name.Replace("(Clone)",null))
            {
                default:
                    break;
                case "PillBottle":
                    ItemsReworkedPlugin.mls.LogWarning("PillBottle detected");
                   
                    new PillBottlePatches().ItemActivate(usedItem.scrapValue);
                    usedItem.scrapValue = 1;
                    break;
            }
        }
    }
}
