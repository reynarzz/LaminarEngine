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

        public static FontAsset DefaultFont { get; private set; }
        private static GameEntityManager _gameEntityManager;
        private static GameUIManager _gameUIManger;

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
            PostProcessingStack.Clear();
            // PostProcessingStack.Push(new BloomPostProcessing());
            //ScreenGrabTest();
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

            LoadTilemap();

            // Debug.Log(ItemsDatabase.GetDatabaseSchemaCsv());

            // GamePrefabs.Enemies.InstantiatePigStandard(Player.Transform.LocalPosition + vec3.Right * 2, -1);

            WaterTest();
            ParticleSystem();
        }

        private void InitializeCamera(Transform target)
        {
            var camActor = new Actor("MainCamera");

            Camera = camActor.AddComponent<Camera>();
            var cameraFollow = camActor.AddComponent<CameraFollow>();
            cameraFollow.Target = target;

            Camera.Transform.WorldPosition = new vec3(0, 0, -12);
            Camera.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
            Camera.OrthographicSize = 288.0f / 2.0f / 16.0f;
            Camera.RenderTexture = new RenderTexture(512 * 2, 288 * 2);
            cameraFollow.SetOnTargetImmediate();
            // Camera.RenderTexture = new RenderTexture(512, 288);

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

                        var isPlayer = entity.Identifier.Equals("Player");
                        GameEntity gameEntity = null;
                        
                        gameEntity = _gameEntityManager.BuildEntity(entity, position, (vec2, isGrid) =>
                        {
                            // TODO: refactor, instead send a helper class that contains convert methods.
                            return ConvertToWorld((int)vec2.x, (int)vec2.y, level, sprite.Texture.PixelPerUnit, layer, isGrid);
                        });

                        if (isPlayer)
                        {
                            if (!Player)
                            {
                                InitializeCamera(gameEntity.Transform);
                                Player = gameEntity.GetComponent<Player>();
                            }
                            GamePrefabs.World.InstantiateDoor(position + new vec2(0, 1), new DoorData() { InteractCondition = x => false });
                        }

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


        protected override void OnUpdate()
        {
#if DEBUG
            Window.Name = EngineInfo.RendererInfoToString() + " | FPS: " + ((int)Time.FPS).ToString();
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
    }
}