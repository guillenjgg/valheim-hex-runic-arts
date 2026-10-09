using HexRunicArts.Core.Constants;
using UnityEngine;

namespace HexRunicArts.Core.Features
{
    internal static class TreeSplinterFeature
    {
        internal static void ActivateSplinterAbility(Player player)
        {
            var playerPosition = player.transform.position;
            var splinteredLogs = 0;

            foreach (var log in Object.FindObjectsByType<TreeLog>(FindObjectsSortMode.None))
            {
                foreach (var collider in log.GetComponentsInChildren<Collider>())
                {
                    if (!collider.enabled || collider.isTrigger)
                    {
                        continue;
                    }

                    var closestPoint = collider.ClosestPoint(playerPosition);
                    var distance = Vector3.Distance(playerPosition, closestPoint);

                    if (distance >= Plugin.SplinterRadius.Value)
                    {
                        continue;
                    }

                    var hit = new HitData();
                    hit.m_variant = DamageIdentifiers.SplinteringDamageID;
                    hit.m_damage.m_chop = 10000f;
                    hit.m_toolTier = 100;
                    hit.m_point = closestPoint;

                    log.Damage(hit);
                    splinteredLogs++;

                    break;
                }
            }

            if (splinteredLogs == 0)
            {
                player.Message(MessageHud.MessageType.Center, "No logs nearby");
                return;
            }
        }
    }
}
