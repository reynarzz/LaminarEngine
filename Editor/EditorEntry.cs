using Engine;
using Engine.Graphics;
using Engine.Layers;
using Engine.Layers.Input;
using Game;
using ImGuiNET;
using SharedTypes;
using System.Numerics;

namespace Editor
{
    // TODO:
    // Hierarchy view
    // Simple object editor: Components(name) (No saving data)
    // Playmode on launch (maybe I implement a proper playmode later: pause, frame step)
    // Rendering info window.
    // Game rendering in a full screen window.

    internal class EditorEntry
    {
        private WindowStandalone _win;
        private ImGuiGLFW _glfwInput;
        private const string PROJECT_FOLDER_NAME = "Editor";
        private EditorGameView _gameWindow;

        internal void Init()
        {
            _win = new WindowStandalone("Editor", 1324, 640, Color.Black);
            _win.CanResize = true;
            _gameWindow = new EditorGameView(_win);

            RenderingLayer.OverlayOptions.Width = _win.Width;
            RenderingLayer.OverlayOptions.Height = _win.Height;

            ImguiImplOpenGL3.Init(_win.Width, _win.Height);
            _glfwInput = new ImGuiGLFW(WindowStandalone.NativeWindow);
            _glfwInput.Init();

            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf(PROJECT_FOLDER_NAME)), Paths.GAME_FOLDER_NAME);

            new GameCooker.GameProject().Initialize(new GameCooker.ProjectConfig() { ProjectFolderRoot = root });

            var engine = new GFSEngine(_gameWindow, new GameApplication(), new InputStandAlonePlatform());

            RenderingLayer.OnDrawOverlay += () =>
            {
                Render();
            };

            _win.OnWindowChanged += (w, h) =>
            {
                RenderingLayer.OverlayOptions.Width = w;
                RenderingLayer.OverlayOptions.Height = h;
                engine.Update();
                _gameWindow.Update();

            };
            
            _gameWindow.OnWindowChanged += (w, h) =>
            {
                engine.Update();
            };

            while (!_win.ShouldClose)
            {
                _gameWindow.Update();
                engine.Update();
            }
        }

        private void Render()
        {
            ImguiImplOpenGL3.SetPerFrameImGuiData(Time.UnscaledDeltaTime, _win.Width, _win.Height);

            ImGui.NewFrame();
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

            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.1f, 0.1f, 0.1f, 1.0f));
            ImGui.Begin("DockSpaceHost", flags);

            ImGui.PopStyleVar(3);

            var dockspaceId = ImGui.GetID("MainDockSpace");
            ImGui.DockSpace(dockspaceId, Vector2.Zero, ImGuiDockNodeFlags.None);

            // call imgui functions here: ---
            _gameWindow.Render();
            RenderingInfoWindow();
            Hierarchy();
            // ------

            ImGui.End();
            ImGui.PopStyleColor();
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

        private Actor _selectedActor;
        private void Hierarchy()
        {
            ImGui.Begin("Scene graph");

            void DrawActor(Actor actor)
            {
                ImGui.PushID(actor.GetID().ToString());

                bool hasChildren = actor.Transform.Children.Count > 0;

                ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth;

                if (!hasChildren)
                    flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;

                if (_selectedActor == actor)
                    flags |= ImGuiTreeNodeFlags.Selected;

                if (!actor.IsActiveInHierarchy)
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f));

                bool open = ImGui.TreeNodeEx(actor.Name, flags);

                if (!actor.IsActiveInHierarchy)
                    ImGui.PopStyleColor();

                // Selection handling
                if (ImGui.IsItemClicked())
                    _selectedActor = actor;

                if (ImGui.BeginPopupContextItem("ActorContext"))
                {
                    _selectedActor = actor;

                    if (ImGui.MenuItem("Rename")) { }
                    if (ImGui.MenuItem("Duplicate")) { }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Delete"))
                    {

                        ImGui.EndPopup();
                        if (open && hasChildren)
                        {
                            ImGui.TreePop();
                        }
                        ImGui.PopID();

                        Actor.Destroy(_selectedActor);
                        return;
                    }

                    ImGui.EndPopup();
                }

                if (open && hasChildren)
                {
                    foreach (var child in actor.Transform.Children)
                        DrawActor(child.Actor);

                    ImGui.TreePop();
                }

                ImGui.PopID();
            }

            for (int i = 0; i < SceneManager.Scenes.Count; i++)
            {
                ImGui.PushID(SceneManager.Scenes[i].GetID().ToString());
                ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth | ImGuiTreeNodeFlags.DefaultOpen;

                bool open = ImGui.TreeNodeEx(SceneManager.Scenes[i].Name, flags);
                if (open)
                {
                    if(SceneManager.Scenes[i].RootActors.Count > 0)
                    {
                        for (int j = 0; j < SceneManager.Scenes[i].RootActors.Count; j++)
                        {
                            DrawActor(SceneManager.Scenes[i].RootActors[j]);
                        }
                    }

                    ImGui.TreePop();
                }
                ImGui.PopID();
            }

            ImGui.End();
        }

        private void GameWindow()
        {
        
        }
    }
}
