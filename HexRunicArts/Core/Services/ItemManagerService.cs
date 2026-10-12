using HexRunicArts.Assets;
using HexRunicArts.Core.Constants;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;

namespace HexRunicArts.Core.Services
{
    internal static class ItemManagerService
    {
        internal static void Init()
        {
            if (AssetBundleHelper.EikthyrHeart == null)
            {
                return;
            }

            var heartItem = new CustomItem(AssetBundleHelper.EikthyrHeart, true, new ItemConfig
            {
                Name = ItemIdentifiers.EikthyrHeart,
                Description = ItemIdentifiers.EikthyrHeartDescription
            });

            ItemManager.Instance.AddItem(heartItem);
        }
    }
}
