using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    internal class LayersManager
    {
        protected LayerBase[] Layers {  get; set; }
        private readonly CleanUpLayer _cleanupLayer = new();
        private MainThreadDispatcher _initializationDispatcher = new();
        internal bool LayersInitialized { get; protected private set; }
        public int Count => Layers.Length;
        public LayersManager([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type[] layersTypes)
        {
            Layers = new LayerBase[layersTypes.Length];
            for (int i = 0; i < layersTypes.Length; i++)
            {
                Layers[i] = (LayerBase)Activator.CreateInstance(layersTypes[i]);
            }

        }

        protected LayersManager()
        {
        }

        public LayersManager(LayerBase[] layers)
        {
            Layers = layers;
        }

        internal virtual void InitializeAsync()
        {
            if (LayersInitialized)
            {
                Debug.EngineError("Layers are already initialized");
                return;
            }

            async Task InitializeLayers()
            {
                
                for (int i = Layers.Length - 1; i >= 0; i--)
                {
                        var layer = Layers[i];

                    try
                    {
                        if (layer != null)
                        {
                            await layer.InitializeAsync();
                            Debug.Log("Initialized layer: " + layer.GetType().Name);
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e.ToString());
                    }
                }
                

                Debug.Log("Layers fully initialized");
                LayersInitialized = true;
            }

            Task.Run(InitializeLayers);
        }

        internal void PushLayer(LayerBase layer, int index)
        {
            if (index < 0 || index >= Layers.Length)
            {
                Debug.Error("Wrong layer index: " + index);
                return;
            }

            Layers[index] = layer;

            layer.InitializeAsync();
        }

        internal void PopLayer(int index)
        {
            Layers[index]?.Close();
            Layers[index] = null;
        }

        internal virtual void Update()
        {
            if (!LayersInitialized)
            {
                _initializationDispatcher.UpdateLayer();
#if DESKTOP
                GLFW.Glfw.PollEvents();
#endif
                return;
            }

            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i]?.UpdateLayer();
            }

            _cleanupLayer.UpdateLayer();
        }

        internal virtual void PublishEvent(EventType type, object value)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i]?.OnEvent(type, value);
            }
        }

        internal virtual void OnClose()
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i]?.Close();
            }
        }
    }
}
