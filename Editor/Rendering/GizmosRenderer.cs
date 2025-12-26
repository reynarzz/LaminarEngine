using Engine;
using Engine.Graphics;
using Engine.Graphics.Client;
using Engine.Rendering;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Rendering
{
    internal class GizmosRenderer : IGizmosRenderer
    {
        private readonly Batcher2D _batcher;
        private readonly DrawCallData _drawCallData;
        private readonly Dictionary<GizmoType, RendererData2D> _renderDatas;
        private List<Batch2D> _batches;
        private PipelineFeatures _pipelineFeatures;

        private List<RendererData2D> _camerasRenderData;
        private enum GizmoType
        {
            Camera,
        }


        public GizmosRenderer()
        {
            _batcher = new Batcher2D(Consts.Graphics.MAX_QUADS_PER_BATCH);
            _renderDatas = new Dictionary<GizmoType, RendererData2D>();
            _drawCallData = new DrawCallData()
            {
                Textures = new GfxResource[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits],
                Uniforms = new UniformValue[10],
            };

            _camerasRenderData = new List<RendererData2D>();
            _pipelineFeatures = new PipelineFeatures();
            _pipelineFeatures.DepthBuffer = true;
        }


        public void OnBegin()
        {
            var cameras = SceneManager.FindAll<Camera>(true);

            for (int i = 0; i < cameras.Count; i++)
            {
                var camera = cameras[i];
                if (_camerasRenderData.Count <= i)
                {
                    _camerasRenderData.Add(new RendererData2D(camera.GetID(), camera.Transform)
                    {
                        IsBillboard = true,
                        SortOrder = 20
                    });
                }
                else
                {
                    _camerasRenderData[i].Transform = camera.Transform;
                    _camerasRenderData[i].IsDirty = true;
                }
            }

            _batches = _batcher.GetBatches(_camerasRenderData);
        }

        public RenderTexture OnRender(ICamera camera, RenderTexture renderTarget)
        {
            foreach (var batch in _batches)
            {
                batch.Flush();

                int boundTex = 0;
                for (; boundTex < batch.Textures.Length; boundTex++)
                {
                    var tex = batch.Textures[boundTex];

                    if (tex == null)
                        break;

                    _drawCallData.Textures[boundTex] = tex.NativeResource;
                }

                // Set material's texture
                foreach (var (uniformName, texture) in batch.Material.Textures)
                {
                    _drawCallData.NamedTextures[uniformName] = texture.NativeResource;
                }

                // _drawCallData.Textures[screenGrabIndex] = pass.IsScreenGrabPass ? screenGrabTarget.NativeResource.SubResources[0] : null;

                // Pipeline
                //_pipelineFeatures.Blending = pass.Blending;
                //_pipelineFeatures.Stencil = pass.Stencil;

                _drawCallData.DrawType = batch.DrawType;
                _drawCallData.DrawMode = batch.DrawMode;
                _drawCallData.IndexedDraw.IndexCount = batch.IndexCount;
                _drawCallData.Shader = batch.Material.Shader.NativeShader;
                _drawCallData.Geometry = batch.Geometry;
                _drawCallData.Features = _pipelineFeatures;
                _drawCallData.RenderTarget = renderTarget.NativeResource;
                _drawCallData.Viewport = new vec4(0, 0, renderTarget.Width, renderTarget.Height);

                var VP = camera.Projection * camera.ViewMatrix;

                // Uniforms
                _drawCallData.Uniforms[0].SetMat4(Consts.VIEW_PROJ_UNIFORM_NAME, VP);
                _drawCallData.Uniforms[1].SetMat4(Consts.VIEW_UNIFORM_NAME, camera.ViewMatrix);
                _drawCallData.Uniforms[2].SetMat4(Consts.PROJECTION_UNIFORM_NAME, camera.Projection);
                _drawCallData.Uniforms[3].SetIntArr(Consts.TEX_ARRAY_UNIFORM_NAME, Batch2D.TextureSlotArray);
                _drawCallData.Uniforms[4].SetMat4(Consts.MODEL_UNIFORM_NAME, batch.WorldMatrix);
                _drawCallData.Uniforms[5].SetVec2(Consts.SCREEN_SIZE_UNIFORM_NAME, new vec2(renderTarget.Width, renderTarget.Height));
                _drawCallData.Uniforms[6].SetVec3(Consts.TIME_UNIFORM_NAME, new vec3(Time.UnscaledTimeWrap, Time.TimeCurrentWrap, Time.DeltaTime));


                GfxDeviceManager.Current.Draw(_drawCallData);
            }
            return renderTarget;
        }

        public void OnEnd()
        {
        }
    }
}
