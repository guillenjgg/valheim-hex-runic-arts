//using HarmonyLib;
//using HexRunicArts.Core.Constants;
//using System.Collections.Generic;

//namespace HexRunicArts.Patches
//{
//    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
//    internal static class DebugPatch
//    {
//        [HarmonyPostfix]
//        private static void AwakePostfix(ObjectDB __instance)
//        {
//            var variants = new Dictionary<short, List<string>>();

//            foreach (var prefab in __instance.m_items)
//            {
//                if (prefab == null)
//                {
//                    continue;
//                }

//                var itemDrop = prefab.GetComponent<ItemDrop>();

//                if (itemDrop == null)
//                {
//                    continue;
//                }

//                var variant = itemDrop.m_itemData.m_shared.m_hitVariant;

//                if (!variants.TryGetValue(variant, out var prefabs))
//                {
//                    prefabs = new List<string>();
//                    variants[variant] = prefabs;
//                }

//                prefabs.Add(prefab.name);
//            }

//            Plugin.Log.LogInfo("========== HIT VARIANT DEBUG ==========");

//            foreach (var entry in variants)
//            {
//                Plugin.Log.LogInfo($"Hit Variant: {entry.Key} | Prefabs: {string.Join(", ", entry.Value)}");
//            }

//            if (variants.TryGetValue(DamageIdentifiers.SplinteringDamageID, out var collisions))
//            {
//                Plugin.Log.LogWarning($"Splintering damage ID collision detected! Prefabs: {string.Join(", ", collisions)}");
//            }
//            else
//            {
//                Plugin.Log.LogInfo($"No registered item prefab uses SplinteringDamageID ({DamageIdentifiers.SplinteringDamageID}).");
//            }

//            Plugin.Log.LogInfo("=======================================");
//        }
//    }
//}