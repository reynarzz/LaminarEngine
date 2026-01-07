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

        public GFSEngine(IWindow window, ApplicationLayer appLayer, InputLayerBase input, BinaryReader assetFileStream)
            : this(window, input, new LayersManager([new TimeLayer(), input,
                                                   appLayer,
                                                   new MainThreadDispatcher(),
                                                   new SceneLayer(),
                                                   new AudioLayer(),
                                                   new PhysicsLayer(),
                                                   new RenderingLayer(),
                                                   new IOLayer()]), assetFileStream)
        {
        }

        internal GFSEngine(IWindow window, InputLayerBase input, LayersManager layerManager, BinaryReader assetFileStream)
        {
            WindowManager.Window = window;
            Input.CurrentInput = input;
            AssetFileStream = assetFileStream;

            window.OnWindowChanged += (x, y) => layerManager.Update();
            window.OnWindowClose += () => { layerManager.OnClose(); };
            if (window.IsInitialized)
            {
                _layersManager = layerManager; 
                _layersManager.Initialize();
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