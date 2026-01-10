using Editor.Serialization;
using Engine;
using Engine.Layers;
using Engine.Layers.Input;
using Engine.Utils;
using Game;
using Newtonsoft.Json;
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

            string TestfilePath = "D:/Scene.txt";

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Save");
                _actors = SceneSerializer.SerializeScene(SceneManager.Scenes[^1]);
                var settings = new JsonSerializerSettings()
                {
                    Converters =
                    {
                        new GFSObjectReferenceConverter(),
                        new StringEnumConverter<SerializedType>(),
                        new StringEnumConverter<ReflectionUtils.CollectionType>()
                    },
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                };

                File.WriteAllText(TestfilePath, JsonConvert.SerializeObject(_actors, Formatting.Indented, settings));
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                Application.IsInPlayMode = false;

                //var file = File.ReadAllText(TestfilePath);
                //var settings = new JsonSerializerSettings();
                //settings.Converters.Add(new GFSObjectReferenceConverter());

                var actors = _actors;// JsonConvert.DeserializeObject<List<ActorDataSceneAsset>>(file, settings);
                Debug.Log("Total actors in scene: " + actors.Count);
                SceneManager.Initialize();
                SceneManager.UnloadAll();
                SceneManager.LoadScene("Reload scene");
                SceneDeserializer.DeserializeScene(actors, SceneManager.ActiveScene);
            }
            base.Update();
        }
    }
}
