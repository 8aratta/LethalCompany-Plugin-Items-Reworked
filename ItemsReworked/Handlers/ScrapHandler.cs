#region usings
using GameNetcodeStuff;
using ItemsReworked;
using ItemsReworked.Scrap;
using System.Collections.Generic;
#endregion

public class ScrapHandler
{
    internal Dictionary<int, BaseScrapItem> scrapItemDictionary = new Dictionary<int, BaseScrapItem>();

    public void RegisterScrapItem(GrabbableObject scrapItem)
    {
        int instanceId = scrapItem.GetInstanceID();
        ItemsReworkedPlugin.mls.LogWarning("Entering RegisterScrapItem");
        ItemsReworkedPlugin.mls.LogWarning($"instanceId: {instanceId}");
        if (!scrapItemDictionary.ContainsKey(instanceId))
        {
            BaseScrapItem newItem = CreateScrapItem(scrapItem);
            if (newItem != null)
            {
                scrapItemDictionary.Add(instanceId, newItem);
                ItemsReworkedPlugin.mls.LogInfo($"{scrapItem.name} registered in ScrapHandler.");
            }
        }
    }

    public void UseScrapItem(GrabbableObject scrapItem, PlayerControllerB player)
    {
        ItemsReworkedPlugin.mls.LogWarning("Entering UseScrapItem");

        int instanceId = scrapItem.GetInstanceID(); // Get the instance ID

        ItemsReworkedPlugin.mls.LogWarning($"scrapItem.GetInstanceID(): {scrapItem.GetInstanceID()}");
        ItemsReworkedPlugin.mls.LogWarning($"scrapItemDictionary.ContainsKey(instanceId): {scrapItemDictionary.ContainsKey(instanceId)}");

        foreach (var item in scrapItemDictionary)
            ItemsReworkedPlugin.mls.LogWarning($"scrapItemDictionary item: {item}");

        if (scrapItemDictionary.ContainsKey(instanceId))
        {
            ItemsReworkedPlugin.mls.LogWarning("Contained - GOOD");
            BaseScrapItem item = scrapItemDictionary[instanceId];
            item.UseItem();
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

    public void SpecialUse(GrabbableObject scrapItem, PlayerControllerB player)
    {
        int instanceId = scrapItem.GetInstanceID();
        if (scrapItemDictionary.ContainsKey(instanceId))
        {
            if (scrapItemDictionary[instanceId].hasSecondaryUse)
            {
                BaseScrapItem item = scrapItemDictionary[instanceId];
                item.SpecialUseItem();
            }
        }
    }

    public void InspectScrapItem(GrabbableObject scrapItem, PlayerControllerB player)
    {
        int instanceId = scrapItem.GetInstanceID();
        if (scrapItemDictionary.ContainsKey(instanceId))
        {
            BaseScrapItem item = scrapItemDictionary[instanceId];
            item.InspectItem();
        }
    }

    private BaseScrapItem CreateScrapItem(GrabbableObject scrapItem)
    {
        BaseScrapItem customScrap = null;

        // Create custom ScrapItems 
        switch (scrapItem.name.Replace("(Clone)", null))
        {
            case "Candy":
                customScrap = new Candy(scrapItem);
                break;
            case "Flask":
                customScrap = new Flask(scrapItem);
                break;
            case "Mug":
                customScrap = new Mug(scrapItem);
                break;
            case "PillBottle":
                customScrap = new PillBottle(scrapItem);
                break;
            case "RedSodaCan":
                customScrap = new RedSodaCan(scrapItem);
                break;
            case "Remote":
                customScrap = new Remote(scrapItem);
                break;
            case "LaserPointer":
                customScrap = new LaserPointer(scrapItem);
                break;
            default:
                ItemsReworkedPlugin.mls.LogInfo($"Unsupported scrap item type {scrapItem.name} picked up");
                break;
        }
        if (customScrap == null)
            ItemsReworkedPlugin.mls.LogError($"Failed to create custom scrap item for {scrapItem.name}");

        return customScrap;
    }
}