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
                    batchOut = batch;
                    return true;
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

            foreach (var batch in _batches)
            {
                var isTotalSizeEnough = batch.MaxVertexSize >= maxVertexSize;
                var hasSpaceLeftForAnother = (batch.MaxVertexSize - batch.VertexCount) > vertexToAdd;
                //var hasSpaceLeftForAnother = batch.VertexCount > vertexToAdd && !batch.Contains(renderer);

                if (isTotalSizeEnough && hasSpaceLeftForAnother && (batch.Material == mat || batch.Material == null) && ((renderer.SortOrder == batch.SortOrder || batch.SortOrder == int.MinValue) || !batch.IsActive))
                {
                    batch.Initialize(renderer);
                    // _batches.Sort((x, y) => x.SortOrder.CompareTo(y.SortOrder));

                    return batch;
                }
            }

            Batch2D newBatch = new Batch2D(maxVertexSize, indexBuffer == null ? _sharedIndexBuffer : indexBuffer);
            newBatch.OnBatchEmpty += OnBatchEmpty;
            // Initialize to clear any old states.
            newBatch.Initialize(renderer);

            _batches.Add(newBatch);
            _batches.Sort((x, y) => x.SortOrder.CompareTo(y.SortOrder));
            Debug.Info($"Create new batch for renderer: {renderer.Name}: ({_batches.Count})");

            return newBatch;
        }

        private void OnBatchEmpty(Batch2D batch)
        {
            // Moves empty batch, and puts it to the end.
            _batches.Remove(batch);
            _batches.Add(batch);
            batch.Clear();
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