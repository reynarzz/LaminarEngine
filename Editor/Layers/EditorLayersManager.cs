using Editor.Serialization;
using Engine;
using Engine.Layers;
using Engine.Layers.Input;
using Engine.Serialization;
using Engine.Utils;
using Game;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        private List<ActorDataSceneAsset> _actors;
        private string TestfilePath { get; } = $"{EditorPaths.AppRoot}Scene.bin";

        private readonly List<(LayerBase layer, int priorityIndex)> _playmodeLayers;
        private JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new StringEnumConverter(),
            },
            ContractResolver = new SerializedFieldContractResolver()
        };

        public EditorLayersManager(InputLayerBase inputLayer) :
            base([_time,                           // 0
                  inputLayer,                      // 1
                  null, // AppLayer                // 2
                  new MainThreadDispatcher(),      // 3
                  null, // SceneLayer              // 4
                  new AudioLayer(),                // 5
                  null, // PhysicsLayer,           // 6
                  new RenderingLayer(),            // 7
                  new IOLayer()])                  // 8
        {
            _editorSceneLayer = new SceneLayer();

            _playmodeLayers = new List<(LayerBase layer, int priorityIndex)>()
            {
                //(new GameApplication(), 2),
                (new SceneLayer(), 4),
                (new PhysicsLayer(), 6),
            };

        }

        internal override void Initialize()
        {
            base.Initialize();
        }
        
        internal override void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PlayModeOn();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                PlayModeOff();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Physics2D.DrawColliders = !Physics2D.DrawColliders;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.DrawUILines = !Debug.DrawUILines;
            }


            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                if (!Application.IsInPlayMode)
                {
                    Debug.Log("Saving scene to: " + TestfilePath);
                    _actors = SceneSerializer.SerializeScene(SceneManager.Scenes[^1]);

                    File.WriteAllText(TestfilePath, JsonConvert.SerializeObject(_actors, Formatting.Indented, _jsonSettings));
                }
                else
                {
                    Debug.Warn("Can't save in playmode.");
                }
              
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                // Application.IsInPlayMode = false;

                var file = File.ReadAllText(TestfilePath);
                var actors = JsonConvert.DeserializeObject<List<ActorDataSceneAsset>>(file, _jsonSettings);
                // var actors = _actors;
                Debug.Log("Total actors in scene: " + actors.Count);
                SceneManager.Initialize();
                SceneManager.UnloadAll();
                SceneManager.LoadScene("Reload scene");
                SceneDeserializer.DeserializeScene(actors, SceneManager.ActiveScene);
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                if (Application.IsInPlayMode)
                {
                    PlayModeOff();
                }
                else
                {
                    PlayModeOn();
                }

                LoadScene();
            }
            base.Update();
        }

        private void PlayModeOn()
        {
            _time.Initialize();
            Application.IsInPlayMode = true;

            for (int i = _playmodeLayers.Count - 1; i >= 0; --i)
            {
                var layerData = _playmodeLayers[i];
                PushLayer(layerData.layer, layerData.priorityIndex);
            }
        }

        private void PlayModeOff()
        {
            foreach (var layerData in _playmodeLayers)
            {
                PopLayer(layerData.priorityIndex);
            }

            Application.IsInPlayMode = false;
        }
        private void LoadScene()
        {
            var file = File.ReadAllText(TestfilePath);
            var actors = JsonConvert.DeserializeObject<List<ActorDataSceneAsset>>(file, _jsonSettings);
            // var actors = _actors;
            Debug.Log("Total actors in scene: " + actors.Count);
            SceneManager.Initialize();
            //SceneManager.UnloadAll();
            //SceneManager.LoadScene("Reload scene");
            SceneDeserializer.DeserializeScene(actors, SceneManager.ActiveScene);
        }
    }
}
