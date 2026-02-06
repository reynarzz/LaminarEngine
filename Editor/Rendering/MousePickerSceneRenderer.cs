using Engine;
using Engine.Graphics;
using Engine.Utils;
using GlmNet;
using Engine;

namespace Editor.Rendering
{
    internal class MousePickerSceneRenderer : SceneRendererBase
    {
        private List<RendererData2D> _worldRenderers;
        private List<RendererData2D> _uiRenderers;
        private List<RendererData2D> _gizmosRenderers; // TODO: include gizmos
        private HashSet<Guid> _pickedRendererLayerList = new();
        public int PickedRenderersCount => _pickedRendererLayerList.Count;
        private readonly DrawCallData _drawCallData;
        private readonly PipelineFeatures _pipelineFeatures;
        private GfxResource _quadGeometry;
        private GeometryDescriptor _geoDesc;

        private readonly Shader _idPickerShader;
        private static readonly Comparison<RendererData2D> _sortingOrderComparer = (a, b) => a.SortOrder.CompareTo(b.SortOrder);
        private RenderTexture _backBuffer;
        public RenderTexture PickedBackBuffer => _backBuffer;

        private string _mousePickerVertShader = @"
          #version 330 core
          layout(location = 0) in vec3 position;
          layout(location = 1) in vec2 uv;
          layout(location = 2) in uint color; 
  
          out vec4 vColor;          
          out vec2 _UV;

          uniform mat4 uVP;
          uniform uint uColor;
          vec4 unpackColor(uint c) 
          {
              float r = float((c >> 24) & 0xFFu) / 255.0;
              float g = float((c >> 16) & 0xFFu) / 255.0;
              float b = float((c >>  8) & 0xFFu) / 255.0;
              float a = float( c        & 0xFFu) / 255.0;
              return vec4(r,g,b,a);
          }
  
          void main() 
          {
              vColor = unpackColor(uColor);
              _UV = uv;
              gl_Position = uVP * vec4(position, 1.0);
          }
        ";

        string _mousePickerFrag = @"
        #version 330 core
        in vec4 vColor;
        in vec2 _UV;

        out vec4 fragColor;
        uniform sampler2D uTex;
        uniform int uDiscard;
        void main()
        {
            fragColor = vColor;
            
            if(uDiscard == 1)
            {
                fragColor = step(0.01, texture(uTex, _UV).a) * vColor;
            
                if(fragColor.a <= 0.0001)
                {
                    discard;
                }
            }
        }
        ";
        private readonly List<(int verticesCount, GfxResource geometry, GeometryDescriptor desc)> _geometries = new();

        private Dictionary<uint, RendererData2D> _renderersByColors = new();
        private Dictionary<uint, RendererData2D> _renderersByColorsBackBuffer = new();
        internal IReadOnlyDictionary<uint, RendererData2D> RenderersIDs => _renderersByColors;
        internal IReadOnlyDictionary<uint, RendererData2D> RenderersIDsBackBuffer => _renderersByColorsBackBuffer;

        private uint _currentColorId = 0;
        public MousePickerSceneRenderer()
        {
            _drawCallData = new DrawCallData()
            {
                Textures = new GfxResource[1],
                Uniforms = new UniformValue[10],
            };

            _pipelineFeatures = new PipelineFeatures();
            _pipelineFeatures.Blending.Enabled = false;

            _quadGeometry = GraphicsHelper.CreateQuadGeometry(ref _geoDesc);

            unsafe
            {
                var vertex = _geoDesc.VertexDesc.BufferDesc as BufferDataDescriptor<Vertex>;
                vertex.Count = sizeof(Vertex) * 4;
            }

            _idPickerShader = new Shader(_mousePickerVertShader, _mousePickerFrag);
            _idPickerShader.Name = "Mouse picker ID";
            RenderTextureIndex = 1;
            _backBuffer = new RenderTexture(1920, 1080);
        }

        protected override void OnPrepareRendering(IReadOnlyCollection<RendererData2D> worldRenderers, IReadOnlyCollection<RendererData2D> uiRenderers)
        {
            _worldRenderers = worldRenderers.ToList();
            _uiRenderers = uiRenderers.ToList();



            _worldRenderers.Sort(_sortingOrderComparer);
            _uiRenderers.Sort(_sortingOrderComparer);
        }


        public void OnPickRenderer(Guid guid)
        {
            _pickedRendererLayerList.Add(guid);
        }

        public void ClearPickedList()
        {
            _pickedRendererLayerList.Clear();
        }

        public override RenderTexture OnRenderScene(RenderingSurface surface, ICamera camera, RenderTexture targetRenderTexture)
        {
            // Note: This code can be heavily improved, but right now is not a priority.
            void RenderAll(RenderTexture currentRenderTarget, Dictionary<uint, RendererData2D> colorsId, bool discardWithPickerList)
            {
                colorsId.Clear();
                _currentColorId = 0;

                _drawCallData.DrawType = DrawType.Indexed;
                _drawCallData.DrawMode = DrawMode.Triangles;
                _drawCallData.Shader = _idPickerShader.NativeShader;
                _drawCallData.Features = _pipelineFeatures;
                _drawCallData.RenderTarget = currentRenderTarget.NativeResource;
                _drawCallData.Viewport = new vec4(0, 0, currentRenderTarget.Width, currentRenderTarget.Height);
                GfxDeviceManager.Current.Clear(new ClearDeviceConfig()
                {
                     Color = Color.Black,
                     RenderTarget = currentRenderTarget.NativeResource
                });
                void DrawRenderers(List<RendererData2D> renderers, mat4 projM, mat4 viewM, mat4 viewProjM, bool discardAlphaWithTexture, bool discardWithPickedList)
                {
                    if (renderers == null || renderers.Count == 0)
                        return;

                    _drawCallData.Uniforms[0].SetMat4(Consts.VIEW_PROJ_UNIFORM_NAME, viewProjM);
                    _drawCallData.Uniforms[1].SetMat4(Consts.VIEW_UNIFORM_NAME, viewM);
                    _drawCallData.Uniforms[2].SetMat4(Consts.PROJECTION_UNIFORM_NAME, projM);
                    _drawCallData.Uniforms[3].SetInt("uDiscard", discardAlphaWithTexture ? 1 : 0);

                    for (int i = 0; i < renderers.Count; i++)
                    {
                        var renderer = renderers[i];
                        if (discardWithPickedList && _pickedRendererLayerList.Contains(renderer.GetID()))
                        {
                            continue;
                        }

                        _currentColorId++;
                        colorsId.Add(_currentColorId, renderer);

                        _drawCallData.Uniforms[4].SetUInt("uColor", _currentColorId);
                        var texture = renderer.Sprite?.Texture ?? Texture2D.White;
                        _drawCallData.Textures[0] = texture.NativeResource;

                        if (renderer.Mesh == null)
                        {
                            _drawCallData.Geometry = _quadGeometry;
                            _drawCallData.IndexedDraw.IndexCount = 6;

                            var chunk = renderer.Sprite?.GetAtlasCell() ?? TextureAtlasCell.DefaultChunk;
                            var worldMatrix = renderer.Transform.GetRenderingWorldMatrix();

                            float ppu = texture.PixelPerUnit;
                            var width = (float)chunk.Width / ppu;
                            var height = (float)chunk.Height / ppu;

                            QuadVertices quadVertices = default;
                            GraphicsHelper.CreateQuad(ref quadVertices, chunk.Uvs, width, height, chunk.Pivot, _currentColorId, worldMatrix);

                            var vertex = _geoDesc.VertexDesc.BufferDesc as BufferDataDescriptor<Vertex>;
                            for (int j = 0; j < QuadVertices.Count; j++)
                            {
                                vertex.Buffer[j] = quadVertices[j];
                            }

                            GfxDeviceManager.Current.UpdateResouce(_quadGeometry, _geoDesc);
                            GfxDeviceManager.Current.Draw(_drawCallData);
                        }
                        else
                        {
                            // Empty meshes cannot be picked
                            if (renderer.Mesh.Vertices.Count == 0 || renderer.Mesh.IndicesToDrawCount == 0)
                                continue;

                            var bestGeometryMatchPair = _geometries.FirstOrDefault(x => x.verticesCount >= renderer.Mesh.Vertices.Count);

                            var geometry = default(GfxResource);
                            var desc = default(GeometryDescriptor);
                            var indicesCount = 0;

                            if (bestGeometryMatchPair == default || bestGeometryMatchPair.geometry == null)
                            {
                                if (renderer.Mesh.Indices != null && renderer.Mesh.Indices.Length > 0)
                                {
                                    indicesCount = renderer.Mesh.Indices.Length;
                                }
                                else
                                {
                                    indicesCount = renderer.Mesh.IndicesToDrawCount;

                                }

                                var indexBuffer = GraphicsHelper.CreateQuadIndexBuffer(indicesCount);
                                geometry = GraphicsHelper.GetEmptyGeometry<Vertex>(renderer.Mesh.Vertices.Count, 0, ref desc, indexBuffer);

                                _geometries.Add((renderer.Mesh.Vertices.Count, geometry, desc));
                            }
                            else
                            {
                                geometry = bestGeometryMatchPair.geometry;
                                desc = bestGeometryMatchPair.desc;
                                indicesCount = renderer.Mesh.IndicesToDrawCount;
                            }
                            var vertex = desc.VertexDesc.BufferDesc as BufferDataDescriptor<Vertex>;

                            unsafe
                            {
                                vertex.Count = sizeof(Vertex) * renderer.Mesh.Vertices.Count;
                            }

                            vertex.Buffer = renderer.Mesh.Vertices.ToArray();

                            _drawCallData.Geometry = geometry;
                            _drawCallData.IndexedDraw.IndexCount = indicesCount;

                            GfxDeviceManager.Current.UpdateResouce(geometry, desc);
                            GfxDeviceManager.Current.Draw(_drawCallData);
                        }
                    }
                }

                DrawRenderers(_worldRenderers, camera.Projection, camera.ViewMatrix, camera.Projection * camera.ViewMatrix, true, discardWithPickerList);

                // Note: UI does not discard the alpha of the current 2D renderer's texture so it can render a whole block of geometry and is easier to pick up for things like text.
                DrawRenderers(_uiRenderers, surface.UIProj, surface.UIView, surface.UIViewProj, false, discardWithPickerList);
            }

            RenderAll(targetRenderTexture, _renderersByColors, true);
            RenderAll(_backBuffer, _renderersByColorsBackBuffer, false);

            return targetRenderTexture;
        }

        protected override void OnRenderingEnd()
        {

        }
    }
}
