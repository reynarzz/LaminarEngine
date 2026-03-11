using Editor.Layers;
using Editor.Utils;
using Engine;
using Engine.Graphics;
using Engine.Layers;
using Engine.Layers.Input;
using Editor.Cooker;
using ImGuiNET;

namespace Editor
{
    internal class EditorEntry
    {
        private WindowStandalone _win;
        private GFSEngine _engine;
        private InputStandAlonePlatform _inputLayer;

        internal void Init()
        {
            Application.IsInPlayMode = false;
            GameProject.Initialize(new ProjectConfig() { ProjectFolderRoot = EditorPaths.GameRoot });

            _win = new WindowStandalone("GFS Editor", 1424, 840, Color.Black, new TextureDescriptor()
            {
                Width = EditorDefaultIcon.Width,
                Height = EditorDefaultIcon.Height,
                Buffer = EditorDefaultIcon.Icon
            });

            NativeLogger.Init();
            EditorNatives.InitImAllGui();

            EditorNatives.InitGLFWImguiInternal(WindowStandalone.NativeWindow.handle);
            MenuItems();


            _win.CanResize = true;

            RenderingLayer.OverlayOptions.Width = _win.PhysicalWidth;
            RenderingLayer.OverlayOptions.Height = _win.PhysicalHeight;

            _inputLayer = new InputStandAlonePlatform();

            var editorLayerManager = new EditorLayersManager(_inputLayer, _win);


            HelpMenu();

            _engine = new GFSEngine(ImGuiLayer.GameWindow, _inputLayer, editorLayerManager, null);

            // Physical window.
            _win.OnWindowChanged += (w, h) =>
            {
                RenderingLayer.OverlayOptions.Width = _win.PhysicalWidth;
                RenderingLayer.OverlayOptions.Height = _win.PhysicalHeight;
                UpdateAll();
            };

            _win.OnWindowFocusChanged += focused =>
            {
                editorLayerManager.PublishEvent(focused ? EventType.WindowFocusEnter : EventType.WindowFocusExit, null);
            };

            //RuntimeCore.InitSonyGamepad();

            while (!_win.ShouldClose)
            {
                UpdateAll();
                //RuntimeCore.UpdateSonyGamepad(ImGui.GetIO().DeltaTime);
            }

            editorLayerManager.OnClose();
        }

        private void HelpMenu()
        {
            // Help
            EditorMenu.PushMenu("Help/Made by Reynardo Perez", () =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://www.reynarz.com",
                    UseShellExecute = true
                });
            });
            //EditorMenu.PushMenu("Help/With toggle", () => { }, true);
            //EditorMenu.PushMenu("Help/Disabled", () => { });

            //EditorMenu.SetEnabled("Help/With toggle", false);
            //EditorMenu.Toggle("Help/With toggle", true);
        }
        private void MenuItems()
        {
            // File
            EditorMenu.PushMenu("File/Open", () => Debug.Warn("Open"));
         //   EditorMenu.PushMenu("File/Save", () => Debug.Warn("Save"), false, "Ctrl+S");
            EditorMenu.PushMenu("File/Save As", () => Debug.Warn("Save As"));
            EditorMenu.PushMenu("File/Settings", () => Debug.Warn("Settings"));
            EditorMenu.PushMenu("File/Quit", () => Debug.Warn("Quit"));

            // Edit
            EditorMenu.PushMenu("Edit/Preferences", () => { });

            // Assets
            EditorMenu.PushMenu("Assets/Create/Folder", () => { });
            EditorMenu.PushMenu("Assets/Create/Scene", () => { });
            EditorMenu.PushMenu("Assets/Create/Material", () => { });
            EditorMenu.PushMenu("Assets/Create/Shader", () => { });
            EditorMenu.PushMenu("Assets/Create/Animation Clip", () => { });
            EditorMenu.PushMenu("Assets/Create/Animator Controller", () => { });

            // Actor
            EditorMenu.PushMenu("Actor/Create Empty", () => { /*new Actor("Actor"); */});
            EditorMenu.PushMenu("Actor/Create Empty Child", () =>
            {
                //if (Selector.SelectedTransform())
                //{
                //    new Actor("Actor").Transform.Parent = Selector.SelectedTransform();
                //}

            });

            EditorMenu.AddSeparator("Actor", 2);
            EditorMenu.PushMenu("Actor/Box", () => { });
            EditorMenu.PushMenu("Actor/Sprite", () => { });
            EditorMenu.PushMenu("Actor/Audio Source", () => { });
        }

        private void UpdateAll()
        {
            _engine.Update();
            _win.SwapBuffers();
        }
    }
}
