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

            var shortcut = Plugin.SplinterHotkey.Value;

            if (!ZInput.GetKeyDown(shortcut.MainKey, true))
            {
                return;
            }

            foreach (var modifier in shortcut.Modifiers)
            {
                if (!ZInput.GetKey(modifier, true))
                {
                    return;
                }
            }

            TreeSplinterFeature.ActivateSplinterAbility(__instance);
        }
    }
}