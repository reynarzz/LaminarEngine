using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    internal class LayersManager
    {
        protected LayerBase[] _layers;
        private CleanUpLayer _cleanupLayer;
        private bool _layersInitialized = false;
        public int Count => _layers.Length;

        public LayersManager([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type[] layersTypes)
        {
            _layers = new LayerBase[layersTypes.Length];
            for (int i = 0; i < layersTypes.Length; i++)
            {
                _layers[i] = (LayerBase)Activator.CreateInstance(layersTypes[i]);
            }

            _cleanupLayer = new CleanUpLayer();
        }

        public LayersManager(LayerBase[] layers)
        {
            _layers = layers;
            _cleanupLayer = new CleanUpLayer();
        }

        internal virtual void Initialize()
        {
            _cleanupLayer.Initialize();

            for (int i = _layers.Length - 1; i >= 0; i--)
            {
                _layers[i]?.Initialize();

                //#if DEBUG
                //                try
                //                {
                //                    _layers[i].Initialize();
                //                }
                //                catch (Exception e)
                //                {
                //                    Debug.Error(e);
                //                }
                //#else
                //                _layers[i].Initialize();
                //#endif
            }

            _layersInitialized = true;
        }

        internal void PushLayer(LayerBase layer, int index)
        {
            if(index < 0 || index >= _layers.Length)
            {
                Debug.Error("Wrong layer index: " + index);
                return;
            }

            _layers[index] = layer;

            layer.Initialize();
        }

        internal void PopLayer(int index)
        {
            _layers[index]?.Close();
            _layers[index] = null;
        }

        internal virtual void Update()
        {
            if (!_layersInitialized)
                return;

            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i]?.UpdateLayer();
            }

            _cleanupLayer.UpdateLayer();
        }

        internal virtual void PublishEvent(LayerEvent currentEvent)
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i]?.OnEvent(currentEvent);
            }
        }

        internal virtual void OnClose()
        {
            for (int i = 0; i < _layers.Length; i++)
            {
                _layers[i]?.Close();
            }
        }
    }
}
