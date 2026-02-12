using Engine.Graphics;
using Newtonsoft.Json;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Serialization;
using Engine.IO;
using Editor.Utils;

namespace Editor.Serialization
{
    internal class ShaderAssetBuilderEditor : IAssetBuilder<Shader, AssetMeta>
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
                    //var ir = EditorJsonUtils.Deserialize<ShaderIR>(text);
                    //var shaderData = new ShaderData();
                    //Deserializer.Deserialize(shaderData, ir.Properties);
                    var shaderData = JsonConvert.DeserializeObject<ShaderData>(text);
                    shaderSources = shaderData.Sources;
                }
            }
            return shaderSources;
        }
    }
}