using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;
using GlmNet;
using Engine;

namespace Engine
{
    internal static class GraphicsHelper
    {
        internal static GfxResource GetEmptyGeometry<T>(int vertCount, int indexCount, ref GeometryDescriptor geoDesc,
                                                        GfxResource indexBuffer = null, bool isSharedIndexBuffer = true)
                                                        where T : unmanaged, IVertex<T>
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
        internal static GfxResource CreateQuadGeometry()
        {
            var geoDesc = default(GeometryDescriptor);

            return CreateQuadGeometry(ref geoDesc);
        }

        internal static GfxResource CreateQuadGeometry(ref GeometryDescriptor geoDesc)
        {
            QuadVertices vertices = default;
            CreateQuad(ref vertices, QuadUV.DefaultUVs, 2, 2, new vec2(0.5f), Color.White, mat4.identity());

            geoDesc = new GeometryDescriptor()
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
            var indices = GetQuadIndices(maxQuads);

            return GfxDeviceManager.Current.CreateIndexBuffer(new BufferDataDescriptor<uint>()
            {
                Buffer = indices,
                Usage = BufferUsage.Static
            });
        }

        internal static uint[] GetQuadIndices(int maxQuads)
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
            return indices;
        }

        internal static GfxResource CreateQuadIndexBuffer(uint[] indices)
        {
            return GfxDeviceManager.Current.CreateIndexBuffer(new BufferDataDescriptor<uint>()
            {
                Buffer = indices,
                Usage = BufferUsage.Static
            });
        }

        // TODO: This should not happen ever, instead a instancing renderer should be used. This is temporal.
        internal static void CreateQuad(ref QuadVertices vertices, QuadUV uvs, float width, float height, vec2 pivot,
                                         ColorPacketRGBA color, mat4 worldMatrix)
        {
            CreateQuad(ref vertices.v0, ref vertices.v1, ref vertices.v2, ref vertices.v3, uvs, width, height, pivot, color, worldMatrix, 0);
        }

        internal static void CreateQuad(ref Vertex v0, ref Vertex v1, ref Vertex v2, ref Vertex v3,
                                   QuadUV uvs, float width, float height, vec2 pivot,
                                   ColorPacketRGBA color, mat4 worldMatrix, int textureIndex)
        {
            float left = -(pivot.x * width);
            float bottom = -(pivot.y * height);
            float right = width + left;
            float top = height + bottom;

            var center = new vec3(worldMatrix[3]);
            var cx = new vec3(worldMatrix[0]); 
            var cy = new vec3(worldMatrix[1]); 

            v0.Position = center + cx * left + cy * bottom;
            v0.UV = uvs.BottomLeftUV;
            v0.Color = color;
            v0.TextureIndex = textureIndex;
            v0.WorldCenter = center;

            v1.Position = center + cx * left + cy * top;
            v1.UV = uvs.TopLeftUV;
            v1.Color = color;
            v1.TextureIndex = textureIndex;
            v1.WorldCenter = center;

            v2.Position = center + cx * right + cy * top;
            v2.UV = uvs.TopRightUV;
            v2.Color = color;
            v2.TextureIndex = textureIndex;
            v2.WorldCenter = center;

            v3.Position = center + cx * right + cy * bottom;
            v3.UV = uvs.BottomRightUV;
            v3.Color = color;
            v3.TextureIndex = textureIndex;
            v3.WorldCenter = center;
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


        public static LineMesh CreateLineMesh2D(IList<vec3> points, float halfWidth, uint color = 0xFFFFFFFF)
        {
            if (points == null || points.Count < 2)
            {
                return new LineMesh
                {
                    Vertices = Array.Empty<Vertex>(),
                    Indices = Array.Empty<uint>()
                };
            }

            var vertices = new List<Vertex>();
            var indices = new List<uint>();
            int vertexIndex = 0;

            for (int i = 0; i < points.Count - 1; i++)
            {
                vec3 p0 = points[i];
                vec3 p1 = points[i + 1];

                vec3 dir = glm.normalize(p1 - p0);
                int baseIndex = vertices.Count;

                vertices.Add(MakeVertex(p0, dir, -1f, color, vertexIndex++));
                vertices.Add(MakeVertex(p0, dir, +1f, color, vertexIndex++));
                vertices.Add(MakeVertex(p1, dir, +1f, color, vertexIndex++));
                vertices.Add(MakeVertex(p1, dir, -1f, color, vertexIndex++));

                indices.Add((uint)(baseIndex + 0));
                indices.Add((uint)(baseIndex + 1));
                indices.Add((uint)(baseIndex + 2));
                indices.Add((uint)(baseIndex + 2));
                indices.Add((uint)(baseIndex + 3));
                indices.Add((uint)(baseIndex + 0));
            }

            Vertex MakeVertex(vec3 position, vec3 dir, float side, uint color, int vertexIndex)
            {
                return new Vertex
                {
                    Position = position,
                    UV = new vec2(side, 0),
                    Color = color,
                    VertexIndex = vertexIndex,
                    WorldCenter = dir.Normalized
                };
            }

            return new LineMesh
            {
                Vertices = vertices.ToArray(),
                Indices = indices.ToArray()
            };
        }
        public static LineMesh CreateNonContiguousLines(IList<vec3> points, float halfWidth, uint color = 0xFFFFFFFF)
        {
            if (points == null || points.Count < 2)
            {
                return new LineMesh
                {
                    Vertices = Array.Empty<Vertex>(),
                    Indices = Array.Empty<uint>()
                };
            }

            if ((points.Count & 1) != 0)
                throw new ArgumentException("Line list must contain an even number of points");

            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            uint indexOffset = 0;

            for (int i = 0; i < points.Count; i += 2)
            {
                vec3 a = points[i];
                vec3 b = points[i + 1];

                // Skip degenerate segments
                if ((b - a).length() < 1e-8f)
                    continue;

                // Generate tube for this single segment
                var segmentMesh = CreateLineMesh3D(
                    new vec3[] { a, b },
                    halfWidth,
                    color);

                // Append vertices
                foreach (var v in segmentMesh.Vertices)
                    vertices.Add(v);

                // Append indices with offset
                foreach (var idx in segmentMesh.Indices)
                    indices.Add(idx + indexOffset);

                indexOffset += (uint)segmentMesh.Vertices.Length;
            }

            return new LineMesh
            {
                Vertices = vertices.ToArray(),
                Indices = indices.ToArray()
            };
        }

        public static LineMesh CreateLineMesh3D(IList<vec3> points, float halfWidth, uint color = 0xFFFFFFFF)
        {
            if (points == null || points.Count < 2)
            {
                return new LineMesh
                {
                    Vertices = Array.Empty<Vertex>(),
                    Indices = Array.Empty<uint>()
                };
            }

            const int radialSegments = 16; // 8–32 depending on quality
            float radius = halfWidth;

            var vertices = new List<Vertex>();
            var indices = new List<uint>();

            int ringCount = points.Count;

            // Compute tangents
            vec3[] tangents = new vec3[ringCount];
            for (int i = 0; i < ringCount; i++)
            {
                if (i == 0)
                    tangents[i] = glm.normalize(points[1] - points[0]);
                else if (i == ringCount - 1)
                    tangents[i] = glm.normalize(points[i] - points[i - 1]);
                else
                    tangents[i] = glm.normalize(points[i + 1] - points[i - 1]);
            }

            // Initial frame
            vec3 n = AnyPerpendicular(tangents[0]);
            vec3 b = glm.normalize(glm.cross(tangents[0], n));
            n = glm.normalize(glm.cross(b, tangents[0]));

            // Generate rings
            for (int i = 0; i < ringCount; i++)
            {
                if (i > 0)
                    TransportFrame(tangents[i - 1], tangents[i], ref n, ref b);

                vec3 center = points[i];

                for (int s = 0; s < radialSegments; s++)
                {
                    float angle = (float)(s * Math.PI * 2.0 / radialSegments);
                    float c = MathF.Cos(angle);
                    float sn = MathF.Sin(angle);

                    vec3 offset = n * c + b * sn;
                    vec3 pos = center + offset * radius;

                    vertices.Add(new Vertex
                    {
                        Position = pos,
                        // Normal = glm.normalize(offset),
                        Color = color
                    });
                }
            }

            // Indices
            for (int i = 0; i < ringCount - 1; i++)
            {
                int base0 = i * radialSegments;
                int base1 = (i + 1) * radialSegments;

                for (int s = 0; s < radialSegments; s++)
                {
                    int a = base0 + s;
                    int b0 = base0 + (s + 1) % radialSegments;
                    int c0 = base1 + (s + 1) % radialSegments;
                    int d = base1 + s;

                    indices.Add((uint)a);
                    indices.Add((uint)b0);
                    indices.Add((uint)c0);

                    indices.Add((uint)c0);
                    indices.Add((uint)d);
                    indices.Add((uint)a);
                }
            }

            return new LineMesh
            {
                Vertices = vertices.ToArray(),
                Indices = indices.ToArray()
            };
        }

        private static void TransportFrame(vec3 prevT, vec3 currT, ref vec3 n, ref vec3 b)
        {
            vec3 v = glm.cross(prevT, currT);
            float c = glm.dot(prevT, currT);

            if (v.length() < 1e-6f)
                return;

            float k = 1.0f / (1.0f + c);

            mat3 R = new mat3(v.x * v.x * k + c, v.x * v.y * k - v.z, v.x * v.z * k + v.y,
                              v.y * v.x * k + v.z, v.y * v.y * k + c, v.y * v.z * k - v.x,
                              v.z * v.x * k - v.y, v.z * v.y * k + v.x, v.z * v.z * k + c);

            n = glm.normalize(R * n);
            b = glm.normalize(glm.cross(currT, n));
        }

        private static vec3 AnyPerpendicular(vec3 v)
        {
            if (MathF.Abs(v.x) < 0.9f)
                return glm.normalize(glm.cross(v, new vec3(1, 0, 0)));
            else
                return glm.normalize(glm.cross(v, new vec3(0, 1, 0)));
        }


        public static List<vec3> CreatePerspectiveFrustumLines(vec3 position, vec3 forward, vec3 right, vec3 up,
                                                               float fovYRadians, float aspect, float nearPlane, float farPlane)
        {
            var lines = new List<vec3>(24);

            // Centers of near and far planes
            vec3 nc = position + forward * nearPlane;
            vec3 fc = position + forward * farPlane;

            //  Half sizes of planes
            float nearHeight = nearPlane * MathF.Tan(fovYRadians * 0.5f);
            float nearWidth = nearHeight * aspect;
            float farHeight = farPlane * MathF.Tan(fovYRadians * 0.5f);
            float farWidth = farHeight * aspect;

            // Corners of near plane
            vec3 n0 = nc + up * nearHeight - right * nearWidth; // top left
            vec3 n1 = nc + up * nearHeight + right * nearWidth; // top right
            vec3 n2 = nc - up * nearHeight + right * nearWidth; // bottom right
            vec3 n3 = nc - up * nearHeight - right * nearWidth; // bottom left

            // Corners of far plane
            vec3 f0 = fc + up * farHeight - right * farWidth; // top left
            vec3 f1 = fc + up * farHeight + right * farWidth; // top right
            vec3 f2 = fc - up * farHeight + right * farWidth; // bottom right
            vec3 f3 = fc - up * farHeight - right * farWidth; // bottom left

            // Helper to add line segment
            void AddLine(vec3 a, vec3 b)
            {
                lines.Add(a);
                lines.Add(b);
            }

            // Near plane rectangle
            AddLine(n0, n1);
            AddLine(n1, n2);
            AddLine(n2, n3);
            AddLine(n3, n0);

            // Far plane rectangle
            AddLine(f0, f1);
            AddLine(f1, f2);
            AddLine(f2, f3);
            AddLine(f3, f0);

            // Connecting edges
            AddLine(n0, f0);
            AddLine(n1, f1);
            AddLine(n2, f2);
            AddLine(n3, f3);

            return lines;
        }

        public static List<vec3> CreateOrthoFrustumLines(vec3 position, vec3 forward, vec3 right, vec3 up,
                                                         float orthoHeight, float aspect, float nearPlane, float farPlane)
        {
            var lines = new List<vec3>(24);

            float halfHeight = orthoHeight * 0.5f;
            float halfWidth = halfHeight * aspect;

            // Centers of near and far planes
            vec3 nc = position + forward * nearPlane;
            vec3 fc = position + forward * farPlane;

            // Near plane corners
            vec3 n0 = nc - right * halfWidth + up * halfHeight; // top left
            vec3 n1 = nc + right * halfWidth + up * halfHeight; // top right
            vec3 n2 = nc + right * halfWidth - up * halfHeight; // bottom right
            vec3 n3 = nc - right * halfWidth - up * halfHeight; // bottom left

            // Far plane corners
            vec3 f0 = fc - right * halfWidth + up * halfHeight;
            vec3 f1 = fc + right * halfWidth + up * halfHeight;
            vec3 f2 = fc + right * halfWidth - up * halfHeight;
            vec3 f3 = fc - right * halfWidth - up * halfHeight;

            void AddLine(vec3 a, vec3 b)
            {
                lines.Add(a);
                lines.Add(b);
            }

            // Near rectangle
            AddLine(n0, n1);
            AddLine(n1, n2);
            AddLine(n2, n3);
            AddLine(n3, n0);

            // Far rectangle
            AddLine(f0, f1);
            AddLine(f1, f2);
            AddLine(f2, f3);
            AddLine(f3, f0);

            // Connecting edges
            AddLine(n0, f0);
            AddLine(n1, f1);
            AddLine(n2, f2);
            AddLine(n3, f3);

            return lines;
        }
        public static List<vec3> CreateGrid(int gridX, int gridY, float tileSize, float y = 0.0f)
        {
            var points = new List<vec3>();

            float width = gridX * tileSize;
            float height = gridY * tileSize;

            float halfW = width * 0.5f;
            float halfH = height * 0.5f;

            // Vertical lines
            for (int x = 0; x <= gridX; x++)
            {
                float px = x * tileSize - halfW;

                points.Add(new vec3(px, y, -halfH));
                points.Add(new vec3(px, y, halfH));
            }

            // Horizontal lines
            for (int yIdx = 0; yIdx <= gridY; yIdx++)
            {
                float py = yIdx * tileSize - halfH;

                points.Add(new vec3(-halfW, y, py));
                points.Add(new vec3(halfW, y, py));
            }

            return points;
        }


    }
}