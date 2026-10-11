using HexRunicArts.Models;
using System;

namespace HexRunicArts.Services
{
    internal static class LoreProgressionService
    {
        private const string SplinteringProgressKey = "HexRunicArts_SplinteringProgress";

        internal static SplinteringProgressModel GetSplinteringProgress(Player player)
        {
            if (player == null)
            {
                return SplinteringProgressModel.NotStarted;
            }

            if (!player.m_customData.TryGetValue(SplinteringProgressKey, out var value))
            {
                return SplinteringProgressModel.NotStarted;
            }

            if (int.TryParse(value, out var progress) &&
                Enum.IsDefined(typeof(SplinteringProgressModel), progress))
            {
                return (SplinteringProgressModel)progress;
            }

            return SplinteringProgressModel.NotStarted;
        }

        internal static void SetSplinteringProgress(Player player, SplinteringProgressModel progress)
        {
            if (player == null)
            {
                return;
            }

            player.m_customData[SplinteringProgressKey] = ((int)progress).ToString();

            Plugin.Log.LogInfo($"Lore | Splintering progress updated: {progress}");
        }

        internal static void ResetSplinteringProgress(Player player)
        {
            if (player == null)
            {
                return;
            }

            if (player.m_customData.Remove(SplinteringProgressKey))
            {
                Plugin.Log.LogInfo("Lore | Splintering progress reset.");
            }
        }
    }
}