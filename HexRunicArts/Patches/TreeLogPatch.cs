using HarmonyLib;
using HexRunicArts.Core.Constants;
using HexRunicArts.Core.Features;
using UnityEngine;

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

            SpawnDrops(__instance.m_dropWhenDestroyed, __instance.transform.position, __instance.transform, hitData, cheatedTool, __instance.m_spawnDistance);

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

                        SpawnDrops(subLog.m_dropWhenDestroyed, point.position, point, hitData, cheatedTool, subLog.m_spawnDistance);
                    }
                }
            }

            ZNetScene.instance.Destroy(__instance.gameObject);

            return false;
        }

        private static void SpawnDrops(
            DropTable dropTable,
            Vector3 origin,
            Transform direction,
            HitData hitData,
            bool cheatedTool,
            float spawnDistance)
        {
            var drops = dropTable.GetDropList();

            for (var i = 0; i < drops.Count; i++)
            {
                var position = origin
                    + direction.up * Random.Range(-spawnDistance, spawnDistance)
                    + Vector3.up * 0.3f * i;

                var rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
                var prefab = drops[i];
                var itemDrop = prefab.GetComponent<ItemDrop>();
                var count = 1;

                if (itemDrop != null)
                {
                    prefab = Game.instance.CheckDropConversion(hitData, itemDrop, prefab, ref count);
                }

                for (var j = 0; j < count; j++)
                {
                    ItemDrop.OnCreateNew(
                        Object.Instantiate(prefab, position, rotation),
                        cheatedTool
                    );
                }
            }
        }
    }
}