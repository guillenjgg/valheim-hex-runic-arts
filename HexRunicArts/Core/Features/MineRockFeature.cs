
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HexRunicArts.Core.Features
{
    internal static class MineRockFeature
    {
        private const float MiningDamage = 10000f;
        private const float MiningRadius = 20f;
        private const float OreCollectionRadius = 25f;
        private const float OreSpawnDelay = 0.5f;
        private const int SectionsPerFrame = 3;

        private static readonly FieldInfo RockNView = AccessTools.Field(typeof(MineRock5), "m_nview");
        private static readonly FieldInfo RockHitAreas = AccessTools.Field(typeof(MineRock5), "m_hitAreas");

        private static readonly FieldInfo DestructibleNView = AccessTools.Field(typeof(Destructible), "m_nview");
        private static readonly FieldInfo DestructibleDestroyed = AccessTools.Field(typeof(Destructible), "m_destroyed");
        private static readonly FieldInfo DestructibleMinToolTier = AccessTools.Field(typeof(Destructible), "m_minToolTier");
        private static readonly FieldInfo DestructibleSpawnWhenDestroyed = AccessTools.Field(typeof(Destructible), "m_spawnWhenDestroyed");

        private static readonly FieldInfo ItemDropInstances = AccessTools.Field(typeof(ItemDrop), "s_instances");
        private static readonly FieldInfo ItemDropNView = AccessTools.Field(typeof(ItemDrop), "m_nview");
        private static readonly FieldInfo ItemDropBody = AccessTools.Field(typeof(ItemDrop), "m_body");
        private static readonly FieldInfo ItemDropData = AccessTools.Field(typeof(ItemDrop), "m_itemData");

        private static readonly FieldInfo ItemDataDropPrefab = AccessTools.Field(typeof(ItemDrop.ItemData), "m_dropPrefab");

        private static bool _isMining;

        internal static int DestroyNearbyRocks(Player player, short toolTier)
        {
            if (player == null || _isMining)
            {
                return 0;
            }

            var position = player.transform.position;
            var destructibles = FindDestructibles(position, toolTier);
            var targets = FindTargets(position, toolTier);

            if (destructibles.Count == 0 && targets.Count == 0)
            {
                return 0;
            }

            var existingOre = GetExistingCopperOre();

            _isMining = true;
            player.StartCoroutine(ProcessMining(player, toolTier, destructibles, targets, existingOre));

            return destructibles.Count + targets.Count;
        }

        private static List<Destructible> FindDestructibles(Vector3 position, short toolTier)
        {
            var results = new List<Destructible>();

            foreach (var destructible in UnityEngine.Object.FindObjectsByType<Destructible>(FindObjectsSortMode.None))
            {
                if (destructible == null ||
                    Vector3.Distance(position, destructible.transform.position) > MiningRadius)
                {
                    continue;
                }

                var nview = RockView(destructible);

                if (nview == null || !nview.IsValid())
                {
                    continue;
                }

                if ((bool)DestructibleDestroyed.GetValue(destructible))
                {
                    continue;
                }

                if (toolTier < Convert.ToInt32(DestructibleMinToolTier.GetValue(destructible)))
                {
                    continue;
                }

                var prefab = DestructibleSpawnWhenDestroyed.GetValue(destructible) as GameObject;

                if (prefab == null || prefab.GetComponent<MineRock5>() == null)
                {
                    continue;
                }

                results.Add(destructible);
            }

            return results;
        }

        private static List<(MineRock5 Rock, Collider Collider)> FindTargets(Vector3 position, short toolTier)
        {
            var results = new List<(MineRock5 Rock, Collider Collider)>();

            foreach (var rock in UnityEngine.Object.FindObjectsByType<MineRock5>(FindObjectsSortMode.None))
            {
                if (rock == null || toolTier < rock.m_minToolTier)
                {
                    continue;
                }

                var nview = RockNView.GetValue(rock) as ZNetView;
                var areas = RockHitAreas.GetValue(rock) as List<MineRock5.HitArea>;

                if (nview == null || !nview.IsValid() || areas == null)
                {
                    continue;
                }

                foreach (var area in areas)
                {
                    if (area == null || area.m_health <= 0f)
                    {
                        continue;
                    }

                    var collider = area.m_collider;

                    if (collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    if (Vector3.Distance(position, collider.bounds.ClosestPoint(position)) > MiningRadius)
                    {
                        continue;
                    }

                    results.Add((rock, collider));
                }
            }

            return results;
        }

        private static ZNetView RockView(Destructible destructible)
        {
            return DestructibleNView.GetValue(destructible) as ZNetView;
        }

        private static List<ItemDrop> GetItemDrops()
        {
            return ItemDropInstances.GetValue(null) as List<ItemDrop>;
        }

        private static ZNetView GetItemDropView(ItemDrop drop)
        {
            return ItemDropNView.GetValue(drop) as ZNetView;
        }

        private static Rigidbody GetItemDropBody(ItemDrop drop)
        {
            return ItemDropBody.GetValue(drop) as Rigidbody;
        }

        private static bool IsCopperOre(ItemDrop drop)
        {
            if (drop == null)
            {
                return false;
            }

            var data = ItemDropData.GetValue(drop) as ItemDrop.ItemData;

            if (data == null)
            {
                return false;
            }

            var prefab = ItemDataDropPrefab.GetValue(data) as GameObject;

            return prefab != null && prefab.name == "CopperOre";
        }

        private static HashSet<ItemDrop> GetExistingCopperOre()
        {
            var existing = new HashSet<ItemDrop>();
            var drops = GetItemDrops();

            if (drops == null)
            {
                return existing;
            }

            foreach (var drop in drops)
            {
                if (IsCopperOre(drop))
                {
                    existing.Add(drop);
                }
            }

            return existing;
        }

        private static IEnumerator ProcessMining(
            Player player,
            short toolTier,
            List<Destructible> destructibles,
            List<(MineRock5 Rock, Collider Collider)> targets,
            HashSet<ItemDrop> existingOre)
        {
            try
            {
                var processed = 0;
                var spawnedNewRocks = false;

                foreach (var destructible in destructibles)
                {
                    if (player == null)
                    {
                        yield break;
                    }

                    if (destructible == null)
                    {
                        continue;
                    }

                    var nview = RockView(destructible);

                    if (nview == null || !nview.IsValid() ||
                        (bool)DestructibleDestroyed.GetValue(destructible))
                    {
                        continue;
                    }

                    var hit = CreateHit(player, toolTier, destructible.transform.position);

                    destructible.Damage(hit);
                    spawnedNewRocks = true;

                    processed++;

                    if (processed >= SectionsPerFrame)
                    {
                        processed = 0;
                        yield return null;
                    }
                }

                if (spawnedNewRocks)
                {
                    yield return null;

                    if (player == null)
                    {
                        yield break;
                    }

                    targets = FindTargets(player.transform.position, toolTier);
                }

                foreach (var target in targets)
                {
                    if (player == null)
                    {
                        yield break;
                    }

                    if (!IsValidTarget(target.Rock, target.Collider))
                    {
                        continue;
                    }

                    var point = target.Collider.bounds.ClosestPoint(player.transform.position);
                    var hit = CreateHit(player, toolTier, point);

                    hit.m_hitCollider = target.Collider;
                    target.Rock.Damage(hit);

                    processed++;

                    if (processed >= SectionsPerFrame)
                    {
                        processed = 0;
                        yield return null;
                    }
                }

                yield return new WaitForSeconds(OreSpawnDelay);

                if (player != null)
                {
                    CollectCopperOre(player, existingOre);
                }
            }
            finally
            {
                _isMining = false;
            }
        }

        private static bool IsValidTarget(MineRock5 rock, Collider collider)
        {
            if (rock == null || collider == null ||
                !collider.enabled || !collider.gameObject.activeInHierarchy)
            {
                return false;
            }

            var nview = RockNView.GetValue(rock) as ZNetView;

            if (nview == null || !nview.IsValid())
            {
                return false;
            }

            var areas = RockHitAreas.GetValue(rock) as List<MineRock5.HitArea>;

            if (areas == null)
            {
                return false;
            }

            foreach (var area in areas)
            {
                if (area != null && area.m_collider == collider)
                {
                    return area.m_health > 0f;
                }
            }

            return false;
        }

        private static void CollectCopperOre(Player player, HashSet<ItemDrop> existingOre)
        {
            var drops = GetItemDrops();

            if (drops == null)
            {
                return;
            }

            var playerPosition = player.transform.position;
            var moved = 0;

            foreach (var drop in drops)
            {
                if (!IsCopperOre(drop) || existingOre.Contains(drop))
                {
                    continue;
                }

                if (Vector3.Distance(drop.transform.position, playerPosition) > OreCollectionRadius)
                {
                    continue;
                }

                var nview = GetItemDropView(drop);

                if (nview == null || !nview.IsValid() || !nview.IsOwner())
                {
                    continue;
                }

                MoveItemDrop(drop, playerPosition + Vector3.up);
                moved++;
            }

            Plugin.Log.LogInfo($"Moved {moved} copper ore drops to player");
        }

        private static void MoveItemDrop(ItemDrop drop, Vector3 position)
        {
            var nview = GetItemDropView(drop);

            if (nview == null || !nview.IsValid() || !nview.IsOwner())
            {
                return;
            }

            var body = GetItemDropBody(drop);

            if (body != null)
            {
                body.position = position;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            drop.transform.position = position;
            nview.GetZDO().SetPosition(position);
        }

        private static HitData CreateHit(Player player, short toolTier, Vector3 point)
        {
            var hit = new HitData();
            hit.m_damage.m_pickaxe = MiningDamage;
            hit.m_toolTier = toolTier;
            hit.m_point = point;
            hit.m_dir = (point - player.transform.position).normalized;
            hit.SetAttacker(player);

            return hit;
        }
    }
}
