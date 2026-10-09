using HarmonyLib;
using HexRunicArts.Core.Services;
using System.Linq;

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

            if (TreeStumpService.StumpPrefabHashKeys.Count > 0)
            {
                foreach (var key in TreeStumpService.StumpPrefabHashKeys)
                {
                    Plugin.Log.LogInfo($"Tree stump prefab hash key: {key}");
                }
            }
        }
    }
}
    