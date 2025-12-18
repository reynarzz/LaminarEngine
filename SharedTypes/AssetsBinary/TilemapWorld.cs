using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    public struct TilemapVertex
    {
        public vec2 Position;
        public vec2 UV;
    }

    public class TilemapEntity
    {

        public vec2 WorldPosition { get; }
    }
    public class TilemapLevel
    {
        public int Index { get; }
        public int[] Layers { get; }
    }

    public class TilemapWorld
    {
        public TilemapEntity[] Entities { get; }
        public TilemapLayer[] Layers { get; }
        public TilemapLevel[] Levels { get; }
    }

    public class TilemapLayer
    {
        public int[] EntitiesIds { get; }
        public int Index { get; }
        public int LevelIndex { get; set; }
        public string Tileset { get; }
        public vec2[] TilesPositions { get; }
        public TilemapVertex[] Vertices { get; }
        public Box[] CollisionBoxes { get; }
        public vec2[] CollisionLines { get; }
        public Bounds Bounds { get; }
    }


    public struct Box
    {
        public vec2 Position;
        public vec2 Size;

        public Box(vec2 pos, vec2 size)
        {
            Position = pos;
            Size = size;
        }
    }

}