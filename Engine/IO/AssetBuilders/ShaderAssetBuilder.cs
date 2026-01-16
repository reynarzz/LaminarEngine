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
            ShaderSource[] shaderSources = null;

            if (length > 0)
            {
                var data = new byte[length];
                var bytesRead = reader.BaseStream.Read(data, 0, (int)length);
                var text = Encoding.UTF8.GetString(data, 0, bytesRead);

                if (!string.IsNullOrEmpty(text))
                {
                    var shaderData = JsonConvert.DeserializeObject<ShaderData>(text);
                    shaderSources = shaderData.Sources;
                }
            }

            return new Shader(shaderSources, info.Path, guid);
        }
    }
}