#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
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

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {
            ItemsReworkedPlugin.mls.LogInfo($"Remaining uses: {uses}.");
        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (uses > 0)
            {
                uses--;
                ActivateRemote();

                player.StartCoroutine(DelayedActivation(player, item, 1.5f, () =>
                {
                    RemoteMalfunction(player, item);
                }));
            }
        }

        private int CalculateUses(int scrapValue)
        {
            const int minScrapValue = 20;
            const int maxScrapValue = 48;
            int minUses = ItemsReworkedPlugin.MinRemoteUses.Value;
            int maxUses = ItemsReworkedPlugin.MaxRemoteUses.Value;

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

        private static bool ActivateRemote()
        {
            var localPlayer = StartOfRound.Instance.localPlayerController;
            var playersFace = localPlayer.gameplayCamera.transform.position;
            var facingDirection = localPlayer.gameplayCamera.transform.forward;

            float remoteRange = 20f;

            var ray = Physics.RaycastAll(playersFace, facingDirection, remoteRange);

            foreach (var hit in ray)
            {
                Landmine landmine = hit.collider.GetComponent<Landmine>();
                Turret turret = hit.collider.GetComponent<Turret>();

                if (turret != null & ItemsReworkedPlugin.ToggleTurrets.Value)
                {
                    ItemsReworkedPlugin.mls.LogInfo("Toggling Turret");
                    turret.ToggleTurretEnabled(!turret.enabled);
                    return true;
                }
                if (landmine != null && ItemsReworkedPlugin.DetonateMines.Value)
                {
                    ItemsReworkedPlugin.mls.LogInfo("HIT MINE");
                    landmine.ExplodeMineServerRpc();
                    return true;
                }
            }
            return false;
        }

        private static void RemoteMalfunction(PlayerControllerB player, GrabbableObject remote)
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, 101);

            // X% Probability to be electrocuted to death
            if (randomNumber <= ItemsReworkedPlugin.RemoteExplosionProbability.Value)
            {
                uses = 0;
                Landmine.SpawnExplosion(remote.transform.position, true);

                if (remote.heldByPlayerOnServer)
                {
                    Vector3 bodyVelocity = (player.gameplayCamera.transform.position + remote.transform.position) / 2f;
                    remote.playerHeldBy.KillPlayer(bodyVelocity, spawnBody: true, CauseOfDeath.Blast);
                    remote.DestroyObjectInHand(remote.playerHeldBy);
                    ItemsReworkedPlugin.Instance.scrapHandler.RemoveScrapItem(remote);
                    ItemsReworkedPlugin.mls.LogInfo($"Remote exploded in the hand of the player '{player.name}'");
                }

                remote.SetScrapValue(1);
            }
            // X% Probability for remote to fry itself
            else if (randomNumber <= ItemsReworkedPlugin.RemoteZapProbability.Value)
            {
                if (remote.heldByPlayerOnServer)
                {
                    AudioHandler.PlaySound(player, "Scrap\\Remote\\Zap.mp3");
                    remote.playerHeldBy.DamagePlayer(10, true, causeOfDeath: CauseOfDeath.Electrocution);
                    ItemsReworkedPlugin.mls.LogInfo($"Remote zapped player '{player.name}'");
                }

                uses = 0;
                remote.SetScrapValue(1);
            }
        }

        public override void SpecialUseItem(PlayerControllerB player, GrabbableObject item)
        {
            throw new System.NotImplementedException();
        }
    }
}
