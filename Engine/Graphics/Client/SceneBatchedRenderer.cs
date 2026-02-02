using Engine.Rendering;
using Engine.Utils;
using GlmNet;

namespace Engine.Graphics
{
    internal class SceneBatchedRenderer : SceneRendererBase
    {
        private readonly Batcher2D _worldBatcher;
        private readonly Batcher2D _uiBatcher;
        private List<Batch2D> _worldBatches;
        private List<Batch2D> _uiBatches;
        private readonly DrawCallData _drawCallData;
        private readonly RenderTexture _screenGrabTarget;
        private readonly PipelineFeatures _pipelineFeatures;

        public SceneBatchedRenderer()
        {
            _worldBatcher = new Batcher2D(Consts.Graphics.MAX_QUADS_PER_BATCH);
            _uiBatcher = new Batcher2D(Consts.Graphics.MAX_QUADS_PER_BATCH);

            _drawCallData = new DrawCallData()
            {
                Textures = new GfxResource[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits],
                Uniforms = new UniformValue[GfxDeviceManager.Current.GetDeviceInfo().MaxUniformsCount],
            };
            _screenGrabTarget = new RenderTexture(Screen.Width, Screen.Height);
            _pipelineFeatures = new PipelineFeatures();
        }

        protected override void OnPrepareRendering(IReadOnlyCollection<RendererData2D> worldRenderers, IReadOnlyCollection<RendererData2D> uiRenderers)
        {
            _worldBatches = _worldBatcher.GetBatches(worldRenderers);
            _uiBatches = _uiBatcher.GetBatches(uiRenderers);
        }

        public override RenderTexture OnRenderScene(RenderingSurface surface, ICamera camera, RenderTexture targetRenderTexture)
        {
            var VP = camera.Projection * camera.ViewMatrix;

            var geoBatchesInfo = RenderBatches(camera, _worldBatches, ref VP, targetRenderTexture);

            var uiBatchesInfo = default(RenderingBatchesInfo);
            if (surface.RenderUI)
            {
                uiBatchesInfo = RenderBatches(camera, _uiBatches, ref surface.UIViewProj, targetRenderTexture, targetRenderTexture);
                EngineInfo.Renderer.UIBatches = uiBatchesInfo.BatchesCount;
                EngineInfo.Renderer.UIGrabScreenPass = uiBatchesInfo.ScreenGrabPasses;
            }

            EngineInfo.Renderer.WBatches = geoBatchesInfo.BatchesCount;
            EngineInfo.Renderer.GrabScreenPass = geoBatchesInfo.ScreenGrabPasses;
            EngineInfo.Renderer.PostProcessingPasses = PostProcessingStack.Passes.Count;
            EngineInfo.Renderer.SavedByBatching = (geoBatchesInfo.TotalRenderers - geoBatchesInfo.BatchesCount) * (uiBatchesInfo.ScreenGrabPasses + 1);

            return targetRenderTexture;
        }

        private RenderingBatchesInfo RenderBatches(ICamera camera, List<Batch2D> batches, ref mat4 VP, RenderTexture sceneRenderTarget, RenderTexture grabBlitTarget = null)
        {
            RenderingBatchesInfo info = default;
            for (int i = 0; i < batches.Count; i++)
            {
                var batch = batches[i];
                if (!batch.IsActive)
                {
                    break;
                }
                info.BatchesCount++;
                info.TotalRenderers += batch.RenderersCount;
                batch.Flush();

                //   if (batch.Material)
                {
                    var isScreenGrabPass = batch.Material.Passes.Any(x => x.IsScreenGrabPass);

                    if (isScreenGrabPass)
                    {
                        //GfxDeviceManager.Current.Clear(new ClearDeviceConfig()
                        //{
                        //    Color = _mainCamera.BackgroundColor,
                        //    RenderTarget = _screenGrabTarget.NativeResource
                        //});

                        if (grabBlitTarget)
                        {
                            GfxDeviceManager.Current.BlitRenderTargetTo(grabBlitTarget.NativeResource, _screenGrabTarget.NativeResource);
                        }

                        info.ScreenGrabPasses++;

                        try
                        {
                            for (int j = 0; j < batches.Count; j++)
                            {
                                var batchGrab = batches[j];
                                if (!batchGrab.IsActive || batchGrab == batch)
                                {
                                    break;
                                }

                                batchGrab.Flush();
                                RenderPass(batchGrab, ref VP, _screenGrabTarget, _screenGrabTarget, camera);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.EngineError(e);
                        }
                    }
                }
                //else
                //{
                //    Debug.EngineError("Fatal batch doesn't have material");
                //}

                RenderPass(batch, ref VP, sceneRenderTarget, _screenGrabTarget, camera);
            }

            return info;
        }

        private void RenderPass(Batch2D batch, ref mat4 VP, RenderTexture renderTarget, RenderTexture screenGrabTarget, ICamera camera)
        {
            if(batch == null)
            {
                Debug.EngineError("Batch is null");
            }
            if (batch.Material == null)
            {
                Debug.EngineError("Material is null");
            }
            if (batch.Material.Passes == null)
            {
                Debug.EngineError("Passes is null");
            }

            for (int i = 0; i < batch.Material.Passes.Count; i++)
            {
                var pass = batch.Material.Passes[i];
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

                int screenGrabIndex = boundTex;

                // Grab the color texture
                _drawCallData.Textures[screenGrabIndex] = pass.IsScreenGrabPass ? screenGrabTarget.NativeResource.SubResources[0] : null;

                // Pipeline
                _pipelineFeatures.Blending = pass.Blending;
                _pipelineFeatures.Stencil = pass.Stencil;

                _drawCallData.DrawType = batch.DrawType;
                _drawCallData.DrawMode = batch.DrawMode;
                _drawCallData.IndexedDraw.IndexCount = batch.IndexCount;
                _drawCallData.Shader = pass.Shader.NativeShader;
                _drawCallData.Geometry = batch.Geometry;
                _drawCallData.Features = _pipelineFeatures;
                _drawCallData.RenderTarget = renderTarget.NativeResource;
                _drawCallData.Viewport = new vec4(0, 0, renderTarget.Width, renderTarget.Height);

                // Uniforms
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.VP_MATRIX].SetMat4(Consts.VIEW_PROJ_UNIFORM_NAME, VP);
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.VIEW_MATRIX].SetMat4(Consts.VIEW_UNIFORM_NAME, camera.ViewMatrix);
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.PROJECTION_MATRIX].SetMat4(Consts.PROJECTION_UNIFORM_NAME, camera.Projection);
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.TEXTURES_ARRAY].SetIntArr(Consts.TEX_ARRAY_UNIFORM_NAME, Batch2D.TextureSlotArray);
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.MODEL_MATRIX].SetMat4(Consts.MODEL_UNIFORM_NAME, batch.WorldMatrix);
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.SCREEN_RENDER_TARGET_GRAB].SetInt(Consts.SCREEN_GRAB_TEX_UNIFORM_NAME, screenGrabIndex);
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.SCREEN_SIZE].SetVec2(Consts.SCREEN_SIZE_UNIFORM_NAME, new vec2(renderTarget.Width, renderTarget.Height));
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.APP_TIME].SetVec3(Consts.TIME_UNIFORM_NAME, new vec3(Time.UnscaledTimeWrap, Time.TimeCurrentWrap, Time.DeltaTime));

                // Clears Next unused uniform, so the device does not send more data than necessary.
                _drawCallData.Uniforms[(int)Consts.Graphics.Uniforms.COUNT] = default;

                // Adds extra uniforms needed by renderers.
                int uniformOffset = 0;
                foreach (var uniform in pass.Uniforms.Values)
                {
                    if (_drawCallData.Uniforms.Length <= uniformOffset)
                    {
                        Debug.Error($"Max uniform per drawcall reached: {_drawCallData.Uniforms.Length}");
                        break;
                    }
                    _drawCallData.Uniforms[uniformOffset + (int)Consts.Graphics.Uniforms.COUNT] = uniform;
                    uniformOffset++;
                }

                // Draw
                GfxDeviceManager.Current.Draw(_drawCallData);
            }
        }

        private struct RenderingBatchesInfo
        {
            public int BatchesCount { get; set; }
            public int ScreenGrabPasses { get; set; }
            public int TotalRenderers { get; set; }
        }
    }
}
