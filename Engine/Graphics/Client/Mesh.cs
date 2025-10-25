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
    public struct QuadVertices
    {
        public Vertex v0 { get; set; }
        public Vertex v1 { get; set; }
        public Vertex v2 { get; set; }
        public Vertex v3 { get; set; }
    }

    internal struct QuadVertices<T> where T : unmanaged, IVertex<T>
    {
        public T v0 { get; set; }
        public T v1 { get; set; }
        public T v2 { get; set; }
        public T v3 { get; set; }
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
        public vec3 Normals;
        public ColorPacketRGBA Color;
        public int _textureIndex;
        public int TextureIndex { get => _textureIndex; set => _textureIndex = value; }

        private unsafe static VertexAtrib[] _attribs =
        [
            new() { Count = 3, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(Vertex), Offset = 0 },                  // Position
            new() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(Vertex), Offset = sizeof(float) * 3 },  // UV
            new() { Count = 3, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(Vertex), Offset = sizeof(float) * 5 },  // Normals
            new() { Count = 1, Normalized = false, Type = GfxValueType.Uint,  Stride = sizeof(Vertex), Offset = sizeof(uint)  * 8 },  // Color
            new() { Count = 1, Normalized = false, Type = GfxValueType.Int,   Stride = sizeof(Vertex), Offset = sizeof(int)   * 9 },  // TextureIndex
        ];

        public static VertexAtrib[] GetVertexAttributes()
        {
            return _attribs;
        }
    }


    public class Mesh : EObject
    {
        internal bool IsDirty { get; private set; }
        public List<Vertex> Vertices { get; }
        public List<uint> Indices { get; }
        public int IndicesToDrawCount { get; set; }

        public Mesh()
        {
            Vertices = new List<Vertex>();
            Indices = new List<uint>();
        }
    }
}
