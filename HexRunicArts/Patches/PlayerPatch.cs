using HarmonyLib;
using HexRunicArts.Core.Features;

namespace HexRunicArts.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.TakeInput))]
    internal static class PlayerPatch
    {
        [HarmonyPostfix]
        private static void TakeInputPostfix(Player __instance, bool __result)
        {
            if (!__result || __instance != Player.m_localPlayer)
            {
                return;
            }

            if (Plugin.SplinterHotkey.Value.IsDown())
            {
                TreeSplinterFeature.ActivateSplinterAbility(__instance);
            }
        }
    }
}