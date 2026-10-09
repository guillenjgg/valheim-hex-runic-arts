//using HarmonyLib;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEngine;

//namespace HexRunicArts.Patches
//{
//    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
//    internal static class DebugPatch
//    {
//        private static readonly FieldInfo NamedPrefabsField = AccessTools.Field(typeof(ZNetScene), "m_namedPrefabs");

//        [HarmonyPostfix]
//        private static void AwakePostfix(ZNetScene __instance)
//        {
//            var namedPrefabs = NamedPrefabsField.GetValue(__instance) as Dictionary<int, GameObject>;

//            if (namedPrefabs == null)
//            {
//                return;
//            }

//            foreach (var prefab in namedPrefabs.Values)
//            {
//                if (prefab == null)
//                {
//                    continue;
//                }

//                var tree = prefab.GetComponent<TreeBase>();

//                if (tree == null || tree.m_stubPrefab == null)
//                {
//                    continue;
//                }

//                Plugin.Log.LogInfo($"Tree: {prefab.name} | Stump: {tree.m_stubPrefab.name}");
//            }
//        }
//    }
//}