using Engine.Graphics;
using Newtonsoft.Json;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.IO
{
    internal class ShaderAssetBuilder : AssetBuilderBase
    {
        internal override AssetResourceBase BuildAsset(AssetInfo info, AssetMetaFileBase meta, Guid guid, BinaryReader reader)
        {
            var length = reader.BaseStream.Length;
            var data = new byte[length];
            var bytesRead = reader.BaseStream.Read(data, 0, (int)length);
            var text = Encoding.UTF8.GetString(data, 0, bytesRead);

            var shaderData = JsonConvert.DeserializeObject<ShaderData>(text);

            return new Shader(shaderData.Sources, info.Path, guid);
        }
    }
}