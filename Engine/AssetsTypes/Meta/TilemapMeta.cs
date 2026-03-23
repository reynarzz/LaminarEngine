using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal struct TilemapLayerConfig
    {
        [SerializedField] public int EntityPixelPerUnit;
        [SerializedField] public SerializableGuid TextureRef;
    }
    internal class TilemapMeta : AssetMeta
    {
        [SerializedField] public TilemapLayerConfig[] Layers { get; set; }
    }
}