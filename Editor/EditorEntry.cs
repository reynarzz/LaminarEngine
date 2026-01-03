using Editor.Rendering;
using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Graphics;
using Engine.GUI;
using Engine.Layers;
using Engine.Layers.Input;
using Game;
using ImGuiNET;
using SharedTypes;
using System.Numerics;

namespace Editor
{
    // TODO:
    // Refactor.
    // Hierarchy view
    // Simple object editor: Components(name) (No saving data)
    // Playmode on launch (maybe I implement a proper playmode later: pause, frame step)
    // Add mouse picker the ability to pick the object behind the current picked object.
    // Draw gizmos (camera icon (and any other object too), arrows handle, object selected)
    // Remove the sides of the screen for the mouse picker, so an object doesn't get picked when dragging a window.

    internal class EditorEntry
    {
        private WindowStandalone _win;
        private ImGuiGLFW _glfwInput;
        private const string PROJECT_FOLDER_NAME = "Editor";
        private EditorGameView _gameWindow;
        private SceneEditorView _editorSceneView;

        private SceneGraphWindow _sceneGraphWindow;
        private GFSEngine _engine;
        private AnimatorEditorView _node;
        private ObjectEditorView _objectEditor;
        private RenderingSurface _gameSurface;
        private RenderingSurface _editorSurface;
        private EditorCamera _editorCamera;
        private InputStandAlonePlatform _inputLayer;

        internal void Init()
        {
            NativeLogger.Init();

            var windowIcon = new TextureDescriptor()
            {
                Width = EditorIcon.Width,
                Height = EditorIcon.Height,
                Buffer = EditorIcon.Icon
            };

            _win = new WindowStandalone("GFS Editor", 1324, 740, Color.Black);
            _win.CanResize = true;
            _sceneGraphWindow = new SceneGraphWindow();
            _objectEditor = new ObjectEditorView();

            RenderingLayer.OverlayOptions.Width = _win.PhysicalWidth;
            RenderingLayer.OverlayOptions.Height = _win.PhysicalHeight;

            ImguiImplOpenGL3.Init(_win);
            _glfwInput = new ImGuiGLFW(WindowStandalone.NativeWindow);
            _glfwInput.Init();
            _node = new AnimatorEditorView();

            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf(PROJECT_FOLDER_NAME)), Paths.GAME_FOLDER_NAME);

            new GameCooker.GameProject().Initialize(new GameCooker.ProjectConfig() { ProjectFolderRoot = root });


            _gameSurface = new RenderingSurface()
            {
                PickCameraFromSceneGraph = true,
                RenderPostProcessing = true,
                RenderUI = true,
                UIViewProj = UICanvas.UIViewProj,
            };

            _inputLayer = new InputStandAlonePlatform();
            _gameWindow = new EditorGameView(_win, _gameSurface, _inputLayer);

            _engine = new GFSEngine(_gameWindow, new GameApplication(), _inputLayer);

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

            _editorSceneView = new SceneEditorView("Scene", _editorSurface, _editorCamera);

            RenderingLayer.InitializeSurfaces([_gameSurface, _editorSurface]);
            RenderingLayer.OnDrawOverlay += () =>
            {
                Render();
            };

            _win.OnWindowChanged += (w, h) =>
            {
                RenderingLayer.OverlayOptions.Width = _win.PhysicalWidth;
                RenderingLayer.OverlayOptions.Height = _win.PhysicalHeight;
                UpdateAll();
            };

            _gameWindow.OnWindowChanged += (w, h) =>
            {
                _engine.Update();
            };

            while (!_win.ShouldClose)
            {
                UpdateAll();
            }
        }

        private void Render()
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

            _win.SwapBuffers();

        }

        private void DockSpace()
        {
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos);
            ImGui.SetNextWindowSize(viewport.Size);
            ImGui.SetNextWindowViewport(viewport.ID);

            ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar |
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

            // call imgui functions here: ---
            _gameWindow.OnDraw();
            _editorSceneView.OnDraw();

            _sceneGraphWindow.OnDraw();
            _objectEditor.OnDraw();
            _node.OnRender();

            RenderingInfoWindow();
            // ------

            ImGui.End();
            //ImGui.PopStyleColor();
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



        private void UpdateAll()
        {
            _engine.Update();
            _gameWindow.Update();
            _sceneGraphWindow.OnUpdate();
            _objectEditor.OnUpdate();
        }
    }
}
