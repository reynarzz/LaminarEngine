using Editor.Build;
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
        public ImGuiLayer(IWindow window, InputLayerBase inputLayer, LayersManager layerManager)
        {
            _win = window;
            _inputLayer = inputLayer;
            _imguiController = new ImGuiController();


            // TODO: move the surface creation to their own classes.
            _gameSurface = new RenderingSurface()
            {
                PickCameraFromSceneGraph = true,
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
                    DrawInitialization();
                }
            };
        }

        public override Task InitializeAsync()
        {
            return MainThreadDispatcher.EnqueueAsync(() =>
            {
                var sceneBatcher = new SceneBatchedRenderer();
                _gameSurface.SceneRenderers = new() { sceneBatcher };
                _editorCamera = new EditorCamera();
                _editorSurface = new RenderingSurface()
                {
                    Cameras = [new WeakReference<ICamera>(_editorCamera)],
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
                    _gameWindow,
                    new ActionBarView(),
                    new FooterBarView(),

                    new SceneEditorView("Scene", _editorSurface, _editorCamera),
                    new SceneGraphWindow(),
                    new ObjectEditorView(),
                    new AnimatorEditorView(),
                    new RenderingStatsView(),
                    new BuildWindow(),
                    new TaskWindow(),
                    new ProjectSettingsWindow(),
                    // new ConsoleEditorView()
                };

                IsInitialized = true;
            });
        }

        private void Draw()
        {
            EditorNatives.BeginGLFWImguiInternal();

            DockSpace();

            EditorNatives.EndGLFWImguiInternal();
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

        // TODO: show real progress.
        private float _fakeProgress = 0;
        private void DrawInitialization()
        {
            EditorNatives.BeginGLFWImguiInternal();
            var winSize = new Vector2(300, 100);
            var viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowSize(winSize, ImGuiCond.Once);
            ImGui.SetNextWindowPos(viewport.Pos.X + viewport.Size.X * 0.5f - winSize.X * 0.5f,
                                   viewport.Pos.Y + viewport.Size.Y * 0.5f - winSize.Y * 0.5f);
            ImGui.Begin("Initializing Editor", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);
            _fakeProgress += ImGui.GetIO().DeltaTime * 0.04f;

            string fakeMessage = null;

            if(_fakeProgress <= 0.3f)
            {
                fakeMessage = "Compiling GameApplication Domain";
            }
            else if(_fakeProgress < 1.0f)
            {
                fakeMessage = "Importing assets";
            }
            else
            {
                fakeMessage = "Finishing up";
            }

            ImGui.Text($"{fakeMessage}");
          
            ImGui.ProgressBar(_fakeProgress, new Vector2(270, 20));
            ImGui.End();
            EditorNatives.EndGLFWImguiInternal();

        }

        public override void Close() { }
    }
}
