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
            var shader = new Shader(shaderSources, meta.GUID);

            if (shader.HasErrors)
            {
                Debug.Error($"Shader error: {info.Path}, refId: {meta.GUID}");
            }

            return shader;
        }

        public void UpdateAsset(ref readonly AssetInfo info, Shader asset, AssetMeta meta, BinaryReader reader)
        {
            var shaderSources = GetSources(reader);
            asset.UpdateResource(shaderSources, meta.GUID);
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