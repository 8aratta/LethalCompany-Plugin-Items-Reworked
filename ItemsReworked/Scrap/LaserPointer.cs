using GameNetcodeStuff;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ItemsReworked.Scrap
{
    internal class LaserPointer : BaseItem
    {
        private bool isTurnedOn = false;
        private bool inSpecialMode = false;
        private ForestGiantAI distractedGiant;
        private InputAction leftMouseButton;

        internal LaserPointer(GrabbableObject laserPointer)
        {
            isTurnedOn = false;
            inSpecialMode = false;
            leftMouseButton = new InputAction(binding: "<Mouse>/leftButton");
            leftMouseButton.Enable();
        }

        public override void InspectItem(PlayerControllerB player, GrabbableObject item)
        {

        }

        public override void UseItem(PlayerControllerB player, GrabbableObject item)
        {
            if (item.insertedBattery.charge > 0)
                ToggleIsTurnedOn(!isTurnedOn);
            if (isTurnedOn)
            {
                // Check if left mouse button is held for 3 seconds
                player.StartCoroutine(ChargeLaser(player, item));
            }
        }

        private void ToggleIsTurnedOn(bool enable)
        {
            ItemsReworkedPlugin.mls.LogInfo($"Setting Laser to {enable}");
            if (isTurnedOn != enable)
                isTurnedOn = enable;

            // Turn off all special events
            if (!isTurnedOn)
            {
                inSpecialMode = false;
            }
        }

        private IEnumerator ChargeLaser(PlayerControllerB player, GrabbableObject item)
        {

            ItemsReworkedPlugin.mls.LogInfo($"Entering ChargeLaser");
            float elapsedTime = 0f;
            float requiredHoldTime = 1f;

            //sfx loading laser

            while (elapsedTime < requiredHoldTime)
            {
                if (!leftMouseButton.WasReleasedThisFrame())  // Check if the left mouse button is pressed
                {
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                else
                {
                    //sfx laser deload

                    // Player released left mouse button before the required time
                    ItemsReworkedPlugin.mls.LogInfo($"Canceling ChargeLaser");
                    yield break;
                }
            }

            // Start the SpecialLaser coroutine
            player.StartCoroutine(SpecialLaser(item));
        }

        private IEnumerator SpecialLaser(GrabbableObject item)
        {
            ItemsReworkedPlugin.mls.LogInfo("Entering Special Laser");

            inSpecialMode = true;
            float batteryDrainRate = 0.1f;

            // Continuously drain the battery until it's empty
            while (item.insertedBattery.charge > 0f && inSpecialMode)
            {
                // Drain the battery
                item.insertedBattery.charge -= batteryDrainRate * Time.deltaTime;

                // Create a ray in the direction of the laser pointer
                Ray ray = new Ray(item.transform.position, item.transform.forward);

                // Define the maximum distance the ray can travel
                float maxDistance = 50f;

                // Check if the ray hits any colliders
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, StartOfRound.Instance.walkableSurfacesMask))
                {

                    if (distractedGiant != null)
                    {
                        DistractingGiant(distractedGiant, hit);
                    }
                    else
                    {
                        // Check if the collider belongs to a ForestGiantAI within 10 ingame meters
                        Collider[] hitColliders = Physics.OverlapSphere(hit.point, 10f);

                        foreach (var hitCollider in hitColliders)
                        {
                            // Check if the collider belongs to a ForestGiantAI
                            ForestGiantAI forestGiant = hitCollider.GetComponent<ForestGiantAI>();
                            if (forestGiant != null)
                            {
                                // A ForestGiantAI is detected within the specified radius
                                ItemsReworkedPlugin.mls.LogWarning("ForestGiantAI detected");
                                distractedGiant = forestGiant;

                            }
                        }
                    }
                }

                yield return null;
            }

            // Battery is depleted, end the distraction process
            inSpecialMode = false;
            isTurnedOn = false;
            distractedGiant = null;
            ItemsReworkedPlugin.mls.LogInfo("Battery depleted, ending Special Laser");
        }

        private void DistractingGiant(ForestGiantAI forestGiant, RaycastHit hit)
        {
            ItemsReworkedPlugin.mls.LogWarning("Starting distraction");

            forestGiant.SetDestinationToPosition(hit.point);
            forestGiant.turnCompass.LookAt(hit.point);
            forestGiant.SwitchToBehaviourState(0);
        }

    }
}
