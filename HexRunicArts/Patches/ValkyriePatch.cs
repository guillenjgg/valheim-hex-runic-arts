using HarmonyLib;
using HexRunicArts.Core.GUI;

namespace HexRunicArts.Patches
{
    [HarmonyPatch(typeof(Valkyrie), nameof(Valkyrie.DropPlayer))]
    internal static class ValkyriePatch
    {
        [HarmonyPostfix]
        private static void DropPlayerPostfix()
        {
            if (Player.m_localPlayer == null)
            {
                return;
            }

            RunicQuestTrackerGui.Show();
        }
    }
}