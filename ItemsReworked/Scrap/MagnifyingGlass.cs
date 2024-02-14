using GameNetcodeStuff;
using UnityEngine;

namespace ItemsReworked.Scrap
{
    internal class MagnifyingGlass : BaseScrapItem
    {
        private static ItemsReworkedPlugin pluginInstance => ItemsReworkedPlugin.Instance;

        internal MagnifyingGlass(GrabbableObject mangifyingGlass) : base(mangifyingGlass)
        {
        }

        public override void InspectItem()
        {
            throw new System.NotImplementedException();
        }

        public override void UseItem()
        {

        }

        public override void SpecialUseItem()
        {
            throw new System.NotImplementedException();
        }

        private void AnalyzeItem()
        {
            // Cast a ray from the LocalPlayer in the direction they are facing
            Ray ray = new Ray(LocalPlayer.transform.position, LocalPlayer.transform.forward);
            float maxDistance = 10f; // Set the max distance of the ray

            // Check if the ray hits any colliders
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                // Check if the hit object is a grabbable object
                var scrapInView = hit.collider.GetComponent<GrabbableObject>();
                if (scrapInView != null)
                {
                    // Get the ID of the grabbable object
                    int objectId = scrapInView.GetInstanceID();
                    if(pluginInstance.scrapHandler.scrapItemDictionary.ContainsKey(objectId))
                    {
                        ItemsReworkedPlugin.mls.LogInfo("IT WORKS!");
                    }
                }
            }
        }
    }
}
