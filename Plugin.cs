using BaboonAPI.Hooks.Initializer;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using System.Net.Configuration;
using TootTallyCore.Utils.Assets;
using TootTallyCore.Utils.TootTallyModules;
using UnityEngine;

namespace TootTallySettings
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("TootTallyCore", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin, ITootTallyModule
    {
        public static Plugin Instance;
        private static Harmony _harmony;

        public ConfigEntry<bool> ModuleConfigEnabled { get; set; }
        public bool IsConfigInitialized { get; set; }
        public string Name { get => "TootTally Settings"; set => Name = value; }

        public static TootTallySettingPage ModulesSettingPage;
        public static TootTallySettingPage MainTootTallySettingPage;

        public static void LogInfo(string msg) => Instance.Logger.LogInfo(msg);
        public static void LogError(string msg) => Instance.Logger.LogError(msg);

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;
            _harmony = new Harmony(Info.Metadata.GUID);


            GameInitializationEvent.Register(Info, TryInitialize);
        }

        private void TryInitialize()
        {
            // Bind to the TTModules Config for TootTally
            ModuleConfigEnabled = TootTallyCore.Plugin.Instance.Config.Bind("Modules", "TootTallySettings", true, "<insert module description here>");
            MainTootTallySettingPage = TootTallySettingsManager.AddNewPage("TootTally", "TootTally", 40f, new Color(.1f, .1f, .1f, .3f));
            TootTallyModuleManager.AddModule(this);
        }

        public void LoadModule()
        {
            AssetManager.LoadAssets(Path.Combine(Path.GetDirectoryName(Instance.Info.Location), "Assets"));
            _harmony.PatchAll(typeof(TootTallySettingsManager));
        }

        public void AddModuleToSettingPage(ITootTallyModule module)
        {
            ModulesSettingPage ??= TootTallySettingsManager.AddNewPage("TTModules", "TTModules", 40f, new Color(0, 0, 0, 0));
            ModulesSettingPage.AddToggle(module.Name, module.ModuleConfigEnabled);
        }

        public void UnloadModule()
        {
            _harmony.UnpatchSelf();
            LogInfo($"Module unloaded!");
        }
    }
}