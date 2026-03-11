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
        public Dictionary<Guid, TilemapLevel> Levels { get; set; }
    }

    public class TilemapLevel
    {
        public int LevelIndex { get; set; }
        public string Identifier { get; set; }
        public Guid IID { get; set; }
        public ivec2 WorldPositionPx { get; set; }
        public ivec2 SizePx { get; set; }
        public int Depth { get; set; }
        public Bounds Bounds { get; set; }
        public Dictionary<Guid, TilemapLevelLayer> Layers { get; set; }
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
        public int LayerIndex { get; set; }
        public string Identifier { get; set; }
        public Guid IID { get; set; }
        public ivec2 SizeGridBased { get; set; }
        public int GridSize { get; set; }
        public float Opacity { get; set; }
        public ivec2 OffsetPx { get; set; }
        public Bounds Bounds { get; set; }
        public TilemapLayerType Type { get; set; }
        internal vec2[] TilesPosition { get; set; }
        internal Box[] CollisionBoxes { get; set; }
        public Dictionary<Guid, TilemapEntity> Entities { get; set; }
        public bool IsVisible { get; internal set; }
        internal TilemapVertex[] Vertices { get; set; }
        internal int IndicesToDraw { get; set; }

        public ReadOnlySpan<vec2> GetTilesPosition()
        {
            return TilesPosition;
        }
    }

    public class TilemapEntity
    {
        public string Identifier { get; set; }
        public Guid IID { get; set; }
        public Guid LayerIID { get; set; }
        public Guid LevelIID { get; set; }
        public vec2 Pivot { get; set; }
        public string[] Tags { get; set; }
        public ivec2 SizeInPixels { get; set; }
        public vec2 WorldPosition { get; set; }

        public Dictionary<string, EntityPropertyData> Properties { get; set; }
    }

    public class EntityPropertyData
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

    public class EntityPropertyValue
    {
        public PropertyValueType Type { get; set; }
        public string String { get; set; }
        public bool Bool { get; set; }
        public int Int { get; set; }
        public float Float { get; set; }
        public vec2 Vec2 { get; set; }
        public EnumValue Enum { get; set; }
        public Color32 Color { get; set; }
        public EntityRef EntityRef { get; set; }
        public TileRef Tile { get; set; }
        public string[] StringArray { get; set; }
        public bool[] BoolArray { get; set; }
        public int[] IntArray { get; set; }
        public float[] FloatArray { get; set; }
        public vec2[] Vec2Array { get; set; }
        public EnumValue[] EnumArray { get; set; }
        public Color32[] ColorArray { get; set; }
        public EntityRef[] EntityRefArray { get; set; }
        public TileRef[] TileArray { get; set; }
    }

    public struct EnumValue
    {
        public string EnumName { get; set; }
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

    [StructLayout(LayoutKind.Sequential)]
    public struct Box
    {
        [SerializedField] public vec2 Position;
        [SerializedField] public vec2 Size;

        public Box(vec2 pos, vec2 size)
        {
            Position = pos;
            Size = size;
        }
    }

}