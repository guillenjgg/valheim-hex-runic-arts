using HexRunicArts.Assets;
using UnityEngine;

namespace HexRunicArts.Core.Services
{
    internal static class CreatureDropService
    {
        internal static void RegisterEikthyrDrops(GameObject eikthyrPrefab)
        {
            AddDrop(eikthyrPrefab, AssetBundleHelper.EikthyrHeart);
        }

        private static void AddDrop(GameObject creaturePrefab, GameObject itemPrefab)
        {
            if (creaturePrefab == null || itemPrefab == null)
            {
                return;
            }

            var characterDrop = creaturePrefab.GetComponent<CharacterDrop>();

            if (characterDrop == null)
            {
                return;
            }

            foreach (var drop in characterDrop.m_drops)
            {
                if (drop.m_prefab == itemPrefab)
                {
                    return;
                }
            }

            characterDrop.m_drops.Add(new CharacterDrop.Drop
            {
                m_prefab = itemPrefab,
                m_amountMin = 1,
                m_amountMax = 1,
                m_chance = 1f,
                m_onePerPlayer = false,
                m_levelMultiplier = false,
                m_dontScale = true
            });
        }
    }
}