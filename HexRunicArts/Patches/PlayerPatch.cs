using HarmonyLib;
using HexRunicArts.Core.Features;
using HexRunicArts.Core.GUI;
using HexRunicArts.Models;
using HexRunicArts.Services.Lore;

namespace HexRunicArts.Patches
{
    [HarmonyPatch]
    internal static class PlayerPatch
    {
        private static Inventory _subscribedInventory;

        [HarmonyPatch(typeof(Player), nameof(Player.TakeInput))]
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

        [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned), new[] { typeof(bool) })]
        [HarmonyPostfix]
        private static void OnSpawnedPostfix(Player __instance, bool spawnValkyrie)
        {
            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            SubscribeToInventory(__instance);

            if (spawnValkyrie)
            {
                return;
            }

            RunicQuestTrackerGui.Show();
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnDestroy))]
        [HarmonyPrefix]
        private static void OnDestroyPrefix(Player __instance)
        {
            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            UnsubscribeFromInventory();
            RunicQuestTrackerGui.Hide();
        }

        private static void SubscribeToInventory(Player player)
        {
            var inventory = player.GetInventory();

            if (inventory == null || _subscribedInventory == inventory)
            {
                return;
            }

            UnsubscribeFromInventory();

            _subscribedInventory = inventory;
            _subscribedInventory.m_onChanged += OnInventoryChanged;
        }

        private static void UnsubscribeFromInventory()
        {
            if (_subscribedInventory == null)
            {
                return;
            }

            _subscribedInventory.m_onChanged -= OnInventoryChanged;
            _subscribedInventory = null;
        }

        private static void OnInventoryChanged()
        {
            var player = Player.m_localPlayer;

            if (player == null || _subscribedInventory != player.GetInventory())
            {
                return;
            }

            if (LoreProgressionService.GetSplinteringProgress(player) != SplinteringProgressModel.CollectingMaterials)
            {
                return;
            }

            RunicQuestTrackerGui.UpdateQuest(player);
        }
    }
}
