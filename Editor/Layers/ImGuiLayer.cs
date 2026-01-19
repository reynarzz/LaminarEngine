using Editor.AssemblyHotReload;
using Editor.Rendering;
using Editor.Views;
using Engine;
using Engine.Graphics;
using Engine.GUI;
using Engine.Layers;
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
        private readonly ImGuiGLFW _glfwInput;
        private static EditorGameView _gameWindow;
        internal static EditorGameView GameWindow => _gameWindow;
    
        private RenderingSurface _editorSurface;
        private RenderingSurface _gameSurface;

        private EditorCamera _editorCamera;
        private readonly IWindow _win;
        private readonly InputLayerBase _inputLayer;

        public ImGuiLayer(IWindow window, InputLayerBase inputLayer)
        {
            _win = window;
            _inputLayer = inputLayer;
            ImguiImplOpenGL3.Init(window);

            _glfwInput = new ImGuiGLFW(WindowStandalone.NativeWindow);
            _glfwInput.Init();

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
                new SceneEditorView("Scene", _editorSurface, _editorCamera),
                new SceneGraphWindow(),
                new ObjectEditorView(),
                new AnimatorEditorView(),
                _gameWindow,

                // new ConsoleEditorView()
            };
        }

        private void Draw()
        {
            ImguiImplOpenGL3.SetPerFrameImGuiData(Time.UnscaledDeltaTime, _win.PhysicalWidth, _win.PhysicalHeight);

            //ImGui.NewFrame();
            ImAllGui.imgui_NewFrame();
            ImguiImplOpenGL3.NewFrame();
            _glfwInput.NewFrame();

            // Render ImGui here:

            DockSpace();

            ImGui.Render();
            ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());

        }

        private void DockSpace()
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos + new Vector2(0, 35));
            ImGui.SetNextWindowSize(viewport.Size - new Vector2(0, 62));

            var flags = ImGuiWindowFlags.NoTitleBar |
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

            RenderingInfoWindow();

            ImGui.End();

            FooterView();
        }

        internal override void UpdateLayer()
        {
            foreach (var windowView in _windows)
            {
                windowView.OnUpdate();
            }
        }
        private void FooterView() // TODO: Move to its own view class.
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowPos(viewport.Pos + new Vector2(0, viewport.Size.Y - 25));
            ImGui.SetNextWindowSize(new Vector2(viewport.Size.X, 25));
            ImGui.SetNextWindowViewport(viewport.ID);

            var footerFlags = ImGuiWindowFlags.NoTitleBar |
                                     ImGuiWindowFlags.NoCollapse |
                                     ImGuiWindowFlags.NoResize |
                                     ImGuiWindowFlags.NoMove |
                                     ImGuiWindowFlags.NoBringToFrontOnFocus |
                                     ImGuiWindowFlags.NoNavFocus |
                                     ImGuiWindowFlags.NoDocking;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.13f, 0.13f, 0.13f, 1.0f));
            ImGui.Begin("FooterSpace", footerFlags);

            if (GameAssemblyBuilder.IsBuilding)
            {
                ImGui.Text("Compiling...");
            }
            ImGui.End();
            ImGui.PopStyleColor();

            ImGui.PopStyleVar(4);

        }

        private void RenderingInfoWindow()
        {
            ImGui.Begin("Rendering Info");
            ImGui.Text($"{nameof(Time.FPS)}: {Time.FPS}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.WBatches)}: {EngineInfo.Renderer.WBatches}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.GrabScreenPass)}: {EngineInfo.Renderer.GrabScreenPass}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.WDrawCalls)}: {EngineInfo.Renderer.WDrawCalls}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.UIBatches)}: {EngineInfo.Renderer.UIBatches}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.UIGrabScreenPass)}: {EngineInfo.Renderer.UIGrabScreenPass}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.UIDrawCalls)}: {EngineInfo.Renderer.UIDrawCalls}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.TotalBatches)}: {EngineInfo.Renderer.TotalBatches}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.TotalDrawCalls)}: {EngineInfo.Renderer.TotalDrawCalls}");
            ImGui.Text($"{nameof(EngineInfo.Renderer.SavedByBatching)}: {EngineInfo.Renderer.SavedByBatching}");
            ImGui.End();
        }

        public override void Close()
        {
        }
    }
}
