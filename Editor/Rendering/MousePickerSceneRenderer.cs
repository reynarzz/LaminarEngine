using Engine;
using Engine.Graphics;
using Engine.Utils;
using GlmNet;

namespace Editor.Rendering
{
    internal class MousePickerSceneRenderer : SceneRendererBase
    {
        private List<RendererData2D> _worldRenderers;
        private List<RendererData2D> _uiRenderers;
        private readonly DrawCallData _drawCallData;
        private readonly PipelineFeatures _pipelineFeatures;
        private GfxResource _quadGeometry;
        private GeometryDescriptor _geoDesc;

        private readonly Shader _idPickerShader;
        private static readonly Comparison<RendererData2D> _sortingOrderComparer = (a, b) => a.SortOrder.CompareTo(b.SortOrder);

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
        internal IReadOnlyDictionary<uint, RendererData2D> RenderersIDs => _renderersByColors;

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
        }

        protected override void OnPrepareRendering(List<RendererData2D> worldRenderers, List<RendererData2D> uiRenderers)
        {
            _worldRenderers = worldRenderers;
            _uiRenderers = uiRenderers;

            _worldRenderers.Sort(_sortingOrderComparer);
            _uiRenderers.Sort(_sortingOrderComparer);
        }

        public override RenderTexture OnRenderScene(RenderingSurface surface, ICamera camera, RenderTexture targetRenderTexture)
        {
            _renderersByColors.Clear();
            _currentColorId = 0;

            _drawCallData.DrawType = DrawType.Indexed;
            _drawCallData.DrawMode = DrawMode.Triangles;
            _drawCallData.Shader = _idPickerShader.NativeShader;
            _drawCallData.Features = _pipelineFeatures;
            _drawCallData.RenderTarget = targetRenderTexture.NativeResource;
            _drawCallData.Viewport = new vec4(0, 0, targetRenderTexture.Width, targetRenderTexture.Height);


            void DrawRenderers(List<RendererData2D> renderers, mat4 projM, mat4 viewM, mat4 viewProjM, bool discardAlphaWithTexture)
            {
                _drawCallData.Uniforms[0].SetMat4(Consts.VIEW_PROJ_UNIFORM_NAME, viewProjM);
                _drawCallData.Uniforms[1].SetMat4(Consts.VIEW_UNIFORM_NAME, viewM);
                _drawCallData.Uniforms[2].SetMat4(Consts.PROJECTION_UNIFORM_NAME, projM);
                _drawCallData.Uniforms[3].SetInt("uDiscard", discardAlphaWithTexture ? 1 : 0);

                foreach (RendererData2D renderer in renderers)
                {
                    _currentColorId++;
                    _renderersByColors.Add(_currentColorId, renderer);

                    _drawCallData.Uniforms[4].SetUInt("uColor", _currentColorId);
                    var texture = renderer.Sprite?.Texture ?? Texture2D.White;
                    _drawCallData.Textures[0] = texture.NativeResource;

                    if (renderer.Mesh == null)
                    {
                        _drawCallData.Geometry = _quadGeometry;
                        _drawCallData.IndexedDraw.IndexCount = 6;

                        var chunk = renderer.Sprite?.GetAtlasChunk() ?? AtlasChunk.DefaultChunk;
                        var worldMatrix = renderer.Transform.GetRenderingWorldMatrix();

                        float ppu = texture.PixelPerUnit;
                        var width = (float)chunk.Width / ppu;
                        var height = (float)chunk.Height / ppu;

                        QuadVertices quadVertices = default;
                        GraphicsHelper.CreateQuad(ref quadVertices, chunk.Uvs, width, height, chunk.Pivot, _currentColorId, worldMatrix);

                        var vertex = _geoDesc.VertexDesc.BufferDesc as BufferDataDescriptor<Vertex>;
                        for (int i = 0; i < QuadVertices.Count; i++)
                        {
                            vertex.Buffer[i] = quadVertices[i];
                        }

                        GfxDeviceManager.Current.UpdateResouce(_quadGeometry, _geoDesc);
                        GfxDeviceManager.Current.Draw(_drawCallData);
                    }
                    else
                    {
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

            DrawRenderers(_worldRenderers, camera.Projection, camera.ViewMatrix, camera.Projection * camera.ViewMatrix, true);

            // Note: UI do not discard using texture so it can render whole block of geometry and is easier to pick up for things like text.
            
            DrawRenderers(_uiRenderers, surface.UIProj, surface.UIView, surface.UIViewProj, false);

            return targetRenderTexture;
        }

        protected override void OnRenderingEnd()
        {

        }
    }
}
