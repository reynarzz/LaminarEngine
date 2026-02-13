using Engine;
using Engine.Serialization;
using Editor.Serialization;
using System;
using System.Linq;
using System.Text;

namespace Editor.Cooker
{
    internal class ShaderProcessorRelease : ShaderAssetProcessor
    {
        private static ShaderData _shaderDataTemp = new ShaderData();

        protected override byte[] GetAsset(ShaderSource[] sources)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream, Encoding.UTF8);

            _shaderDataTemp.Sources = sources;
            var properties = Serializer.Serialize(_shaderDataTemp);

            if (properties.Length > 1)
            {
                throw new Exception($"Invalid shader property count: {properties.Length}, has to be '1'");
            }

            var ir = new ShaderIR()
            {
                Properties = properties
            };

            BinaryIRSerializer.Serialize(ir, writer);

            writer.Flush();
            
            return stream.ToArray();
        }
    }
}
