using Engine.Graphics;
using Newtonsoft.Json;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.IO
{
    internal class ShaderAssetBuilder : IAssetBuilder<Shader, AssetMeta>
    {
        public Shader BuildAsset(ref readonly AssetInfo info, AssetMeta meta, BinaryReader reader)
        {
            var shaderSources = GetSources(reader);
            return new Shader(shaderSources, info.Path, meta.GUID);
        }

        public void UpdateAsset(ref readonly AssetInfo info, Shader asset, AssetMeta meta, BinaryReader reader)
        {
            var shaderSources = GetSources(reader);
            asset.UpdateResource(shaderSources, info.Path, meta.GUID);
        }

        private ShaderSource[] GetSources(BinaryReader reader)
        {
            var length = (int)reader.BaseStream.Length;
            ShaderSource[] shaderSources = null;

            if (length > 0)
            {
                var data = new byte[length];
                reader.BaseStream.ReadExactly(data, 0, length);
                var text = Encoding.UTF8.GetString(data, 0, length);

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