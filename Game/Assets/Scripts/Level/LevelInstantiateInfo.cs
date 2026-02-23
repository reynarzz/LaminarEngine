using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class TilemapInstanceData
    {
        public string Name { get; set; }
        public bool EnableCollision { get; set; }
        public vec2 ColliderOffset { get; set; }
        public bool IsTriggerCollision { get; set; }
        public int SortingOrder { get; set; }
        public int SpriteIndex { get; set; }
        public Material Material { get; set; }
        public int LayerIndex { get; set; }
        public Action<TilemapRenderer> TilemapAction { get; set; }
    }

    public class LevelInstantiateInfo
    {
        public int LevelIndex { get; set; }
        public Sprite[] TilemapSprites { get; set; }
        public List<TilemapInstanceData> Tilemaps { get; set; } = new List<TilemapInstanceData>();
        public int WorldSpacePixelsPerUnit { get; set; }
    }
}
