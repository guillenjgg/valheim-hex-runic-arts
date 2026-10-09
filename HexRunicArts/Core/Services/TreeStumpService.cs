using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HexRunicArts.Core.Services
{
    internal static class TreeStumpService
    {
        private static readonly FieldInfo NamedPrefabsField = AccessTools.Field(typeof(ZNetScene), "m_namedPrefabs");

        internal static HashSet<int> StumpPrefabHashKeys { get; private set; } = new HashSet<int>();

        internal static void Init(ZNetScene __instance)
        {
            StumpPrefabHashKeys.Clear();

            if(NamedPrefabsField == null)
            {
                return;
            }

            var namedPrefabs = NamedPrefabsField.GetValue(__instance) as Dictionary<int, GameObject>;

            if (namedPrefabs == null)
            {
                return;
            }

            foreach (var prefab in namedPrefabs.Values)
            {
                if (prefab == null)
                {
                    continue;
                }

                var tree = prefab.GetComponent<TreeBase>();

                if (tree == null || tree.m_stubPrefab == null)
                {
                    continue;
                }

                StumpPrefabHashKeys.Add(tree.m_stubPrefab.name.GetStableHashCode());
            }
        }

        internal static int DestroyNearbyStumps(Player player, short toolTier)
        {
            var destroyedStumps = 0;
            var playerPosition = player.transform.position;

            foreach (var stump in Object.FindObjectsByType<Destructible>(FindObjectsSortMode.None))
            {
                var nview = stump.GetComponent<ZNetView>();

                if (nview == null || !nview.IsValid())
                {
                    continue;
                }

                var zdo = nview.GetZDO();

                if (zdo == null || !StumpPrefabHashKeys.Contains(zdo.GetPrefab()))
                {
                    continue;
                }

                foreach (var collider in stump.GetComponentsInChildren<Collider>())
                {
                    if (!collider.enabled || collider.isTrigger)
                    {
                        continue;
                    }

                    var closestPoint = collider.ClosestPoint(playerPosition);

                    if (Vector3.Distance(playerPosition, closestPoint) >= Plugin.SplinterRadius.Value)
                    {
                        continue;
                    }

                    if (toolTier < stump.m_minToolTier)
                    {
                        break;
                    }

                    var hit = new HitData();
                    hit.m_damage.m_chop = 10000f;
                    hit.m_toolTier = toolTier;
                    hit.m_point = closestPoint;
                    hit.SetAttacker(player);

                    stump.Damage(hit);
                    destroyedStumps++;
                    break;
                }
            }

            return destroyedStumps;
        }
    }
}
