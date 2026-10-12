using HexRunicArts.Assets;
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
                Name = "Eikthyr's Heart",
                Description = "The heart of the Horned One, still pulsing with the power of the storm."
            });

            ItemManager.Instance.AddItem(heartItem);
        }
    }
}
