using HarmonyLib;
using HexRunicArts.Core.Services;

namespace HexRunicArts.Patches
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    internal static class ZNetScenePatch
    {
        [HarmonyPostfix]
        private static void Postfix(ZNetScene __instance)
        {
            if (__instance == null)
            {
                return;
            }

            TreeStumpService.Init(__instance);
        }
    }
}
    