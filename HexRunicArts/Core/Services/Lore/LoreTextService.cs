using HexRunicArts.Core.Constants;

namespace HexRunicArts.Services.Lore
{
    internal static class LoreTextService
    {
        internal static string SplinteringIntroduction =>
            Localization.instance.Localize("$hexrunicarts_lore_splintering_introduction");

        internal static string SplinteringUnlocked =>
            string.Format(
                Localization.instance.Localize("$hexrunicarts_lore_splintering_unlocked"),
                Plugin.SplinterHotkey.Value);

        internal static string SplinteringAlreadyUnlocked =>
            string.Format(
                Localization.instance.Localize("$hexrunicarts_lore_splintering_already_unlocked"),
                Plugin.SplinterHotkey.Value);

        internal static string GetSplinteringCollectionText(Player player)
        {
            var progress = LoreObjectiveService.GetSplinteringMaterialProgress(player);
            var heart = progress[ItemIdentifiers.EikthyrHeart];
            var heartName = Localization.instance.Localize(ItemIdentifiers.EikthyrHeart);

            return string.Format(
                Localization.instance.Localize("$hexrunicarts_lore_splintering_collection"),
                heartName,
                heart.Current,
                heart.Required);
        }
    }
}