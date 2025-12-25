using Engine.Graphics;
using Engine.GUI;
using Engine.Utils;
using GlmNet;

namespace Engine.Layers
{
    internal class RenderingLayer : LayerBase
    {
        internal static event Action OnDrawOverlay;

        private DrawCallData _screenQuadDrawCallData;
        private PipelineFeatures _screenPipelineFeatures;
        private GfxResource _screenGeometry;
        private List<Renderer2D> _renderers;
        private List<Renderer2D> _UIElementRenderers;
        internal static DrawOverlayOptions OverlayOptions { get; } = new DrawOverlayOptions();

        private static readonly List<RenderingSurface> _renderingSurfaces = new();
        private Action<Shader, RenderTexture, RenderTexture, UniformValue[]> _drawPostProcessCallback;
        private ICamera _sceneCamera;
        internal static event Action OnRenderingEnd;
        private RenderTexture _defaultRenderTexture;
        public RenderingLayer() : base()
        {
            _drawPostProcessCallback = PostProcessDraw;
        }

        public override void Initialize()
        {
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

        internal override void UpdateLayer()
        {
            EngineInfo.Renderer.Clear();

            CollectRenderers();

            foreach (var surface in _renderingSurfaces)
            {
                foreach (var renderer in surface.SceneRenderers)
                {
                    renderer.OnPrepare(_renderers, _UIElementRenderers);
                }

                if (surface.Cameras == null || surface.Cameras.Length == 0 || surface.Cameras[0] == null || !surface.Cameras[0].IsAlive)
                {
                    if (surface.PickCameraFromSceneGraph)
                    {
                        if (_sceneCamera == null || !_sceneCamera.IsAlive)
                        {
                            _sceneCamera = SceneManager.FindComponent<Camera>(findDisabled: false);
                        }

                        // TODO: maybe putting a camera in the array can cause problems
                        if (_sceneCamera != null && _sceneCamera.IsAlive && _sceneCamera.IsEnabled)
                        {
                            if (surface.Cameras == null)
                            {
                                surface.Cameras = new ICamera[1];
                            }

                            surface.Cameras[0] = _sceneCamera;
                            RenderScene(surface, _sceneCamera);
                        }
                    }
                    continue;
                }

                foreach (var camera in surface.Cameras)
                {
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

        }

        private void RenderScene(RenderingSurface surface, ICamera camera)
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
                    if (surface.RenderTextures != null && surface.RenderTextures.Length > 0)
                    {
                        for (int i = 0; i < surface.RenderTextures.Length; i++)
                        {
                            ClearScreenToColor(Color.Black, surface.RenderTextures[i]);
                        }
                    }

                    RenderOverlayToScreen();
                }

                EngineInfo.Renderer.Clear();
                isCameraAvailable = false;
                return;
            }

            foreach (var sceneRenderer in surface.SceneRenderers)
            {
                bool IsCameraRenderTexture = camera.RenderTexture;
                var targetRenderTexture = IsCameraRenderTexture ? camera.RenderTexture : surface.RenderTextures != null ? surface.RenderTextures[sceneRenderer.RenderTextureIndex] : _defaultRenderTexture;

                if (!targetRenderTexture)
                    continue;

                GfxDeviceManager.Current.SetViewport(new vec4(0, 0, targetRenderTexture.Width, targetRenderTexture.Height));

                // Clear main render target.
                GfxDeviceManager.Current.Clear(new ClearDeviceConfig()
                {
                    Color = camera.BackgroundColor,
                    RenderTarget = targetRenderTexture.NativeResource
                });

                // TODO: begin
                sceneRenderer.OnBegin();

                var processedRenderTexture = sceneRenderer.OnRenderScene(surface, camera, targetRenderTexture);
#if DEBUG
                if (surface.RenderDebug)
                {
                    SceneManager.OnDrawGizmos();
                    var VP = camera.Projection * camera.ViewMatrix;
                    Debug.DrawGeometries(VP, surface.UIViewProj, processedRenderTexture.NativeResource);
                }
#endif
                if (surface.RenderPostProcessing)
                {
                    RenderPostProcessing(ref processedRenderTexture);
                }

                if (IsCameraRenderTexture)
                {
                    camera.OutRenderTexture = processedRenderTexture;
                }
                else
                {
                    surface.RenderTextures[sceneRenderer.RenderTextureIndex] = processedRenderTexture;
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

                sceneRenderer.OnEnd();
            }
        }

        private void RenderOverlayToScreen()
        {
            ClearScreenToColor(Color.Black, null, OverlayOptions.Width, OverlayOptions.Height);
            GfxDeviceManager.Current.Draw(OnDrawOverlay, null);
            GfxDeviceManager.Current.Present();
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

    }
}