using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace HexRunicArts
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "com.hex.runicarts";
        private const string PluginName = "HexRunicArts";
        private const string PluginVersion = "1.0.0";

        private Harmony _harmonyInstance;

        internal static ConfigEntry<KeyboardShortcut> SplinterHotkey;

        internal static ManualLogSource Log;
        internal static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            SplinterHotkey = Config.Bind("Hotkeys", "SplinterHotkey", new KeyboardShortcut(KeyCode.F, KeyCode.LeftControl), "Hotkey to activate Splinter ability.");

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll(assembly);

            Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
        }

        private void OnDestroy()
        {
            Log.LogInfo($"{PluginName} v{PluginVersion} unloaded.");

            _harmonyInstance?.UnpatchSelf();
            _harmonyInstance = null;
            Instance = null;
            Log = null;
        }
    }
}
