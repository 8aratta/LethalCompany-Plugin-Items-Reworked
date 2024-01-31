#region usings
using GameNetcodeStuff;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class Remote : BaseItem
    {
        private static int uses;

        internal Remote(GrabbableObject remote)
        {
            uses = CalculateUses(remote.scrapValue);
        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (uses > 0)
            {
                uses--;
                ItemsReworkedPlugin.mls.LogInfo($"Remaining uses: {uses}.");
                TriggerLandMines();
                RemoteMalfunction(player, item);
            }
        }
        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {
            ItemsReworkedPlugin.mls.LogInfo($"Remaining uses: {uses}.");
        }

        private int CalculateUses(int scrapValue)
        {
            const int minScrapValue = 20;
            const int maxScrapValue = 48;
            const int minUses = 1; //CONFIG
            const int maxUses = 10; //CONFIG

            if (scrapValue <= minScrapValue)
            {
                return minUses;
            }
            else if (scrapValue >= maxScrapValue)
            {
                return maxUses;
            }
            else
            {
                float percentage = (float)(scrapValue - minScrapValue) / (maxScrapValue - minScrapValue);
                int uses = Mathf.RoundToInt(Mathf.Lerp(minUses, maxUses, percentage));
                if (uses == 0)
                    ItemsReworkedPlugin.mls.LogError("Calculation error in remote uses");
                return uses;
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
                if (turret != null)
                {
                    ItemsReworkedPlugin.mls.LogInfo("HIT TURRET");
                    turret.enabled = false;
                    return true;
                }
                if (Landmine != null)
                {
                    ItemsReworkedPlugin.mls.LogInfo("HIT MINE");
                    Landmine.ExplodeMineServerRpc();
                    return true;
                }
            }
            return false;
        }

        private static void RemoteMalfunction(PlayerControllerB player, GrabbableObject remote)
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, 101);

            //10% Chance to be electrocuted to death //CONFIG
            if (randomNumber <= 10)
            {
                uses = 0;
                Landmine.SpawnExplosion(remote.transform.position, true);
                Vector3 bodyVelocity = (player.gameplayCamera.transform.position + remote.transform.position) / 2f;
                remote.playerHeldBy.KillPlayer(bodyVelocity, spawnBody: true, CauseOfDeath.Blast);
                remote.DestroyObjectInHand(remote.playerHeldBy);
                ItemsReworkedPlugin.mls.LogInfo($"Remote exploded in the hand of the player '{player.name}'");
                remote.SetScrapValue(1);
            }
            //15% Chance for remote to fry itself
            else if (randomNumber <= 15)
            {
                remote.playerHeldBy.DamagePlayer(10, true, causeOfDeath: CauseOfDeath.Electrocution);
                uses = 0;
                ItemsReworkedPlugin.mls.LogInfo($"Remote zapped player '{player.name}'");
                remote.SetScrapValue(1);
            }
        }
    }
}
