using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;
using GlmNet;

namespace Engine
{
    internal static class GraphicsHelper
    {
        internal static GfxResource GetEmptyGeometry<T>(int vertCount, int indexCount, ref GeometryDescriptor geoDesc, GfxResource indexBuffer = null) where T : unmanaged, IVertex<T>
        {
            if (geoDesc == null)
            {
                geoDesc = new GeometryDescriptor();
            }

            if (indexCount <= 0)
            {
                geoDesc.IndexDesc = null;
            }
            else
            {
                geoDesc.IndexDesc = new BufferDataDescriptor<uint>()
                {
                    Usage = BufferUsage.Dynamic,
                    Buffer = new uint[indexCount]
                };
            }

            geoDesc.SharedIndexBuffer = indexBuffer;

            geoDesc.VertexDesc = new VertexDataDescriptor()
            {
                Attribs = T.GetVertexAttributes(),
                BufferDesc = new BufferDataDescriptor<T>()
                {
                    Buffer = new T[vertCount],
                    Usage = BufferUsage.Dynamic,
                }
            };

            return GfxDeviceManager.Current.CreateGeometry(geoDesc);
        }

        internal static GfxResource GetScreenQuadGeometry()
        {
            QuadVertices vertices = default;
            CreateQuad(ref vertices, QuadUV.DefaultUVs, 2, 2, new vec2(0.5f), Color.White, mat4.identity());


            var geoDesc = new GeometryDescriptor()
            {
                IndexDesc = new BufferDataDescriptor<uint>()
                {
                    Buffer = [0, 1, 2, 0, 2, 3],
                    Usage = BufferUsage.Static
                },
                VertexDesc = new VertexDataDescriptor()
                {
                    Attribs = GetVertexAttribs<Vertex>(),
                    BufferDesc = new BufferDataDescriptor<Vertex>()
                    {
                        Buffer = [vertices.v0, vertices.v1, vertices.v2, vertices.v3],
                        Usage = BufferUsage.Static
                    }
                }
            };

            return GfxDeviceManager.Current.CreateGeometry(geoDesc);
        }
        internal static VertexAtrib[] GetVertexAttribs<T>() where T : unmanaged, IVertex<T>
        {
            return T.GetVertexAttributes();
        }

        internal static GfxResource CreateQuadIndexBuffer(int maxQuads)
        {
            var indices = new uint[maxQuads * 6];

            for (uint i = 0; i < maxQuads; i++)
            {
                indices[i * 6 + 0] = i * 4 + 0;
                indices[i * 6 + 1] = i * 4 + 1;
                indices[i * 6 + 2] = i * 4 + 2;
                indices[i * 6 + 3] = i * 4 + 2;
                indices[i * 6 + 4] = i * 4 + 3;
                indices[i * 6 + 5] = i * 4 + 0;
            }

            return GfxDeviceManager.Current.CreateIndexBuffer(new BufferDataDescriptor<uint>()
            {
                Buffer = indices,
                Usage = BufferUsage.Static
            });
        }

        internal static void CreateQuad(ref QuadVertices vertices, QuadUV uvs, float width, float height, vec2 pivot,
                                         ColorPacketRGBA color, mat4 worldMatrix)
        {
            float px = pivot.x * width;
            float py = pivot.y * height;

            vertices.v0 = new Vertex()
            {
                Color = color,
                Position = new vec3(worldMatrix * new vec4(-px, -py, 0, 1)),
                UV = uvs.BottomLeftUV,
            };

            vertices.v1 = new Vertex()
            {
                Color = color,
                Position = new vec3(worldMatrix * new vec4(-px, height - py, 0, 1)),
                UV = uvs.TopLeftUV,
            };

            vertices.v2 = new Vertex()
            {
                Color = color,
                Position = new vec3(worldMatrix * new vec4(width - px, height - py, 0, 1)),
                UV = uvs.TopRightUV,
            };

            vertices.v3 = new Vertex()
            {
                Color = color,
                Position = new vec3(worldMatrix * new vec4(width - px, -py, 0, 1)),
                UV = uvs.BottomRightUV
            };
        }

        internal static QuadVertices GetUIQuadVertices(Rect rect, Color color)
        {
            var size = rect.Size;
            QuadVertices vertices = default;

            vec2 bottomLeft = rect.Min;
            vec2 topLeft = new vec2(rect.Min.x, rect.Min.y + size.y);
            vec2 topRight = rect.Min + size;
            vec2 bottomRight = new vec2(rect.Min.x + size.x, rect.Min.y);

            var uvs = QuadUV.DefaultUVs;

            vertices.v0 = new Vertex { Color = color, Position = bottomLeft, UV = uvs.BottomLeftUV };
            vertices.v1 = new Vertex { Color = color, Position = topLeft, UV = uvs.TopLeftUV };
            vertices.v2 = new Vertex { Color = color, Position = topRight, UV = uvs.TopRightUV };
            vertices.v3 = new Vertex { Color = color, Position = bottomRight, UV = uvs.BottomRightUV };

            return vertices;
        }
        internal static QuadVertices GetUIQuadVerticesLocal(QuadUV uvs, vec2 size, vec2 pivot, Color color, mat4 worldMatrix)
        {
            QuadVertices q = default;
            float x0 = -pivot.x * size.x, y0 = -pivot.y * size.y;
            float x1 = x0 + size.x, y1 = y0 + size.y;

            var u = QuadUV.FlipUV(uvs, false, true);

            vec4 p0 = worldMatrix * new vec4(x0, y0, 0, 1);
            vec4 p1 = worldMatrix * new vec4(x0, y1, 0, 1);
            vec4 p2 = worldMatrix * new vec4(x1, y1, 0, 1);
            vec4 p3 = worldMatrix * new vec4(x1, y0, 0, 1);

            q.v0 = new Vertex { Color = color, Position = new vec2(p0.x, p0.y), UV = u.BottomLeftUV };
            q.v1 = new Vertex { Color = color, Position = new vec2(p1.x, p1.y), UV = u.TopLeftUV };
            q.v2 = new Vertex { Color = color, Position = new vec2(p2.x, p2.y), UV = u.TopRightUV };
            q.v3 = new Vertex { Color = color, Position = new vec2(p3.x, p3.y), UV = u.BottomRightUV };

            return q;
        }
    }
}
