using Engine;
using Engine.Layers;
using Engine.Layers.Input;
using Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class EditorLayersManager : LayersManager
    {
        private static TimeLayer _time = new();
        private SceneLayer _editorSceneLayer;

        public EditorLayersManager(InputLayerBase inputLayer) :
            base([_time, inputLayer,
                  null, // AppLayer
                  new MainThreadDispatcher(),
                  null, // SceneLayer
                  new AudioLayer(),
                  null, // PhysicsLayer,
                  new RenderingLayer(),
                  new IOLayer()])
        {
            _editorSceneLayer = new SceneLayer();
        }

        internal override void Initialize()
        {
            base.Initialize();


        }

        internal override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Enter))
            {
                _time.Initialize();
                PushLayer(new PhysicsLayer(), 6);
                PushLayer(new SceneLayer(), 4);
                PushLayer(new GameApplication(), 2);
            }
            if(Input.GetKeyDown(KeyCode.Backspace)) 
            {
                PopLayer(2);
                PopLayer(4);
                PopLayer(6);

                //
            }

            base.Update();
        }
    }
}
