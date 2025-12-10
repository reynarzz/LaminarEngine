using Engine.Graphics;
using Engine.Graphics.OpenGL;
using Engine.Layers;
using Engine.Utils;
using System.Text;

namespace Engine
{
    public class GFSEngine
    {
        private LayersManager _layersManager;
        internal static BinaryReader AssetFileStream { get; private set; }
        public GFSEngine(IWindow window, ApplicationLayer appLayer) : this(window, appLayer, null)
        {

        }
        public GFSEngine(IWindow window, ApplicationLayer appLayer, BinaryReader assetFileStream)
        {
            WindowManager.Window = window;
            AssetFileStream = assetFileStream;

            window.OnWindowChanged += (x, y) => _layersManager.Update();
            window.OnWindowClose += () => { _layersManager.OnClose(); };
            if (window.IsInitialized)
            {
                _layersManager = new LayersManager([new TimeLayer(),
                                                    new Input(),
                                                    appLayer,
                                                    new MainThreadDispatcher(),
                                                    new SceneLayer(),
                                                    new AudioLayer(),
                                                    new PhysicsLayer(),
                                                    new RenderingLayer(),
                                                    new IOLayer()]);

                _layersManager.Initialize();
            }

        }
        
        public void Run()
        {
            if (_layersManager == null)
            {
                Debug.EngineError("FATAL: Engine couldn't not be initialized correctly.");
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