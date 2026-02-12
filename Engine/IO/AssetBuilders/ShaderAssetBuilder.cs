using Engine.Graphics;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Serialization;

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
            var shaderIR = BinaryIRDeserializer.DeserializeShader(reader);
            var shaderData = new ShaderData();
            Deserializer.Deserialize(shaderData, shaderIR.Properties);
            return shaderData.Sources;
        }
    }
}