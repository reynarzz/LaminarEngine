using Editor.Build;
using Editor.Data;
using Editor.Rendering;
using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Graphics;
using Engine.GUI;
using Engine.Layers;
using GLFW;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Layers
{
    internal class ImGuiLayer : LayerBase
    {
        private List<EditorWindow> _windows;
        private static EditorGameView _gameWindow;
        internal static EditorGameView GameWindow => _gameWindow;

        private RenderingSurface _editorSurface;
        private RenderingSurface _gameSurface;

        private EditorCamera _editorCamera;
        private readonly IWindow _win;
        private readonly InputLayerBase _inputLayer;
        private readonly ImGuiController _imguiController;
        private readonly InitializationWindow _initWindow;
        private readonly ProjectWizardWindow _projectWizardWindow;
        public ImGuiLayer(IWindow window, InputLayerBase inputLayer, LayersManager layerManager)
        {
            _win = window;
            _inputLayer = inputLayer;
            _imguiController = new ImGuiController();
            _initWindow = new InitializationWindow();
            _projectWizardWindow = new ProjectWizardWindow();


            // TODO: move the surface creation to their own classes.
            _gameSurface = new RenderingSurface()
            {
                PickCamerasFromSceneGraph = true,
                RenderPostProcessing = true,
                RenderTextures = new RenderTexture[1],
                RenderUI = true,
                UIViewProj = UICanvas.UIViewProj,
            };
            _gameWindow = new EditorGameView(_win, _gameSurface, _inputLayer);

            RenderingLayer.OnDrawOverlay += () =>
            {
                if (layerManager.LayersInitialized)
                {
                    Draw();
                }
                else
                {
                    GfxDeviceManager.Current.Clear(new ClearDeviceConfig()
                    {
                        Color = new Color(0.17f, 0.17f, 0.17f, 1.0f),
                        RenderTarget = null
                    });

                    DrawPreInitialization();
                }
            };
        }

        public override Task<LayerInitResult> InitializeAsync()
        {
            return MainThreadDispatcher.EnqueueAsync(() =>
            {
                var sceneBatcher = new SceneBatchedRenderer();
                _gameSurface.SceneRenderers = new() { sceneBatcher };
                _editorCamera = new EditorCamera();
                _editorSurface = new RenderingSurface()
                {
                    Cameras = [_editorCamera],
                    RenderDebug = true,
                    RenderPostProcessing = false,
                    RenderUI = true,
                    BlitToScreen = false,
                    DrawGizmos = true,
                    GizmosRenderer = new GizmosRenderer(),
                    RenderTextures = [new RenderTexture(1920, 1080) { Name = "Scene view Render Texture" },
                                  new RenderTexture(1920, 1080) { Name = "Mouse picker Render Texture" }],
                    SceneRenderers =
                    {
                        sceneBatcher,
                    },
                };

                RenderingLayer.InitializeSurfaces([_gameSurface, _editorSurface]);

                _win.OnWindowChanged += (w, h) =>
                {
                    UpdateLayer();
                };

                _windows = new List<EditorWindow>()
                {
                    //new ProfilerWindow(),
                    new AssetsViewWindow(),
                    new ActionBarView(),
                    new FooterBarView(),
                    new AnimationWindow(),

                    new SceneGraphWindow(),
                    new ObjectEditorView(),
                    new AnimatorEditorView(),
                    new RenderingStatsView(),
                    new BuildWindow(),
                    new TaskWindow(),
                    new ProjectSettingsWindow(),
                    // new ConsoleEditorView(),


                    _gameWindow,
                    new SceneEditorView("Scene", _editorSurface, _editorCamera),

                };

                IsInitialized = true;
                return LayerInitResult.Success;
            });
        }

        private void Draw()
        {
            try
            {
                EditorNatives.BeginGLFWImguiInternal();

                DockSpace();

                EditorNatives.EndGLFWImguiInternal();
            }
            catch (System.Exception e)
            {
                Debug.Error(e);
            }
        }

        private void DockSpace()
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos + new Vector2(0, 40));
            ImGui.SetNextWindowSize(viewport.Size - new Vector2(0, 67));

            const ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar |
                                      ImGuiWindowFlags.NoCollapse |
                                      ImGuiWindowFlags.NoResize |
                                      ImGuiWindowFlags.NoMove |
                                      ImGuiWindowFlags.NoBringToFrontOnFocus |
                                      ImGuiWindowFlags.NoNavFocus |
                                      ImGuiWindowFlags.NoBackground;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);

            //ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
            ImGui.Begin("DockSpaceHost", flags);

            ImGui.PopStyleVar(3);

            var dockspaceId = ImGui.GetID("MainDockSpace");
            ImGui.DockSpace(dockspaceId, Vector2.Zero, ImGuiDockNodeFlags.PassthruCentralNode);

            ImGui.BeginDisabled(BuildSystem.IsAnyBuilding && !BuildSystem.IsBuilding(PlatformBuild.GameAppDomain));
            foreach (var windowView in _windows)
            {
                windowView.OnDraw();
            }
            ImGui.EndDisabled();
            ImGui.End();
        }

        internal override void UpdateLayer()
        {
            foreach (var windowView in _windows)
            {
                windowView.OnUpdate();
            }
        }

        private void DrawPreInitialization()
        {
            EditorNatives.BeginGLFWImguiInternal();

            if (EditorConfigManager.IsProjectLoaded())
            {
                _initWindow.OnDraw();
            }
            else
            {
                _projectWizardWindow.OnDraw();
            }

            EditorNatives.EndGLFWImguiInternal();
        }

        public override void Close() { }
    }
}
