using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace HexRunicArts.Core.Services
{
    internal static class StartTempleService
    {
        private const float RuneStoneDistance = 15f;

        private static readonly FieldInfo PrefabField = AccessTools.Field(typeof(ZoneSystem.ZoneLocation), "m_prefab");

        internal static bool IsInitialized { get; private set; }
        internal static Vector3 TemplePosition { get; private set; }
        internal static Vector3 RuneStonePosition { get; private set; }

        internal static void Reset()
        {
            IsInitialized = false;
            TemplePosition = Vector3.zero;
            RuneStonePosition = Vector3.zero;
        }

        internal static void FindStartTemple()
        {
            if (IsInitialized || ZoneSystem.instance == null || PrefabField == null || WorldGenerator.instance == null)
            {
                return;
            }

            foreach (var location in ZoneSystem.instance.GetLocationList())
            {
                if (location.m_location == null)
                {
                    continue;
                }

                var prefab = PrefabField.GetValue(location.m_location);

                if (prefab == null || !prefab.ToString().StartsWith("StartTemple.prefab"))
                {
                    continue;
                }

                TemplePosition = location.m_position;
                RuneStonePosition = TemplePosition + Vector3.forward * RuneStoneDistance;
                
                RuneStonePosition = new Vector3(
                    RuneStonePosition.x,
                    WorldGenerator.instance.GetHeight(RuneStonePosition),
                    RuneStonePosition.z
                );

                IsInitialized = true;

                return;
            }
        }
    }
}