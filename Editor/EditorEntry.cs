using Editor.Build;
using Editor.Data;
using Editor.Layers;
using Editor.Rendering;
using Editor.Utils;
using Editor.Views;
using Engine;
using Engine.Graphics;
using Engine.GUI;
using Engine.IO;
using Engine.Layers;
using Engine.Layers.Input;
using GameCooker;
using GlmNet;
using ImGuiNET;
using SharedTypes;
using System.Numerics;

namespace Editor
{
    // TODO:
    // -The current code is very slow, and contains tons of bad practices to cut corners, leading to memory leaks, and slowdowns,
    //    most of it is in 'prototype' phase a nice (big) refactor is on the way.
    // -Refactor.
    // -Changing cameras causes to render prevCamera.
    // -Weird rendering issue, related to mouse picking pink materials, is it the RenderingSystem, batcher?
    // -Implement Asset files for: Tilemap.
    // -The serializer has a possible bug related to delegates,
    //   collections do not search for all delegates(privates/public), but classes do.
    // -If I load the runtime mode, and the applicationLayer is enabled, the camera renders black.
    // GameCooker, the asset database sometimes doesn't remove old assets correctly.

    // Fix:
    // Investigate why when the scene is reloaded old objects are still alive.
    // Rebuild the rendering system.
    // Compilation can get stuck forever.
    // If a collider change from Trigger to normal (and viceversa), the ontrigerExit/onCollisionExit must be called.

    // Serialization
    // - Forbid serialization of dictionaries that contains EObject as key?
    // - For runtime build, create a dictionary with all the types, so types can be saved and found quickly: Dictionary<Guid, Type>
    // - The TypeRegistry should only be used in ship builds. While binary serializing the assets, also add types to the TypeRegistryClassGenerator.



    // Performance fix:
    // UI performance is pretty bad, rewrite the entire UI system.

    // Assets:
    // Implement MaterialAssetBuilder.
    // Write binary serializer for runtime build.
    // Metadata of every asset should be binary on final build.

    // Features
    // - Fully async scene load in the background: async asset loading (mainThreadSync for graphic assets)

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

            while (!_win.ShouldClose)
            {
                UpdateAll();
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
            EditorMenu.PushMenu("File/Save", () => Debug.Warn("Save"), false, "Ctrl+S");
            EditorMenu.PushMenu("File/Save As", () => Debug.Warn("Save As"));
            EditorMenu.PushMenu("File/Settings", () => Debug.Warn("Settings"));
            EditorMenu.PushMenu("File/Quit", () => Debug.Warn("Quit"));

            // Edit
            EditorMenu.PushMenu("Edit/Project Settings", () => Debug.Warn("proj"));
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
