using HarmonyLib;
using UnityEngine;

namespace ItemsReworked.Patches
{
    [HarmonyPatch(typeof(RemoteProp))]
    internal class RemotePropPatches
    {
        [HarmonyPatch("ItemActivate")]
        [HarmonyPostfix]
        private static void UseRemote(RemoteProp __instance, bool used, bool buttonDown = true)
        {
            TriggerLandMines(__instance);
        }

        private static bool TriggerLandMines(RemoteProp remote)
        {
            var localPlayer = StartOfRound.Instance.localPlayerController;
            var playersFace = localPlayer.gameplayCamera.transform.position;
            var facingDirection = localPlayer.gameplayCamera.transform.forward;

            // Maximum distance for the infection spreadDistance
            float spreadDistance = 10f;

            // Casting a ray in looking direction
            var ray = Physics.RaycastAll(playersFace, facingDirection, spreadDistance, 1, QueryTriggerInteraction.Ignore);

            foreach (var hit in ray)
            {
                Landmine Landmine = hit.collider.GetComponent<Landmine>();

                //Avoid detecting self
                if (Landmine != null)
                {
                    Landmine.Detonate();
                }
            }

            return true;
        }
    }
}
