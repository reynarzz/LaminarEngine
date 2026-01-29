using Engine.Graphics;
using Engine.GUI;
using Engine.Utils;
using GlmNet;

namespace Engine.Layers
{
    internal class RenderingLayer : LayerBase
    {

        private DrawCallData _screenQuadDrawCallData;
        private PipelineFeatures _screenPipelineFeatures;
        private GfxResource _screenGeometry;
        private List<Renderer2D> _renderers;
        private List<Renderer2D> _UIElementRenderers;

        private List<RendererData2D> _renderersData;
        private List<RendererData2D> _UIElementRenderersData;

        internal static DrawOverlayOptions OverlayOptions { get; } = new DrawOverlayOptions();

        private static readonly List<RenderingSurface> _renderingSurfaces = new();
        private Action<Shader, RenderTexture, RenderTexture, UniformValue[]> _drawPostProcessCallback;
        private WeakReference<ICamera> _sceneCamera = new WeakReference<ICamera>(null);
        internal static event Action OnRenderingEnd;
        internal static event Action OnDrawOverlay;
        private RenderTexture _defaultRenderTexture;
        public RenderingLayer() : base()
        {
            _drawPostProcessCallback = PostProcessDraw;
        }

        public override Task Initialize()
        {
            MainThreadDispatcher.EnqueueAsync(() =>
            {
                GfxDeviceManager.Init();

                _screenPipelineFeatures = new PipelineFeatures();
                _defaultRenderTexture = new RenderTexture(Screen.Width, Screen.Height);

                _renderers = new();
                _UIElementRenderers = new();

                _screenQuadDrawCallData = new DrawCallData()
                {
                    Textures = new GfxResource[GfxDeviceManager.Current.GetDeviceInfo().MaxValidTextureUnits],
                    Uniforms = new UniformValue[GfxDeviceManager.Current.GetDeviceInfo().MaxUniformsCount],
                };

                // Default surface
                InitializeSurfaces([new RenderingSurface()
                {
                   PickCameraFromSceneGraph = true,
                   RenderPostProcessing = true,
                   BlitToScreen = true,
                   RenderUI = true,
                   UIViewProj = UICanvas.UIViewProj,
                   SceneRenderers = { new SceneBatchedRenderer() },
    #if DEBUG
                   RenderDebug = true,
    #endif
                }]);
                _screenGeometry = GraphicsHelper.CreateQuadGeometry();
                WindowManager.Window.OnWindowChanged += OnWindowsChanged;

            });

            return Task.CompletedTask;
        }

        private void OnWindowsChanged(int w, int h)
        {
            _defaultRenderTexture.UpdateTarget(w, h);
        }

        internal static void InitializeSurfaces(RenderingSurface[] configs)
        {
            _renderingSurfaces.Clear();

            for (int i = 0; i < configs.Length; i++)
            {
                var config = configs[i];
            }

            _renderingSurfaces.AddRange(configs);
        }

        private bool IsValidCamera(WeakReference<ICamera> camera)
        {
            if (camera != null && camera.TryGetTarget(out var target))
            {
                return target != null && target.IsAlive;
            }

            return false;
        }

        internal override void UpdateLayer()
        {
            EngineInfo.Renderer.Clear();

            CollectRenderers();

            for (int i = 0; i < _renderingSurfaces.Count; i++)
            {
                var surface = _renderingSurfaces[i];
                //for (int j = 0; j < surface.SceneRenderers.Count; j++)
                //{
                //    var sceneRenderer = surface.SceneRenderers[j];
                //    sceneRenderer.OnPrepare(_renderersData, _UIElementRenderersData);
                //    Debug.Log(surface.SceneRenderers[j].GetType().Name);
                //}

                if (surface.Cameras == null || surface.Cameras.Length == 0 || !IsValidCamera(surface.Cameras[0]))
                {
                    if (surface.PickCameraFromSceneGraph)
                    {
                        if (_sceneCamera == null || !_sceneCamera.TryGetTarget(out var cam) || cam == null || !cam.IsAlive)
                        {
                            _sceneCamera.SetTarget(SceneManager.FindComponent<Camera>(findDisabled: false));
                        }

                        // TODO: Putting a camera in the array can cause problems
                        var existCamera = _sceneCamera.TryGetTarget(out var camera);

                        if (existCamera && camera != null && camera.IsAlive && camera.IsEnabled)
                        {
                            if (surface.Cameras == null)
                            {
                                surface.Cameras = new WeakReference<ICamera>[1];
                            }

                            surface.Cameras[0] = _sceneCamera;
                            RenderScene(surface, camera);
                        }
                    }
                    continue;
                }

                for (int j = 0; j < surface.Cameras.Length; j++)
                {
                    surface.Cameras[j].TryGetTarget(out var camera);
                    RenderScene(surface, camera);
                }
            }

            OnRenderingEnd?.Invoke();
        }

        private void CollectRenderers()
        {
            // TODO: improve this, don't ask for renderers but add/remove with events.
            SceneManager.OnPreRenderUpdate();

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

            // TODO: This is too slow, this is here for quick tests.
            _renderersData = _renderers.Select(x => x.RendererData as RendererData2D).ToList();
            _UIElementRenderersData = _UIElementRenderers.Select(x => x.RendererData as RendererData2D).ToList();
        }

        private void RenderScene(RenderingSurface surface, ICamera camera)
        {
            var isCameraAvailable = camera != null && camera.IsAlive && camera.IsEnabled;

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
                    if (surface.RenderTextures != null && surface.RenderTextures.Length > 0)
                    {
                        for (int i = 0; i < surface.RenderTextures.Length; i++)
                        {
                            ClearScreenToColor(Color.Black, surface.RenderTextures[i]);
                        }
                    }

                    // RenderOverlayToScreen();
                }

                EngineInfo.Renderer.Clear();
                isCameraAvailable = false;
                return;
            }

            if (surface.DrawGizmos)
            {
                surface.GizmosRenderer?.OnBegin(camera);
            }

            for (int i = 0; i < surface.SceneRenderers.Count; i++)
            {
                var sceneRenderer = surface.SceneRenderers[i];
                var targetRenderTexture = GetCurrentRenderTexture(surface, camera, sceneRenderer);

                if (!targetRenderTexture)
                {
                    continue;
                }

                GfxDeviceManager.Current.SetViewport(new vec4(0, 0, targetRenderTexture.Width, targetRenderTexture.Height));

                // Clear main render target.
                GfxDeviceManager.Current.Clear(new ClearDeviceConfig()
                {
                    Color = camera.BackgroundColor,
                    RenderTarget = targetRenderTexture.NativeResource
                });
                sceneRenderer.OnPrepare(_renderersData, _UIElementRenderersData);
                // TODO: begin
                sceneRenderer.OnBegin();

                var processedRenderTexture = sceneRenderer.OnRenderScene(surface, camera, targetRenderTexture);

                if (surface.RenderDebug)
                {
#if DEBUG || EDITOR
                    SceneManager.OnDrawGizmos();
                    var VP = camera.Projection * camera.ViewMatrix;
                    Debug.DrawGeometries(VP, surface.UIViewProj, processedRenderTexture.NativeResource);
#endif
                }
                if (surface.DrawGizmos)
                {
                    processedRenderTexture = surface?.GizmosRenderer?.OnRender(camera, surface, processedRenderTexture);
                }

                if (surface.RenderPostProcessing)
                {
                    RenderPostProcessing(ref processedRenderTexture);
                }

                bool IsCameraRenderTexture = camera.RenderTexture;
                if (IsCameraRenderTexture)
                {
                    camera.OutRenderTexture = processedRenderTexture;
                }
                else
                {
                    if (surface.RenderTextures != null && surface.RenderTextures.Length > 0)
                    {
                        surface.RenderTextures[sceneRenderer.RenderTextureIndex] = processedRenderTexture;
                    }
                }

                if (surface.BlitToScreen)
                {
                    // Draw any overlays such as debug UI
                    GfxDeviceManager.Current.Draw(OnDrawOverlay, processedRenderTexture.NativeResource);
                    GfxDeviceManager.Current.Present(processedRenderTexture.NativeResource);
                }
                else
                {
                    RenderOverlayToScreen();
                }
                if (surface.DrawGizmos)
                {
                    surface?.GizmosRenderer?.OnEnd();
                }
                sceneRenderer.OnEnd();
            }
        }

        private RenderTexture GetCurrentRenderTexture(RenderingSurface surface, ICamera camera, SceneRendererBase sceneRenderer)
        {
            bool IsCameraRenderTexture = camera.RenderTexture;

            if (IsCameraRenderTexture)
            {
                return camera.RenderTexture;
            }
            else if (surface.RenderTextures != null && surface.RenderTextures.Length > sceneRenderer.RenderTextureIndex &&
                     surface.RenderTextures[sceneRenderer.RenderTextureIndex])
            {
                return surface.RenderTextures[sceneRenderer.RenderTextureIndex];
            }

            return _defaultRenderTexture;
        }

        private void RenderOverlayToScreen()
        {
            ClearScreenToColor(Color.Black, null, OverlayOptions.Width, OverlayOptions.Height);
            GfxDeviceManager.Current.Draw(OnDrawOverlay, null);
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
            if (_sceneCamera.TryGetTarget(out var camera))
            {
                DrawScreenQuad(shader, inTex, outTex, uniforms, camera);
            }
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
                    if (type == UniformType.Texture2D)
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

    }
}