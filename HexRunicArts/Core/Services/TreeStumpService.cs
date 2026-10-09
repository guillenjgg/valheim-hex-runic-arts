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
    }
}
