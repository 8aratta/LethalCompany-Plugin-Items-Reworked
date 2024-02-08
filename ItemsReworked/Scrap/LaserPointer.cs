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

        internal LaserPointer(GrabbableObject laserPointer)
        {
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
                    ToggleIsTurnedOn(!isTurnedOn);

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

        private void ToggleIsTurnedOn(bool enable)
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
            ItemsReworkedPlugin.mls.LogInfo($"Entering EmitLaserRay");

            //sfx loading laser

            while (isTurnedOn && item.insertedBattery.charge > 0 && item != null)
            {
                // Create a ray in the direction of the laser pointer
                Ray ray = new Ray(item.transform.position, item.transform.forward);

                // Define the maximum distance the ray can travel
                float maxDistance = 100f;

                // Check if the ray hits any colliders
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, StartOfRound.Instance.walkableSurfacesMask))
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
            isTurnedOn = false;
            inSpecialScenario = false;
            distractedGiant = null;
            ItemsReworkedPlugin.mls.LogInfo("Ending EmitLaser");
        }

        private IEnumerator SpecialLaser(GrabbableObject item, RaycastHit hit) // make this get RayCastHit instead of generatin a new laser
        {
            ItemsReworkedPlugin.mls.LogInfo("Entering Special Laser");

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
            isTurnedOn = false;
            inSpecialScenario = false;
            distractedGiant = null;
            ItemsReworkedPlugin.mls.LogInfo("Ending Special Laser");
        }

        private void DistractingGiant(ForestGiantAI forestGiant, RaycastHit hit)
        {
            // Set the destination to the point
            if (forestGiant.currentBehaviourStateIndex != 0)
            {
                forestGiant.currentBehaviourStateIndex = 0;
                forestGiant.investigatePosition = hit.point;
                forestGiant.investigating = true;
            }

            forestGiant.turnCompass.LookAt(hit.point);
            forestGiant.SetDestinationToPosition(hit.point);
            forestGiant.moveTowardsDestination = true;
        }
    }
}
