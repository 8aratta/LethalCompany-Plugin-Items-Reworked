using GameNetcodeStuff;
using ItemsReworked.Scrap;
using System.Collections.Generic;

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
                scrapItemDictionary.Add(scrapItem.GetInstanceID(), newItem);
                ItemsReworkedPlugin.mls.LogInfo($"{scrapItem.name} registered in ScrapHandler.");
            }
            else
            {
                ItemsReworkedPlugin.mls.LogInfo($"{scrapItem.name} already registered in ScrapHandler.");
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
            else
            {
                ItemsReworkedPlugin.mls.LogError($"Item with ID {instanceId} not found in ScrapHandler.");
            }
        }

        private BaseItem CreateScrapItem(GrabbableObject scrapItem)
        {
            // Implement logic to create the appropriate BaseItem based on scrapItem.name
            switch (scrapItem.name.Replace("(Clone)", null))
            {
                case "7Ball":
                    return new SevenBall();
                case "Candy":
                    return new Candy();
                case "CashRegister":
                    return new CashRegister();
                case "Dentures":
                    return new Dentures();
                case "FancyPainting":
                    return new FancyPainting();
                case "Flask":
                    return new Flask();
                case "Hairdryer":
                    return new Hairdryer();
                case "MagnifyingGlass":
                    return new MagnifyingGlass();
                case "Mug":
                    return new Mug();
                case "PerfumeBottle":
                    return new PerfumeBottle();
                case "Phone":
                    return new Phone();
                case "PickleJar":
                    return new PickleJar();
                case "PillBottle":
                    return new PillBottle();
                case "RubberDuck":
                    return new RubberDuck();
                case "SodaCanRed":
                    return new SodaCanRed();
                default:
                    ItemsReworkedPlugin.mls.LogError($"Unknown scrap item type: {scrapItem.name}");
                    return null;
            }
        }
    }
}
