using Engine.Graphics;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct QuadVertices
    {
        public Vertex v0;
        public Vertex v1;
        public Vertex v2;
        public Vertex v3;
        public const int Count = 4;
        public Vertex this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return v0;
                        case 1:
                            return v1;
                        case 2:
                            return v2;
                        case 3:
                            return v3;
                    default:
                        {
                            Debug.Error($"Index out of range: {index}");
                            return default;
                        }
                }
            }
        }
    }

    internal struct QuadVertices<T> where T : unmanaged, IVertex<T>
    {
        public T v0;
        public T v1;
        public T v2;
        public T v3;
    }

    internal interface IVertex
    {
    }

    internal interface IVertex<T> : IVertex where T : struct
    {
        internal static abstract VertexAtrib[] GetVertexAttributes();
    }

    internal interface IVertex2D<T> : IVertex<T> where T : struct
    {
        internal int TextureIndex { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex : IVertex2D<Vertex>
    {
        public vec3 Position;
        public vec2 UV;
        public uint Color;
        private int _textureIndex;
        public int VertexIndex;
        public vec3 WorldCenter;

        public int TextureIndex { get => _textureIndex; set => _textureIndex = value; }

        private unsafe static VertexAtrib[] _attribs =
        [
            new() { Count = 3, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(Vertex), Offset = 0 },                  // Position
            new() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(Vertex), Offset = sizeof(float) * 3 },  // UV
            new() { Count = 1, Normalized = false, Type = GfxValueType.Uint,  Stride = sizeof(Vertex), Offset = sizeof(uint)  * 5 },  // Color
            new() { Count = 1, Normalized = false, Type = GfxValueType.Int,   Stride = sizeof(Vertex), Offset = sizeof(int)   * 6 },  // TextureIndex
            new() { Count = 1, Normalized = false, Type = GfxValueType.Int,   Stride = sizeof(Vertex), Offset = sizeof(int)   * 7 },  // VertexIndex
            new() { Count = 3, Normalized = false, Type = GfxValueType.Float,   Stride = sizeof(Vertex), Offset = sizeof(int)   * 8 },  // VertexIndex
        ];

        static VertexAtrib[] IVertex<Vertex>.GetVertexAttributes()
        {
            return _attribs;
        }
    }

    //public struct Vertex : IVertex2D<Vertex>
    //{
    //    public vec3 Position;
    //    public vec2 UV;
    //    public uint Color;
    //    private int _textureIndex;
    //    public int VertexIndex;
    //    public float Rotation;
    //    public vec2 Scale;

    //    public int TextureIndex { get => _textureIndex; set => _textureIndex = value; }

    //    private unsafe static VertexAtrib[] _attribs =
    //    [
    //        new() { Count = 3, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(Vertex), Offset = 0 },                  // Position
    //        new() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(Vertex), Offset = sizeof(float) * 3 },  // UV
    //        new() { Count = 1, Normalized = false, Type = GfxValueType.Uint,  Stride = sizeof(Vertex), Offset = sizeof(uint)  * 5 },  // Color
    //        new() { Count = 1, Normalized = false, Type = GfxValueType.Int,   Stride = sizeof(Vertex), Offset = sizeof(int)   * 6 },  // TextureIndex
    //        new() { Count = 1, Normalized = false, Type = GfxValueType.Int,   Stride = sizeof(Vertex), Offset = sizeof(int)   * 7 },  // VertexIndex

    //        new() { Count = 1, Normalized = false, Type = GfxValueType.Float,   Stride = sizeof(Vertex), Offset = sizeof(float) * 8 },  
    //        new() { Count = 2, Normalized = false, Type = GfxValueType.Float,   Stride = sizeof(Vertex), Offset = sizeof(float) * 9 },  
    //    ];

    //    static VertexAtrib[] IVertex<Vertex>.GetVertexAttributes()
    //    {
    //        return _attribs;
    //    }
    //}


    public class Mesh : EObject
    {
        internal bool IsDirty { get; private set; }
        public IList<Vertex> Vertices { get; set; }
        public uint[] Indices { get; set; }
        public int IndicesToDrawCount { get; set; }

        public Mesh()
        {
            Vertices = new List<Vertex>();
        }
    }
}
