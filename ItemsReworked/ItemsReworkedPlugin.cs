#region usings
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ItemsReworked.Handlers;
#endregion

namespace ItemsReworked
{
    [BepInPlugin("ItemsReworkedPlugin", "ItemsReworkedPlugin", "1.0.0")]

    public class ItemsReworkedPlugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("ItemsReworkedPlugin");
        internal static ItemsReworkedPlugin Instance { get; private set; }
        internal static ManualLogSource mls;

        internal ScrapHandler scrapHandler = new ScrapHandler();

        #region Config
        /// <summary>
        /// Config - Example
        /// </summary>
        #region BrackenSettings
        internal static ConfigEntry<bool> ConfigEntry;
        #endregion

        #endregion

        void Awake()
        {
            #region Configs
            //TO BE ADDED
            #endregion
            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource("Loading Items Reworked Plugin");

            harmony.PatchAll();


            mls.LogInfo("Items Reworked Plugin loaded sucessfully.");
        }
    }
}
