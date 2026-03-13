using Editor.Utils;
using Engine;
using Engine.IO;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal abstract class JsonBasedAssetBuilder<TAsset, TMeta, TTypeIR> : IAssetBuilder<TAsset, TMeta> where TAsset : AssetResourceBase
                                                                                                         where TMeta : AssetMeta
                                                                                                         where TTypeIR : TypeIRBase
    {
        TAsset IAssetBuilder<TAsset, TMeta>.BuildAsset(ref readonly AssetInfo info, TMeta meta, BinaryReader reader)
        {
            var length = reader.BaseStream.Length;
            var data = new byte[length];
            int bytesRead = reader.BaseStream.Read(data, 0, (int)length);
            string text = Encoding.UTF8.GetString(data, 0, bytesRead);

            var assetInstance = Activator.CreateInstance(typeof(TAsset), BindingFlags.Instance | BindingFlags.NonPublic,
                                                         null, [info.Path, meta.GUID], null);
            var ir = EditorJsonUtils.Deserialize<TTypeIR>(text);

            if(ir != default)
            {
                Deserializer.Deserialize(assetInstance, ir.Properties);
            }

            return assetInstance as TAsset;
        }
        public abstract void UpdateAsset(ref readonly AssetInfo info, TAsset asset, TMeta meta, BinaryReader reader);
    }
}
