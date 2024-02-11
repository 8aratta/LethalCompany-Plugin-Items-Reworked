#region usings
using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace ItemsReworked.Scrap
{
    internal class LaserPointer : BaseItem
    {
        private bool isTurnedOn = false;
        private Dictionary<ForestGiantAI, Vector3> distractedGiants;
        private ForestGiantAI distractedGiant;

        internal LaserPointer(GrabbableObject laserPointer)
        {
            distractedGiants = new Dictionary<ForestGiantAI, Vector3>();
            distractedGiant = null;
            hasSpecialUse = false;
            isTurnedOn = false;
        }

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {
            HUDManager.Instance.DisplayTip("Laser Pointer", "Giants are easily distracted by its light");
        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (item != null && player != null)
            {
                if (item.insertedBattery.charge > 0)
                    ToggleLaserPointerPower(!isTurnedOn);

                if (isTurnedOn)
                    player.StartCoroutine(EmitLaserRay(player, item));
            }
            else
                ItemsReworkedPlugin.mls.LogError($"Error during using of {item.name}");
        }

        public override void SpecialUseItem(PlayerControllerB player, GrabbableObject item)
        {
            throw new System.NotImplementedException();
        }

        private void ToggleLaserPointerPower(bool enable)
        {
            ItemsReworkedPlugin.mls.LogInfo($"Setting Laser to {enable}");
            if (isTurnedOn != enable)
                isTurnedOn = enable;

            if (!isTurnedOn)
            {
                distractedGiants.Clear();
                distractedGiant = null;
            }
        }

        private IEnumerator EmitLaserRay(PlayerControllerB player, GrabbableObject item)
        {
            while (isTurnedOn && item.insertedBattery.charge > 0 && item != null)
            {
                // Drain Battery
                item.insertedBattery.charge -= ItemsReworkedPlugin.LaserPointerBatteryDrain.Value * Time.deltaTime;

                // Create a ray in the direction of the laser pointer
                Ray ray = new Ray(item.transform.position, item.transform.forward);
                float maxDistance = 800f;

                // Check if the ray hits any colliders
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, StartOfRound.Instance.walkableSurfacesMask))
                {
                    if (distractedGiant == null)
                    {
                        // Check if the collider belongs to a ForestGiantAI within 50 ingame meters
                        Collider[] hitColliders = Physics.OverlapSphere(hit.point, 50f);
                        foreach (var hitCollider in hitColliders)
                        {
                            // Check if the collider belongs to a ForestGiantAI
                            distractedGiant = hitCollider.GetComponent<ForestGiantAI>();
                            if (distractedGiant != null && !distractedGiants.ContainsKey(distractedGiant))
                                distractedGiants.Add(distractedGiant, new Vector3());
                        }
                    }
                    if (distractedGiants.Count > 0)
                        DistractGiants(distractedGiants, hit);
                }
                yield return null;
            }
            // Battery is depleted, end the distraction process
            if (item.insertedBattery.charge <= 0)
                ToggleLaserPointerPower(false);
        }

        private void DistractGiants(Dictionary<ForestGiantAI, Vector3> forestGiants, RaycastHit laser)
        {
            foreach (var giant in forestGiants)
            {
                // Ensure that laser is within ROV & giant is not in the middle of stun or eating of player animation
                if (!giant.Key.inSpecialAnimation &&
                    Vector3.Distance(giant.Key.transform.position, laser.transform.position) < ItemsReworkedPlugin.GiantsRangeOfView.Value)
                {
                    // Set this point as last seen spot
                    distractedGiants[giant.Key] = laser.point;

                    // Look at laser
                    giant.Key.lookTarget.position = laser.point;
                    giant.Key.turnCompass.LookAt(laser.point);

                    // Move to laser
                    giant.Key.SetDestinationToPosition(laser.point);

                    // He need some milk
                    giant.Key.SwitchToBehaviourState(0);
                }
                else
                {
                    // Laser was out of sight, let giant investigate the last spot it saw the laser at
                    giant.Key.investigatePosition = distractedGiants[giant.Key];
                    giant.Key.SetDestinationToPosition(distractedGiants[giant.Key], true);
                }
            }
        }
    }
}
