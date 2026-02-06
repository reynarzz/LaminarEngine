using Engine.Graphics;
using Newtonsoft.Json;
using Engine;
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
            var shaderSources = GetSources(reader);
            return new Shader(shaderSources, info.Path, guid);
        }

        internal override void UpdateAsset(AssetResourceBase asset, AssetMetaFileBase meta, BinaryReader reader)
        {
            var shaderSources = GetSources(reader);
            asset.UpdateResource(shaderSources, string.Empty, meta.GUID);
        }

        private ShaderSource[] GetSources(BinaryReader reader)
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
            return shaderSources;
        }
    }
}