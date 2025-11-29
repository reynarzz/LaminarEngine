using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using GlmNet;
using Engine.Graphics.OpenGL;

namespace Engine.Rendering
{
    internal class Batch2D : IDisposable
    {
        internal int MaxVertexSize { get; }
        internal Material Material { get; private set; }
        internal GfxResource Geometry { get; private set; }
        internal Texture[] Textures { get; private set; }
        internal static int[] TextureSlotArray { get; }

        // TODO: cache this
        internal int VertexCount
        {
            get
            {
                var vertexCount = 0;
                foreach (var renderers in _renderers.Values)
                {
                    vertexCount += renderers.VertexCount;
                }

                return vertexCount;
            }
        }

        // TODO: cache this
        internal int IndexCount
        {
            get
            {
                var amount = 0;
                foreach (var renderers in _renderers.Values)
                {
                    amount += renderers.IndexCount;
                }

                return amount;
            }
        }

        // internal int IndexCount { get; private set; }
        internal bool IsActive { get; private set; }
        internal DrawMode DrawMode { get; set; } = DrawMode.Triangles;
        internal DrawType DrawType { get; set; } = DrawType.Indexed;
        internal mat4 WorldMatrix { get; set; } = mat4.identity();
        internal int SortOrder { get; set; } = int.MinValue;
        internal event Action<Batch2D> OnBatchEmpty;

        private bool _isDirty;
        private int _vertexOffset = int.MaxValue;
        private Dictionary<Guid, RendererIds> _renderers;
        public int RenderersCount => _renderers.Count;
        private GeometryDescriptor _geoDescriptor;
        private Vertex[] _verticesData;
        private struct RendererIds
        {
            public Renderer Renderer;
            public int RendererId;
            public int TextureId;
            public int VertexCount;
            public int IndexCount;
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

        internal Batch2D(int maxVertexSize, GfxResource sharedIndexBuffer, uint[] rawIndices)
        {
            MaxVertexSize = maxVertexSize;
            _verticesData = new Vertex[MaxVertexSize];
            Textures = new Texture[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits - 5];
            _renderers = new Dictionary<Guid, RendererIds>();

            // Create geometry buffer for this batch
            _geoDescriptor = new GeometryDescriptor();
            if (rawIndices != null)
            {
                _geoDescriptor.IndexDesc = new BufferDataDescriptor<uint>()
                {
                    Buffer = rawIndices,
                    Usage = BufferUsage.Dynamic
                };
            }
            else
            {
                _geoDescriptor.SharedIndexBuffer = sharedIndexBuffer;
            }
            _geoDescriptor.VertexDesc = new VertexDataDescriptor()
            {
                BufferDesc = new BufferDataDescriptor<Vertex>() { Buffer = _verticesData, Usage = BufferUsage.Dynamic },
                Attribs = GraphicsHelper.GetVertexAttribs<Vertex>()
            };

            Geometry = GfxDeviceManager.Current.CreateGeometry(_geoDescriptor);
        }
        internal string GetRenderersNames()
        {
            var str = new StringBuilder();

            foreach (var item in _renderers.Values)
            {
                str.Append("\n" + item.Renderer.Name);
            }

            return str.ToString();
        }
        internal bool Initialize(Renderer2D renderer)
        {
            if (IsActive)
            {
                return false;
            }
            Clear();
            SortOrder = renderer.SortOrder;
            IsActive = true;
            return true;
        }

        internal void Clear()
        {
            SortOrder = int.MinValue;
            Material = null;
            _isDirty = false;
            IsActive = false;
            _vertexOffset = int.MaxValue;
            _renderers.Clear();

            for (int i = 0; i < Textures.Length; i++)
            {
                Textures[i] = null;
            }

            SendGeometryUpdate();
        }

        internal void PushGeometry(Renderer renderer, Material material, Texture texture, int indicesCount, Span<Vertex> vertices)
        {
            _isDirty = true;
            IsActive = true;
            if (!Material)
            {
                Material = material;
            }

            int textureIndex = 0;
            // Adds texture to a empty slot
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

            renderer.OnDestroyRenderer -= RemoveRenderer;
            renderer.OnDestroyRenderer += RemoveRenderer;

            var startIndex = 0;
            var existId = _renderers.TryGetValue(renderer.GetID(), out var rendererIds);

            if (existId)
            {
                rendererIds.VertexCount = vertices.Length;
                rendererIds.IndexCount = indicesCount;
                rendererIds.TextureId = textureIndex; // added recently, if causes problems remove
                _renderers[renderer.GetID()] = rendererIds;

                startIndex = rendererIds.RendererId;
            }
            else
            {
                startIndex = VertexCount;
                _renderers.Add(renderer.GetID(), new RendererIds()
                {
                    Renderer = renderer,
                    RendererId = startIndex,
                    TextureId = textureIndex,
                    VertexCount = vertices.Length,
                    IndexCount = indicesCount
                });
            }

            // Copies vertices data
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].TextureIndex = textureIndex;
                _verticesData[startIndex + i] = vertices[i];
                _vertexOffset = Math.Min(_vertexOffset, startIndex + i);
            }
        }

        public void RemoveRenderer(Renderer renderer)
        {
            renderer.OnDestroyRenderer -= RemoveRenderer;
            Debug.Log("Remove renderer from batch: " + renderer.Name);
            // If renderer doesn't exist, do nothing
            if (!_renderers.TryGetValue(renderer.GetID(), out var removedInfo))
            {
                Debug.EngineError("Wrong batch for renderer: " + renderer.Name);
                return;
            }

            int removedVertexStart = removedInfo.RendererId;
            int removedTextureId = removedInfo.TextureId;

            int removedVertexCount;
            int removedIndexCount;

            if (renderer.Mesh == null)
            {
                removedVertexCount = 4;
                removedIndexCount = 6;
            }
            else
            {
                removedVertexCount = renderer.Mesh.Vertices.Count;
                removedIndexCount = renderer.Mesh.IndicesToDrawCount;
            }

            int oldVertexCount = VertexCount;
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

            bool textureUnused = !_renderers.Values.Any(v => v.TextureId == removedTextureId);

            if (textureUnused)
            {
                Textures[removedTextureId] = null;

                if (removedTextureId < Textures.Length - 1)
                {
                    Array.Copy(
                        Textures,
                        removedTextureId + 1,
                        Textures,
                        removedTextureId,
                        Textures.Length - (removedTextureId + 1)
                    );
                }
                var renderers = _renderers.Keys.ToList();

                foreach (var key in renderers)
                {
                    var info = _renderers[key];

                    if (info.TextureId > removedTextureId)
                    {
                        info.TextureId--;
                        _renderers[key] = info;

                        // if (info.RendererId < _verticesData.Length)
                        {
                            for (int i = 0; i < info.VertexCount; i++)
                            {
                                _verticesData[info.RendererId + i].TextureIndex = info.TextureId;
                            }
                        }
                    }
                }
            }

            int trailingVertexCount = oldVertexCount - (removedVertexStart + removedVertexCount);

            if (trailingVertexCount > 0)
            {
                Array.Copy(
                    _verticesData,
                    removedVertexStart + removedVertexCount,
                    _verticesData,
                    removedVertexStart,
                    trailingVertexCount
                );
            }

            foreach (var key in _renderers.Keys.ToList())
            {
                var info = _renderers[key];

                if (info.RendererId > removedVertexStart)
                {
                    info.RendererId -= removedVertexCount;
                    _renderers[key] = info;
                }
            }
        }

        internal void Flush()
        {
            // TODO: only update changed vertices
            if (_isDirty)
            {
                SendGeometryUpdate();
                _vertexOffset = VertexCount;
            }

            _isDirty = false;
        }

        private void SendGeometryUpdate()
        {
            var vertDataDescriptor = _geoDescriptor.VertexDesc.BufferDesc;

            unsafe
            {
                // vertDataDescriptor.Offset = sizeof(Vertex) * _vertexOffset;
                //vertDataDescriptor.Count = sizeof(Vertex) * (VertexCount - _vertexOffset);

                vertDataDescriptor.Offset = sizeof(Vertex) * 0;
                vertDataDescriptor.Count = sizeof(Vertex) * VertexCount;
            }

            //(vertDataDescriptor as BufferDataDescriptor<Vertex>).Buffer = _verticesData;

            GfxDeviceManager.Current.UpdateGeometry(Geometry, _geoDescriptor);
        }

        internal bool CanPushGeometry(Renderer2D renderer, int vertexCount, int neededBatchVertexSize, Texture texture, Material mat)
        {
            var isMaxSizeEnough = MaxVertexSize >= neededBatchVertexSize;
            var hasSpaceLeftForAnother = (MaxVertexSize - VertexCount) >= vertexCount;
            var isBatchSizeEnough = isMaxSizeEnough && hasSpaceLeftForAnother;
            var isSameSortOrder = renderer.SortOrder == SortOrder || SortOrder == int.MinValue;
            var isValidMaterial = Material == mat || !Material;

            var isvalidLayout = isBatchSizeEnough && ((isValidMaterial && isSameSortOrder) || !IsActive);

            if (!isvalidLayout)
            {
                return false;
            }
            //if (renderer.SortOrder != SortOrder || SortOrder != int.MinValue)
            //{
            //    return false;
            //}
            //if (vertexCount + VertexCount - subsTractVertices > MaxVertexSize)
            //{
            //    return false;
            //}
            //if (!Material) 
            //    return true;

            //if (mat != Material)
            //    return false;

            // Also removes the textures from the material to avoid binding more textures than the plaform supports.

            if (Material)
            {
                for (int i = 0; i < Textures.Length - Material.Textures.Count; i++)
                {
                    if (Textures[i] == null || texture.NativeResource == Textures[i].NativeResource)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            GfxDeviceManager.Current.DestroyResource(Geometry);

            foreach (var renderers in _renderers.Values.ToList())
            {
                renderers.Renderer.OnDestroyRenderer -= RemoveRenderer;
            }
            _renderers.Clear();
            Material = null;
            Geometry = null;
            _verticesData = null;
            Textures = null;
            _geoDescriptor = null;

        }

        internal bool Contains(Renderer renderer)
        {
            return _renderers.ContainsKey(renderer.GetID());
        }
    }
}
