using HarmonyLib;
using HexRunicArts.Core.Features;
using HexRunicArts.Models;
using HexRunicArts.Services;

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
                if (LoreProgressionService.GetSplinteringProgress(__instance) != SplinteringProgressModel.Unlocked)
                {
                    __instance.Message(MessageHud.MessageType.Center, "Splintering has not been unlocked");
                    return;
                }

                TreeSplinterFeature.ActivateSplinterAbility(__instance);
            }
        }
    }
}