using Editor.AssemblyHotReload;
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
        private List<IEditorWindow> _windows;
        private static EditorGameView _gameWindow;
        internal static EditorGameView GameWindow => _gameWindow;

        private RenderingSurface _editorSurface;
        private RenderingSurface _gameSurface;

        private EditorCamera _editorCamera;
        private readonly IWindow _win;
        private readonly InputLayerBase _inputLayer;
        private readonly ImGuiController _imguiController;
        public ImGuiLayer(IWindow window, InputLayerBase inputLayer)
        {
            _win = window;
            _inputLayer = inputLayer;
            _imguiController = new ImGuiController();

            EditorNatives.InitGLFWImguiInternal(window.NativeWindow);

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
                Draw();
            };
        }

        public override void Initialize()
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

            _windows = new List<IEditorWindow>()
            {
                _gameWindow,
                new ActionBarView(),
                new FooterBarView(),

                new SceneEditorView("Scene", _editorSurface, _editorCamera),
                new SceneGraphWindow(),
                new ObjectEditorView(),
                new AnimatorEditorView(),
                new RenderingInfoView(),
                // new ConsoleEditorView()
            };
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

            foreach (var windowView in _windows)
            {
                windowView.OnDraw();
            }

            ImGui.End();
        }

        internal override void UpdateLayer()
        {
            foreach (var windowView in _windows)
            {
                windowView.OnUpdate();
            }
        }

        public override void Close() { }
    }
}
