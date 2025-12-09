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
        public GFSEngine(IWindow window, Type gameLayerType) : this(window, gameLayerType, null)
        {

        }
        public GFSEngine(IWindow window, Type gameLayerType, BinaryReader assetFileStream)
        {
            WindowManager.Window = window;
            AssetFileStream = assetFileStream;

            window.OnWindowChanged += (x, y) => _layersManager.Update();
            window.OnWindowClose += () => { _layersManager.OnClose(); };
            if (window.IsInitialized)
            {
                _layersManager = new LayersManager([typeof(TimeLayer),
                                                    typeof(Input),
                                                    gameLayerType,
                                                    typeof(MainThreadDispatcher),
                                                    typeof(SceneLayer),
                                                    typeof(AudioLayer),
                                                    typeof(PhysicsLayer),
                                                    typeof(RenderingLayer),
                                                    typeof(IOLayer)]);

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