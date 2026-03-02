using System;
using System.Linq;
using System.Reflection;
using Engine;
using Engine.Graphics;
using Engine.GUI;
using Engine.Utils;
using GlmNet;
using System.Collections;
using Engine;

namespace Game
{
    public class GameConsts
    {
        public const string Default = "Default";
        public const string PLAYER = "Player";
        public const string FLOOR = "Floor";
        public const string ENEMY = "Enemy";
        public const string CHARACTER_DEAD = "character_dead";
        public const string ENEMY_CONFUSED = "EnemyConfused"; // enemy that attacks other enemies.
        public const string PLATFORM = "Platform";
        public const string COLLECTIBLE = "Collectible";
        public const string Interactable = "Interactable";
        public const string CHARACTER_IGNORE = "character_Ignore";
        public const string NO_WALKABLE = "no_walkable";
        public const string BULLET = "bullet";

        public const int ChestRenderSorting = 3;
        public static ulong GROUND_MASK { get; } = LayerMask.NameToBit(FLOOR) |
                                                   LayerMask.NameToBit(PLATFORM) |
                                                   LayerMask.NameToBit(Default);

        public static ulong CHARACTER_MASK { get; } = LayerMask.NameToBit(ENEMY) |
                                                      LayerMask.NameToBit(PLAYER) |
                                                      LayerMask.NameToBit(ENEMY_CONFUSED);
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
            DefaultFont = Assets.GetFont("Fonts/windows-bold[1].ttf");
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

            //for (int i = 0; i < names.Length; i++)
            //{
            //    LayerMask.AssignName(i, names[i]);
            //}

            LayerMask.TurnOff(GameConsts.PLAYER, GameConsts.PLAYER);
            LayerMask.TurnOff(GameConsts.PLAYER, GameConsts.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameConsts.PLAYER, GameConsts.CHARACTER_DEAD);
            LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.PLAYER);
            LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.CHARACTER_DEAD);
            LayerMask.TurnOff(GameConsts.COLLECTIBLE, GameConsts.CHARACTER_DEAD);
            LayerMask.TurnOff(GameConsts.COLLECTIBLE, GameConsts.ENEMY);
            // LayerMask.TurnOff(GameConsts.ENEMY, GameConsts.ENEMY);
            LayerMask.TurnOff(GameConsts.ENEMY_CONFUSED, GameConsts.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameConsts.COLLECTIBLE, GameConsts.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameConsts.PLATFORM, GameConsts.COLLECTIBLE);
            LayerMask.TurnOffAll(GameConsts.BULLET);
            LayerMask.TurnOn(GameConsts.BULLET, GameConsts.COLLECTIBLE);

            //LayerMask.TurnOn(GameLayers.PLAYER, GameLayers.Default);
        }

        private void InitializeWorld()
        {
            PostProcessingStackInternal.Clear();
            ScreenGrabTest();
#if DESKTOP
            PostProcessingStackInternal.Push(new BloomPostProcessing());
            ScreenGrabTest3();
#endif
            _fadeInOutManager = new Actor("FadeInOutManager").AddComponent<FadeInOutManager>();
            _fadeInOutManager.Transform.Parent = Transform;
            _fadeInOutManager.FadeOut(1);



            //  IEnumerator next()
            {
                //  yield return null;
                // yield return null;

                var music = new Actor<AudioSource>("Music Manager");
                var musicAudio = music.GetComponent<AudioSource>();
                musicAudio.Loop = true;
                // musicAudio.Clip = Assets.GetAudioClip("Audio/music/streamloops/Stream Loops 2024-02-14_L01.wav");
                musicAudio.Clip = Assets.GetAudioClip("Audio/MinifantasyMusic/Goblins_Dance_(Battle).wav");
                musicAudio.Mixer = new AudioMixer("Music");
                musicAudio.Transform.Parent = Transform;
                //musicAudio.Play();

                _gameUIManger = new Actor("GameUIManager").AddComponent<GameUIManager>();
                _gameUIManger.Transform.Parent = Transform;

                // Begin from first level.
                BuildLevel(levelIndex: 0);
            }
            // StartCoroutine(next());
        }

        public void BuildLevel(int levelIndex, vec2 playerStartPos = default)
        {
            SceneManager.LoadScene("Level: " + levelIndex);
            // WaterTest();
            new Actor<TestScript>("Test Editor script");
            CreateCamera();

            var result = _tilemapManager.BuildLevel(new LevelInstantiateInfo()
            {
                LevelIndex = levelIndex,
                TilemapSprites = [GameTextures.GetAtlas("sunny_land_tileset")[0], GameTextures.GetAtlas("stark_full_tileset")[0]],
                WorldSpacePixelsPerUnit = GameTextures.GetAtlas("sunny_land_tileset")[0].Texture.PixelPerUnit,
                Tilemaps =
                {
                    new TilemapInstanceData()
                    {
                        Name = "Foreground tilemap",
                        EnableCollision = true,
                        LayerIndex = 4,
                        SortingOrder = 5,
                        SpriteIndex = 0
                    },
                    new TilemapInstanceData()
                    {
                        Name = "Background tilemap",
                        EnableCollision = false,
                        LayerIndex = 5,
                        SortingOrder = -2,
                        SpriteIndex = 0
                    },
                    new TilemapInstanceData()
                    {
                        Name = "Decoration tilemap",
                        EnableCollision = false,
                        LayerIndex = 1,
                        SortingOrder = 6,
                        SpriteIndex = 0
                    },
                    new TilemapInstanceData()
                    {
                        Name = "Deadly tilemap",
                        EnableCollision = true,
                        IsTriggerCollision = true,
                        LayerIndex = 0,
                        SortingOrder = 6,
                        SpriteIndex = 1,
                        ColliderOffset = new vec2(0, -0.8f),
                        TilemapAction = x =>
                        {
                            var damageTo = x.AddComponent<DamageTo>();

                            damageTo.DamageAmount = 2;
                            damageTo.Mask = GameConsts.CHARACTER_MASK;
                        }
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

            if (playerStartPos != default)
            {
                Player.Transform.WorldPosition = playerStartPos;
            }
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

        private void CreateCamera()
        {
            if (!Camera)
            {
                Camera = new Actor<CameraFollow, CameraShake>("MainCamera").AddComponent<Camera>();
                Camera.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
                Camera.OrthographicSize = GameResolution.y / 2.0f / 16.0f;
                Camera.ProjectionMode = CameraProjectionMode.Orthographic;
                Camera.RenderTexture = new RenderTexture((int)GameResolution.x * 2, (int)GameResolution.y * 2);
                Camera.RenderTexture.Name = "Game Camera RenderTexture";
                Camera.Transform.Parent = Transform;
            }

        }
        private void InitializeCamera(Transform target, Bounds levelBounds)
        {
            
            var cameraFollow = Camera.GetComponent<CameraFollow>();
            cameraFollow.Target = target;
            cameraFollow.LevelBounds = levelBounds;
            cameraFollow.SetOnTargetImmediate();
        }

        protected override void OnUpdate()
        {
#if DEBUG
            // Debug.DrawBox(ForegroundTilemap.Bounds.Center, ForegroundTilemap.Bounds.Size, Color.Red);
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    Physics2D.DrawColliders = !Physics2D.DrawColliders;
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    Debug.DrawUILines = !Debug.DrawUILines;
            //} 

            if (Input.GetKeyDown(KeyCode.T))
            {
                BuildLevel(0);
            }
#endif
            if (Input.GetKeyDown(KeyCode.Enter) || Input.Gamepad.Main.GetButtonState(GamePadButton.Start) == InputState.Down)
            {
                if (!_gameUIManger)
                {
                    Debug.Log("Is null");
                }
                _gameUIManger.PauseMenu.OnPause();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F11))
            {
                Screen.IsFullScreen = !Screen.IsFullScreen;
                WindowManager.Window.CursorVisible = !WindowManager.Window.IsFullScreen;
            }
        }

        private void WaterTest()
        {
            var waterActor = new Actor<SpriteRenderer>();
            var renderer = waterActor.GetComponent<SpriteRenderer>();
            renderer.SortOrder = 4;

            var mainShader = Shader.FromPath("Shaders/SpriteVert.vert", "Shaders/WaterFrag.frag");

            renderer.Material = new Material(mainShader);
            renderer.Material.Name = "Water Material";

            var pass = renderer.Material.GetPass(0);
            pass.Stencil.Enabled = true;
            pass.Stencil.Func = StencilFunc.Equal;
            pass.Stencil.Ref = 3;
            pass.Stencil.ZFailOp = StencilOp.Keep;
            renderer.Material.SetProperty(0, "uWaveAmplitude", 0.08f);
            renderer.Material.SetProperty(0, "uWaveFrequency", 8.0f);
            renderer.Material.SetProperty(0, "uWaveSpeed", 3.0f);
            renderer.Material.SetProperty(0, "uNoiseScale", 15.0f);
            renderer.Material.SetProperty(0, "uNoiseStrength", 0.15f);
            renderer.Material.SetProperty(0, "uWaterColor", new vec3(0.2f, 0.4f, 0.7f));
            renderer.Material.SetProperty(0, "uOutlineHeight", 0.85f);
            renderer.Material.SetProperty(0, "uOutlineColor", new vec3(1.0f, 1.0f, 1.0f));
            renderer.Material.SetProperty(0, "uOutlineThickness", 0.04f);


            /*uniform vec3  uTime;                     
uniform float uWaveAmplitude    = 0.08f    
uniform float uWaveFrequency    = 8.0 f    
uniform float uWaveSpeed        = 3.0f     
uniform float uNoiseScale       = 15.0f    
uniform float uNoiseStrength    = 0.15f    
uniform vec3  uWaterColor       = vec3(0.2f, 0.2f, 1.0f)
uniform float uOutlineHeight    = 0.85f     
uniform vec3  uOutlineColor     = vec3(1.0f, 1.0f, 1.0f)
uniform float uOutlineThickness = 0.04f  
            */

            renderer.Material.AddTexture("uParticles", Assets.GetTexture("particles.png"));

            var pass2 = renderer.Material.PushPass(mainShader);
            pass2.IsScreenGrabPass = true;
            pass2.Stencil.Enabled = true;
            pass2.Stencil.Func = StencilFunc.NotEqual;
            pass2.Stencil.Ref = 3;
            pass2.Stencil.ZFailOp = StencilOp.Keep;

            renderer.Material.SetProperty(1, "uWaveAmplitude", 0.08f);
            renderer.Material.SetProperty(1, "uWaveFrequency", 8.0f);
            renderer.Material.SetProperty(1, "uWaveSpeed", 3.0f);
            renderer.Material.SetProperty(1, "uNoiseScale", 15.0f);
            renderer.Material.SetProperty(1, "uNoiseStrength", 0.15f);
            renderer.Material.SetProperty(1, "uWaterColor", new vec3(0.1f, 0.50f, 0.84f));
            renderer.Material.SetProperty(1, "uOutlineHeight", 0.85f);
            renderer.Material.SetProperty(1, "uOutlineColor", new vec3(1.0f, 1.0f, 1.0f));
            renderer.Material.SetProperty(1, "uOutlineThickness", 0.04f);

            waterActor.Transform.LocalScale = new vec3(10, 3, 1);
            waterActor.Transform.LocalPosition = new vec3(2.5f, -11, 1);
        }

        private void ScreenGrabTest()
        {
            var screenShader = Shader.FromPath("Shaders/ScreenVert.vert", "Shaders/CTRTv_Cheap.frag");
            var pass = new PostProcessingSinglePass(screenShader);

            pass.SetValue("uBackgroundColor", new vec3(0.07f));
            pass.SetValue("uDistortionStrength", 0);
            pass.SetValue("uCornerTL", 0);
            pass.SetValue("uCornerTR", 0);
            pass.SetValue("uCornerBL", 0);
            pass.SetValue("uCornerBR", 0);
            pass.SetValue("uEdgeSoftness", 0.000f);
            pass.SetValue("uScanlineIntensity", 0.4f);
            pass.SetValue("uScanlineSpacing", 4.0f);
            pass.SetValue("uPhosphorGlow", 0.05f);
            pass.SetValue("uRGBOffset", 0.0f);
            pass.SetValue("uBrightness", 1.52f);
            pass.SetValue("uContrast", 1.05f);
            pass.SetValue("uRGBBalance", new vec3(1.0f, 0.8f, 0.84f));
            pass.SetValue("uGlassReflectStrength", 0.0f);
            pass.SetValue("uAberrationStrength", 0.00f);
            pass.SetValue("uMaskStrength", 0.01f);
            pass.SetValue("uMaskScale", 1.0f);
            pass.SetValue("uNoiseStrength", 0.0f);
            pass.SetValue("uVignetteStrength", 0.01f);
            pass.SetValue("uJitterStrength", 0.5f);

            PostProcessingStackInternal.Insert(pass, 0);
        }

        private void ScreenGrabTest3()
        {
            var vertex = Assets.GetText("Shaders/ScreenVert.vert").Text;
            //var screenShader = new Shader(vertex, Assets.GetText("Shaders/Ripple.frag").Text);
            //PostProcessingStack.Push(new PostProcessingSinglePass(screenShader));

            var chormaticAberration = new PostProcessingSinglePass(new Shader(vertex, Assets.GetText("Shaders/ChromaticAberration.frag").Text));
            chormaticAberration.SetValue("uAberrationStrength", 0.004f);

            PostProcessingStackInternal.Push(chormaticAberration);

            var scanlines = new PostProcessingSinglePass(new Shader(vertex, Assets.GetText("Shaders/ScanLines.frag").Text));
            scanlines.SetValue("uScanlineIntensity", 0.2f);
            scanlines.SetValue("uScanlineSpacing", 2);
            PostProcessingStackInternal.Push(scanlines);
        }
    }
}