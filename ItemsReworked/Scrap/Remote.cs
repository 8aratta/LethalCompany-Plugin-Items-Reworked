#region usings
using GameNetcodeStuff;
using ItemsReworked.Handlers;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class Remote : BaseScrapItem
    {
        private static int uses;

        internal Remote(GrabbableObject remote) : base(remote)
        {
            uses = CalculateUses();
        }

        public override void InspectItem()
        {
            ItemsReworkedPlugin.mls?.LogInfo($"Remaining uses: {uses}.");
        }

        public override void UseItem()
        {
            if (LocalPlayer != null && uses > 0)
            {
                uses--;
                ActivateRemote();

                LocalPlayer.StartCoroutine(DelayedActivation( 1.5f, () =>
                {
                    RemoteMalfunction();
                }));
            }
        }

        public override void SpecialUseItem()
        {
            throw new System.NotImplementedException();
        }

        private int CalculateUses()
        {
            const int minScrapValue = 20;
            const int maxScrapValue = 48;
            int minUses = ItemsReworkedPlugin.MinRemoteUses.Value;
            int maxUses = ItemsReworkedPlugin.MaxRemoteUses.Value;

            if (BaseScrap.scrapValue <= minScrapValue)
            {
                return minUses;
            }
            else if (BaseScrap.scrapValue >= maxScrapValue)
            {
                return maxUses;
            }
            else
            {
                float percentage = (float)(BaseScrap.scrapValue - minScrapValue) / (maxScrapValue - minScrapValue);
                int uses = Mathf.RoundToInt(Mathf.Lerp(minUses, maxUses, percentage));
                if (uses == 0)
                    ItemsReworkedPlugin.mls?.LogError("Calculation error in remote uses");
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
                    ItemsReworkedPlugin.mls?.LogInfo("Toggling Turret");
                    turret.ToggleTurretEnabled(!turret.enabled);
                    return true;
                }
                if (landmine != null && ItemsReworkedPlugin.DetonateMines.Value)
                {
                    ItemsReworkedPlugin.mls?.LogInfo("HIT MINE");
                    landmine.ExplodeMineServerRpc();
                    return true;
                }
            }
            return false;
        }

        private void RemoteMalfunction()
        {
            System.Random random = new System.Random();
            int randomNumber = random.Next(0, 101);

            // X% Probability to be electrocuted to death
            if (randomNumber <= ItemsReworkedPlugin.RemoteExplosionProbability.Value)
            {
                uses = 0;
                Landmine.SpawnExplosion(BaseScrap.transform.position, true);

                if (BaseScrap.heldByPlayerOnServer)
                {
                    Vector3 bodyVelocity = (LocalPlayer.gameplayCamera.transform.position + BaseScrap.transform.position) / 2f;
                    BaseScrap.playerHeldBy.KillPlayer(bodyVelocity, spawnBody: true, CauseOfDeath.Blast);
                    BaseScrap.DestroyObjectInHand(BaseScrap.playerHeldBy);
                    ItemsReworkedPlugin.Instance.scrapHandler.RemoveScrapItem(BaseScrap);
                    ItemsReworkedPlugin.mls?.LogInfo($"Remote exploded in the hand of the LocalPlayer '{LocalPlayer.name}'");
                }

                BaseScrap.SetScrapValue(1);
            }
            // X% Probability for remote to fry itself
            else if (randomNumber <= ItemsReworkedPlugin.RemoteZapProbability.Value)
            {
                if (BaseScrap.heldByPlayerOnServer)
                {
                    AudioHandler.PlaySound(LocalPlayer, "Scrap\\Remote\\Zap.mp3");
                    BaseScrap.playerHeldBy.DamagePlayer(10, true, causeOfDeath: CauseOfDeath.Electrocution);
                    ItemsReworkedPlugin.mls?.LogInfo($"Remote zapped LocalPlayer '{LocalPlayer.name}'");
                }

                uses = 0;
                BaseScrap.SetScrapValue(1);
            }
        }


    }
}
