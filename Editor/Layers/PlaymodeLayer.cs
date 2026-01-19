using Editor.Serialization;
using Engine;
using Engine.Layers;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Layers
{
    internal class PlaymodeController
    {
        private readonly LayersManager _manager;
        private readonly TimeLayer _time;
        private readonly List<(LayerBase layer, int priorityIndex)> _playmodeLayers;

        public PlaymodeController(LayersManager manager, TimeLayer time)
        {
            _manager = manager;
            _time = time;
            _playmodeLayers = new List<(LayerBase layer, int priorityIndex)>()
            {
                (default, 2),
                (new SceneLayer(), 4),
                (new PhysicsLayer(), 6),
            };
        }


        internal void PlayModeOn()
        {
            _time.Initialize();
            Application.IsInPlayMode = true;

            var gameLayer = _playmodeLayers[0];
            gameLayer.layer = ReflectionUtils.GetDefaultValueInstance(GfsTypeRegistry.GameAppType) as LayerBase;
            _playmodeLayers[0] = gameLayer;
            for (int i = _playmodeLayers.Count - 1; i >= 0; --i)
            {
                var layerData = _playmodeLayers[i];
                _manager.PushLayer(layerData.layer, layerData.priorityIndex);
            }
        }

        internal void PlayModeOff()
        {
            foreach (var layerData in _playmodeLayers)
            {
                _manager.PopLayer(layerData.priorityIndex);
            }

            Application.IsInPlayMode = false;
        }
    }
}
