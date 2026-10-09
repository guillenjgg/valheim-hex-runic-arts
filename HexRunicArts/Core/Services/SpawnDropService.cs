using UnityEngine;

namespace HexRunicArts.Core.Services
{
    internal static class SpawnDropService
    {
        internal static void SpawnDrops(DropTable dropTable, Vector3 origin, Transform direction, HitData hitData, bool cheatedTool, float spawnDistance)
        {
            var drops = dropTable.GetDropList();

            for (var i = 0; i < drops.Count; i++)
            {
                var position = origin
                    + direction.up * Random.Range(-spawnDistance, spawnDistance)
                    + Vector3.up * 0.3f * i;

                var rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
                var prefab = drops[i];
                var itemDrop = prefab.GetComponent<ItemDrop>();
                var count = 1;

                if (itemDrop != null)
                {
                    prefab = Game.instance.CheckDropConversion(hitData, itemDrop, prefab, ref count);
                }

                for (var j = 0; j < count; j++)
                {
                    ItemDrop.OnCreateNew(Object.Instantiate(prefab, position, rotation), cheatedTool);
                }
            }
        }
    }
}
