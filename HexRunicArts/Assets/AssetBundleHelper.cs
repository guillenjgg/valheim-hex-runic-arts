using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HexRunicArts.Assets
{
    internal static class AssetBundleHelper
    {
        private const string AssetBundleName = "hexrunicarts";
        private static AssetBundle HexRunicArtsAssetBundle;
        private const string HeartPrefabPath = "Assets/_CustomItems/HexRunicArts/EikthyrHeart.prefab";

        internal static GameObject EikthyrHeart;

        internal static void LoadAssetBundle()
        {
            HexRunicArtsAssetBundle = GetAssetBundleFromResources(AssetBundleName);

            if (HexRunicArtsAssetBundle == null)
            {
                Plugin.Log.LogError($"AssetBundle | Failed to load: {AssetBundleName}");
                return;
            }

            EikthyrHeart = HexRunicArtsAssetBundle.LoadAsset<GameObject>(HeartPrefabPath);

            if (EikthyrHeart == null)
            {
                Plugin.Log.LogError($"AssetBundle | Prefab not found: {HeartPrefabPath}");
                return;
            }
        }

        private static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = assembly.GetManifestResourceNames()
                .SingleOrDefault(name => name.EndsWith(fileName));

            if (resourceName == null)
            {
                Plugin.Log.LogError($"AssetBundle | Embedded resource not found: {fileName}");
                return null;
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Plugin.Log.LogError($"AssetBundle | Failed to open resource: {resourceName}");
                    return null;
                }

                return AssetBundle.LoadFromStream(stream);
            }
        }
    }
}
