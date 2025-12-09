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
        private WindowManager _windowManager;

        public GFSEngine Initialize<T>(string winName, int width, int height) where T: ApplicationLayer
        {
            return Initialize<T>(winName, width, height, Color.Black); 
        }
        public GFSEngine Initialize<T>(string winName, int width, int height, Color windowColor) where T : ApplicationLayer
        {
            _windowManager = new WindowManager(winName, width, height, windowColor);

            WindowManager.Window.OnWindowChanged += (x, y) => _layersManager.Update();
            WindowManager.Window.OnWindowClose += () => { _layersManager.OnClose(); };
            if (WindowManager.Window.IsInitialized)
            {
                _layersManager = new LayersManager([typeof(TimeLayer),
                                                    typeof(Input),
                                                    typeof(T),
                                                    typeof(MainThreadDispatcher),
                                                    typeof(SceneLayer),
                                                    typeof(AudioLayer),
                                                    typeof(PhysicsLayer),
                                                    typeof(RenderingLayer),
                                                    typeof(IOLayer)]);

                _layersManager.Initialize();
            }

            return this;
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
                _layersManager.Update();
            }
        }
    }
}