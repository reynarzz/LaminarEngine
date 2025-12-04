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
        public const string CHARACTER_DEAD = "character_dead";
        public const string ENEMY_CONFUSED = "EnemyConfused";
        public const string PLATFORM = "Platform";
        public const string COLLECTIBLE = "Collectible";
        public const string CHARACTER_IGNORE = "character_Ignore";
        public const string NO_WALKABLE = "no_walkable";

        public const int ChestRenderSorting = 3;
        public static ulong GROUND_MASK { get; } = LayerMask.NameToBit(FLOOR) |
                                                   LayerMask.NameToBit(PLATFORM) |
                                                   LayerMask.NameToBit(Default);
    }

    internal class GameManager : ScriptBehavior
    {
        public Camera Camera { get; private set; }
        private ItemsDatabase _itemsDatabase;
        private FadeInOutManager _fadeInOutManager;
        public Player Player { get; private set; }
        private TilemapResult ForegroundTilemap { get; set; }
        public static FontAsset DefaultFont { get; private set; }
        private GameEntityManager _gameEntityManager;
        private GameUIManager _gameUIManger;
        private TilemapWorldBuilderManager _tilemapManager;
        public static GameManager Instance { get; private set; }
        //public static vec2 GameResolution { get; } = new vec2(640, 360);
        public static vec2 GameResolution { get; } = new vec2(512, 288);

        protected override void OnAwake()
        {
            Instance = this;
            Actor.DontDestroyOnLoad(this);

            InitializeActorLayers();
            InitializeData();
            InitializeWorld();

        }

        private void InitializeData()
        {
            _itemsDatabase = new ItemsDatabase("Data/ItemsDatabase.csv");
            DefaultFont = Assets.Get<FontAsset>("Fonts/windows-bold[1].ttf");
            _gameEntityManager = new GameEntityManager();
            _tilemapManager = new TilemapWorldBuilderManager("Tilemap/WorldTilemap.ldtk", _gameEntityManager);
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
            LayerMask.TurnOff(GameConsts.PLAYER, GameConsts.CHARACTER_DEAD);
            LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.PLAYER);
            LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.CHARACTER_DEAD);
            // LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.ENEMY);
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
            _fadeInOutManager = new Actor("FadeInOutManager").AddComponent<FadeInOutManager>();
            _fadeInOutManager.Transform.Parent = Transform;

            var music = new Actor<AudioSource>("Music Manager");
            var musicAudio = music.GetComponent<AudioSource>();
            musicAudio.Loop = true;
            musicAudio.Clip = Assets.GetAudioClip("Audio/music/streamloops/Stream Loops 2024-02-14_L01.wav");
            musicAudio.Transform.Parent = Transform;
            musicAudio.Play();

            _gameUIManger = new Actor("GameUIManager").AddComponent<GameUIManager>();
            _gameUIManger.Transform.Parent = Transform;

            // Begin from first level.
            BuildLevel(levelIndex: 0);
        }

        public void BuildLevel(int levelIndex)
        {
            SceneManager.LoadScene("Level: " + levelIndex);
            WaterTest();

            var result = _tilemapManager.BuildLevel(new LevelInstantiateInfo()
            {
                LevelIndex = levelIndex,
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

            if (Player != result.Player)
            {
                Player = result.Player;
                Player.OnCharacterDead += OnPlayerDead;
            }
            else
            {
                Player.InitLevel();
            }

            ForegroundTilemap = result.Tilemaps[0];
            InitializeCamera(Player.Transform, ForegroundTilemap.Bounds);
        }

        private void OnPlayerDead()
        {
#if RELEASE
            TimedExecute(() => 
            {
                FadeInOutManager.Instance.FadeIn(1, () =>
                {
                    BuildLevel(0);
                    FadeInOutManager.Instance.FadeOut(1);
                    Player.Restart();
                });
            }, 2.3f);
#endif
        }

        private void InitializeCamera(Transform target, Bounds levelBounds)
        {
            if (!Camera)
            {
                Camera = new Actor<CameraFollow, CameraShake>("MainCamera").AddComponent<Camera>();
                Camera.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
                Camera.OrthographicSize = GameResolution.y / 2.0f / 16.0f;
                Camera.ProjectionMode = CameraProjectionMode.Orthographic;
                Camera.RenderTexture = new RenderTexture((int)GameResolution.x * 2, (int)GameResolution.y * 2);
                Camera.Transform.Parent = Transform;
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
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Physics2D.DrawColliders = !Physics2D.DrawColliders;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.DrawUILines = !Debug.DrawUILines;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                BuildLevel(0);
            }
#endif
            if (Input.GetKeyDown(KeyCode.Enter))
            {
                _gameUIManger.PauseMenu.OnPause();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F11))
            {
                Window.FullScreen(!Window.IsFullScreen);
                // Window.MouseVisible = !Window.IsFullScreen;
            }
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
            //var screenShader = new Shader(vertex, Assets.GetText("Shaders/Ripple.frag").Text);
            //PostProcessingStack.Push(new PostProcessingSinglePass(screenShader));

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