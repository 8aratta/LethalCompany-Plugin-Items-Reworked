using GameNetcodeStuff;
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
        private static void GrabObject(NetworkObjectReference grabbedObject)
        {
            grabbedObject.TryGet(out var networkObject);
            var Item = networkObject.gameObject.GetComponentInChildren<GrabbableObject>();
            if (!pluginInstance.ItemList.Contains(Item))
                pluginInstance.ItemList.Add(Item);
            ItemsReworkedPlugin.mls.LogWarning($"Item: {Item.name} Id: {Item.GetInstanceID()} added to item list");

            foreach (var entry in pluginInstance.ItemList)
            {
                ItemsReworkedPlugin.mls.LogWarning($"Entry: Item: {Item.name} Id: {Item.GetInstanceID()}");
            }

        }

        [HarmonyPatch("ActivateItem_performed")]
        [HarmonyPrefix]
        private static void UseScrapItem(PlayerControllerB __instance, InputAction.CallbackContext context, ref GrabbableObject ___currentlyHeldObjectServer)
        {
            ItemsReworkedPlugin.mls.LogWarning("Activate Performed");
            if (___currentlyHeldObjectServer != null)
            {
                ItemsReworkedPlugin.mls.LogWarning($"Holding item: {___currentlyHeldObjectServer.name}");

                switch (___currentlyHeldObjectServer.name.Replace("(Clone)", null))
                {
                    default:
                        break;
                    case "PillBottle":
                        BaseItem pillBottle = new PillBottle();
                        pillBottle.UseItem(__instance, ___currentlyHeldObjectServer);
                        break;
                    case "Flask":
                        BaseItem flask = new Flask();
                        flask.UseItem(__instance, ___currentlyHeldObjectServer);
                        break;
                    case "Mug":
                        break;

                }
            }

        }

        //My Methods
        private static void HealPlayer(PlayerControllerB player, int hp)
        {
            player.health += hp;
            HUDManager.Instance.UpdateHealthUI(player.health);
        }
    }
}
