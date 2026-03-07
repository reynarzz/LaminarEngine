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
    // TODO:
    // -The current code is very slow, and contains tons of bad practices to cut corners, leading to memory leaks, and slowdowns,
    //    most of it is in 'prototype' phase a nice (big) refactor is on the way.
    // -Refactor.
    // -Changing cameras causes to render prevCamera.
    // -Weird rendering issue, related to mouse picking pink materials, is it the RenderingSystem, batcher?
    // -If I load the runtime mode, and the applicationLayer is enabled, the camera renders black.
    // Editor.Cooker, the asset database sometimes doesn't remove old assets correctly.
    // - Improve Editor property UI

    // Fix:
    // - Investigate why when the scene is reloaded old objects are still alive.
    // - Rebuild the rendering system.
    // - Compilation can get stuck forever.
    // - If a collider change from Trigger to normal (and viceversa), the ontrigerExit/onCollisionExit must be called.
    // - The renderData is not removed from renderingLayer dictionary if the Actor starts disabled in the first frame, and then try to enable it.
    // - Forbid materials to send system uniforms: example: uTime, MVp, etc...
    // - Screen render target size should take into account the camera viewport values.
    // - The Game.dll must exist before doing a build because TypeGenerationStage.GetAssemblyTypes(str) searches
    //     for it to generate the typeRegistry, the build should already have a game.dll compiled. Use it from the library folder for now,
    //     but the game.dll should be compiled for the build type(Release/Debug), and turning on all the directives defined for the target platform.
    // - Mouse picker should take into account all the vertices types (ex, FontVertex, SpriteVertex), it currently only supports 'Vertex' type.

    // Serialization:
    // - Forbid serialization of dictionaries that contains EObject as key?
    // - If a dictionary has a complexClass as a key/value, and a EObject as key/value, the dictionary should not
    //   be recognized as ReferenceCollection (pure), but as complexCollection.
    // - (Serializer IR) Fix: in 'GetSerializedType()' get all the elements in the collection so they
    //   can be checked in case there is a reference.
    // - Collection of simple types such as long makes the json to serialize the whole 'VariantIRValue', which causes data corruption
    // - Bool write is not consistent in dictionary and 1d collection write for binary serializer.


    // Refactor:
    // - Batch2D, this class maintains the vertices in the ram at all times, once the vertex data is send to the gpu, the ram should be freed.

    // Performance fix:
    // UI performance is pretty bad, rewrite the entire UI system.

    // Assets:
    // Metadata of every asset should be binary on final build.

    // Features
    // - Fully async scene load in the background: async asset loading (mainThreadSync for graphic assets)
    // - Audio mixer editor ui
    // - Animator using nodes
    // - Assets lazy loading: Selecting assets in the inspector should not load the asset, but when the playmode starts,
    //   create a ImmediateAssetLoad<T> to load as soon as it gets in the property inspector.
    // - Generate the assets to be added to the build reading all the scenes that will be added to the build.
    // - When an asset is updated, other assets that depend on it should be notified/updated as well, ex: Texture2D->Tilemap


    // Save Editor config:
    // Current loaded scene name.
    // Layers.
    // Camera position.
    // Project settings: project name, icon, physics, audio etc..
    // Project build settings: 

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
