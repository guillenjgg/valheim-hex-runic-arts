using UnityEngine;

namespace HexRunicArts.Core.Services
{
    internal static class SplinterEffectService
    {
        private const string LightningPrefabName = "lightningAOE";

        internal static void Spawn(Vector3 position)
        {
            if (ZNetScene.instance == null)
            {
                return;
            }

            var prefab = ZNetScene.instance.GetPrefab(LightningPrefabName);

            if (prefab == null)
            {
                return;
            }

            var effect = Object.Instantiate(prefab, position, Quaternion.identity);

            foreach (var aoe in effect.GetComponentsInChildren<Aoe>(true))
            {
                foreach (var collider in aoe.GetComponents<Collider>())
                {
                    if (collider.isTrigger)
                    {
                        collider.enabled = false;
                    }
                }

                aoe.enabled = false;
            }
        }
    }
}
