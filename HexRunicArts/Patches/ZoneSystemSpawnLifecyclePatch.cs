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
            StartTempleService.FindStartTemple();
        }
    }
}