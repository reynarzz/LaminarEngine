using Editor;
using Editor.Cooker;
using Newtonsoft.Json;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class DevModeDisk : DiskBase
    {
        public override bool Initialize()
        {
            AssetDatabaseInfo = JsonConvert.DeserializeObject<AssetsDatabaseInfo>(File.ReadAllText(Paths.GetAssetDatabaseFilePath()));

            return AssetDatabaseInfo != null;
        }

        private bool ExistsPhysicalAsset(Guid guid)
        {
            if (AssetDatabaseInfo.Assets.TryGetValue(guid, out var assetInfo))
            {
                return File.Exists(EditorPaths.GetAbsolutePathSafe(assetInfo.Path));
            }

            return false;
        }

        protected override byte[] LoadAssetFromDisk(Guid guid)
        {
            if (ExistsPhysicalAsset(guid))
            {
                return File.ReadAllBytes(Paths.CreateBinFilePath(Paths.GetAssetDatabaseFolder(), guid.ToString()));
            }

            return null;
        }

        protected override async Task<byte[]> LoadAssetFromDiskAsync(Guid guid)
        {
            if (ExistsPhysicalAsset(guid))
            {
                return await File.ReadAllBytesAsync(Paths.CreateBinFilePath(Paths.GetAssetDatabaseFolder(), guid.ToString()));
            }

            return null;
        }

        protected override byte[] LoadMetaFromDisk(Guid guid)
        {
            if (AssetDatabaseInfo.Assets.TryGetValue(guid, out var assetInfo))
            {
                return File.ReadAllBytes(EditorPaths.GetAbsolutePathSafe(assetInfo.Path + Paths.ASSET_META_EXT_NAME));
            }
            return null;
        }
    }
}