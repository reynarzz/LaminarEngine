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
        protected List<LayerBase> _layers;
        private CleanUpLayer _cleanupLayer;
        private bool _layersInitialized = false;
        public int Count => _layers.Count;

        public LayersManager([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type[] layersTypes)
        {
            _layers = new List<LayerBase>();
            for (int i = 0; i < layersTypes.Length; i++)
            {
                _layers.Add((LayerBase)Activator.CreateInstance(layersTypes[i]));
            }

            _cleanupLayer = new CleanUpLayer();
        }

        public LayersManager(LayerBase[] layers)
        {
            _layers = layers.ToList();
            _cleanupLayer = new CleanUpLayer();
        }

        internal virtual void Initialize()
        {
            _cleanupLayer.Initialize();

            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                _layers[i].Initialize();

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

        internal void PushLayer(LayerBase layer, int index = -1)
        {
            if(index < 0)
            {
                _layers.Add(layer);
            }
            else
            {
                _layers.Insert(index, layer);
            }
        }

        internal void PopLayer(LayerBase layer)
        {
            _layers.Remove(layer);
        }

        internal virtual void Update()
        {
            if (!_layersInitialized)
                return;

            for (int i = 0; i < _layers.Count; i++)
            {
                _layers[i].UpdateLayer();
            }

            _cleanupLayer.UpdateLayer();
        }

        internal virtual void PublishEvent(LayerEvent currentEvent)
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                _layers[i].OnEvent(currentEvent);
            }
        }

        internal virtual void OnClose()
        {
            for (int i = 0; i < _layers.Count; i++)
            {
                _layers[i].Close();
            }
        }
    }
}
