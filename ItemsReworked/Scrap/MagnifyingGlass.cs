using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemsReworked.Scrap
{
    internal class MagnifyingGlass : BaseScrapItem
    {
        private static ItemsReworkedPlugin pluginInstance => ItemsReworkedPlugin.Instance;
        private List<int> inspectedIDs = new List<int>();

        internal MagnifyingGlass(GrabbableObject mangifyingGlass) : base(mangifyingGlass)
        {
            ItemDescription = "Inspect scrap to unveil their hidden properties.";
        }

        public override void UpdateItem()
        {
            // Reset modified state
            ItemModified = false;
        }

        public override void InspectItem()
        {
            HUDManager.Instance.DisplayTip($"{ItemName}", $"{ItemDescription}");
        }

        public override void UseItem()
        {
            ToggleInspectingStatus(!HoldingPlayer.IsInspectingItem);

            if (HoldingPlayer.IsInspectingItem)
                HoldingPlayer.StartCoroutine(LookThroughGlass());
        }

        public override void SecondaryUseItem()
        {
            throw new System.NotImplementedException();
        }
        //LIST OF LOOKED ITEM IDS
        private IEnumerator LookThroughGlass()
        {
            while (HoldingPlayer.IsInspectingItem)
            {
                // Cast a ray from the HoldingPlayer in the direction they are facing
                Ray ray = new Ray(HoldingPlayer.bodyParts[0].transform.position, HoldingPlayer.bodyParts[0].transform.forward);
                float maxDistance = 0.5f; // Set the max distance of the ray
                ItemsReworkedPlugin.mls?.LogInfo($"Casting");

                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(hit.point, 0.1f);
                    foreach (var hitCollider in hitColliders)
                    {
                        // Check if the hit object is a grabbable object
                        var scrapInView = hitCollider.GetComponent<GrabbableObject>();
                        if (scrapInView != null)
                        {
                            int objectId = scrapInView.GetInstanceID();

                            // Check if the hit object is not the currently held magnifying glass or one of the previously inspected items
                            if (objectId != BaseScrap.GetInstanceID() && !inspectedIDs.Contains(objectId))
                            {
                                // Try to register scrap in case it was not found before
                                pluginInstance.scrapHandler.RegisterScrapItem(scrapInView);

                                // Check if scrap is a custom one and previously unidentified one
                                if (pluginInstance.scrapHandler.scrapItemDictionary.ContainsKey(objectId)
                                    && !pluginInstance.scrapHandler.scrapItemDictionary[objectId].ItemPropertiesDiscovered)
                                {
                                    // Mark scrap as discovered
                                    pluginInstance.scrapHandler.scrapItemDictionary[objectId].ItemPropertiesDiscovered = true;
                                    pluginInstance.scrapHandler.scrapItemDictionary[objectId].ItemModified = true;
                                }
                                else
                                    HUDManager.Instance.DisplayTip($"{scrapInView.gameObject.GetComponentInChildren<ScanNodeProperties>().headerText}", "Just some random scrap...");

                                inspectedIDs.Add(objectId);
                            }
                        }
                    }
                }
                yield return null;
            }
        }

        private void ToggleInspectingStatus(bool enable)
        {
            ItemsReworkedPlugin.mls?.LogInfo($"Setting IsInspectingItem to {enable}");

            if (HoldingPlayer.IsInspectingItem != enable)
            {
                BaseScrap.itemProperties.canBeInspected = enable;
                HoldingPlayer.IsInspectingItem = enable;
            }

            if (!enable)
            {
                // Clear Inspected items list
                inspectedIDs.Clear();
                HoldingPlayer.StopCoroutine(LookThroughGlass());
            }
        }

    }
}
