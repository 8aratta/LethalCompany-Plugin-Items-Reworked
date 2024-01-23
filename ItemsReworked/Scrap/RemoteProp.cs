#region usings
using HarmonyLib;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    [HarmonyPatch(typeof(RemoteProp))]
    internal class RemotePropPatches
    {
        private static int uses = 100; //MAKE CONFIG

        [HarmonyPatch("ItemActivate")]
        [HarmonyPostfix]
        private static void ItemActivate(RemoteProp __instance, bool used, bool buttonDown = true)
        {
            if (uses > 0)
            {
                uses--;
                ItemsReworkedPlugin.mls.LogInfo($"Remaining uses: {uses}.");
                TriggerLandMines();
                ExplodeRemote(__instance);
            }
        }

        private static bool TriggerLandMines()
        {
            var localPlayer = StartOfRound.Instance.localPlayerController;
            var playersFace = localPlayer.gameplayCamera.transform.position;
            var facingDirection = localPlayer.gameplayCamera.transform.forward;

            // Maximum distance for the infection spreadDistance
            float spreadDistance = 15f;

            // Casting a ray in looking direction
            var ray = Physics.RaycastAll(playersFace, facingDirection, spreadDistance);

            foreach (var hit in ray)
            {
                Landmine Landmine = hit.collider.GetComponent<Landmine>();
                Turret turret = hit.collider.GetComponent<Turret>();

                //Avoid detecting self
                if (Landmine != null)
                {
                    ItemsReworkedPlugin.mls.LogInfo("HIT MINE");
                    Landmine.ExplodeMineServerRpc();
                    return true;
                }

                if (turret != null)
                    ItemsReworkedPlugin.mls.LogInfo("HIT TURRET");

            }
            return false;
        }

        private static void ExplodeRemote(RemoteProp remote)
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, 101);
            if (randomNumber <= 1) // MAKE CONFIG
            {
                // ADD SFX
                //uses = 0;
                remote.playerHeldBy.DamagePlayer(10);
                ItemsReworkedPlugin.mls.LogInfo("Remote defective");
            }
        }
    }
}
