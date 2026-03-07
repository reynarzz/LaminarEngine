using Engine.Graphics;
using System;
using GlmNet;
using System.Runtime.CompilerServices;

namespace Engine.Rendering
{
    internal class Batch2D : IDisposable
    {
        internal int MaxVertexSize { get; private set; }
        internal Material Material { get; private set; }
        internal GfxResource Geometry { get; private set; }
        internal Texture[] Textures { get; private set; }
        internal static int[] TextureSlotArray { get; }

        private int _vertexCount;
        private int _indexCount;

        internal int VertexCount => _vertexCount;
        internal int IndexCount => _indexCount;

        internal bool IsActive { get; private set; }
        internal DrawMode DrawMode { get; set; } = DrawMode.Triangles;
        internal DrawType DrawType { get; set; } = DrawType.Indexed;
        internal mat4 WorldMatrix { get; set; } = mat4.identity();
        internal int SortOrder { get; set; } = int.MinValue;
        internal event Action<Batch2D> OnBatchEmpty;

        private bool _isDirty;
        private Dictionary<Guid, RendererIds> _renderers;
        private bool _isPrivateBatch;
        internal Type VertexType { get; private set; }
        public int RenderersCount => _renderers.Count;
        private GeometryDescriptor _geoDescriptor;
        private VertexCollection _vertexCollection;
        internal abstract class VertexCollection
        {
            internal abstract int Count { get; }
            internal abstract void SetVertex<T>(int startIndex, T value) where T : unmanaged, IVertex2D<T>;
            internal abstract void SetMeshVertices(int startIndex, int textureIndex, Mesh mesh, bool isPrivateBatch);
            internal abstract void SetVertexTextureIndex(int index, int textureIndex);
            internal abstract void PackVertices(int startIndex, int destinationIndex, int length);
        }

        private class VertexCollection<T> : VertexCollection where T : unmanaged, IVertex2D<T>
        {
            private T[] _vertices;
            public T[] Vertices => _vertices;
            internal override int Count => _vertices.Length;
            internal VertexCollection(int count)
            {
                _vertices = new T[count];
            }

            internal override void SetVertex<T1>(int index, T1 vertex)
            {
                ref var vertexT = ref Unsafe.As<T1, T>(ref vertex);
                _vertices[index] = vertexT;
            }
            internal override void SetVertexTextureIndex(int index, int textureIndex)
            {
                _vertices[index].TextureIndex = textureIndex;
            }

            internal override void PackVertices(int startIndex, int destinationIndex, int length)
            {
                Array.Copy(_vertices, startIndex, _vertices, destinationIndex, length);
            }

            internal override void SetMeshVertices(int startIndex, int textureIndex, Mesh mesh, bool isPrivateBatch)
            {
                var meshT = mesh as Mesh<T>;

                //if (!_isPrivateBatch)
                //{
                //    //_vertices = meshT.Vertices;
                //}
                //else
                {
                    for (int i = 0; i < meshT.Vertices.Count; i++)
                    {
                        var vert = meshT.Vertices[i];
                        vert.TextureIndex = textureIndex;
                        _vertices[startIndex + i] = vert;
                    }
                }
            }
        }

        private struct RendererIds
        {
            public RendererData2D Renderer;
            public int RendererId;
            public int TextureId;
            public int VertexCount;
            public int IndexCount;
        }
        private Batch2D()
        {
        }

        static Batch2D()
        {
            if (TextureSlotArray == null)
            {
                TextureSlotArray = new int[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits - 5];
                for (int i = 0; i < TextureSlotArray.Length; i++)
                {
                    TextureSlotArray[i] = i;
                }
            }
        }

        internal static Batch2D CreateBatch<T>(int maxVertexSize, GfxResource sharedIndexBuffer, uint[] rawIndices) where T : unmanaged, IVertex2D<T>
        {
            var verticesCollection = new VertexCollection<T>(maxVertexSize);

            var batch = new Batch2D()
            {
                MaxVertexSize = maxVertexSize,
                VertexType = typeof(T),
                Textures = new Texture[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits - 5],
                _renderers = new Dictionary<Guid, RendererIds>(),
                _geoDescriptor = new GeometryDescriptor(),
                _vertexCollection = verticesCollection
            };

            if (rawIndices != null)
            {
                batch._geoDescriptor.IndexDesc = new BufferDataDescriptor<uint>()
                {
                    Buffer = rawIndices,
                    Usage = BufferUsage.Dynamic
                };
            }
            else
            {
                batch._geoDescriptor.SharedIndexBuffer = sharedIndexBuffer;
            }

            unsafe
            {
                batch._geoDescriptor.VertexDesc = new VertexDataDescriptor()
                {
                    BufferDesc = new BufferDataDescriptor<T>()
                    {
                        Buffer = verticesCollection.Vertices,
                        Usage = BufferUsage.Dynamic,
                        Count = sizeof(T) * verticesCollection.Vertices.Length,
                        Offset = 0,
                    },
                    Attribs = GraphicsHelper.GetVertexAttribs<T>()
                };
            }
            batch.Geometry = GfxDeviceManager.Current.CreateGeometry(batch._geoDescriptor);

            return batch;
        }

        internal bool Initialize(RendererData2D renderer)
        {
            if (IsActive)
                return false;

            Clear();
            SortOrder = renderer.SortOrder;
            IsActive = true;
            _isPrivateBatch = renderer.PrivateBatch;
            return true;
        }

        internal void Clear()
        {
            SortOrder = int.MinValue;
            Material = null;
            _isDirty = false;
            IsActive = false;
            _vertexCount = 0;
            _indexCount = 0;
            _isPrivateBatch = false;
            _renderers.Clear();

            for (int i = 0; i < Textures.Length; i++)
            {
                Textures[i] = null;
            }
        }

        private bool SetTextureToEmptySlot(Texture texture, out int textureIndex)
        {
            textureIndex = -1;
            for (int i = 0; i < Textures.Length; i++)
            {
                if (Textures[i] == null)
                {
                    Textures[i] = texture;
                    textureIndex = i;
                    break;
                }
                else if (Textures[i].NativeResource == texture.NativeResource)
                {
                    textureIndex = i;
                    break;
                }
            }
            return textureIndex >= 0;
        }
        private void ReserveGeometry(RendererData2D renderer, Material material, Texture texture, int indicesCount, int verticesCount,
                                    ref int textureIndex, ref int startIndex)
        {
            _isDirty = true;
            IsActive = true;
            if (!Material)
            {
                Material = material;
            }

            bool textureFound = SetTextureToEmptySlot(texture, out textureIndex);
            if (!textureFound)
            {
                Debug.EngineError("Tried to add texture to a full batch");
            }

            renderer.OnDestroyRenderer -= RemoveRenderer;
            renderer.OnDestroyRenderer += RemoveRenderer;

            var id = renderer.GetID();
            startIndex = 0;

            if (_renderers.TryGetValue(id, out var rendererIds))
            {
                _vertexCount -= rendererIds.VertexCount;
                _indexCount -= rendererIds.IndexCount;

                rendererIds.VertexCount = verticesCount;
                rendererIds.IndexCount = indicesCount;
                rendererIds.TextureId = textureIndex;
                _renderers[id] = rendererIds;

                startIndex = rendererIds.RendererId;
            }
            else
            {
                startIndex = _vertexCount;
                _renderers.Add(id, new RendererIds()
                {
                    Renderer = renderer,
                    RendererId = startIndex,
                    TextureId = textureIndex,
                    VertexCount = verticesCount,
                    IndexCount = indicesCount
                });
            }

            _vertexCount += verticesCount;
            _indexCount += indicesCount;
        }

        internal void PushGeometry(RendererData2D renderer, Material material, Texture texture, int indicesCount, Mesh mesh)
        {
            int textureIndex = 0;
            int startIndex = 0;
            ReserveGeometry(renderer, material, texture, indicesCount, mesh.VertexCount, ref textureIndex, ref startIndex);

            _vertexCollection.SetMeshVertices(startIndex, textureIndex, mesh, renderer.PrivateBatch);
        }

        internal void PushGeometry<T>(RendererData2D renderer, Material material, Texture texture, int indicesCount, int verticesCount,
                                   ref int textureIndex, ref int startIndex, ref T[] vertices)
            where T : unmanaged, IVertex2D<T>
        {
            vertices = (_vertexCollection as VertexCollection<T>).Vertices;
            ReserveGeometry(renderer, material, texture, indicesCount, verticesCount, ref textureIndex, ref startIndex);
        }

        internal bool ReplaceTexture(RendererData2D renderer, Texture texture)
        {
            if (_renderers.TryGetValue(renderer.GetID(), out var currentRendererId))
            {
                bool anotherUsesTexture = false;
                foreach (var rendererId in _renderers.Values)
                {
                    if (rendererId.Renderer == renderer)
                        continue;

                    if (rendererId.TextureId == currentRendererId.TextureId)
                    {
                        anotherUsesTexture = true;
                        break;
                    }
                }

                if (anotherUsesTexture)
                {
                    return SetTextureToEmptySlot(texture, out _);
                }

                Textures[currentRendererId.TextureId] = texture;
                return true;
            }
            return false;
        }

        public void RemoveRenderer(RendererData renderer)
        {
            renderer.OnDestroyRenderer -= RemoveRenderer;
            if (!_renderers.TryGetValue(renderer.GetID(), out var removedInfo))
                return;

            int removedVertexStart = removedInfo.RendererId;
            int removedTextureId = removedInfo.TextureId;
            int removedVertexCount = renderer.Mesh == null ? 4 : removedInfo.VertexCount;
            int removedIndexCount = renderer.Mesh == null ? 6 : removedInfo.IndexCount;

            _vertexCount -= removedVertexCount;
            _indexCount -= removedIndexCount;
            _isDirty = true;

            _renderers.Remove(renderer.GetID());

            if (_renderers.Count == 0)
            {
                if (removedTextureId >= 0 && removedTextureId < Textures.Length)
                    Textures[removedTextureId] = null;

                IsActive = false;
                OnBatchEmpty?.Invoke(this);
                return;
            }

            bool textureUnused = true;
            foreach (var r in _renderers.Values)
            {
                if (r.TextureId == removedTextureId)
                {
                    textureUnused = false;
                    break;
                }
            }

            if (textureUnused)
            {
                Textures[removedTextureId] = null;

                if (removedTextureId < Textures.Length - 1)
                {
                    Array.Copy(Textures, removedTextureId + 1, Textures, removedTextureId, Textures.Length - removedTextureId - 1);
                }

                foreach (var key in _renderers.Keys)
                {
                    var info = _renderers[key];
                    if (info.TextureId > removedTextureId)
                    {
                        info.TextureId--;
                        _renderers[key] = info;

                        for (int i = 0; i < info.VertexCount; i++)
                        {
                            _vertexCollection.SetVertexTextureIndex(info.RendererId + i, info.TextureId);
                        }
                    }
                }
            }

            int trailingVertexCount = VertexCount - (removedVertexStart + removedVertexCount);
            if (trailingVertexCount > 0)
            {
                _vertexCollection.PackVertices(removedVertexStart + removedVertexCount, removedVertexStart, trailingVertexCount);
            }

            foreach (var key in _renderers.Keys)
            {
                var info = _renderers[key];
                if (info.RendererId > removedVertexStart)
                {
                    info.RendererId -= removedVertexCount;
                    if (info.RendererId < 0)
                    {
                        Debug.EngineError($"RendererId is less than 0: {info.RendererId}");
                        info.RendererId = 0;
                    }
                    _renderers[key] = info;
                }
            }
        }

        internal void Flush()
        {
            if (_isDirty)
            {
                GfxDeviceManager.Current.UpdateResouce(Geometry, _geoDescriptor);
                _isDirty = false;
            }
        }

        internal bool CanPushGeometry(RendererData2D renderer, int vertexCount, int neededBatchVertexSize, Texture texture, Material mat)
        {
            var isMaxSizeEnough = MaxVertexSize >= neededBatchVertexSize;
            var hasSpace = (MaxVertexSize - VertexCount) >= vertexCount;
            var isSameSortOrder = renderer.SortOrder == SortOrder || SortOrder == int.MinValue;
            var isValidMaterial = Material == mat || !Material;

            bool validLayout = isMaxSizeEnough && hasSpace && ((isValidMaterial && isSameSortOrder) || !IsActive);
            if (!validLayout)
                return false;

            if (Material)
            {
                for (int i = 0; i < Textures.Length - Material.Textures.Count; i++)
                {
                    if (Textures[i] == null || texture.NativeResource == Textures[i].NativeResource)
                    {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            GfxDeviceManager.Current.DestroyResource(Geometry);
            foreach (var rendererId in _renderers.Values)
            {
                rendererId.Renderer.OnDestroyRenderer -= RemoveRenderer;
            }
            _renderers.Clear();
            Material = null;
            Geometry = null;
            _vertexCollection = null;
            Textures = null;
            _geoDescriptor = null;
        }

        internal bool Contains(RendererData2D renderer)
        {
            return _renderers.ContainsKey(renderer.GetID());
        }
    }
}