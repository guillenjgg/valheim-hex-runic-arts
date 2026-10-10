using HarmonyLib;
using HexRunicArts.Core.Services;

namespace HexRunicArts.Patches
{
    [HarmonyPatch]
    internal static class ZoneSystemSpawnLifecyclePatch
    {
        [HarmonyPatch(typeof(ZoneSystem), "Load")]
        [HarmonyPostfix]
        private static void LoadPostfix()
        {
            Plugin.Log.LogInfo("[Lifecycle] ZoneSystem.Load completed.");

            StartTempleService.FindStartTemple();
        }
    }
}