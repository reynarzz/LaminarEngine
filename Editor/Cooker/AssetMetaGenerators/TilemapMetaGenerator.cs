using Engine;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class TilemapMetaGenerator : IAssetMetaGenerator
    {
        public AssetMeta GetDefaultMeta(BinaryReader reader)
        {
            var buff = new byte[reader.BaseStream.Length];
            reader.BaseStream.ReadExactly(buff);
            var json = Encoding.UTF8.GetString(buff);
            var project = LdtkJson.FromJson(json);

            var meta = new TilemapMeta();
            meta.Layers = new TilemapLayerConfig[project.Defs.Layers.Length];
            for (int i = 0; i < project.Defs.Layers.Length; i++)
            {
                meta.Layers[i] = new TilemapLayerConfig()
                {
                    EntityPixelPerUnit = 16,
                    TextureRef = Guid.Empty
                };
            }

            return meta;
        }
    }
}
