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

        private InputStandAlonePlatform _editorInput;



        public EditorLayersManager(InputLayerBase inputLayer) : base([
                                          new TimeLayer(), inputLayer,
                                          new MainThreadDispatcher(),
                                          // new SceneLayer(),
                                          new AudioLayer(),
                                          // new PhysicsLayer(),
                                          new RenderingLayer(),
                                          new IOLayer()
                                            ])
        {
            
        }

        internal override void Initialize()
        {
            base.Initialize();

            // new GameApplication()
            // new TimeLayer()
            // new SceneLayer()
        }

        internal override void Update()
        {


            base.Update();
        }
    }
}
