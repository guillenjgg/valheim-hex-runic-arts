using HarmonyLib;
using HexRunicArts.Core.Constants;
using HexRunicArts.Core.Services;
using System.Reflection;
using UnityEngine;

namespace HexRunicArts.Core.Features
{
    internal static class TreeSplinterFeature
    {
        private static readonly MethodInfo GetRightItemMethod = AccessTools.Method(typeof(Humanoid), "GetRightItem");

        internal static void ActivateSplinterAbility(Player player)
        {
            var weapon = GetRightItemMethod?.Invoke(player, null) as ItemDrop.ItemData;

            if (weapon == null ||
                weapon.m_shared.m_skillType != Skills.SkillType.Axes ||
                weapon.m_shared.m_damages.m_chop <= 0f)
            {
                player.Message(MessageHud.MessageType.Center, "Equip an axe to use Splinter");
                return;
            }

            var toolTier = (short)weapon.m_shared.m_toolTier;
            var playerPosition = player.transform.position;
            var splinteredLogs = 0;
            var insufficientTier = false;

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

                    if (toolTier < log.m_minToolTier)
                    {
                        insufficientTier = true;
                        break;
                    }

                    var hit = new HitData();
                    hit.m_variant = DamageIdentifiers.SplinteringDamageID;
                    hit.m_damage.m_chop = 10000f;
                    hit.m_toolTier = toolTier;
                    hit.m_point = closestPoint;
                    hit.SetAttacker(player);

                    log.Damage(hit);
                    splinteredLogs++;

                    break;
                }
            }

            if (insufficientTier)
            {
                player.Message(MessageHud.MessageType.Center, "Your axe is too weak to splinter some nearby logs");
            }
            else if (splinteredLogs == 0)
            {
                player.Message(MessageHud.MessageType.Center, "No logs nearby");
            }

            if (splinteredLogs > 0 && Plugin.EnableSplinterEffect.Value)
            {
                SplinterEffectService.Spawn(playerPosition);
            }
        }

        internal static void OnLogSplintered()
        {
            Player.m_localPlayer?.RaiseSkill(Skills.SkillType.WoodCutting, 0.5f);
        }
    }
}