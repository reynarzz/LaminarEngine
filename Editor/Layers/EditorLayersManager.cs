using Editor.Layers;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Serialization;
using Engine.Utils;
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

        private PlaymodeController _playmodeController; // Remove from here
        private static HotReloadLayer _hotReload;
        public EditorLayersManager(InputLayerBase inputLayer, WindowStandalone win) :
            base([_time,                           
                  inputLayer,                      
                  null, // AppLayer                
                  new MainThreadDispatcher(),      
                  null, // SceneLayer              
                  new AudioLayer(),                
                  null, // PhysicsLayer,           
                  new ImGuiLayer(win, inputLayer),
                  new RenderingLayer(),            
                  new EditorIOLayer(),
                  _hotReload = new HotReloadLayer()])                     
        {
            _editorSceneLayer = new SceneLayer();
            _playmodeController = new PlaymodeController(this, _time, _hotReload);

        }

        internal override void Initialize()
        {
            base.Initialize();
        }
        private Shader _test;
        internal override void Update()
        {
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
                    _actors = SceneSerializer.SerializeScene(SceneManager.Scenes[^1])?.ActorsData;

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
                var clip = Assets.Get<AnimationClip>("Animation/AnimClip.anim");
                var clipController = Assets.Get<AnimatorController>("Animation/AnimController.animctrl");

                _test = Assets.GetShader("Shaders/Test/ShaderTest.shader");


                LoadScene();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                LoadScene();
            }
            base.Update();
        }

        private void LoadScene()
        {
            var file = File.ReadAllText(TestfilePath);
            var actors = EditorJsonUtils.Deserialize<List<ActorDataSceneAsset>>(file);
            // var actors = _actors;
            Debug.Log("Total actors in scene: " + actors.Count);
            SceneManager.Initialize();
            SceneDeserializer.DeserializeScene(actors, SceneManager.ActiveScene);
        }
    }
}