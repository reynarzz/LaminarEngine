using Engine.Graphics;
using Engine.Layers;
using Engine.Utils;
using System.Text;

namespace Engine
{
    public class GFSEngine
    {
        private LayersManager _layersManager;
        internal static BinaryReader AssetFileStream { get; private set; }
        public GFSEngine(IWindow window, ApplicationLayer appLayer, InputLayerBase input) :
            this(window, appLayer, input, null)
        { }

        public GFSEngine(IWindow window, ApplicationLayer appLayer, InputLayerBase input, BinaryReader assetFileStream) :
            this(window, appLayer, input, new RuntimeIOLayer(), assetFileStream)
        {

        }
        public GFSEngine(IWindow window, ApplicationLayer appLayer, InputLayerBase input, IOLayer ioLayer, BinaryReader assetFileStream)
            : this(window, input, new LayersManager([new TimeLayer(), input,
                                                   appLayer,
                                                   new MainThreadDispatcher(),
                                                   new SceneLayer(),
                                                   new AudioLayer(),
                                                   new PhysicsLayer(),
                                                   new RenderingLayer(),
                                                   ioLayer]), assetFileStream)
        {
        }

        internal GFSEngine(IWindow window, InputLayerBase input, LayersManager layerManager, BinaryReader assetFileStream)
        {
#if DEBUG
#if WIN32
                    Debug.Log("Engine running windows");
#elif ANDROID
                    Debug.Log("Engine running android");
#elif MACOS
                    Debug.Log("Engine running macOs");
#elif EDITOR
                    Debug.Log("Engine running editor");
#elif IOS
                    Debug.Log("Engine running ios");
#endif
#endif
            WindowManager.Window = window;
            Input.CurrentInput = input;
            AssetFileStream = assetFileStream;

            window.OnWindowChanged += (x, y) => layerManager.Update();
            window.OnWindowClose += () => { layerManager.OnClose(); };

            if (window.IsInitialized)
            {
                _layersManager = layerManager;
                _layersManager.InitializeAsync();
            }
        }

        public void Run()
        {
            if (_layersManager == null)
            {
                Debug.EngineError("FATAL: Engine layers couldn't be initialized.");
                return;
            }

            while (!WindowManager.Window.ShouldClose)
            {
                Update();
            }
        }

        public void Update()
        {
            _layersManager.Update();
        }
    }
}