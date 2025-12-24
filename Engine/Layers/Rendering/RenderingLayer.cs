using Engine.Graphics;
using Engine.GUI;
using Engine.Rendering;
using Engine.Utils;
using GlmNet;

namespace Engine.Layers
{
    internal class RenderingLayer : LayerBase
    {
        private static Batcher2D _sceneBatches;
        private static Batcher2D _uiBatches;
        private DrawCallData _drawCallData;
        private DrawCallData _screenQuadDrawCallData;
        private PipelineFeatures _pipelineFeatures;
        private PipelineFeatures _screenPipelineFeatures;
        private RenderTexture _screenGrabTarget;
        private GfxResource _screenGeometry;
        private List<Renderer2D> _renderers;
        private List<Renderer2D> _UIElementRenderers;
        private readonly Action<Shader, RenderTexture, RenderTexture, UniformValue[]> _drawPostProcessCallback;
        internal static event Action OnDrawOverlay;
        internal class DrawOverlayOptions
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
        internal class RenderingSurface
        {
            public ICamera[] Cameras { get; set; }
            public bool PickCameraFromSceneGraph { get; set; }
            public RenderTexture RenderTexture { get; set; }
            public bool RenderPostProcessing { get; set; }
            public bool RenderDebug { get; set; }
            public bool BlitToScreen { get; set; }

        }
        internal static DrawOverlayOptions OverlayOptions { get; } = new DrawOverlayOptions();

        private static readonly List<RenderingSurface> _renderingSurfaces = new();
        private ICamera _sceneCamera;

        public RenderingLayer() : base()
        {
            _drawPostProcessCallback = PostProcessDraw;
        }

        public override void Initialize()
        {
            _screenGrabTarget = new RenderTexture(Screen.Width, Screen.Height);
            ClearScreenToColor(WindowManager.Window.StartWindowColor, null);
            GfxDeviceManager.Current.Present();

            _pipelineFeatures = new PipelineFeatures();
            _screenPipelineFeatures = new PipelineFeatures();

            _sceneBatches = new Batcher2D(Consts.Graphics.MAX_QUADS_PER_BATCH);
            _uiBatches = new Batcher2D(Consts.Graphics.MAX_QUADS_PER_BATCH);

            _renderers = new();
            _UIElementRenderers = new();
            _drawCallData = new DrawCallData()
            {
                Textures = new GfxResource[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits],
                Uniforms = new UniformValue[GfxDeviceManager.Current.GetDeviceInfo().MaxUniformsCount],
            };

            _screenQuadDrawCallData = new DrawCallData()
            {
                Textures = new GfxResource[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits],
                Uniforms = new UniformValue[GfxDeviceManager.Current.GetDeviceInfo().MaxUniformsCount],
            };

            _screenGeometry = GraphicsHelper.GetScreenQuadGeometry();

            // Default surface
            InitializeSurfaces([new RenderingSurface()
            {
               PickCameraFromSceneGraph = true,
               RenderPostProcessing = true,
               BlitToScreen = true,
#if DEBUG
               RenderDebug = true,
#endif

            }]);
        }

        internal static void InitializeSurfaces(RenderingSurface[] configs)
        {
            _renderingSurfaces.Clear();

            for (int i = 0; i < configs.Length; i++)
            {
                var config = configs[i];

                if (config.RenderTexture == null)
                {
                    config.RenderTexture = new RenderTexture(Screen.Width, Screen.Height);
                }
            }

            _renderingSurfaces.AddRange(configs);
        }

        internal override void UpdateLayer()
        {
            EngineInfo.Renderer.Clear();

            bool isSceneGraphCollected = false;
            var batches = default(List<Batch2D>);
            var uibatches = default(List<Batch2D>);

            foreach (var surface in _renderingSurfaces)
            {
                if (surface.Cameras == null || surface.Cameras.Length == 0 || surface.Cameras[0] == null || !surface.Cameras[0].IsAlive)
                {
                    if (surface.PickCameraFromSceneGraph)
                    {
                        if (_sceneCamera == null)
                        {
                            _sceneCamera = SceneManager.FindComponent<Camera>(findDisabled: false);
                        }

                        if (_sceneCamera != null && _sceneCamera.IsAlive && _sceneCamera.IsEnabled)
                        {
                            if (surface.Cameras == null)
                            {
                                surface.Cameras = new ICamera[1];
                            }

                            surface.Cameras[0] = _sceneCamera;
                            RenderScene(surface, _sceneCamera, ref batches, ref uibatches);
                        }
                    }
                    continue;
                }

                foreach (var camera in surface.Cameras)
                {
                    RenderScene(surface, camera, ref batches, ref uibatches);
                }
            }
        }

        private void RenderScene(RenderingSurface surface, ICamera camera, ref List<Batch2D> batches, ref List<Batch2D> uibatches)
        {
            var isCameraAvailable = camera.IsAlive && camera.IsEnabled;

            if (!isCameraAvailable)
            {
                if (surface.BlitToScreen)
                {
                    //ClearScreenToColor(Color.Black, _defaultSceneRenderTexture);
                    //GfxDeviceManager.Current.Draw(OnDrawOverlay, _defaultSceneRenderTexture.NativeResource);
                    //GfxDeviceManager.Current.Present(_defaultSceneRenderTexture.NativeResource);
                }
                else
                {
                    ClearScreenToColor(Color.Black, surface.RenderTexture);
                    RenderOverlayToScreen();
                }

                EngineInfo.Renderer.Clear();
                isCameraAvailable = false;
                return;
            }

            var finalRenderTexture = camera.RenderTexture ?? surface.RenderTexture;
            GfxDeviceManager.Current.SetViewport(new vec4(0, 0, finalRenderTexture.Width, finalRenderTexture.Height));

            // Clear main render target.
            GfxDeviceManager.Current.Clear(new ClearDeviceConfig()
            {
                Color = camera.BackgroundColor,
                RenderTarget = finalRenderTexture.NativeResource
            });


            if (batches == null)
            {

                SceneManager.OnPreRenderUpdate();

                // TODO: improve this, don't ask for renderers but add/remove with events.
                _renderers.Clear();
                _UIElementRenderers.Clear();
                SceneManager.FindAll(_renderers, x =>
                {
                    return x.IsEnabled && x is not UIElement;
                });

                SceneManager.FindAll(_UIElementRenderers, x =>
                {
                    return x.IsEnabled && x is UIGraphicsElement;
                });

                batches = _sceneBatches.GetBatches(_renderers);
                uibatches = _uiBatches.GetBatches(_UIElementRenderers);
            }
            var VP = camera.Projection * camera.ViewMatrix;

            var geoBatchesInfo = RenderBatches(camera, batches, ref VP, finalRenderTexture);
            var uiBatchesInfo = RenderBatches(camera, uibatches, ref UICanvas.UIViewProj, finalRenderTexture, finalRenderTexture);
#if DEBUG
            EngineInfo.Renderer.WBatches = geoBatchesInfo.BatchesCount;
            EngineInfo.Renderer.GrabScreenPass = geoBatchesInfo.ScreenGrabPasses;
            EngineInfo.Renderer.UIBatches = uiBatchesInfo.BatchesCount;
            EngineInfo.Renderer.UIGrabScreenPass = uiBatchesInfo.ScreenGrabPasses;
            EngineInfo.Renderer.PostProcessingPasses = PostProcessingStack.Passes.Count;
            EngineInfo.Renderer.SavedByBatching = (geoBatchesInfo.TotalRenderers - geoBatchesInfo.BatchesCount) * (uiBatchesInfo.ScreenGrabPasses + 1);

            if (surface.RenderDebug)
            {
                SceneManager.OnDrawGizmos();
                Debug.DrawGeometries(VP, UICanvas.UIViewProj, finalRenderTexture.NativeResource);
            }
#endif
            if (surface.RenderPostProcessing)
            {
                RenderPostProcessing(ref finalRenderTexture);
                camera.OutRenderTexture = finalRenderTexture;
            }

            if (surface.BlitToScreen)
            {
                // Draw any overlays such as debug UI
                GfxDeviceManager.Current.Draw(OnDrawOverlay, finalRenderTexture.NativeResource);
                GfxDeviceManager.Current.Present(finalRenderTexture.NativeResource);
            }
            else
            {
                RenderOverlayToScreen();
            }
        }
        private void RenderOverlayToScreen()
        {
            ClearScreenToColor(Color.Black, null, OverlayOptions.Width, OverlayOptions.Height);
            GfxDeviceManager.Current.Draw(OnDrawOverlay, null);
            GfxDeviceManager.Current.Present();
        }

        private RenderingBatchesInfo RenderBatches(ICamera camera, List<Batch2D> batches, ref mat4 VP, RenderTexture sceneRenderTarget, RenderTexture grabBlitTarget = null)
        {
            RenderingBatchesInfo info = default;
            foreach (var batch in batches)
            {
                if (!batch.IsActive)
                {
                    break;
                }
                info.BatchesCount++;
                info.TotalRenderers += batch.RenderersCount;
                batch.Flush();

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

                    foreach (var batchGrab in batches)
                    {
                        if (!batchGrab.IsActive || batchGrab == batch)
                        {
                            break;
                        }

                        batchGrab.Flush();
                        RenderPass(batchGrab, ref VP, _screenGrabTarget, _screenGrabTarget, camera);
                    }
                }

                RenderPass(batch, ref VP, sceneRenderTarget, _screenGrabTarget, camera);
            }

            return info;
        }

        private void RenderPass(Batch2D batch, ref mat4 VP, RenderTexture renderTarget, RenderTexture screenGrabTarget, ICamera camera)
        {
            foreach (var pass in batch.Material.Passes)
            {
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
        private void RenderPostProcessing(ref RenderTexture screenRenderTexture)
        {
            foreach (var pass in PostProcessingStack.Passes)
            {
                screenRenderTexture = pass.Render(screenRenderTexture, _drawPostProcessCallback);
            }
        }
        private void PostProcessDraw(Shader shader, RenderTexture inTex, RenderTexture outTex, UniformValue[] uniforms)
        {
            // TODO: Fix selecting the current rendering camera from the surface, for now its using the scene camera.
            DrawScreenQuad(shader, inTex, outTex, uniforms, _sceneCamera);
        }

        private void DrawScreenQuad(Shader shader, RenderTexture sceneRenderTarget, RenderTexture renderTarget,
                                    UniformValue[] uniforms, ICamera camera)
        {
            ClearUniforms(_screenQuadDrawCallData);
            // Texture 0 is the screen
            _screenQuadDrawCallData.Textures[0] = sceneRenderTarget.NativeResource.SubResources[0];

            int uniformIndex = 0;
            if (uniforms != null)
            {
                for (int i = 0; i < uniforms.Length; i++)
                {
                    var type = uniforms[i].Type;
                    if (type == UniformType.Invalid)
                        continue;
                    if (type == UniformType.RenderTexture)
                    {
                        // Texture + 1 is the texture that will be used by the shader.
                        _screenQuadDrawCallData.Textures[uniformIndex + 1] = uniforms[i].RenderTextureValue.NativeResource.SubResources[0];
                        _screenQuadDrawCallData.Uniforms[uniformIndex].SetInt(uniforms[i].Name, uniformIndex + 1);
                    }
                    else
                    {
                        _screenQuadDrawCallData.Uniforms[uniformIndex] = uniforms[i];
                    }

                    uniformIndex++;
                }
            }

            // Pipeline
            _screenQuadDrawCallData.DrawType = DrawType.Indexed;
            _screenQuadDrawCallData.DrawMode = DrawMode.Triangles;
            _screenQuadDrawCallData.IndexedDraw.IndexCount = 6;
            _screenQuadDrawCallData.Shader = shader.NativeShader;
            _screenQuadDrawCallData.Geometry = _screenGeometry;
            _screenQuadDrawCallData.Features = _screenPipelineFeatures;
            _screenQuadDrawCallData.RenderTarget = renderTarget?.NativeResource;
            _screenQuadDrawCallData.Viewport = new vec4(0, 0, renderTarget?.Width ?? Screen.Width, renderTarget?.Height ?? Screen.Height);

            // Uniforms
            _screenQuadDrawCallData.Uniforms[uniformIndex + 0].SetMat4(Consts.VIEW_PROJ_UNIFORM_NAME, mat4.identity());
            _screenQuadDrawCallData.Uniforms[uniformIndex + 1].SetVec2(Consts.SCREEN_SIZE_UNIFORM_NAME, new vec2(_screenQuadDrawCallData.Viewport.z, _screenQuadDrawCallData.Viewport.w));
            _screenQuadDrawCallData.Uniforms[uniformIndex + 2].SetVec3(Consts.TIME_UNIFORM_NAME, new vec3(Time.UnscaledTimeWrap, Time.TimeCurrentWrap, Time.DeltaTime));
            _screenQuadDrawCallData.Uniforms[uniformIndex + 3].SetInt(Consts.SCREEN_GRAB_TEX_UNIFORM_NAME, 0);
            _screenQuadDrawCallData.Uniforms[uniformIndex + 4].SetFloat(Consts.FRAME_SEED_UNIFORM_NAME, Random.Shared.NextSingle());
            _screenQuadDrawCallData.Uniforms[uniformIndex + 5].SetMat4(Consts.VIEW_UNIFORM_NAME, camera.ViewMatrix);

            // Draw
            GfxDeviceManager.Current.Draw(_screenQuadDrawCallData);
        }


        private void ClearScreenToColor(Color color, RenderTexture texture)
        {
            ClearScreenToColor(color, texture, Screen.Width, Screen.Height);
        }

        private void ClearScreenToColor(Color color, RenderTexture texture, int width, int height)
        {
            GfxDeviceManager.Current.SetViewport(new vec4(0, 0, width, height));
            GfxDeviceManager.Current.Clear(new ClearDeviceConfig()
            {
                Color = color,
                RenderTarget = texture?.NativeResource
            });
        }

        private void ClearUniforms(DrawCallData drawCall)
        {
            // Clear uniforms
            for (int i = 0; i < drawCall.Uniforms.Length; i++)
            {
                drawCall.Uniforms[i] = default;
            }
        }
        public override void Close()
        {

        }

        private struct RenderingBatchesInfo
        {
            public int BatchesCount { get; set; }
            public int ScreenGrabPasses { get; set; }
            public int TotalRenderers { get; set; }
        }
    }
}