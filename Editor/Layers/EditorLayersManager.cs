using Editor.Build;
using Editor.Layers;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Serialization;
using Engine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SharedTypes;

namespace Editor
{
    internal class EditorLayersManager : LayersManager
    {
        private static TimeLayer _time = new();
        private List<ActorDataSceneAsset> _actors;
        private string TestfilePath => $"{EditorPaths.AppRoot}Scene.bin";
        private string AnimClipPath => $"{EditorPaths.AppRoot}AnimClip.bin";
        private string AnimControllerPath => $"{EditorPaths.AppRoot}AnimController.bin";
        private string MaterialPath => $"{EditorPaths.AppRoot}Material.bin";

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
                  _hotReload = new HotReloadLayer()
                  ])
        {
            _playmodeController = new PlaymodeController(this, _time, _hotReload);

        }

        internal override Task InitializeAsync()
        {
            return base.InitializeAsync();
        }

        private Shader _test;
        private Material _materialTest;
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
                        var renderer = Actor.Find("Portal").Transform.Children[1].GetComponent<SpriteRenderer>();
                        var materialIR = Serializer.Serialize(renderer.Material);

                        var material = Assets.GetMaterial("Materials/Material.material");

                        File.WriteAllText(AnimClipPath, EditorJsonUtils.Serialize(animIR));
                        File.WriteAllText(AnimControllerPath, EditorJsonUtils.Serialize(animControlerIR));
                        File.WriteAllText(MaterialPath, EditorJsonUtils.Serialize(materialIR));



                    }

                    File.WriteAllText(TestfilePath, EditorJsonUtils.Serialize(_actors));
                    _actors.Clear();
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

                _materialTest = Assets.GetMaterial("Materials/Material.material");
                Selector.Selected = Assets.GetTexture("starkTileset.png");//_materialTest;


                //GenerateIconAndroidSizes(Assets.GetTexture("Icons/playerhead.png"), 
                //                         Assets.GetTexture("Icons/playerhead_foreground.png"), 
                //                         Assets.GetTexture("Icons/playerhead_background.png"));


                //ExportSlicedSprites();
                //var material = Assets.GetMaterial("__InternalAssets__/Materials/SpriteDefault.material");
                // material = Assets.GetMaterial("Materials/Material.material");
                //Selector.Selected = material;
                var obj = Actor.Find("Chest");
                if (obj)
                {
                    var value = obj.GetComponent<SpriteRenderer>();
                    Selector.Selected = value.Sprite.Texture;
                }
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                //var clip = Assets.Get<AnimationClip>("Animation/AnimClip.anim");
                //var clipController = Assets.Get<AnimatorController>("Animation/AnimController.animctrl");

                //_test = Assets.GetShader("Shaders/Test/ShaderTest.shader");


                LoadScene();
            }

            base.Update();
        }

        private void GenerateIconAndroidSizes(Texture defaultTexture, Texture foreground, Texture background)
        {
            var resolutions = Enum.GetValues<AndroidIconSizes>();
            var dir = Path.Combine(EditorPaths.AppRoot, "Platforms/Android/Resources");

            var channels = 4;

            void AddIcons(string name, Texture texture)
            {
                if (!texture)
                {
                    Debug.Error("Icon is null, can't convert: " + name);
                    return;
                }

                for (int i = 0; i < resolutions.Length; i++)
                {
                    var resEnum = resolutions[i];

                    int dim = (int)resEnum;
                    var outBytes = new byte[dim * dim * channels];

                    EditorImage.Resize(texture.Data, channels, texture.Width, texture.Height, outBytes, dim, dim);

                    var format = EditorImageWriteFormat.Png;
                    var iconPath = Path.Combine(dir, $"mipmap-{resEnum}", $"{name}.{format.ToString().ToLower()}");
                    EditorImage.Write(iconPath, dim, dim, channels, outBytes, format);
                }
            }

            AddIcons("appicon", defaultTexture);
            AddIcons("appicon_foreground", foreground);
            AddIcons("appicon_background", background);

        }

        private void ExportSlicedSprites()
        {
            void Slice(Texture2D texture, int sliceX, int sliceY, float pivotX, float pivotY)
            {
                var meta = EditorAssetUtils.GetAssetMeta(texture) as TextureMetaFile;
                meta.Config.IsAtlas = true;
                TextureAtlasUtils.SliceTiles(meta.AtlasData, sliceX, sliceY, texture.Width, texture.Height, pivotX, pivotY);
                AssetUtils.WriteMeta(texture.Path, meta);
            }

            var paths = new string[]
            {
                "KingsAndPigsSprites/01-King Human/Idle (78x58)S.png",
                "KingsAndPigsSprites/01-King Human/Run (78x58)S.png",
                "KingsAndPigsSprites/01-King Human/Jump (78x58)S.png",
                "KingsAndPigsSprites/01-King Human/Fall (78x58)S.png",
                "KingsAndPigsSprites/01-King Human/Hit (78x58).png",
                "KingsAndPigsSprites/01-King Human/Dead (78x58).png",
                "KingsAndPigsSprites/01-King Human/Attack (78x58).png",
                "KingsAndPigsSprites/01-King Human/Door In (78x58).png",
                "KingsAndPigsSprites/01-King Human/Door Out (78x58).png"





            };

            foreach (var path in paths)
            {
                Slice(Assets.GetTexture(path), 78, 58, 0.4f, 0.42f);
            }
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