using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace HexRunicArts.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.TakeInput))]
    internal static class PlayerPatch
    {
        private const float SplinteringRadius = 10f;

        internal const short SplinteringVariant = 12345;

        [HarmonyPostfix]
        private static void TakeInputPostfix(Player __instance, bool __result)
        {
            if (!__result || __instance != Player.m_localPlayer)
            {
                return;
            }

            var shortcut = Plugin.SplinterHotkey.Value;

            if (!ZInput.GetKeyDown(shortcut.MainKey, true))
            {
                return;
            }

            foreach (var modifier in shortcut.Modifiers)
            {
                if (!ZInput.GetKey(modifier, true))
                {
                    return;
                }
            }

            var playerPosition = __instance.transform.position;
            var closestDistance = SplinteringRadius;
            TreeLog closestLog = null;

            foreach (var log in Object.FindObjectsByType<TreeLog>(FindObjectsSortMode.None))
            {
                var colliders = log.GetComponentsInChildren<Collider>();

                foreach (var collider in colliders)
                {
                    if (!collider.enabled || collider.isTrigger)
                    {
                        continue;
                    }

                    var closestPoint = collider.ClosestPoint(playerPosition);
                    var distance = Vector3.Distance(playerPosition, closestPoint);

                    if (distance >= closestDistance)
                    {
                        continue;
                    }

                    closestLog = log;
                    closestDistance = distance;
                }
            }

            if (closestLog == null)
            {
                __instance.Message(MessageHud.MessageType.Center, "No logs nearby");
                return;
            }

            var hit = new HitData();
            hit.m_variant = SplinteringVariant;
            hit.m_damage.m_chop = 10000f;
            hit.m_toolTier = 100;
            hit.m_point = closestLog.transform.position;

            closestLog.Damage(hit);
        }
    }
}