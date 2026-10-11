using HarmonyLib;
using HexRunicArts.Models;
using HexRunicArts.Services;

namespace HexRunicArts.Patches
{
    [HarmonyPatch(typeof(RuneStone), nameof(RuneStone.Interact))]
    internal static class RuneStoneInteractionPatch
    {
        private const string RuneStoneIdentifierKey = "HexRunicArts_RuneStoneId";
        private const string RuneStoneIdentifier = "StartTempleRuneStone";

        [HarmonyPrefix]
        private static bool InteractPrefix(RuneStone __instance, Humanoid character, bool hold, ref bool __result)
        {
            var nview = __instance.GetComponentInParent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return true;
            }

            var zdo = nview.GetZDO();

            if (zdo == null || zdo.GetString(RuneStoneIdentifierKey, "") != RuneStoneIdentifier)
            {
                return true;
            }

            if (hold)
            {
                __result = false;
                return false;
            }

            var player = character as Player;

            if (player == null || player != Player.m_localPlayer)
            {
                __result = false;
                return false;
            }

            var progress = LoreProgressionService.GetSplinteringProgress(player);
            var text = "";

            if (progress == SplinteringProgressModel.NotStarted)
            {
                LoreProgressionService.SetSplinteringProgress(
                    player,
                    SplinteringProgressModel.CollectingMaterials);

                text = LoreTextService.SplinteringIntroduction;
            }
            else if (progress == SplinteringProgressModel.CollectingMaterials)
            {
                if (LoreObjectiveService.TryConsumeSplinteringMaterials(player))
                {
                    LoreProgressionService.SetSplinteringProgress(
                        player,
                        SplinteringProgressModel.Unlocked);

                    text = LoreTextService.SplinteringUnlocked;
                }
                else
                {
                    text = LoreTextService.GetSplinteringCollectionText(player);
                }
            }
            else if (progress == SplinteringProgressModel.Unlocked)
            {
                text = LoreTextService.SplinteringUnlocked;
            }

            TextViewer.instance.ShowText(
                TextViewer.Style.Rune,
                "The First Rune",
                text,
                true);

            __result = false;
            return false;
        }
    }
}