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

        public GFSEngine Initialize<T>(string winName, int width, int height) where T: ApplicationLayer
        {
            return Initialize<T>(winName, width, height, Color.Black); 
        }
        public GFSEngine Initialize<T>(string winName, int width, int height, Color windowColor) where T : ApplicationLayer
        {
            var win = new Window(winName, width, height, windowColor);

            Window.OnWindowChanged += (x, y) => _layersManager.Update();
            if (win.IsInitialized)
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
                Debug.Error("FATAL: Engine couldn't not be initialized correctly.");
                return;
            }

            while (!Window.ShouldClose)
            {
                _layersManager.Update();
            }
        }
    }
}