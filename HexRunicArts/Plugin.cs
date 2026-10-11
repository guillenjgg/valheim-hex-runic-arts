using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HexRunicArts
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "com.hex.runicarts";
        private const string PluginName = "HexRunicArts";
        private const string PluginVersion = "1.0.0";
        private const string AssetBundleName = "hexrunicarts";
        private static AssetBundle HexRunicArtsAssetBundle;
        private const string HeartPrefabPath = "Assets/_CustomItems/HexRunicArts/EikthyrHeart.prefab";
        
        internal static GameObject EikthyrHeart;

        private Harmony _harmonyInstance;

        internal static ConfigEntry<KeyboardShortcut> SplinterHotkey;
        internal static ConfigEntry<int> SplinterRadius;
        internal static ConfigEntry<bool> EnableSplinterEffect;
        internal static ConfigEntry<bool> EnableStumpDestroy;

        internal static ManualLogSource Log;
        internal static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            HexRunicArtsAssetBundle = GetAssetBundleFromResources(AssetBundleName);

            if (HexRunicArtsAssetBundle == null)
            {
                Log.LogError($"AssetBundle | Failed to load: {AssetBundleName}");
                return;
            }

            EikthyrHeart = HexRunicArtsAssetBundle.LoadAsset<GameObject>(HeartPrefabPath);

            if (EikthyrHeart == null)
            {
                Log.LogError($"AssetBundle | Prefab not found: {HeartPrefabPath}");
                return;
            }

            Log.LogInfo($"AssetBundle | Loaded prefab: {EikthyrHeart.name}");

            var heartItem = new CustomItem(EikthyrHeart, true, new ItemConfig
            {
                Name = "Eikthyr's Heart",
                Description = "The heart of the Horned One, still pulsing with the power of the storm."
            });

            ItemManager.Instance.AddItem(heartItem);

            Log.LogInfo("ItemManager | Registered EikthyrHeart.");

            SplinterHotkey = Config.Bind("Splinter", "SplinterHotkey", new KeyboardShortcut(KeyCode.F, KeyCode.LeftShift), "Hotkey to activate Splinter ability.");
            
            SplinterRadius = Config
                .Bind("Splinter", "SplinterRadius", 10, 
                new ConfigDescription("Radius within which logs will be splintered. Measured in meters.", new AcceptableValueRange<int>(1, 50)));

            EnableSplinterEffect = Config.Bind("Splinter", "EnableSplinterEffect", true, "Enable or disable the visual effect when logs are splintered.");
            EnableStumpDestroy = Config.Bind("Splinter", "EnableTreeTrunkDestroy", true, "Enable or disable the destruction of tree trunks.");

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

        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames()
                .SingleOrDefault(name => name.EndsWith(fileName));

            if (resourceName == null)
            {
                Log.LogError($"AssetBundle | Embedded resource not found: {fileName}");
                return null;
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Log.LogError($"AssetBundle | Failed to open resource: {resourceName}");
                    return null;
                }

                return AssetBundle.LoadFromStream(stream);
            }
        }
    }
}
