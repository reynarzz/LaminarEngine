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
        private JsonSerializerSettings _jsonSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Converters = 
            {
                new StringEnumConverter(),
            },
            ContractResolver = new SerializedFieldContractResolver()
        };

        internal override void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _time.Initialize();
                Application.IsInPlayMode = true;

                PushLayer(new PhysicsLayer(), 6);
                PushLayer(new SceneLayer(), 4);
                PushLayer(new GameApplication(), 2);

            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                PopLayer(2);
                PopLayer(4);
                PopLayer(6);

                Application.IsInPlayMode = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Physics2D.DrawColliders = !Physics2D.DrawColliders;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.DrawUILines = !Debug.DrawUILines;
            }
            
            string TestfilePath = $"{EditorPaths.AppRoot}Scene.bin";

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Saving scene to: " + TestfilePath);
                _actors = SceneSerializer.SerializeScene(SceneManager.Scenes[^1]);

                File.WriteAllText(TestfilePath, JsonConvert.SerializeObject(_actors, Formatting.Indented, _jsonSettings));
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
            base.Update();
        }

        private void LoadScene()
        {

        }
    }
}
