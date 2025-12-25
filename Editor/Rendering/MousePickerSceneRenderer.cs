using Engine;
using Engine.Graphics;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Rendering
{
    internal class MousePickerSceneRenderer : SceneRendererBase
    {
        private List<Renderer2D> _worldRenderers;
        private List<Renderer2D> _uiRenderers;

        private readonly DrawCallData _drawCallData;
        private readonly PipelineFeatures _pipelineFeatures;
        private GfxResource _quadGeometry;
        private GeometryDescriptor _geoDesc;

        private readonly Shader _idPickerShader;
        private static readonly Comparison<Renderer2D> _sortingOrderComparer = (a, b) => a.SortOrder.CompareTo(b.SortOrder);

        private string _mousePickerVertShader = @"
          #version 330 core
          layout(location = 0) in vec3 position;
          layout(location = 1) in vec2 uv;
          layout(location = 2) in uint color; 
  
          out vec4 vColor;          
          out vec2 _UV;

          uniform mat4 uVP;

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
              vColor = unpackColor(color);
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
        void main()
        {
            fragColor = texture2D(uTex, _UV).a * vColor;

            if(fragColor.a <= 0.0001)
            {
                discard;
            }
        }
        ";

        private Dictionary<Guid, uint> _colorId = new();
        private uint _currentId = 0x10000000;
        private readonly List<(int verticesCount, GfxResource geometry, GeometryDescriptor desc)> _geometries = new();
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
        }

        protected override void OnPrepareRendering(List<Renderer2D> worldRenderers, List<Renderer2D> uiRenderers)
        {
            _worldRenderers = worldRenderers;
            _uiRenderers = uiRenderers;

            _worldRenderers.Sort(_sortingOrderComparer);
            _uiRenderers.Sort(_sortingOrderComparer);
        }


        public override RenderTexture OnRenderScene(RenderingSurface surface, ICamera camera, RenderTexture targetRenderTexture)
        {
            _drawCallData.DrawType = DrawType.Indexed;
            _drawCallData.DrawMode = DrawMode.Triangles;
            _drawCallData.Shader = _idPickerShader.NativeShader;
            _drawCallData.Features = _pipelineFeatures;
            _drawCallData.RenderTarget = targetRenderTexture.NativeResource;
            _drawCallData.Viewport = new vec4(0, 0, targetRenderTexture.Width, targetRenderTexture.Height);


            void DrawRenderers(List<Renderer2D> renderers, mat4 projM, mat4 viewM, mat4 viewProjM)
            {
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.VP_MATRIX].SetMat4(Consts.VIEW_PROJ_UNIFORM_NAME, viewProjM);
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.VIEW_MATRIX].SetMat4(Consts.VIEW_UNIFORM_NAME, viewM);
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.PROJECTION_MATRIX].SetMat4(Consts.PROJECTION_UNIFORM_NAME, projM);

                int TestColorIndex = 0;
                foreach (Renderer2D renderer in renderers)
                {
                    if (!_colorId.TryGetValue(renderer.GetID(), out var color))
                    {
                        // TODO: add list of releasedIds to, reuse.
                        _colorId.Add(renderer.GetID(), (uint)Random.Shared.Next());

                        //--color = _currentId;
                        //--_colorId.Add(renderer.GetID(), _currentId);
                        //--_currentId++; 

                        TestColorIndex++;
                    }

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
                        GraphicsHelper.CreateQuad(ref quadVertices, chunk.Uvs, width, height, chunk.Pivot, color, worldMatrix);

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
                        var vertices = new Vertex[renderer.Mesh.Vertices.Count];
                        for (int i = 0; i < renderer.Mesh.Vertices.Count; i++)
                        {
                            var cvertex = renderer.Mesh.Vertices[i];
                            cvertex.Color = color;
                            vertices[i] = cvertex;
                        }
                        vertex.Buffer = vertices;

                        GfxDeviceManager.Current.UpdateResouce(geometry, desc);
                        _drawCallData.Geometry = geometry;
                        _drawCallData.IndexedDraw.IndexCount = indicesCount;
                        
                        GfxDeviceManager.Current.Draw(_drawCallData);
                    }
                }
            }

            DrawRenderers(_worldRenderers, camera.Projection, camera.ViewMatrix, camera.Projection * camera.ViewMatrix);
            DrawRenderers(_uiRenderers, surface.UIProj, surface.UIView, surface.UIViewProj);

            return targetRenderTexture;
        }
    }
}
