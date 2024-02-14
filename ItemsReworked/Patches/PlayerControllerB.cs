#region usings
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine.InputSystem;
#endregion

namespace ItemsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatches
    {
        private static ItemsReworkedPlugin pluginInstance => ItemsReworkedPlugin.Instance;

        [HarmonyPatch("GrabObjectServerRpc")]
        [HarmonyPostfix]
        private static void PickUpScrapItem(NetworkObjectReference grabbedObject)
        {
            grabbedObject.TryGet(out var networkObject);
            var scrapItem = networkObject.gameObject.GetComponentInChildren<GrabbableObject>();
            pluginInstance.scrapHandler.RegisterScrapItem(scrapItem);
            ItemsReworkedPlugin.mls.LogInfo($"{scrapItem.name} picked up.");
        }

        [HarmonyPatch("ActivateItem_performed")]
        [HarmonyPrefix]
        private static void UseScrapItem(PlayerControllerB __instance, InputAction.CallbackContext context, ref GrabbableObject ___currentlyHeldObjectServer)
        {
            if (___currentlyHeldObjectServer != null)
                pluginInstance.scrapHandler.UseScrapItem(___currentlyHeldObjectServer, __instance);
        }

        [HarmonyPatch("InspectItem_performed")]
        [HarmonyPrefix]
        private static void InspectItem_performed(PlayerControllerB __instance, InputAction.CallbackContext context, ref GrabbableObject ___currentlyHeldObjectServer)
        {
            if (___currentlyHeldObjectServer != null)
                pluginInstance.scrapHandler.InspectScrapItem(___currentlyHeldObjectServer, __instance);
        }

        [HarmonyPatch("ItemSecondaryUse_performed")]
        [HarmonyPrefix]
        private static void ItemSecondaryUse_performed(PlayerControllerB __instance, InputAction.CallbackContext context, ref GrabbableObject ___currentlyHeldObjectServer)
        {
            ItemsReworkedPlugin.mls.LogInfo($"Player '{__instance.name}' with id:{__instance.actualClientId} secondary use");

            if (___currentlyHeldObjectServer != null)
                pluginInstance.scrapHandler.SpecialUse(___currentlyHeldObjectServer, __instance);
        }

        //[HarmonyPatch("ItemSold")]
        //[HarmonyPrefix]
        //private static void SellScrapItem(GrabbableObject scrapItemSold)
        //{
        //    // Remove the scrap item from the ScrapHandler when sold
        //    pluginInstance.scrapHandler.RemoveScrapItem(scrapItemSold);

        //    ItemsReworkedPlugin.mls.LogInfo($"{scrapItemSold.name} sold.");
        //}
    }
}
