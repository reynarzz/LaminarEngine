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
        private vec3 _playerStartPosTest;
        private static UIText _coinCounterTest;
        private ItemsDatabase _itemsDatabase;
        private PauseMenu _pauseMenu;
        private FadeInOutManager _fadeInOutManager;
        public static Player Player { get; private set; }
        public static Material DefaultMaterial => MaterialUtils.SpriteMaterial;

        public static FontAsset DefaultFont { get; private set; }
        private static GameEntityManager _gameEntityManager;

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

            _pauseMenu = new Actor("Pause menu").AddComponent<PauseMenu>();

            Player = new Actor("Player").AddComponent<Player>();
            Player.Transform.WorldPosition = new vec3();
            Player.Actor.Layer = LayerMask.NameToLayer(GameConsts.PLAYER);

            var camActor = new Actor("MainCamera");

            Camera = camActor.AddComponent<Camera>();
            var cameraFollow = camActor.AddComponent<CameraFollow>();
            cameraFollow.Target = Player.Transform;

            Camera.Transform.WorldPosition = new vec3(0, 0, -12);
            Camera.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
            Camera.OrthographicSize = 288.0f / 2.0f / 16.0f;
            Camera.RenderTexture = new RenderTexture(512 * 2, 288 * 2);
            // Camera.RenderTexture = new RenderTexture(512, 288);

            LoadTilemap();

            Player.Init(new CharacterConfig()
            {
                JumpForce = 15,
                WalkSpeed = 5.35f,
                YGravityScale = 3.5f,
                ColliderConfig = new BodyColliderOptions() { Size = new vec2(1.0f, 1.7f), Offset = new vec2(0, 0.25f) },
                LayerName = GameConsts.PLAYER,
                SortOrder = 2,
                StartPosition = _playerStartPosTest,
                Material = MaterialUtils.SpriteMaterial,
                StartingLife = 5,
                SpriteLookDir = 1,
                InventoryMaxSlots = 10,
                Ground = new GroundDetectionOptions()
                {
                    Enabled = true,
                    MinX = -0.5f,
                    MaxX = 0.5f,
                    RaysCount = 3,
                    SizeY = 0.7f,
                    YOffset = 0,
                    GroundMask = GameConsts.GROUND_MASK | LayerMask.NameToBit(GameConsts.ENEMY)
                },
                WalkSounds = ["Audio/HALFTONE/UI/2. Clicks/Click_4.wav",
                              "Audio/HALFTONE/UI/2. Clicks/Click_5.wav",
                              "Audio/HALFTONE/UI/2. Clicks/Click_10.wav"],
                AttackSounds = ["Audio/HALFTONE/Gameplay/Bullet_1.wav"],
                JumpSounds = ["Audio/HALFTONE/Gameplay/Jump_3.wav"],
                GroundSounds = ["Audio/HALFTONE/Gameplay/Hit_4.wav"]

            });



            cameraFollow.SetOnTargetImmediate();

            // Debug.Log(ItemsDatabase.GetDatabaseSchemaCsv());

            // GamePrefabs.Enemies.InstantiatePigStandard(Player.Transform.LocalPosition + vec3.Right * 2, -1);

            PostProcessingStack.Clear();
            // PostProcessingStack.Push(new BloomPostProcessing());
            //ScreenGrabTest();
            ScreenGrabTest3();

            Canvas();

            Portal();
            Portal().Transform.LocalPosition = new vec3(33, -9.1f);
            Portal().Transform.LocalPosition = new vec3(43, -1);

            WaterTest();
            ParticleSystem();

            _fadeInOutManager = new Actor("FadeInOutManager").AddComponent<FadeInOutManager>();
            Actor.DontDestroyOnLoad(_fadeInOutManager);
        }

        private void LoadTilemap()
        {
            var testPathNow = "Tilemap";

            //var filepath = rootPathTest + "\\Tilemap\\World.ldtk";

            var filepath = testPathNow + "/WorldTilemap.ldtk";
            //var filepath = testPathNow + "/Test.ldtk";
            string json = Assets.GetText(filepath).Text;
            string json2 = Assets.GetText(testPathNow + "/Test_Grass.ldtk").Text;

            var project = ldtk.LdtkJson.FromJson(json);
            var color = project.BgColor;

            vec2 ConvertToWorld(long x, long y, Level level, float pixelPerUnit, LayerInstance layer, bool isGridPos = false)
            {
                if (isGridPos)
                {
                    x *= layer.GridSize;
                    y *= layer.GridSize;
                }
                return new vec2(MathF.Floor((level.WorldX + x + layer.PxOffsetX) / pixelPerUnit), MathF.Ceiling((-level.WorldY + -y + -layer.PxOffsetY) / pixelPerUnit));
            }

            var sprite = GameTextureAtlases.GetAtlas("sunny_land_tileset")[0];


            foreach (var level in project.Levels)
            {
                foreach (var layer in level.LayerInstances)
                {
                    foreach (var entity in layer.EntityInstances)
                    {
                        var position = ConvertToWorld(entity.Px[0], entity.Px[1], level, sprite.Texture.PixelPerUnit, layer);
                        //Debug.Log("Entity: " + entity.Identifier);
                        if (entity.Identifier.Equals("Player"))
                        {
                            _playerStartPosTest = position;
                            GamePrefabs.World.InstantiateDoor(position + new vec2(0, 1), new DoorData() { InteractCondition = x => false });
                        }

                        _gameEntityManager.BuildEntity(entity, position, (vec2, isGrid) =>
                        {
                            // TODO: refactor, instead send a helper class that contains convert methods.
                            return ConvertToWorld((int)vec2.x, (int)vec2.y, level, sprite.Texture.PixelPerUnit, layer, isGrid);
                        });
                    }
                }
            }
            var tilemapMaterial = MaterialUtils.SpriteMaterialWorld;
            //cam.BackgroundColor = new Color32(project.BackgroundColor.R, project.BackgroundColor.G, project.BackgroundColor.B, project.BackgroundColor.A);
            //cam.BackgroundColor = new Color32(23, 28, 57, project.BackgroundColor.A);

            var tilemapActor = new Actor<TilemapRenderer>("Foreground tilemap");
            var tilemap = tilemapActor.GetComponent<TilemapRenderer>();
            tilemap.Material = tilemapMaterial;
            tilemap.Sprite = sprite;

            var tilemapActor2 = new Actor<TilemapRenderer>("Background tilemap");
            var tilemap2 = tilemapActor2.GetComponent<TilemapRenderer>();
            tilemap2.Material = tilemapMaterial;
            tilemap2.Sprite = sprite;

            var tilemapActor3 = new Actor<TilemapRenderer>("Grass tilemap");
            var tilemap3 = tilemapActor3.GetComponent<TilemapRenderer>();
            tilemap3.Material = tilemapMaterial;
            tilemap3.Sprite = sprite;

            // tilemap.SetTilemapLDtk(project, new LDtkOptions() { RenderIntGridLayer = true, RenderTilesLayer = true, RenderAutoLayer = true });
            tilemap.SetTilemapLDtk(project, new LDtkOptions()
            {
                RenderIntGridLayer = true,
                RenderTilesLayer = true,
                RenderAutoLayer = true,
                LayersToLoadMask = 1 << 2,
                WorldDepth = 0
            });
            tilemap2.SetTilemapLDtk(project, new LDtkOptions()
            {
                RenderIntGridLayer = true,
                RenderTilesLayer = true,
                RenderAutoLayer = true,
                LayersToLoadMask = 1 << 3,
                WorldDepth = 0
            });

            //tilemap3.SetTilemapLDtk(json2, new LDtkOptions()
            //{
            //    RenderIntGridLayer = true,
            //    RenderTilesLayer = true,
            //    RenderAutoLayer = true,
            //    LayersToLoadMask = 1 << 2,
            //    WorldDepth = 0
            //});

            tilemap2.SortOrder = -2;
            tilemap.SortOrder = 3;
            tilemap3.SortOrder = 3;
            tilemap.AddComponent<TilemapCollider2D>();
            tilemap.Actor.Layer = 0;
        }

        private void Canvas()
        {
            var canvas = new Actor("Canvas").AddComponent<UICanvas>();

            //var text = new Actor("Text test").AddComponent<UIText>();
            //text.Transform.Parent = canvas.Transform;
            //text.SetText("This is a text child of a canvas");
            //text.Font = Assets.Get<FontAsset>("Fonts/windows-bold[1].ttf");
            //text.Material = new Material(new Shader(Assets.GetText("Shaders/Font/FontVert.vert").Text,
            //                                        Assets.GetText("Shaders/Font/FontFrag.frag").Text));
            //text.SortOrder = 10;
            //text.RectTransform.Pivot = new vec2(0.0f, 0.5f);

            var sprites = GameTextureAtlases.GetAtlas("small_heart_idle");

            UIImage Image(string name, vec2 position, vec2 size, Sprite sprite, Transform parent)
            {
                var image = new Actor(name).AddComponent<UIImage>();
                image.Transform.Parent = parent.Transform;
                image.RectTransform.Pivot = new vec2(0.0f, 0.0f);
                image.RectTransform.Size = size;
                image.Material = MaterialUtils.UIMaterial;
                image.Sprite = sprite;
                image.PreserveAspect = true;
                image.Transform.LocalPosition = position;
                // image.AddComponent<Button>();
                return image;
            }

            float uiMult = 3;
            var lifebarSprites = GameTextureAtlases.GetAtlas("health_bar_frame");
            var lifeBar = Image("Life bar", new vec2(10, 10), new vec2(143, 34) * uiMult, lifebarSprites[3], canvas.Transform);

            Image("Heart1", new vec2(56, 40), new vec2(8, 7) * uiMult, sprites[0], lifeBar.Transform);
            Image("Heart2", new vec2(88, 40), new vec2(8, 7) * uiMult, sprites[0], lifeBar.Transform);
            Image("Heart3", new vec2(120, 40), new vec2(8, 7) * uiMult, sprites[0], lifeBar.Transform);
        }

        private Actor Portal()
        {
            var screenGrabTest = new Actor<SpriteRenderer, Rotate>("Portal");
            var renderer = screenGrabTest.GetComponent<SpriteRenderer>();
            renderer.SortOrder = 14;

            var screenShader = new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/Portal.frag").Text);
            renderer.Material = new Material(screenShader);
            renderer.Material.Name = "Portal Material";
            var pass = renderer.Material.GetPass(0);
            pass.IsScreenGrabPass = true;

            screenGrabTest.Transform.LocalScale = new vec3(6, 6);
            screenGrabTest.Transform.LocalPosition = new vec3(-9, -7);
            renderer.Material.AddTexture("uStarsTex", Assets.GetTexture("stars.png"));

            return screenGrabTest;
        }

        protected override void OnUpdate()
        {
#if DEBUG
            Window.Name = Time.FPS.ToString();
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
                _pauseMenu.OnPause();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                SceneManager.LoadScene("Game");
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
            //var screenShader = new Shader(vertex, Assets.GetText("Shaders/Ripple.frag").Text);
            //PostProcessingStack.Push(new PostProcessingSinglePass(screenShader));

            var chormaticAberration = new PostProcessingSinglePass(new Shader(vertex, Assets.GetText("Shaders/ChromaticAberration.frag").Text));
            chormaticAberration.SetValue("uAberrationStrength", 0.007f);

            PostProcessingStack.Push(chormaticAberration);

            var scalines = new PostProcessingSinglePass(new Shader(vertex, Assets.GetText("Shaders/ScanLines.frag").Text));
            scalines.SetValue("uScanlineIntensity", 0.2f);
            scalines.SetValue("uScanlineSpacing", 2);
            PostProcessingStack.Push(scalines);
        }

        internal static void UpdateCoinUI()
        {
            if (!_coinCounterTest)
            {
                _coinCounterTest = new Actor("CounterText").AddComponent<UIText>();
                _coinCounterTest.Font = DefaultFont;
            }

            //_coinCounterTest.SetText(Player.Inventory.GetSlot.Coins.ToString()); // Remove from here, just for testing
            //_coinCounterTest.Material = _defaultSpriteMaterial;
            //_coinCounterTest.Transform.WorldPosition = new vec3(20, 20);

        }
    }
}