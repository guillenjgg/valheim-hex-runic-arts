//using HarmonyLib;
//using System;
//using System.Collections.Generic;

//namespace HexRunicArts.Patches
//{
//    [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.Load))]
//    internal static class ZoneSystemLocationDebugPatch
//    {
//        [HarmonyPostfix]
//        private static void ZoneSystemLoadPostfix()
//        {
//            var zoneSystem = ZoneSystem.instance;

//            if (zoneSystem == null)
//            {
//                return;
//            }

//            var field = AccessTools.Field(typeof(ZoneSystem), "m_locations");
//            var locations = field?.GetValue(zoneSystem) as List<ZoneSystem.ZoneLocation>;

//            if (locations == null)
//            {
//                Plugin.Log.LogWarning("ZoneSystem | Location registry unavailable.");
//                return;
//            }

//            Plugin.Log.LogInfo($"ZoneSystem | Registered locations: {locations.Count}");

//            var invalidCount = 0;

//            foreach (var location in locations)
//            {
//                if (location == null)
//                {
//                    continue;
//                }

//                try
//                {
//                    var name = location.m_prefab.Name;

//                    if (name.IndexOf("runestone", StringComparison.OrdinalIgnoreCase) >= 0 ||
//                        name.IndexOf("runestone", StringComparison.OrdinalIgnoreCase) >= 0)
//                    {
//                        Plugin.Log.LogInfo($"ZoneSystem | Runestone location: {name} | Hash: {location.Hash}");
//                    }
//                }
//                catch (Exception)
//                {
//                    invalidCount++;
//                }
//            }

//            Plugin.Log.LogInfo($"ZoneSystem | Invalid location references: {invalidCount}");
//        }
//    }
//}