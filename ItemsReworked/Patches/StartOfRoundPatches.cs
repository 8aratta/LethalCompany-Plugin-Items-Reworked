using HarmonyLib;
using System.Linq;

namespace ItemsReworked.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatches
    {

        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void NewGameAboutToBegin(StartOfRound __instance)
        {
            // GET ALL ITEMS
            foreach (var item in __instance.allItemsList.itemsList.OrderBy(i => i.name))
            {
                if (item.isScrap)
                {
                    ItemsReworkedPlugin.mls.LogInfo($"name = '{item.name}");
                }
            }
        }
    }
}
