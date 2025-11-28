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

        public BatchesPool(GfxResource sharedIndexBuffer)
        {
            _sharedIndexBuffer = sharedIndexBuffer;
            _batches = new List<Batch2D>();
        }

        internal bool GetCurrentBatch(Renderer2D renderer, out Batch2D batchOut)
        {
            batchOut = null;
            foreach (var batch in _batches)
            {
                if (batch.Contains(renderer))
                {
                    if (batch.SortOrder == renderer.SortOrder)
                    {
                        batchOut = batch;
                        return true;
                    }
                    else
                    {
                        Debug.Warn($"Changed sorting: from {batch.SortOrder}, to: {renderer.SortOrder}" + renderer.Name);
                        batch.RemoveRenderer(renderer);
                        return false;
                    }
                }
            }

            return false;
        }

        internal Batch2D Get(Renderer2D renderer, int vertexToAdd, int maxVertexSize, Material mat, GfxResource indexBuffer = null)
        {
            {
                if (GetCurrentBatch(renderer, out var batch))
                {
                    return batch;
                }
            }

            var selectedBatch = default(Batch2D);

            // Try to find the best batch for the renderer.
            foreach (var batch in _batches)
            {
                var isMaxSizeEnough = batch.MaxVertexSize >= maxVertexSize;
                var hasSpaceLeftForAnother = (batch.MaxVertexSize - batch.VertexCount) >= vertexToAdd;
                var isBatchSizeEnough = isMaxSizeEnough && hasSpaceLeftForAnother;
                var isSameSortOrder = renderer.SortOrder == batch.SortOrder || batch.SortOrder == int.MinValue;
                var isValidMaterial = batch.Material == mat || !batch.Material;

                // TODO: find the smallest batch first
                if (isBatchSizeEnough && ((isValidMaterial && isSameSortOrder) || !batch.IsActive))
                {
                    if (selectedBatch == null)
                    {
                        selectedBatch = batch;
                    }
                    else if ((selectedBatch.MaxVertexSize > batch.MaxVertexSize || selectedBatch.VertexCount > batch.VertexCount)) // Checks if this is a smaller batch that it can fit in
                    {
                        selectedBatch = batch;
                    }
                }
            }

            if (selectedBatch != null)
            {
                if (selectedBatch.Initialize(renderer))
                {
                    SortBatches();
                    Debug.Log("Found empty batch for: " + renderer.Name);
                }
                else
                {
                    Debug.Log("Found existing batch for: " + renderer.Name + ", Batch: Sorting: " + selectedBatch.SortOrder);
                    SortBatches();
                }

                return selectedBatch;
            }
            var newBatch = new Batch2D(maxVertexSize, indexBuffer == null ? _sharedIndexBuffer : indexBuffer);
            newBatch.OnBatchEmpty += OnBatchEmpty;
            // Initialize to clear any old states.
            newBatch.Initialize(renderer);

            _batches.Add(newBatch);
            SortBatches();
            Debug.Info($"Create new batch for: {renderer.GetType().Name}: {renderer.Name}: sort: {renderer.SortOrder} ({_batches.Count})");

            return newBatch;
        }

        private void OnBatchEmpty(Batch2D batch)
        {
            // Moves empty batch, and puts it to the end.
            batch.Clear();
            if (_batches.Remove(batch))
            {
                _batches.Add(batch);
                Debug.Log("Empty batch: ");
            }
            else
            {
                Debug.EngineError("Could not remove batch");
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

        internal List<Batch2D> GetActiveBatches()
        {
            return _batches;
        }
    }
}