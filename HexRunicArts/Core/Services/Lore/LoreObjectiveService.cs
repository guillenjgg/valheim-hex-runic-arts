using HexRunicArts.Core.Constants;
using System.Collections.Generic;

namespace HexRunicArts.Services.Lore
{
    internal static class LoreObjectiveService
    {
        private static readonly Dictionary<string, int> SplinteringMaterials = new Dictionary<string, int>
        {
            { ItemIdentifiers.EikthyrHeart, 1 }
        };

        internal static bool HasRequiredMaterials(Player player)
        {
            if (player == null)
            {
                return false;
            }

            var inventory = player.GetInventory();

            foreach (var material in SplinteringMaterials)
            {
                if (inventory.CountItems(material.Key, matchWorldLevel: false) < material.Value)
                {
                    return false;
                }
            }

            return true;
        }

        internal static Dictionary<string, (int Current, int Required)> GetSplinteringMaterialProgress(Player player)
        {
            var progress = new Dictionary<string, (int Current, int Required)>();

            if (player == null)
            {
                return progress;
            }

            var inventory = player.GetInventory();

            foreach (var material in SplinteringMaterials)
            {
                var current = inventory.CountItems(material.Key, matchWorldLevel: false);

                progress[material.Key] = (current, material.Value);
            }

            return progress;
        }

        internal static bool TryConsumeSplinteringMaterials(Player player)
        {
            if (!HasRequiredMaterials(player))
            {
                return false;
            }

            var inventory = player.GetInventory();

            foreach (var material in SplinteringMaterials)
            {
                inventory.RemoveItem(material.Key, material.Value, worldLevelBased: false);
            }

            return true;
        }
    }
}