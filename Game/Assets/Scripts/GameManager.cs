using System;
using System.Linq;
using System.Reflection;
using Engine;
using Engine.Graphics;
using Engine.GUI;
using Engine.Utils;
using ldtk;
using GlmNet;

namespace Game
{
    public class GameConsts
    {
        public const string Default = "Default";
        public const string PLAYER = "Player";
        public const string FLOOR = "Floor";
        public const string ENEMY = "Enemy";
        public const string ENEMY_CONFUSED = "EnemyConfused";
        public const string PLATFORM = "Platform";
        public const string COLLECTIBLE = "Collectible";
        public const string CHARACTER_IGNORE = "character_Ignore";

        public const int ChestRenderSorting = 3;
        public static ulong GROUND_MASK { get; } = LayerMask.NameToBit(FLOOR) |
                                                   LayerMask.NameToBit(PLATFORM) |
                                                   LayerMask.NameToBit(Default);
    }

    internal class GameManager : ScriptBehavior
    {
        public static Camera Camera { get; private set; }
        private ItemsDatabase _itemsDatabase;
        private static FadeInOutManager _fadeInOutManager;
        public static Player Player { get; private set; }
        public static Material DefaultMaterial => MaterialUtils.SpriteMaterial;
        public static TilemapResult ForegroundTilemap { get; set; }
        public static FontAsset DefaultFont { get; private set; }
        private GameEntityManager _gameEntityManager;
        private static GameUIManager _gameUIManger;
        private static LevelBuilderManager _tilemapManager;

        //public static vec2 GameResolution { get; } = new vec2(640, 360);
        public static vec2 GameResolution { get; } = new vec2(512, 288);

        protected override void OnAwake()
        {
            InitializeActorLayers();
            InitializeData();
            InitializeWorld();
        }

        private void InitializeData()
        {
            _itemsDatabase = new ItemsDatabase("Data/ItemsDatabase.csv");
            DefaultFont = Assets.Get<FontAsset>("Fonts/windows-bold[1].ttf");
            _gameEntityManager = new GameEntityManager();
            _tilemapManager = new LevelBuilderManager(_gameEntityManager);
        }

        private void InitializeActorLayers()
        {
            static string[] GetConstStringValues(Type type)
            {
                return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                           .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                           .Select(f => (string)f.GetRawConstantValue())
                           .ToArray();
            }

            var names = GetConstStringValues(typeof(GameConsts));

            for (int i = 0; i < names.Length; i++)
            {
                LayerMask.AssignName(i, names[i]);
            }

            LayerMask.TurnOff(GameConsts.PLAYER, GameConsts.PLAYER);
            LayerMask.TurnOff(GameConsts.PLAYER, GameConsts.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameConsts.ENEMY_CONFUSED, GameConsts.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameConsts.PLATFORM, GameConsts.COLLECTIBLE);
            //LayerMask.TurnOn(GameLayers.PLAYER, GameLayers.Default);
        }

        private void InitializeWorld()
        {
            PostProcessingStack.Clear();
            ScreenGrabTest();
            PostProcessingStack.Push(new BloomPostProcessing());

            ScreenGrabTest3();
            if (!_fadeInOutManager)
            {
                _fadeInOutManager = new Actor("FadeInOutManager").AddComponent<FadeInOutManager>();
                Actor.DontDestroyOnLoad(_fadeInOutManager);
            }

            var manager = Actor.Find("Music Manager");

            if (!manager)
            {
                var music = new Actor<AudioSource>("Music Manager");
                var musicAudio = music.GetComponent<AudioSource>();
                musicAudio.Loop = true;
                musicAudio.Clip = Assets.GetAudioClip("Audio/music/streamloops/Stream Loops 2024-02-14_L01.wav");
                // musicAudio.Play();
                Actor.DontDestroyOnLoad(music);
            }

            if (!_gameUIManger)
            {
                _gameUIManger = new Actor("GameUIManager").AddComponent<GameUIManager>();
            }

            var result = _tilemapManager.BuildLevel(new LevelData()
            {
                LevelIndex = 0,
                TilemapPath = "Tilemap/WorldTilemap.ldtk",
                TilemapSprites = GameTextures.GetAtlas("sunny_land_tileset"),
                WorldSpacePixelsPerUnit = GameTextures.GetAtlas("sunny_land_tileset")[0].Texture.PixelPerUnit,
                Tilemaps =
                {
                    new TilemapData()
                    {
                        Name = "Foreground tilemap",
                        EnableCollision = true,
                        LayersToDraw = 1 << 2,
                        SortingOrder = 3,
                        SpriteIndex = 0
                    },
                    new TilemapData()
                    {
                        Name = "Background tilemap",
                        EnableCollision = false,
                        LayersToDraw = 1 << 3,
                        SortingOrder = -2,
                        SpriteIndex = 0
                    },
                }
            });
            Player = result.Player;
            ForegroundTilemap = result.Tilemaps[0];

            InitializeCamera(Player.Transform, ForegroundTilemap.Bounds);
            // Debug.Log(ItemsDatabase.GetDatabaseSchemaCsv());

            WaterTest();
            ParticleSystem();
        }

        private void InitializeCamera(Transform target, Bounds levelBounds)
        {
            if (!Camera)
            {
                Camera = new Actor<CameraFollow, CameraShake>("MainCamera").AddComponent<Camera>();
                Camera.Transform.WorldPosition = new vec3(0, 0, -12);
                Camera.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
                Camera.OrthographicSize = GameResolution.y / 2.0f / 16.0f;
                Camera.ProjectionMode = CameraProjectionMode.Orthographic;
                Camera.RenderTexture = new RenderTexture((int)GameResolution.x * 2, (int)GameResolution.y * 2);
            }

            var cameraFollow = Camera.GetComponent<CameraFollow>();
            cameraFollow.Target = target;
            cameraFollow.LevelBounds = levelBounds;
            cameraFollow.SetOnTargetImmediate();
        }


        protected override void OnUpdate()
        {
#if DEBUG
            Window.Name = EngineInfo.RendererInfoToString() + " | FPS: " + ((int)Time.FPS).ToString();
            // Debug.DrawBox(ForegroundTilemap.Bounds.Center, ForegroundTilemap.Bounds.Size, Color.Red);
#endif
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Physics2D.DrawColliders = !Physics2D.DrawColliders;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.DrawUILines = !Debug.DrawUILines;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F11))
            {
                Window.FullScreen(!Window.IsFullScreen);
                // Window.MouseVisible = !Window.IsFullScreen;
            }

            if (Input.GetKeyDown(KeyCode.Enter))
            {
                _gameUIManger.PauseMenu.OnPause();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                SceneManager.LoadScene("Game");
                PostProcessingStack.Clear();
                new Actor<GameManager>("GameManager");
            }

        }

        private void ParticleSystem()
        {
            var particleSystem = new Actor<ParticleSystem2D, Move>("ParticleSystem").GetComponent<ParticleSystem2D>();
            particleSystem.Transform.WorldPosition = new vec3(-34, -6);

            particleSystem.EmitRate = 152;
            particleSystem.ParticleLife = 3;
            particleSystem.SortOrder = 17;
            particleSystem.StartColor = Color.White;
            particleSystem.EndColor = Color.White;// new Color(0, 0, 0, 0);
            particleSystem.EndSize = new vec2(0, 0);
            particleSystem.Spread = new vec2(0.0f, 0);
            particleSystem.SimulationSpeed = 1;
            particleSystem.StartSize = new vec2(0.3f);
            particleSystem.IsWorldSpace = true;
            particleSystem.AngularVelocity = 40;
            var mainShader = new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text);

            var mat1 = new Material(mainShader);
            mat1.Name = "Particle material";
            particleSystem.Material = mat1;

            //var screenShader = new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/ScreenGrabWobble.frag").Text);
            //particleSystem.Material = new Material(screenShader);
            //particleSystem.Material.GetPass(0).IsScreenGrabPass = true;

            //particleSystem.Material.Passes.ElementAt(0).Blending.Enabled = false;
            particleSystem.Sprite = new Sprite();
        }

        private void WaterTest()
        {
            var waterActor = new Actor<SpriteRenderer>();
            var renderer = waterActor.GetComponent<SpriteRenderer>();
            renderer.SortOrder = 9;

            var mainShader = new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/WaterFrag.frag").Text);

            renderer.Material = new Material(mainShader);
            renderer.Material.Name = "Water Material";

            var pass = renderer.Material.GetPass(0);
            pass.Stencil.Enabled = true;
            pass.Stencil.Func = StencilFunc.Equal;
            pass.Stencil.Ref = 3;
            pass.Stencil.ZFailOp = StencilOp.Keep;
            renderer.Material.SetProperty("uWaterColor", new vec3(0.2f, 0.4f, 0.7f));
            renderer.Material.AddTexture("uParticles", Assets.GetTexture("particles.png"));

            var pass2 = renderer.Material.PushPass(mainShader);
            pass2.IsScreenGrabPass = true;
            pass2.Stencil.Enabled = true;
            pass2.Stencil.Func = StencilFunc.NotEqual;
            pass2.Stencil.Ref = 3;
            pass2.Stencil.ZFailOp = StencilOp.Keep;

            renderer.Material.SetProperty(1, "uWaterColor", new vec3(0.1f, 0.50f, 0.84f));

            waterActor.Transform.LocalScale = new vec3(10, 3, 1);
            waterActor.Transform.LocalPosition = new vec3(2.5f, -11, 1);
        }

        private void ScreenGrabTest()
        {
            var screenShader = new Shader(Assets.GetText("Shaders/ScreenVert.vert").Text, Assets.GetText("Shaders/CTRTv.frag").Text);
            PostProcessingStack.Insert(new PostProcessingSinglePass(screenShader), 0);
        }

        private void ScreenGrabTest3()
        {
            var vertex = Assets.GetText("Shaders/ScreenVert.vert").Text;
            var screenShader = new Shader(vertex, Assets.GetText("Shaders/Ripple.frag").Text);
            PostProcessingStack.Push(new PostProcessingSinglePass(screenShader));

            var chormaticAberration = new PostProcessingSinglePass(new Shader(vertex, Assets.GetText("Shaders/ChromaticAberration.frag").Text));
            chormaticAberration.SetValue("uAberrationStrength", 0.004f);

            PostProcessingStack.Push(chormaticAberration);

            var scalines = new PostProcessingSinglePass(new Shader(vertex, Assets.GetText("Shaders/ScanLines.frag").Text));
            scalines.SetValue("uScanlineIntensity", 0.2f);
            scalines.SetValue("uScanlineSpacing", 2);
            PostProcessingStack.Push(scalines);
        }
    }
}