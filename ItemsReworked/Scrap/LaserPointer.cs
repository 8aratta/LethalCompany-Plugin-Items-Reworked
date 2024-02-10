using GameNetcodeStuff;
using System.Collections;
using UnityEngine;

namespace ItemsReworked.Scrap
{
    internal class LaserPointer : BaseItem
    {
        private bool isTurnedOn = false;
        private ForestGiantAI distractedGiant;
        private RaycastHit laserPoint;
        private Vector3 lastValidLaserSpot;
        private int currentRayMask;

        internal LaserPointer(GrabbableObject laserPointer)
        {
            distractedGiant = null;
            inSpecialScenario = false;
            hasSpecialUse = true;
            isTurnedOn = false;
        }

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {

        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (item != null && player != null)
            {
                if (item.insertedBattery.charge > 0)
                    ToggleLaserPointerPower(!isTurnedOn);

                if (isTurnedOn && distractedGiant == null)
                    player.StartCoroutine(EmitLaserRay(player, item));
            }
            else
                ItemsReworkedPlugin.mls.LogError($"Error during using of {item.name}");
        }

        public override void SpecialUseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (distractedGiant != null && !inSpecialScenario)
            {
                HUDManager.Instance.DisplayTip("Laser Pointer", "Distracting Giant...", true);
                inSpecialScenario = true;
                player.StartCoroutine(SpecialLaser(item, laserPoint));
            }
        }

        private void ToggleLaserPointerPower(bool enable)
        {
            ItemsReworkedPlugin.mls.LogInfo($"Setting Laser to {enable}");
            if (isTurnedOn != enable)
                isTurnedOn = enable;

            if (!isTurnedOn)
            {
                distractedGiant = null;
                inSpecialScenario = false;
            }
        }

        private IEnumerator EmitLaserRay(PlayerControllerB player, GrabbableObject item)
        {
            while (isTurnedOn && item.insertedBattery.charge > 0 && item != null)
            {
                // Create a ray in the direction of the laser pointer
                Ray ray = new Ray(item.transform.position, item.transform.forward);

                float maxDistance = 75f;

                if (distractedGiant != null)
                    currentRayMask = StartOfRound.Instance.walkableSurfacesMask;
                else
                    currentRayMask = StartOfRound.Instance.allPlayersCollideWithMask;
                //distractedGiant.gameObject.layer; --> is null



                // Check if the ray hits any colliders
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, currentRayMask))
                {
                    laserPoint = hit;

                    if (distractedGiant == null)
                    {
                        // Check if the collider belongs to a ForestGiantAI within 1 ingame meters
                        Collider[] hitColliders = Physics.OverlapSphere(hit.point, 1f);

                        foreach (var hitCollider in hitColliders)
                        {
                            // Check if the collider belongs to a ForestGiantAI
                            ForestGiantAI forestGiant = hitCollider.GetComponent<ForestGiantAI>();
                            if (forestGiant != null)
                            {
                                // A ForestGiantAI is detected within the specified radius
                                HUDManager.Instance.DisplayTip("Laser Pointer", "Giant Detected! Press 'Q' to distract");

                                distractedGiant = forestGiant;
                            }
                        }
                    }
                }
                yield return null;
            }

            // Battery is depleted, end the distraction process
            ToggleLaserPointerPower(false);
        }

        private IEnumerator SpecialLaser(GrabbableObject item, RaycastHit hit)
        {
            // Drain the battery
            float batteryDrainRate = 0.1f;
            item.insertedBattery.charge -= batteryDrainRate * Time.deltaTime;

            // Continuously drain the battery until it's empty
            while (isTurnedOn && item.insertedBattery.charge > 0f && item != null)
            {
                // Drain the battery
                item.insertedBattery.charge -= batteryDrainRate * Time.deltaTime;

                // Distract the Giant to where the laser is pointed at
                DistractingGiant(distractedGiant, hit);

                yield return null;
            }

            // Battery is depleted, end the distraction process
            ToggleLaserPointerPower(false);
        }

        private void DistractingGiant(ForestGiantAI forestGiant, RaycastHit laserPoint)
        {
            // Check if laserPoint is in the vicinity of giant
            if (Vector3.Distance(forestGiant.transform.position, laserPoint.transform.position) < 8f)
                lastValidLaserSpot = laserPoint.point;
            else
            {
                //Set this point as last valid spot
                forestGiant.investigatePosition = lastValidLaserSpot;

                // Set the destination to the point
                if (forestGiant.currentBehaviourStateIndex != 0)
                    forestGiant.currentBehaviourStateIndex = 0;

                forestGiant.turnCompass.LookAt(laserPoint.point);
                forestGiant.SetDestinationToPosition(laserPoint.point);
            }
        }
    }
}
