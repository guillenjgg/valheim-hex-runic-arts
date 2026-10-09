using HarmonyLib;
using HexRunicArts.Core.Constants;
using HexRunicArts.Core.Services;

namespace HexRunicArts.Patches
{
    [HarmonyPatch(typeof(TreeLog), nameof(TreeLog.Destroy))]
    internal static class TreeLogPatch
    {
        [HarmonyPrefix]
        private static bool DestroyPrefix(TreeLog __instance, HitData hitData, bool cheatedTool)
        {
            if (hitData == null || hitData.m_variant != DamageIdentifiers.SplinteringDamageID)
            {
                return true;
            }

            SpawnDropService.SpawnDrops(__instance.m_dropWhenDestroyed, __instance.transform.position, __instance.transform, hitData, cheatedTool, __instance.m_spawnDistance);

            if (__instance.m_subLogPrefab != null && __instance.m_subLogPoints != null)
            {
                var subLog = __instance.m_subLogPrefab.GetComponent<TreeLog>();

                if (subLog != null)
                {
                    foreach (var point in __instance.m_subLogPoints)
                    {
                        if (point == null)
                        {
                            continue;
                        }

                        SpawnDropService.SpawnDrops(subLog.m_dropWhenDestroyed, point.position, point, hitData, cheatedTool, subLog.m_spawnDistance);
                    }
                }
            }

            ZNetScene.instance.Destroy(__instance.gameObject);

            return false;
        }
    }
}