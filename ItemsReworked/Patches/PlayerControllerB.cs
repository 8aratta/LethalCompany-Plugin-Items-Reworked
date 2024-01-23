﻿using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine.InputSystem;

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

            // Register the scrap item in the ScrapHandler
            pluginInstance.scrapHandler.RegisterScrapItem(scrapItem);

            ItemsReworkedPlugin.mls.LogInfo($"{scrapItem.name} picked up.");
        }

        [HarmonyPatch("ActivateItem_performed")]
        [HarmonyPrefix]
        private static void UseScrapItem(PlayerControllerB __instance, InputAction.CallbackContext context, ref GrabbableObject ___currentlyHeldObjectServer)
        {
            if (___currentlyHeldObjectServer != null)
            {
                // Use the scrap item through the ScrapHandler
                pluginInstance.scrapHandler.UseScrapItem(___currentlyHeldObjectServer, __instance);

                ItemsReworkedPlugin.mls.LogInfo($"{___currentlyHeldObjectServer.name} used.");
            }
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