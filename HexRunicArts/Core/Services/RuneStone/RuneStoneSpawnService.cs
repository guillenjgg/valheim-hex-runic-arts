using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HexRunicArts.Core.Services.RuneStone
{
    internal static class RuneStoneSpawnService
    {
        private const string LocationName = "Runestone_Meadows";
        private const float DuplicateDistance = 5f;
        private const string RuneStoneIdentifierKey = "HexRunicArts_RuneStoneId";
        private const string RuneStoneIdentifier = "StartTempleRuneStone";

        private static readonly FieldInfo LocationProxyPrefabField = AccessTools.Field(typeof(ZoneSystem), "m_locationProxyPrefab");
        private static readonly MethodInfo GetLocationMethod = AccessTools.Method(typeof(ZoneSystem), "GetLocation", new[] { typeof(int) });

        internal static bool TrySpawn(Vector3 position)
        {
            if (ZoneSystem.instance == null || ZDOMan.instance == null || ZNet.instance == null ||
                LocationProxyPrefabField == null || GetLocationMethod == null)
            {
                return false;
            }

            if (!ZNet.instance.IsServer())
            {
                return false;
            }

            var prefab = LocationProxyPrefabField.GetValue(ZoneSystem.instance) as GameObject;

            if (prefab == null)
            {
                Plugin.Log.LogWarning("RuneStone | LocationProxy prefab not found.");
                return false;
            }

            var locationHash = LocationName.GetStableHashCode();
            var location = (ZoneSystem.ZoneLocation)GetLocationMethod.Invoke(ZoneSystem.instance, new object[] { locationHash });

            if (location == null)
            {
                Plugin.Log.LogWarning($"RuneStone | Location not found: {LocationName}");
                return false;
            }

            var objects = new List<ZDO>();
            var sector = ZoneSystem.GetZone(position);

            ZDOMan.instance.FindSectorObjects(sector, new SimulationDistance(1, 0, false), objects);

            var proxyHash = prefab.name.GetStableHashCode();

            foreach (var zdo in objects)
            {
                if (zdo.GetPrefab() != proxyHash)
                {
                    continue;
                }

                if (zdo.GetInt(ZDOVars.s_location, 0) != locationHash)
                {
                    continue;
                }

                if (Vector3.Distance(zdo.GetPosition(), position) > DuplicateDistance)
                {
                    continue;
                }

                var existingIdentifier = zdo.GetString(RuneStoneIdentifierKey, "");

                if (string.IsNullOrEmpty(existingIdentifier))
                {
                    zdo.Set(RuneStoneIdentifierKey, RuneStoneIdentifier);

                    Plugin.Log.LogInfo(
                        $"RuneStone | Existing proxy tagged | ID: {RuneStoneIdentifier} | Position: {zdo.GetPosition()}"
                    );
                }
                else if (existingIdentifier != RuneStoneIdentifier)
                {
                    Plugin.Log.LogWarning(
                        $"RuneStone | Conflicting proxy identifier: {existingIdentifier} | Position: {zdo.GetPosition()}"
                    );

                    return false;
                }
                else
                {
                    Plugin.Log.LogInfo(
                        $"RuneStone | Existing proxy found | ID: {existingIdentifier} | Position: {zdo.GetPosition()}"
                    );
                }

                return true;
            }

            var proxy = ZDOMan.instance.CreateNewZDO(position, proxyHash);

            proxy.Persistent = true;
            proxy.Type = ZDO.ObjectType.Solid;
            proxy.Distant = false;
            proxy.SetPrefab(proxyHash);
            proxy.SetRotation(Quaternion.identity);
            proxy.Set(ZDOVars.s_location, locationHash, false);
            proxy.Set(ZDOVars.s_seed, 0, false);
            proxy.Set(RuneStoneIdentifierKey, RuneStoneIdentifier);

            Plugin.Log.LogInfo(
                $"RuneStone | Persistent location proxy created | ID: {RuneStoneIdentifier} | Position: {position}"
            );

            return true;
        }

        internal static bool Exists(Vector3 position)
        {
            if (ZoneSystem.instance == null || ZDOMan.instance == null || LocationProxyPrefabField == null)
            {
                return false;
            }

            var prefab = LocationProxyPrefabField.GetValue(ZoneSystem.instance) as GameObject;

            if (prefab == null)
            {
                return false;
            }

            var objects = new List<ZDO>();
            var sector = ZoneSystem.GetZone(position);

            ZDOMan.instance.FindSectorObjects(sector, new SimulationDistance(1, 0, false), objects);

            var proxyHash = prefab.name.GetStableHashCode();
            var locationHash = LocationName.GetStableHashCode();

            foreach (var zdo in objects)
            {
                if (zdo.GetPrefab() != proxyHash)
                {
                    continue;
                }

                if (zdo.GetInt(ZDOVars.s_location, 0) != locationHash)
                {
                    continue;
                }

                if (Vector3.Distance(zdo.GetPosition(), position) > DuplicateDistance)
                {
                    continue;
                }

                if (zdo.GetString(RuneStoneIdentifierKey, "") != RuneStoneIdentifier)
                {
                    continue;
                }

                Plugin.Log.LogInfo($"RuneStone | Existing runestone verified | Position: {zdo.GetPosition()}");

                return true;
            }

            return false;
        }
    }
}