using Engine;
using Engine.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal interface IAssetBuilder
    {
        internal Asset BuildAsset(ref readonly AssetInfo info, AssetMeta meta, BinaryReader reader);
        internal void UpdateAsset(ref readonly AssetInfo info, Asset asset, AssetMeta meta, BinaryReader reader);
    }

    internal interface IAssetBuilder<TAsset, TMeta> : IAssetBuilder where TAsset : Asset where TMeta : AssetMeta
    {
        Asset IAssetBuilder.BuildAsset(ref readonly AssetInfo info, AssetMeta meta, BinaryReader reader)
        {
            return BuildAsset(in info, meta as TMeta, reader);
        }
        void IAssetBuilder.UpdateAsset(ref readonly AssetInfo info, Asset asset, AssetMeta meta, BinaryReader reader)
        {
            UpdateAsset(in info, asset as TAsset, meta as TMeta, reader);
        }
        protected TAsset BuildAsset(ref readonly AssetInfo info, TMeta meta, BinaryReader reader);
        protected void UpdateAsset(ref readonly AssetInfo info, TAsset asset, TMeta meta, BinaryReader reader);
    }
}