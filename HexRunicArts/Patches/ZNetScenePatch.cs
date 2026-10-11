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

            var eikthyrPrefab = __instance.GetPrefab("Eikthyr");

            if (eikthyrPrefab != null)
            {
                CreatureDropService.RegisterEikthyrDrops(eikthyrPrefab);
            }
            else
            {
                Plugin.Log.LogWarning("CreatureDrop | Eikthyr prefab not found.");
            }
        }
    }
}