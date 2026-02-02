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
            var path = EditorPaths.GetAbsolutePathSafe(asset.Path) + Paths.ASSET_META_EXT_NAME;
            var meta = AssetUtils.GetMeta(path, EditorIOLayer.Database.GetAssetInfo(asset).Type);
            return meta;
        }

        internal static AssetMetaFileBase GetAssetMeta(string asstPathRelative, AssetType type)
        {
            var path = EditorPaths.GetAbsolutePathSafe(asstPathRelative) + Paths.ASSET_META_EXT_NAME;
            var meta = AssetUtils.GetMeta(path, type);
            return meta;
        }

        internal static void RefreshAssetDatabase()
        {
            EditorIOLayer.Instance.Refresh();
        }
    }
}
