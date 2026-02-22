using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class TilemapLevelConfig
    {
        public Guid[] LayersTextureRef { get; set; }
    }
    internal class TilemapMeta : AssetMeta
    {
        public TilemapLevelConfig[] LevelConfig { get; set; }
    }
}
