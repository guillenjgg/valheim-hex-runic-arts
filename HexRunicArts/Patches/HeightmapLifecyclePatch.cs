using HarmonyLib;
using HexRunicArts.Core.Services;
using UnityEngine;

namespace HexRunicArts.Patches
{
    [HarmonyPatch]
    internal static class HeightmapLifecyclePatch
    {
        private static bool _placementValidated;

        internal static void Reset()
        {
            _placementValidated = false;
        }

        [HarmonyPatch(typeof(Heightmap), "Generate")]
        [HarmonyPostfix]
        private static void HeightmapGeneratePostfix(Heightmap __instance, bool __result)
        {
            if (!__result || _placementValidated || !StartTempleService.IsInitialized)
            {
                return;
            }

            var heightmapPosition = __instance.transform.position;
            var targetPosition = StartTempleService.RuneStonePosition;

            if (Mathf.Abs(heightmapPosition.x - targetPosition.x) > 64f ||
                Mathf.Abs(heightmapPosition.z - targetPosition.z) > 64f)
            {
                return;
            }

            if (RuneStoneSpawnService.Exists(targetPosition))
            {
                _placementValidated = true;
                return;
            }

            if (!RuneStonePlacementService.TryGetValidPosition(targetPosition, out var validPosition))
            {
                return;
            }

            Plugin.Log.LogInfo($"RuneStone | Placement validated | Position: {validPosition}");

            if (RuneStoneSpawnService.TrySpawn(validPosition))
            {
                _placementValidated = true;
            }
        }
    }
}