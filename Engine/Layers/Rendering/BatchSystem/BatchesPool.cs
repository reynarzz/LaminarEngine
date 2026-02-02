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
        public int MaxEmptyBatches { get; set; } = 5;
        private bool _recalculateMaxBatches = false;

        public BatchesPool(GfxResource sharedIndexBuffer)
        {
            _sharedIndexBuffer = sharedIndexBuffer;
            _batches = new List<Batch2D>();
        }

        internal bool GetCurrentBatch(RendererData2D renderer, Texture texture, out Batch2D batchOut)
        {
            batchOut = null;
            foreach (var batch in _batches)
            {
                if (batch.Contains(renderer))
                {
                    if (batch.Material != renderer.Material || batch.SortOrder != renderer.SortOrder 
                        || (renderer.Mesh?.Indices != null && renderer.Mesh.IndicesToDrawCount != batch.IndexCount))
                    {
                        batch.RemoveRenderer(renderer);

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
                            return false;
                        }
                    }
                    
                    batchOut = batch;
                    return true;
                }
            }

            return false;
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
                    SortBatches();
                    // Debug.Log("Found empty batch for: " + renderer.Name);
                }
                else
                {
                    // Debug.Log("Found existing batch for: " + renderer.Name + ", Batch: Sorting: " + selectedBatch.SortOrder);
                    // SortBatches();
                }

                return selectedBatch;
            }
            var newBatch = new Batch2D(maxVertexSize, _sharedIndexBuffer, rawIndices);
            newBatch.OnBatchEmpty += OnBatchEmpty;
            // Initialize to clear any old states.
            newBatch.Initialize(renderer);

            _batches.Add(newBatch);
            SortBatches();
            // Debug.Info($"Create new batch for: {renderer.GetType().Name}: {renderer.Name}: sort: {renderer.SortOrder} ({_batches.Count})");

            return newBatch;
        }

        private void OnBatchEmpty(Batch2D batch)
        {
            // Moves empty batch, and puts it to the end.
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
            if (_batches.Remove(batch))
            {
                batch.Dispose();
            }
        }
        private void SortBatches()
        {
            _batches.Sort((x, y) =>
            {
                if (x.IsActive && !y.IsActive)
                {
                    return -1;
                }
                if (!x.IsActive && y.IsActive)
                {
                    return 1;
                }

                // If both are active, sort by SortOrder.
                if (x.IsActive && y.IsActive)
                {
                    return x.SortOrder.CompareTo(y.SortOrder);
                }

                // if both are inactive, keep original relative order.
                return 0;
            });

            _recalculateMaxBatches = true;
        }

        // TODO: Delete all batches that are not being used for too long, and are also big.
        internal void ClearPool()
        {
            foreach (var batch in _batches)
            {
                batch.Dispose();
            }
            _batches.Clear();
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

                for (int i = 0; i < removeCount; i++)
                {
                    _batches[i + index].Dispose();
                }

                _batches.RemoveRange(index, removeCount);

                Debug.Warn("Removed empty batches: " + (removeCount) + ", ValidCount: " + index);

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