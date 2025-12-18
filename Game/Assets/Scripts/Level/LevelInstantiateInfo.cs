using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class TilemapData
    {
        public string Name { get; set; }
        public bool EnableCollision { get; set; }
        public vec2 ColliderOffset { get; set; }
        public bool IsTriggerCollision { get; set; }
        public int SortingOrder { get; set; }
        public int SpriteIndex { get; set; }
        public Material Material { get; set; }
        public ulong LayersToDraw { get; set; }
    }

    public class LevelInstantiateInfo
    {
        public int LevelIndex { get; set; }
        public Sprite[] TilemapSprites { get; set; }
        public List<TilemapData> Tilemaps { get; set; } = new List<TilemapData>();
        public int WorldSpacePixelsPerUnit { get; set; }
    }
}
