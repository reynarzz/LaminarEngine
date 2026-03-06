using Engine.Graphics;
using Engine.Utils;

namespace Engine.Rendering
{
    internal class Batcher2D
    {
        private int MaxQuadsPerBatch { get; set; }
        private int MaxBatchVertexSize => MaxQuadsPerBatch * VerticesPerQuad;
        internal int IndicesToDraw { get; private set; }

        private GfxResource _sharedIndexBuffer;

        private const int IndicesPerQuad = 6;
        private const int VerticesPerQuad = 4;
        private BatchesPool _batchesPool;
        private static Material _pinkMaterial;
        private readonly Vertex[] _quadVertexArray = new Vertex[4];
        private readonly List<RendererData2D> _visibleRenderers = new();
        private readonly RendererBatchComparer _rendererComparer = new();
        private sealed class RendererBatchComparer : IComparer<RendererData2D>
        {
            public int Compare(RendererData2D a, RendererData2D b)
            {
                var matA = a.Material;
                var matB = b.Material;

                if (matA == null || matB == null)
                {
                    if (matA == matB)
                    {
                        return a.SortOrder.CompareTo(b.SortOrder);
                    }

                    return matA == null ? -1 : 1;
                }

                int material = matA.MaterialInstanceId.CompareTo(matB.MaterialInstanceId);

                if (material != 0)
                {
                    return material;
                }

                return a.SortOrder.CompareTo(b.SortOrder);
            }
        }


        public Batcher2D(int maxQuadsPerBatch)
        {
            MaxQuadsPerBatch = maxQuadsPerBatch;

            if (_pinkMaterial == null)
            {
                _pinkMaterial = new Material(InternalShaderUtils.GetShaderPink());
                _pinkMaterial.Name = "Pink Material";
            }

            Initialize();
        }

        internal void Initialize()
        {
            _sharedIndexBuffer = GraphicsHelper.CreateQuadIndexBuffer(MaxQuadsPerBatch);
            _batchesPool = new BatchesPool(_sharedIndexBuffer);
        }

        internal List<Batch2D> GetBatches<T>(IReadOnlyCollection<T> renderers) where T : RendererData2D
        {
            _visibleRenderers.Clear();

            foreach (var renderer in renderers)
            {
                if (!renderer.IsEnabled)
                {
                    continue;
                }

                _visibleRenderers.Add(renderer);
            }

            _visibleRenderers.Sort(_rendererComparer);

            foreach (var renderer in _visibleRenderers)
            {
                renderer.Draw();

                if (!renderer.IsDirty && !renderer.NeedsInterpolation())
                {
                    continue;
                }
                else
                {
                    renderer.MarkNotDirty();
                }

                var texture = renderer.Sprite?.Texture ?? Texture2D.White;
                var material = renderer.Material;

                if (!material || material.Shader == null || material.Shader.HasErrors /*|| !material.Shader.NativeShader.IsInitialized*/)
                {
                    material = _pinkMaterial;
                }

                if (renderer.Mesh == null)
                {
                    var chunk = renderer.Sprite?.GetAtlasCell() ?? TextureAtlasCell.DefaultChunk;

                    float ppu = texture.PixelPerUnit;
                    var width = (float)chunk.Width / ppu;
                    var height = (float)chunk.Height / ppu;

                    var currentBatch = _batchesPool.Get(renderer, VerticesPerQuad, MaxBatchVertexSize, texture, material);

                    var worldMatrix = renderer.GetRenderingWorldMatrix();

                    QuadVertices quad = default;
                    GraphicsHelper.CreateQuad(ref quad, chunk.Uvs, width, height, chunk.Pivot, renderer.Color, worldMatrix);

                    _quadVertexArray[0] = quad.v0;
                    _quadVertexArray[1] = quad.v1;
                    _quadVertexArray[2] = quad.v2;
                    _quadVertexArray[3] = quad.v3;

                    currentBatch.PushGeometry(renderer, material, texture, IndicesPerQuad, _quadVertexArray);
                }
                else
                {
                    // TODO: implement proper mesh drawing, for now, since it is used just for tilemap, this works
                    var vertexCount = Math.Max(MaxBatchVertexSize, renderer.Mesh.Vertices.Count);

                    var indices = default(uint[]);

                    if (renderer.Mesh.Indices == null)
                    {
                        if (vertexCount > MaxBatchVertexSize)
                        {
                            indices = GraphicsHelper.GetQuadIndices(vertexCount / VerticesPerQuad);
                        }
                    }
                    else
                    {
                        indices = renderer.Mesh.Indices;
                    }
                    var currentBatch = _batchesPool.Get(renderer, renderer.Mesh.Vertices.Count, vertexCount, texture, material, indices);

                    currentBatch.PushGeometry(renderer, material, texture, renderer.Mesh.IndicesToDrawCount, renderer.Mesh.Vertices);
                }
            }

            return _batchesPool.GetActiveBatches();
        }

        internal void Clear()
        {
            _batchesPool.ClearPool();

            GfxDeviceManager.Current.DestroyResource(_sharedIndexBuffer);
            Initialize();
        }
    }
}