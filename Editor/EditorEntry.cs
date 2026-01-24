using Editor.AssemblyHotReload;
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
    // Bug with mouse picker: cliking almost the top border of the scene window doesn't register a pick, and performance degrades.

    // Fix:
    // Copy native dependencies to bin directories.
    // Investigate why when the scene is reloaded old objects are still alive.
    // Rebuild the rendering system.
    // Fix CollisionDispatcher Hashset use instead of dictionary.

    // Assets:
    // Implement material asset build.
    // load spriteAtlas from texture 2d metadata.
    // Write binary serializer for runtime build.

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
            NativeLogger.Init();

            _win = new WindowStandalone("GFS Editor", 1424, 840, Color.Black, new TextureDescriptor()
            {
                Width = EditorDefaultIcon.Width,
                Height = EditorDefaultIcon.Height,
                Buffer = EditorDefaultIcon.Icon
            });

            _win.CanResize = true;

            RenderingLayer.OverlayOptions.Width = _win.PhysicalWidth;
            RenderingLayer.OverlayOptions.Height = _win.PhysicalHeight;

            _inputLayer = new InputStandAlonePlatform();

            var editorLayerManager = new EditorLayersManager(_inputLayer, _win);

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

        private void UpdateAll()
        {
            _engine.Update();
            _win.SwapBuffers();
        }
    }
}
