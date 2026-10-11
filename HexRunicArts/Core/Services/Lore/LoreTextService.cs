namespace HexRunicArts.Services.Lore
{
    internal static class LoreTextService
    {
        internal const string SplinteringIntroduction =
            "In the days before iron, when mortals first walked beneath the gaze of the gods, " +
            "Thor's thunder shook the roots of Yggdrasil.\n\n" +
            "From that divine fury, the ancient runemasters learned to awaken " +
            "the forces hidden within nature.\n\n" +
            "The first of these forgotten arts was Splintering. Through the axe, " +
            "they channeled the thunder of Thor, commanding the very fibers of timber " +
            "to fracture and yield.\n\n" +
            "Yet the gods do not grant their gifts freely.\n\n" +
            "Seek Eikthyr, the Horned One, whose antlers command the lightning of the storm. " +
            "Slay the beast and retrieve his heart, for within it lingers a spark of divine thunder.\n\n" +
            "Bring forth the heart of the Horned One as your offering.";

        internal static string SplinteringUnlocked =>
            "The gods have accepted your offering.\n\n" +
            "The thunder once wielded by the Horned One now answers to your axe. " +
            "The ancient knowledge of Splintering awakens within you.\n\n" +
            $"To cast Splintering, fell a tree and with an axe equipped aim near the log and press {Plugin.SplinterHotkey.Value} to splinter the log.";

        internal static string SplinteringAlreadyUnlocked =>
            "You have already mastered the ancient art of Splintering. The thunder of Thor now answers your call.\n\n" +
            $"To cast Splintering, fell a tree and with an axe equipped aim near the log and press {Plugin.SplinterHotkey.Value} to splinter the log.";

        internal static string GetSplinteringCollectionText(Player player)
        {
            var progress = LoreObjectiveService.GetSplinteringMaterialProgress(player);
            var heart = progress["Eikthyr's Heart"];

            return
                "The gods await your offering.\n\n" +
                "Retrieve the heart of the Horned One, " +
                "still charged with the fury of the storm.\n\n" +
                $"Eikthyr's Heart: {heart.Current}/{heart.Required}";
        }
    }
}