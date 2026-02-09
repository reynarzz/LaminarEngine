using Editor.Build;
using Editor.Layers;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Serialization;
using Engine.Utils;
using Editor.Cooker;
using Engine;
using System.Reflection;

namespace Editor
{
    internal class EditorLayersManager : LayersManager
    {
        private string TestfilePath => $"{EditorPaths.AppRoot}Scene.bin";
        private string AnimClipPath => $"{EditorPaths.AppRoot}AnimClip.bin";
        private string AnimControllerPath => $"{EditorPaths.AppRoot}AnimController.bin";
        private string MaterialPath => $"{EditorPaths.AppRoot}Material.bin";

        private PlaymodeController _playmodeController; // Remove from here

        private readonly LayerBase[] _initializationLayers;
        public EditorLayersManager(InputLayerBase inputLayer, WindowStandalone win)
        {
            var timeLayer = new TimeLayer();
            var mainThreadDispLayer = new MainThreadDispatcher();
            var audioLayer = new AudioLayer();
            var imguiLayer = new ImGuiLayer(win, inputLayer, this);
            var renderingLayer = new RenderingLayer();
            var editorLayer = new EditorIOLayer();
            var hotReloadLayer = new HotReloadLayer();

            Layers =
            [
                timeLayer,
                inputLayer,
                null, // App Layer
                mainThreadDispLayer,
                null, // Scene layer
                audioLayer,
                null, // Physics layer.
                editorLayer,
                hotReloadLayer,
                imguiLayer,
                renderingLayer,
            ];

            _initializationLayers =
            [
                renderingLayer
            ];

            _playmodeController = new PlaymodeController(this, timeLayer, hotReloadLayer);
        }

        internal override void InitializeAsync()
        {
            if (LayersInitialized)
            {
                Debug.EngineError("Layers are already initialized");
                return;
            }

            async Task InitializeLayers()
            {
                for (int i = Layers.Length - 1; i >= 0; i--)
                {
                    var layer = Layers[i];

                    if (layer != null)
                    {
                        await layer.InitializeAsync();
                        Debug.Log("Initialized layer: " + layer.GetType().Name);
                    }
                }

                Debug.Log("Layers fully initialized");
                LayersInitialized = true;
            }

            Task.Run(InitializeLayers);
        }

        private Shader _test;
        private Material _materialTest;
        internal override void Update()
        {
            base.Update();

            if (!LayersInitialized)
            {
                // Updates layers needed to present initialization info to the user.
                foreach (var layer in _initializationLayers)
                {
                    if (layer.IsInitialized)
                    {
                        layer.UpdateLayer();
                    }
                }
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
                if (Application.IsInPlayMode || SceneManager.Scenes.FirstOrDefault() != null)
                {

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

                    Debug.Log("Saving scene to: " + TestfilePath);
                    var sceneIR = SceneSerializer.SerializeScene(SceneManager.Scenes[^1]);

                    var fil = File.Open(EditorPaths.AppRoot + "/SceneBinary.bin", FileMode.Create, FileAccess.Write);
                    using var bw = new BinaryWriter(fil, System.Text.Encoding.UTF8);
                    BinaryIRSerializer.Serialize(sceneIR, bw);

                    File.WriteAllText(TestfilePath, EditorJsonUtils.Serialize(sceneIR));

                }

                // InternalAssetsCreator.GenerateAll();
                //else
                //{
                //    Debug.Warn("Can't save in playmode.");
                //}

            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
            {
                if (File.Exists(AnimClipPath))
                {
                    var anim = new AnimationClip("Name");
                    var ir = EditorJsonUtils.Deserialize<List<SerializedPropertyIR>>(File.ReadAllText(AnimClipPath));

                    Deserializer.Deserialize(anim, ir);
                    var anim2 = Deserializer.Deserialize<AnimationClip>(ir);
                }

                _materialTest = Assets.GetMaterial("Materials/Material.material");
                Selector.Selected = Assets.GetTexture("starkTileset.png");//_materialTest;


                //GenerateIconAndroidSizes(Assets.GetTexture("Icons/playerhead.png"), 
                //                         Assets.GetTexture("Icons/playerhead_foreground.png"), 
                //                         Assets.GetTexture("Icons/playerhead_background.png"));


                //ExportSlicedSprites();
                // var material = Assets.GetMaterial("Materials/Portal_mobile.material");
                // var json = EditorJsonUtils.Serialize(Serializer.Serialize(material));

                //File.WriteAllText(EditorPaths.GameRoot + "/Assets/Materials/Portal_mobile.material", json);

                // material = Assets.GetMaterial("Materials/Material.material");
                //Selector.Selected = material;
                var obj = Actor.Find("Chest");
                if (obj)
                {
                    var value = obj.GetComponent<SpriteRenderer>();
                    Selector.Selected = value.Sprite.Texture;
                }
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.W))
            {

                //var a = new TypeRegistry();
                //if (a.GetType(Guid.Parse("155b8de8a92b2bb630a8026d46704c61"), out Type type))
                //{
                //    if (type != null)
                //    {
                //        Debug.Log($"Success: {type.FullName}");
                //    }
                //}
                Cooker.Generator.TypeGenerationStage.GenerateTypeRegistry();
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                //var clip = Assets.Get<AnimationClip>("Animation/AnimClip.anim");
                //var clipController = Assets.Get<AnimatorController>("Animation/AnimController.animctrl");

                //_test = Assets.GetShader("Shaders/Test/ShaderTest.shader");


                // SerializerTypesFixer();
                // LoadScene();

                //if (File.Exists(TestfilePath))
                //{
                //    var sceneFile = File.ReadAllText(TestfilePath);
                //    var sceneFileIR = EditorJsonUtils.Deserialize<SceneIR>(sceneFile);
                //    LoadScene(sceneFileIR);
                //}

                var file = File.Open(EditorPaths.AppRoot + "/SceneBinary.bin", FileMode.Open, FileAccess.Read);
                var reader = new BinaryReader(file);

                var scene = BinaryIRDeserializer.DeserializeScene(reader);
                LoadScene(scene);
            }
        }

        private void SerializerTypesFixer()
        {
            var files = new List<string>();
            var shaders = Directory.GetFiles(EditorPaths.AppRoot, "*.shader", SearchOption.AllDirectories);
            var materials = Directory.GetFiles(EditorPaths.AppRoot, "*.material", SearchOption.AllDirectories);
            var metas = Directory.GetFiles(EditorPaths.AppRoot, "*.mt", SearchOption.AllDirectories);
            var data = Directory.GetFiles(EditorPaths.AppRoot, "*.dat", SearchOption.AllDirectories);
            files.AddRange(shaders);
            files.AddRange(materials);
            files.AddRange(metas);
            files.AddRange(data);
            string shared = "Engine";

            foreach (var file in files)
            {
                Console.WriteLine(file);
                var txt = File.ReadAllText(file);
                txt = txt.Replace("Engine.DictionaryData`2[[System.Object, System.Private.CoreLib],[System.Object, System.Private.CoreLib]], Engine",
                    "Engine.DictionaryData, Engine");

                File.WriteAllText(file, txt);
            }
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

        private void LoadScene(SceneIR scene)
        {

            if (scene != null)
            {

                // var actors = _actors;
                Debug.Log("Total actors in scene: " + scene.Actors.Count);
                SceneManager.Initialize();

                SceneDeserializer.DeserializeScene(scene, SceneManager.ActiveScene);
            }
        }
    }
}