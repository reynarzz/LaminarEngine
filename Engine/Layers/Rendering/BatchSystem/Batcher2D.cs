using Engine.Graphics;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        private Dictionary<BucketKey, List<RendererData2D>> _renderBuckets;
        private BatchesPool _batchesPool;
        private static Material _pinkMaterial;
        private readonly Vertex[] _quadVertexArray = new Vertex[4];
        private readonly List<List<RendererData2D>> _sortedBuckets = new();
        private static readonly Comparison<List<RendererData2D>> _bucketSorter = (a, b) => a[0].SortOrder.CompareTo(b[0].SortOrder);

        private struct BucketKey : IEquatable<BucketKey>
        {
            private readonly Material Material;
            private readonly int SortOrder;

            public BucketKey(Material material, int sortOrder)
            {
                Material = material;
                SortOrder = sortOrder;
            }

            public bool Equals(BucketKey other) => Equals(Material, other.Material) && SortOrder == other.SortOrder;
            public override bool Equals(object obj) => obj is BucketKey other && Equals(other);
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + (Material != null ? Material.GetHashCode() : 0);
                    hash = hash * 31 + SortOrder;
                    return hash;
                }
            }

            public static bool operator ==(BucketKey a, BucketKey b) => a.Equals(b);
            public static bool operator !=(BucketKey a, BucketKey b) => !a.Equals(b);
        }

        public Batcher2D(int maxQuadsPerBatch)
        {
            MaxQuadsPerBatch = maxQuadsPerBatch;
            _renderBuckets = new Dictionary<BucketKey, List<RendererData2D>>();

            if(_pinkMaterial == null)
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
            // TODO: Do frustum culling
            _renderBuckets.Clear();

            foreach (var renderer in renderers)
            {
                if (!renderer.IsEnabled)
                {
                    // TODO: notify if need to be removed from a batch
                    continue;
                }

                var key = new BucketKey(renderer.Material, renderer.SortOrder);

                if (!_renderBuckets.ContainsKey(key))
                {
                    _renderBuckets.Add(key, new List<RendererData2D>());
                }

                _renderBuckets[key].Add(renderer);
            }


            _sortedBuckets.Clear();
            _sortedBuckets.AddRange(_renderBuckets.Values);
            _sortedBuckets.Sort(_bucketSorter);

            // TODO: improve performance of order by sorting, is allocating every frame
            foreach (var bucket in _sortedBuckets)
            {
                foreach (var renderer in bucket)
                {
                    renderer.Draw();

                    if (!renderer.IsDirty && !renderer.Transform.NeedsInterpolation)
                    {
                        continue;
                    }
                    else
                    {
                        renderer.MarkNotDirty();
                    }

                    var texture = renderer.Sprite?.Texture ?? Texture2D.White;
                    var material = renderer.Material ?? _pinkMaterial;

                    if (material != null && !material.Shader.NativeShader.IsInitialized)
                    {
                        material = _pinkMaterial;
                    }

                    if (renderer.Mesh == null)
                    {
                        var chunk = renderer.Sprite?.GetAtlasChunk() ?? AtlasChunk.DefaultChunk;

                        float ppu = texture.PixelPerUnit;
                        var width = (float)chunk.Width / ppu;
                        var height = (float)chunk.Height / ppu;

                        var currentBatch = _batchesPool.Get(renderer, VerticesPerQuad, MaxBatchVertexSize, texture, material);

                        var worldMatrix = renderer.Transform.GetRenderingWorldMatrix();

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

                        if (!_batchesPool.GetCurrentBatch(renderer, texture, out var currentBatch))
                        {
                            var indices = default(uint[]);

                            if (renderer.Mesh.Indices == null)
                            {
                                indices = GraphicsHelper.GetQuadIndices(vertexCount / VerticesPerQuad);
                            }
                            else
                            {
                                indices = renderer.Mesh.Indices;
                            }
                            currentBatch = _batchesPool.Get(renderer, renderer.Mesh.Vertices.Count, vertexCount, texture, material, indices);
                        }

                        currentBatch.PushGeometry(renderer, material, texture, renderer.Mesh.IndicesToDrawCount, CollectionsMarshal.AsSpan(renderer.Mesh.Vertices));
                    }
                }
            }

            return _batchesPool.GetActiveBatches();
        }

        internal void Clear()
        {
            _batchesPool.ClearPool();
            _renderBuckets.Clear();
            _sortedBuckets.Clear();

            GfxDeviceManager.Current.DestroyResource(_sharedIndexBuffer);
            Initialize();
        }
    }
}