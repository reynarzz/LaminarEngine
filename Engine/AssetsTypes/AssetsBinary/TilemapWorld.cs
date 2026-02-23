using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class TilemapData
    {
        public uint Version { get; set; }
        public TilemapLevel[] Levels { get; set; }
    }

    public class TilemapLevel
    {
        public string Identifier { get; set; }
        public Guid IID { get; set; }
        public ivec2 WorldPositionPx { get; set; }
        public ivec2 SizePx { get; set; }
        public int Depth { get; set; }
        public Bounds Bounds { get; set; }
        public TilemapLevelLayer[] Layers { get; set; }
        public Color32 BackgroundColor { get; internal set; }
    }

    public enum TilemapLayerType
    {
        IntGrid,
        Entities,
        Tiles,
        AutoLayer
    }

    public class TilemapLevelLayer
    {
        public string Identifier { get; set; }
        public Guid IID { get; set; }
        public ivec2 SizeGridBased { get; set; }
        public int GridSize { get; set; }
        public float Opacity { get; set; }
        public ivec2 OffsetPx { get; set; }
        public Bounds Bounds { get; set; }
        public TilemapLayerType Type { get; set; }
        public vec2[] TilesPosition { get; set; }
        public TilemapEntity[] Entities { get; set; }
        public bool IsVisible { get; internal set; }
        internal Vertex[] Vertices { get; set; }
        internal int IndicesToDraw { get; set; }

        public ReadOnlySpan<vec2> GetTilesPosition()
        {
            return TilesPosition;
        }
    }

    public class TilemapEntity
    {
        public string Identifier { get; set; }

        // Instance identifier
        public Guid IID { get; set; }
        public Guid LayerIID { get; set; }
        public Guid LevelIID { get; set; }
        public vec2 Pivot { get; set; }
        public string[] Tags { get; set; }
        public ivec2 SizeInPixels { get; set; }
        public vec2 WorldPosition { get; set; }

        public EntityProperty[] Properties { get; set; }
    }

    public class EntityProperty
    {
        public string Identifier { get; set; }
        public EntityPropertyValue Value { get; set; }
    }

    public enum PropertyValueType
    {
        Unknown,
        String,
        Bool,
        Int,
        Float,
        Vec2,
        Enum,
        Color,
        EntityRef,
        Tile,

        StringArray,
        BoolArray,
        IntArray,
        FloatArray,
        Vec2Array,
        EnumArray,
        ColorArray,
        EntityRefArray,
        TileArray
    }

    public struct EntityPropertyValue
    {
        public PropertyValueType Type { get; set; }
        public int IntValue { get; set; }
        public int ColorValue { get; set; }
        public string StringValue { get; set; }
        public EnumValue EnumValue { get; set; }
        public float FloatValue { get; set; }
        public bool BoolValue { get; set; }
        public vec2 PointValue { get; set; }
        public EntityRef EntityRef { get; set; }
    }

    public struct EnumValue
    {
        public string EnumTitle { get; set; }
        public string EnumValStr { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EntityRef
    {
        public Guid entityIid;
        public Guid layerIid;
        public Guid levelIid;
        public Guid worldIid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TileRef
    {
        public int TilesetUid;
        public ivec2 SizePx;
        public ivec2 PositionPx;
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