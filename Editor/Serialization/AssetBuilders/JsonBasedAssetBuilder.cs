using Editor.Utils;
using Engine;
using Engine.IO;
using Engine.Serialization;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal abstract class JsonBasedAssetBuilder<T> : AssetBuilderBase
    {
        internal override AssetResourceBase BuildAsset(AssetInfo info, AssetMetaFileBase meta, Guid guid, BinaryReader reader)
        {
            var length = reader.BaseStream.Length;
            var data = new byte[length];
            int bytesRead = reader.BaseStream.Read(data, 0, (int)length);
            string text = Encoding.UTF8.GetString(data, 0, bytesRead);

            var assetInstance = Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic,
                                                         null, [info.Path, guid], null);
            var ir = EditorJsonUtils.Deserialize<List<SerializedPropertyData>>(text);

            Deserializer.Deserialize(assetInstance, ir);

            return assetInstance as AssetResourceBase;
        }
    }
}
