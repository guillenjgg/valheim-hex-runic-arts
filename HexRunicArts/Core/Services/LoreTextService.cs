namespace HexRunicArts.Services
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
            "Bring forth ten pieces of wood and five stones as an offering.";

        internal static string SplinteringUnlocked = 
            "The gods have accepted your offering.\n\n" +
            "The ancient knowledge of Splintering awakens within you." +
            $"To cast Splintering, fell a tree and with an axe equipped aim near the log and press {Plugin.SplinterHotkey.Value} to splinter the log.";;

        internal static string SplinteringAlreadyUnlocked =>
    "You have already mastered the ancient art of Splintering. The thunder of Thor now answers your call.\n\n" +
    $"To cast Splintering, fell a tree and with an axe equipped aim near the log and press {Plugin.SplinterHotkey.Value} to splinter the log.";

        internal static string GetSplinteringCollectionText(Player player)
        {
            var progress = LoreObjectiveService.GetSplinteringMaterialProgress(player);

            var wood = progress["$item_wood"];
            var stone = progress["$item_stone"];

            return
                "The gods await your offering.\n\n" +
                $"Wood: {wood.Current}/{wood.Required}\n" +
                $"Stone: {stone.Current}/{stone.Required}";
        }
    }
}