using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Editor
{
    internal class EditorLayersManager : LayersManager
    {
        private static TimeLayer _time = new();
        private SceneLayer _editorSceneLayer;
        private List<ActorDataSceneAsset> _actors;
        private string TestfilePath => $"{EditorPaths.AppRoot}Scene.bin";
        private string AnimClipPath => $"{EditorPaths.AppRoot}AnimClip.bin";
        private string AnimControllerPath => $"{EditorPaths.AppRoot}AnimController.bin";


        private readonly List<(LayerBase layer, int priorityIndex)> _playmodeLayers;

        public EditorLayersManager(InputLayerBase inputLayer) :
            base([_time,                           // 0
                  inputLayer,                      // 1
                  null, // AppLayer                // 2
                  new MainThreadDispatcher(),      // 3
                  null, // SceneLayer              // 4
                  new AudioLayer(),                // 5
                  null, // PhysicsLayer,           // 6
                  new RenderingLayer(),            // 7
                  new EditorIOLayer()])            // 8
        {
            _editorSceneLayer = new SceneLayer();

            _playmodeLayers = new List<(LayerBase layer, int priorityIndex)>()
            {
                (new Game.GameApplication(), 2),
                (new SceneLayer(), 4),
                (new PhysicsLayer(), 6),
            };
        }

        internal override void Initialize()
        {
            base.Initialize();
        }
        private Shader _test;
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
                //if (!Application.IsInPlayMode)
                {
                    Debug.Log("Saving scene to: " + TestfilePath);
                    _actors = SceneSerializer.SerializeScene(SceneManager.Scenes[^1]);

                    var obj = Actor.Find("Player");
                    if (obj)
                    {
                        var animator = obj.GetComponent<Animator>();
                        var animIR = Serializer.Serialize(animator.CurrentState.Clip);
                        var animControlerIR = Serializer.Serialize(animator.Controller);

                        File.WriteAllText(AnimClipPath, EditorJsonUtils.Serialize(animIR));
                        File.WriteAllText(AnimControllerPath, EditorJsonUtils.Serialize(animControlerIR));
                    }

                    File.WriteAllText(TestfilePath, EditorJsonUtils.Serialize(_actors));
                }
                //else
                //{
                //    Debug.Warn("Can't save in playmode.");
                //}

            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
            {
                var anim = new AnimationClip("Name");
                var ir = EditorJsonUtils.Deserialize<List<SerializedPropertyData>>(File.ReadAllText(AnimClipPath));

                Deserializer.Deserialize(anim, ir);
                var anim2 = Deserializer.Deserialize<AnimationClip>(ir);

            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                // Application.IsInPlayMode = false;

                var file = File.ReadAllText(TestfilePath);
                var actors = EditorJsonUtils.Deserialize<List<ActorDataSceneAsset>>(file);

                var clip = Assets.Get<AnimationClip>("Animation/AnimClip.anim");
                var clipController = Assets.Get<AnimatorController>("Animation/AnimController.animcontroller");

                _test = Assets.GetShader("Shaders/Test/ShaderTest.shader");
                

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
            var actors = EditorJsonUtils.Deserialize<List<ActorDataSceneAsset>>(file);
            // var actors = _actors;
            Debug.Log("Total actors in scene: " + actors.Count);
            SceneManager.Initialize();
            //SceneManager.UnloadAll();
            //SceneManager.LoadScene("Reload scene");
            SceneDeserializer.DeserializeScene(actors, SceneManager.ActiveScene);
        }
    }
}
