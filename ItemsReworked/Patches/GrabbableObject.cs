using HarmonyLib;

namespace ItemsReworked.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    internal class GrabbableObjectPatches
    {
        private static ItemsReworkedPlugin pluginInstance => ItemsReworkedPlugin.Instance;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdateBaseScrap(GrabbableObject __instance)
        {
                pluginInstance.scrapHandler.UpdateScrapItem(__instance);
        }
    }
}
