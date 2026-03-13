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
    // Refactor this class
    internal class PlaymodeController
    {
        private readonly LayersManager _manager;
        private readonly TimeLayer _time;
        private readonly HotReloadLayer _hotReload;
        private readonly List<(LayerBase layer, int priorityIndex)> _playmodeLayers;
        private static PlaymodeController _instanceTest;
        public static PlaymodeController Instance => _instanceTest; // Remove, refactor
        public static bool IsPaused { get; private set; }
        public event Action OnBeforePlaymode;
        public event Action OnAfterEditMode;

        public PlaymodeController(LayersManager manager, TimeLayer time, HotReloadLayer hotReload)
        {
            _instanceTest = this;
            _manager = manager;
            _time = time;
            _hotReload = hotReload;
            _playmodeLayers = new List<(LayerBase layer, int priorityIndex)>()
            {
                (default, 2),
                (new PhysicsLayer(), 6),
            };
        }

        internal void PlayModeOn()
        {
            if (!Application.IsInPlayMode)
            {
                // Serialize all the opened scenes.
                SceneManagerEditor.SerializeScenesPlaymode();
                var prevSelectedActorId = Guid.Empty;

                if (Selector.SelectedTransform())
                {
                    prevSelectedActorId = Selector.SelectedTransform().Actor.GetID();
                }
                Application.IsInPlayMode = true;

                _time.InitializeAsync();

                // Push the needed layers for playmode.
                var gameLayer = _playmodeLayers[0];
                gameLayer.layer = ReflectionUtils.GetDefaultValueInstance(GfsTypeRegistryEditor.GameAppType) as LayerBase;
                _playmodeLayers[0] = gameLayer;
                for (int i = _playmodeLayers.Count - 1; i >= 0; --i)
                {
                    var layerData = _playmodeLayers[i];
                    _manager.PushLayer(layerData.layer, layerData.priorityIndex);
                }

                // Deserialize all scenes that will be used in playmode.
                SceneManagerEditor.DeserializePlaymodeScene();

                if (prevSelectedActorId != Guid.Empty)
                {
                    Selector.Selected = SceneManager.FindActorByID(prevSelectedActorId);
                }
            }
        }

        internal void PlayModeOff()
        {
            var prevSelectedActorId = Guid.Empty;

            if (Selector.SelectedTransform())
            {
                prevSelectedActorId = Selector.SelectedTransform().Actor.GetID();
            }

            if (Application.IsInPlayMode)
            {
                _time.InitializeAsync();

                foreach (var layerData in _playmodeLayers)
                {
                    _manager.PopLayer(layerData.priorityIndex);
                }
                _playmodeLayers[0] = (null, _playmodeLayers[0].priorityIndex);
            }

            Application.IsInPlayMode = false;

            SceneManagerEditor.DeserializePlaymodeScene();
            if (prevSelectedActorId != Guid.Empty)
            {
                Selector.Selected = SceneManager.FindActorByID(prevSelectedActorId);
            }
        }
    }
}
