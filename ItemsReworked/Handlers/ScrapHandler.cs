#region usings
using GameNetcodeStuff;
using ItemsReworked.Scrap;
using ItemsReworked.Scrap.ItemsReworked.Scrap;
using System.Collections.Generic;
#endregion

namespace ItemsReworked.Handlers
{
    public class ScrapHandler
    {
        private Dictionary<int, BaseItem> scrapItemDictionary = new Dictionary<int, BaseItem>();

        public void RegisterScrapItem(GrabbableObject scrapItem)
        {
            if (!scrapItemDictionary.ContainsKey(scrapItem.GetInstanceID()))
            {
                BaseItem newItem = CreateScrapItem(scrapItem);
                if (newItem != null)
                {
                    scrapItemDictionary.Add(scrapItem.GetInstanceID(), newItem);
                    ItemsReworkedPlugin.mls.LogInfo($"{scrapItem.name} registered in ScrapHandler.");
                }
            }
        }

        public void RemoveScrapItem(GrabbableObject scrapItem)
        {
            int instanceId = scrapItem.GetInstanceID();
            if (scrapItemDictionary.ContainsKey(instanceId))
            {
                if (scrapItemDictionary.Remove(instanceId))
                {
                    ItemsReworkedPlugin.mls.LogInfo($"{scrapItem.name} removed from ScrapHandler.");
                    return;
                }
            }
            ItemsReworkedPlugin.mls.LogError($"{scrapItem.name} not found in ScrapHandler.");
        }

        public void UseScrapItem(GrabbableObject scrapItem, PlayerControllerB player)
        {
            int instanceId = scrapItem.GetInstanceID();
            if (scrapItemDictionary.ContainsKey(instanceId))
            {
                BaseItem item = scrapItemDictionary[instanceId];
                item.UseItem(player, scrapItem);
            }
        }

        public void SpecialUse(GrabbableObject scrapItem, PlayerControllerB player)
        {
            int instanceId = scrapItem.GetInstanceID();
            if (scrapItemDictionary.ContainsKey(instanceId))
            {
                if (scrapItemDictionary[instanceId].hasSpecialUse)
                {
                    BaseItem item = scrapItemDictionary[instanceId];
                    item.SpecialUseItem(player, scrapItem);
                }
            }
        }

        public void InspectScrapItem(GrabbableObject scrapItem, PlayerControllerB player)
        {
            int instanceId = scrapItem.GetInstanceID();
            if (scrapItemDictionary.ContainsKey(instanceId))
            {
                BaseItem item = scrapItemDictionary[instanceId];
                item.InspectItem(player, scrapItem);
            }
        }

        private BaseItem CreateScrapItem(GrabbableObject scrapItem)
        {
            // Create custom ScrapItems 
            switch (scrapItem.name.Replace("(Clone)", null))
            {
                case "Candy":
                    return new Candy();
                case "Flask":
                    return new Flask();
                case "Mug":
                    return new Mug(scrapItem);
                case "PillBottle":
                    return new PillBottle(scrapItem);
                case "RedSodaCan":
                    return new RedSodaCan(scrapItem);
                case "Remote":
                    return new Remote(scrapItem);
                case "LaserPointer":
                    return new LaserPointer(scrapItem);
                default:
                    ItemsReworkedPlugin.mls.LogInfo($"Unsupported scrap item type {scrapItem.name} picked up");
                    return null;
            }
        }
    }
}
