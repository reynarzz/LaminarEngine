using Engine;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class EditorAssetUtils
    {
        internal static AssetMetaFileBase GetAssetMeta(AssetResourceBase asset)
        {
            var path = Paths.GetAbsoluteAssetPath(asset.Path) + Paths.ASSET_META_EXT_NAME;
            var meta = AssetUtils.GetMeta(path, EditorIOLayer.Database.GetAssetInfo(asset).Type) as TextureMetaFile;
            return meta;
        }
    }
}
