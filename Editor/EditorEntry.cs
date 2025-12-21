using Engine;
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
        private Window _win;
        private ImGuiGLFW _glfwInput;
        private const string PROJECT_FOLDER_NAME = "Editor";

        internal void Init()
        {
            _win = new Window("Editor", 1024, 640, Color.Black);
            _win.CanResize = true;

            ImguiImplOpenGL3.Init(_win.Width, _win.Height);
            _glfwInput = new ImGuiGLFW(Window.NativeWindow);
            _glfwInput.Init();

            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf(PROJECT_FOLDER_NAME)), Paths.GAME_FOLDER_NAME);

            new GameCooker.GameProject().Initialize(new GameCooker.ProjectConfig() { ProjectFolderRoot = root });

            var engine = new GFSEngine(_win, new GameApplication(), new InputStandAlonePlatform());

            RenderingLayer.OnDrawOverlay += () =>
            {
                Render();
            };

            _win.OnWindowChanged += (w, h) =>
            {
                engine.Update();
            };

            while (!_win.ShouldClose)
            {
                engine.Update();
            }
        }

        private void Render()
        {
            ImguiImplOpenGL3.SetPerFrameImGuiData(Time.DeltaTime, _win.Width, _win.Height);

            ImGui.NewFrame();

            _glfwInput.NewFrame();

            RenderingInfoWindow();
            Hierarchy();

            ImGui.Render();
            ImguiImplOpenGL3.RenderDrawData(ImGui.GetDrawData());
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

                if (!actor.IsActiveSelf)
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f));

                bool open = ImGui.TreeNodeEx(actor.Name, flags);

                if (!actor.IsActiveSelf)
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
                ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanFullWidth;

                bool open = ImGui.TreeNodeEx(SceneManager.Scenes[i].Name, flags);
                if (open && SceneManager.Scenes[i].RootActors.Count > 0)
                {
                    for (int j = 0; j < SceneManager.Scenes[i].RootActors.Count; j++)
                    {
                        DrawActor(SceneManager.Scenes[i].RootActors[j]);
                    }

                    ImGui.TreePop();
                }
                ImGui.PopID();
            }

            ImGui.End();
        }

    }
}
