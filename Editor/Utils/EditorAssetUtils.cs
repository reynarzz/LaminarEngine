using Engine;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class EditorAssetUtils
    {
        internal static AssetMeta GetAssetMeta(AssetResourceBase asset)
        {
            var path = EditorPaths.GetAbsolutePathSafe(asset.Path) + Paths.ASSET_META_EXT_NAME;
            var meta = AssetUtils.GetMeta(path, EditorIOLayer.Database.GetAssetInfo(asset.GetID()).Type);
            return meta;
        }

        internal static AssetMeta GetAssetMeta(string asstPathRelative, AssetType type)
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
