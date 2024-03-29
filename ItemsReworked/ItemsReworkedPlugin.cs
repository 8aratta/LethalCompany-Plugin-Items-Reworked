﻿#region usings
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using ItemsReworked.Handlers;
#endregion

namespace ItemsReworked
{
    [BepInPlugin("ItemsReworkedPlugin", "ItemsReworkedPlugin", "1.6.1")]

    public class ItemsReworkedPlugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("ItemsReworkedPlugin");
        internal static ItemsReworkedPlugin? Instance { get; private set; }
        internal static ManualLogSource? mls;
        internal ScrapHandler scrapHandler = new ScrapHandler();
        #region Config

        #region Flask Configs
        /// <summary>
        /// NoEffectProbability - Probability for Flask to do nothing
        /// IntoxicationEffectProbability - Probability for Flask to intoxicate the HoldingPlayer
        /// PoisoningEffectProbability - Probability for Flask to poison the HoldingPlayer
        /// HealingEffectProbability - Probability for Flask to heal the HoldingPlayer a certain amount of hp
        /// MaxHealing - Max amount of healing a flask can do
        /// </summary>
        internal static ConfigEntry<int> NoEffectProbability;
        internal static ConfigEntry<int> IntoxicationEffectProbability;
        internal static ConfigEntry<int> PoisoningEffectProbability;
        internal static ConfigEntry<int> MaxPoison;
        internal static ConfigEntry<int> HealingEffectProbability;
        internal static ConfigEntry<int> MaxHealing;
        #endregion

        #region Laser Pointer Configs
        /// <summary>
        /// LaserPointerBatteryDrain - Additional drain for balancing
        /// GiantsRangeOfView - ROV so Giants can lose track of the laser
        /// </summary>
        internal static ConfigEntry<float> LaserPointerBatteryDrain;
        internal static ConfigEntry<float> GiantsRangeOfView;
        #endregion

        #region Mug Configs
        /// <summary>
        /// Min Duration - Min duration of stamina boost in seconds
        /// Max Duration - Max duration of stamina boost in seconds
        /// </summary>
        internal static ConfigEntry<float> MinDurationStaminaBoost;
        internal static ConfigEntry<float> MaxDurationStaminaBoost;
        #endregion

        #region PillBottle Configs
        /// <summary>
        /// Min Pills - Min amount of pills in a Bottle
        /// Max Pills - Max amount of pills in a Bottle
        /// </summary>
        internal static ConfigEntry<int> MinPills;
        internal static ConfigEntry<int> MaxPills;
        #endregion

        #region Soda Configs
        /// <summary>
        /// Min Duration - Min duration of jump boost in seconds
        /// Max Duration - Max duration of jump boost in seconds
        /// </summary>
        internal static ConfigEntry<float> MinDurationJumpBoost;
        internal static ConfigEntry<float> MaxDurationJumpBoost;
        #endregion

        #region Remote Configs
        /// <summary>
        /// Min Uses - Min amount of uses for a remote
        /// Max Uses - Max amount of uses for a remote
        /// Remote Explosion Probability - Probability for the remote to explode and kill the HoldingPlayer
        /// Remote Zep Probability - Probability for the remote to zap the HoldingPlayer
        /// </summary>
        internal static ConfigEntry<int> MinRemoteUses;
        internal static ConfigEntry<int> MaxRemoteUses;
        internal static ConfigEntry<int> RemoteExplosionProbability;
        internal static ConfigEntry<int> RemoteZapProbability;
        internal static ConfigEntry<bool> DetonateMines;
        internal static ConfigEntry<bool> ToggleTurrets;
        #endregion

        #endregion

        void Awake()
        {
            #region Configs

            #region Flask Default Settings
            NoEffectProbability = Config.Bind("Flask",
                                              "NoEffectProbability",
                                              20,
                                              "Probability of flasks to have of having no effect.");

            IntoxicationEffectProbability = Config.Bind("Flask",
                                                        "IntoxicationEffectProbability",
                                                        50,
                                                        "Probability of flasks to intoxicate the LocalPlayer.");

            PoisoningEffectProbability = Config.Bind("Flask",
                                                     "PoisoningEffectProbability",
                                                     50,
                                                     "Probability of flasks to poison the LocalPlayer.");

            MaxPoison = Config.Bind("Flask",
                                         "MaxPoison",
                                         1,
                                         "Health left after being poisoned. 0 Kills the LocalPlayer.");

            HealingEffectProbability = Config.Bind("Flask",
                                                   "HealingEffectProbability",
                                                   1,
                                                   "Probability of flasks to heal the LocalPlayer.");

            MaxHealing = Config.Bind("Flask",
                                     "MaxHealing",
                                     33,
                                     "Max amount of healing a flask can do. (Based on Scrap Value)");
            #endregion

            #region Laser Pointer Default Settings
            LaserPointerBatteryDrain = Config.Bind("Laser Pointer",
                                                   "LaserPointerBatteryDrain",
                                                   0.1f,
                                                   "Additional battery drain for the laser pointer. (Recommendation: 0.02 - 0.2)");

            GiantsRangeOfView = Config.Bind("Laser Pointer",
                                            "GiantsRangeOfView",
                                            200f,
                                            "Determines how far a giant can follow the laser without losing track of it. (Recommendation: 75 - 300)");
            #endregion

            #region Mug Default Settings
            MinDurationStaminaBoost = Config.Bind("Mug",
                                                  "MinDuration",
                                                  10f,
                                                  "Min duration of stamina boost. (Based on Scrap Value)");

            MaxDurationStaminaBoost = Config.Bind("Mug",
                                                  "MaxDuration",
                                                  30f,
                                                  "Max duration of stamina boost. (Based on Scrap Value)");
            #endregion

            #region PillBottle Default Settings
            MinPills = Config.Bind("Pill Bottle",
                                   "MinPills",
                                   2,
                                   "Min amount of pills in a Bottle. (Based on Scrap Value)");

            MaxPills = Config.Bind("Pill Bottle",
                                   "MaxPills",
                                   33,
                                   "Max amount of pills in a Bottle. (Based on Scrap Value)");
            #endregion

            #region Soda Default Settings
            MinDurationJumpBoost = Config.Bind("Soda",
                                               "MinDuration",
                                               5f,
                                               "Min duration of jump boost. (Based on Scrap Value)");

            MaxDurationJumpBoost = Config.Bind("Soda",
                                               "MaxDuration",
                                               30f,
                                               "Max duration of jump boost. (Based on Scrap Value)");
            #endregion

            #region Remote Default Settings
            MinRemoteUses = Config.Bind("Remote",
                                        "MinDuration",
                                        2,
                                        "Min amount of uses for a remote. (Based on Scrap Value)");

            MaxRemoteUses = Config.Bind("Remote",
                                        "MaxDuration",
                                        10,
                                        "Max amount of uses for a remote. (Based on Scrap Value)");

            RemoteExplosionProbability = Config.Bind("Remote",
                                                "RemoteExplosionProbability",
                                                5,
                                                "Probability for the Remote to explode and kill the LocalPlayer when used.");

            RemoteZapProbability = Config.Bind("Remote",
                                          "RemoteZapProbability",
                                          15,
                                          "Probability for the Remote to zap and damage the LocalPlayer when used.");

            DetonateMines = Config.Bind("Remote",
                                        "DetonateMines",
                                        true,
                                        "Enables the detonation of Mines by using the remote.");

            ToggleTurrets = Config.Bind("Remote",
                                        "ToggleTurrets",
                                        true,
                                        "Enables the Disabling/Reenabling of Turrets by using the remote.");

            #endregion

            #endregion

            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource("Loading Items Reworked Plugin");
            harmony.PatchAll();
            mls.LogInfo("Items Reworked Plugin loaded sucessfully.");
        }
    }
}
