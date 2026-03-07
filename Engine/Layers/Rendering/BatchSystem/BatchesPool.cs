using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rendering
{
    internal class BatchesPool
    {
        private readonly GfxResource _sharedIndexBuffer;
        private List<Batch2D> _batches;
        private readonly Dictionary<Guid, Batch2D> _rendererToBatch = new();
        public int MaxEmptyBatches { get; set; } = 5;
        private bool _recalculateMaxBatches = false;

        private static readonly Comparison<Batch2D> _batchSorter = (x, y) =>
        {
            if (x.IsActive != y.IsActive)
                return x.IsActive ? -1 : 1;
            if (x.IsActive)
                return x.SortOrder.CompareTo(y.SortOrder);
            return 0;
        };

        public BatchesPool(GfxResource sharedIndexBuffer)
        {
            _sharedIndexBuffer = sharedIndexBuffer;
            _batches = new List<Batch2D>();
        }

        internal bool GetCurrentBatch(RendererData2D renderer, Texture texture, out Batch2D batchOut)
        {
            batchOut = null;

            if (!_rendererToBatch.TryGetValue(renderer.GetID(), out var batch))
                return false;

            if (batch.Material != renderer.Material || batch.SortOrder != renderer.SortOrder ||
               (renderer.Mesh?.Indices != null && renderer.Mesh.IndicesToDrawCount != batch.IndexCount) ||
               (renderer.Mesh?.VertexCount > batch.VertexCount))
            {
                batch.RemoveRenderer(renderer);
                _rendererToBatch.Remove(renderer.GetID());

                if (renderer.PrivateBatch)
                {
                    DestroyBatch(batch);
                }
                return false;
            }
            else if (!batch.Textures.Contains(texture))
            {
                if (!batch.ReplaceTexture(renderer, texture))
                {
                    batch.RemoveRenderer(renderer);
                    _rendererToBatch.Remove(renderer.GetID());
                    return false;
                }
            }

            batchOut = batch;
            return true;
        }

        internal Batch2D Get(RendererData2D renderer, int vertexToAdd, int maxVertexSize, Texture texture, Material mat, uint[] rawIndices = null)
        {
            {
                if (GetCurrentBatch(renderer, texture, out var batch))
                {
                    return batch;
                }
            }

            var selectedBatch = default(Batch2D);

            // Try to find the best batch for the renderer.
            if (!renderer.PrivateBatch)
            {
                foreach (var batch in _batches)
                {
                    // TODO: find the smallest batch first
                    var canPush = batch.CanPushGeometry(renderer, vertexToAdd, maxVertexSize, texture, mat);
                    if (canPush)
                    {
                        if (selectedBatch == null)
                        {
                            selectedBatch = batch;
                        }
                        // Checks if this is a smaller compatible batch that this renderer can fit in.
                        else if ((selectedBatch.MaxVertexSize > batch.MaxVertexSize || selectedBatch.VertexCount > batch.VertexCount) &&
                                   batch.Material == selectedBatch.Material && batch.SortOrder == selectedBatch.SortOrder)
                        {
                            selectedBatch = batch;
                        }
                    }
                }
            }

            if (selectedBatch != null)
            {
                if (selectedBatch.Initialize(renderer))
                {
                    _rendererToBatch[renderer.GetID()] = selectedBatch;
                    SortBatches();
                }
                else
                {
                    _rendererToBatch[renderer.GetID()] = selectedBatch;
                }

                return selectedBatch;
            }

            // TODO: check for the vertex type.
            var newBatch = CreateBatch(renderer, maxVertexSize, _sharedIndexBuffer, rawIndices);
            _rendererToBatch[renderer.GetID()] = newBatch;

            _batches.Add(newBatch);
            renderer.BatchId = _batches.Count; // TODO:

            SortBatches();

            return newBatch;
        } 

        private Batch2D CreateBatch(RendererData2D renderer, int maxVertexSize, GfxResource sharedIndexBuffer, uint[] rawIndices)
        {
            Batch2D newBatch = null;
            if (renderer.VertexType == null)
            {
                throw new NotImplementedException($"Vertex type is null for rendererData '{renderer.ID}'.");
            }
            if (renderer.VertexType == typeof(Vertex))
            {
                newBatch = Batch2D.CreateBatch<Vertex>(maxVertexSize, _sharedIndexBuffer, rawIndices);
            }
            else
            {
                throw new NotImplementedException($"Vertex type '{renderer.VertexType}' is not handled by the batch pool.");
            }
            newBatch.OnBatchEmpty += OnBatchEmpty;
            newBatch.Initialize(renderer);

            return newBatch;
        }
        private void OnBatchEmpty(Batch2D batch)
        {
            // Remove all renderer mappings pointing to this batch
            var toRemove = new List<Guid>();
            foreach (var kvp in _rendererToBatch)
            {
                if (kvp.Value == batch)
                    toRemove.Add(kvp.Key);
            }
            foreach (var key in toRemove)
            {
                _rendererToBatch.Remove(key);
            }

            batch.Clear();
            if (_batches.Remove(batch))
            {
                _batches.Add(batch);
            }
            else
            {
                Debug.EngineError("Could not remove batch");
            }
        }

        private void DestroyBatch(Batch2D batch)
        {
            // Remove all renderer mappings pointing to this batch
            var toRemove = new List<Guid>();
            foreach (var kvp in _rendererToBatch)
            {
                if (kvp.Value == batch)
                {
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var key in toRemove)
            {
                _rendererToBatch.Remove(key);
            }

            if (_batches.Remove(batch))
            {
                batch.Dispose();
            }
        }

        private void SortBatches()
        {
            _batches.Sort(_batchSorter);
            _recalculateMaxBatches = true;
        }

        internal void ClearPool()
        {
            foreach (var batch in _batches)
            {
                batch.Dispose();
            }
            _batches.Clear();
            _rendererToBatch.Clear();
        }

        private void RemoveExtraBatches()
        {
            var index = _batches.FindIndex(x => !x.IsActive);

            if (index < 0)
                return;

            var emptyCount = _batches.Count - index;

            if (emptyCount > MaxEmptyBatches)
            {
                var removeCount = emptyCount - MaxEmptyBatches;
                int removeStart = _batches.Count - removeCount;

                for (int i = removeStart; i < _batches.Count; i++)
                {
                    _batches[i].Dispose();
                }

                _batches.RemoveRange(removeStart, removeCount);

                Debug.Warn("Removed empty batches: " + removeCount + ", ValidCount: " + index);
            }
        }

        internal List<Batch2D> GetActiveBatches()
        {
            if (_recalculateMaxBatches)
            {
                _recalculateMaxBatches = false;
                RemoveExtraBatches();
            }

            return _batches;
        }
    }
}