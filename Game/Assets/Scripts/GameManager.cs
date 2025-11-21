using Engine;
using Engine.GUI;
using Engine.Utils;
using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class GameLayers
    {
        public const string Default = "Default";
        public const string PLAYER = "Player";
        public const string FLOOR = "Floor";
        public const string ENEMY = "Enemy";
        public const string ENEMY_CONFUSED = "EnemyConfused";
        public const string PLATFORM = "Platform";
        public const string COLLECTIBLE = "Collectible";
        public const string CHARACTER_IGNORE = "character_Ignore";

        public static ulong GROUND_MASK { get; } = LayerMask.NameToBit(FLOOR) |
                                                   LayerMask.NameToBit(PLATFORM) |
                                                   LayerMask.NameToBit(Default);
    }

    internal class GameManager : ScriptBehavior
    {
        public static Camera Camera { get; private set; }
        private static Material _defaultSpriteMaterial;
        private static Material _stencylMaterial;
        private vec3 _playerStartPosTest;
        private static UIText _coinCounterTest;
        private ItemsDatabase _itemsDatabase;
        private PauseMenu _pauseMenu;

        public static Player Player { get; private set; }
        public static Material DefaultMaterial => _stencylMaterial;

        public static FontAsset DefaultFont { get; internal set; }

        public override void OnAwake()
        {
            InitializeMaterials();
            InitializeActorLayers();
            InitializeData();
            InitializeWorld();
        }

        private void InitializeData()
        {
            _itemsDatabase = new ItemsDatabase("Data/ItemsDatabase.csv");
            DefaultFont = Assets.Get<FontAsset>("Fonts/windows-bold[1].ttf");
        }
        private void InitializeMaterials()
        {
            _defaultSpriteMaterial = GetMaterial("DefaultSpriteMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");
            _stencylMaterial = GetMaterial("PlayerMaterial", "Shaders/SpriteVert.vert", "Shaders/SpriteFrag.frag");

            var PlayerMatPass = _stencylMaterial.Passes.ElementAt(0);
            PlayerMatPass.Stencil.Enabled = true;
            PlayerMatPass.Stencil.Func = StencilFunc.Always;
            PlayerMatPass.Stencil.Ref = 3;
            PlayerMatPass.Stencil.ZPassOp = StencilOp.Replace;
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

            var names = GetConstStringValues(typeof(GameLayers));

            for (int i = 0; i < names.Length; i++)
            {
                LayerMask.AssignName(i, names[i]);
            }

            LayerMask.TurnOff(GameLayers.PLAYER, GameLayers.PLAYER);
            LayerMask.TurnOff(GameLayers.PLAYER, GameLayers.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameLayers.ENEMY, GameLayers.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameLayers.ENEMY_CONFUSED, GameLayers.CHARACTER_IGNORE);
            LayerMask.TurnOff(GameLayers.PLATFORM, GameLayers.COLLECTIBLE);
            //LayerMask.TurnOn(GameLayers.PLAYER, GameLayers.Default);
        }


        private static Material GetMaterial(string name, string vertexCode, string shaderCode)
        {
            var material = new Material(new Shader(Assets.GetText(vertexCode).Text, Assets.GetText(shaderCode).Text));
            material.Name = name;

            return material;
        }

        private void InitializeWorld()
        {
            var music = new Actor<AudioSource>("Music Manager");
            var musicAudio = music.GetComponent<AudioSource>();
            musicAudio.Loop = true;
            musicAudio.Clip = Assets.GetAudioClip("Audio/music/streamloops/Stream Loops 2024-02-14_L01.wav");
            musicAudio.Play();
            _pauseMenu = new Actor("Pause menu").AddComponent<PauseMenu>();

            Player = new Actor("Player").AddComponent<Player>();
            Player.Transform.WorldPosition = new vec3();
            Player.Actor.Layer = LayerMask.NameToLayer(GameLayers.PLAYER);

            var camActor = new Actor("MainCamera");

            Camera = camActor.AddComponent<Camera>();
            var follow = camActor.AddComponent<CameraFollow>();
            follow.Target = Player.Transform;

            Camera.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
            Camera.OrthographicSize = 288.0f / 2.0f / 16.0f;
            Camera.RenderTexture = new RenderTexture(512 * 2, 288 * 2);
            // Camera.RenderTexture = new RenderTexture(512, 288);

            Camera.Transform.WorldPosition = new vec3(Player.Transform.WorldPosition.x,
                                                      Player.Transform.WorldPosition.y, -12);

            LoadTilemap();


            Player.Init(new CharacterConfig()
            {
                JumpForce = 15,
                WalkSpeed = 5.35f,
                YGravityScale = 3.5f,
                ColliderConfig = new BodyColliderOptions() { Size = new vec2(1.0f, 1.7f), Offset = new vec2(0, 0.25f) },
                LayerName = GameLayers.PLAYER,
                SortOrder = 2,
                StartPosition = _playerStartPosTest,
                Material = _stencylMaterial,
                StartingLife = 5,
                SpriteLookDir = 1,
                Ground = new GroundDetectionOptions()
                {
                    Enabled = true,
                    MinX = -0.5f,
                    MaxX = 0.5f,
                    RaysCount = 3,
                    SizeY = 0.7f,
                    YOffset = 0,
                    GroundMask = GameLayers.GROUND_MASK
                },
                WalkSounds = ["Audio/HALFTONE/UI/2. Clicks/Click_4.wav",
                              "Audio/HALFTONE/UI/2. Clicks/Click_5.wav",
                              "Audio/HALFTONE/UI/2. Clicks/Click_10.wav"],
                AttackSounds = ["Audio/HALFTONE/Gameplay/Slash_1.wav"],
                JumpSounds = ["Audio/HALFTONE/Gameplay/Jump_3.wav"],
                GroundSounds = ["Audio/HALFTONE/Gameplay/Hit_4.wav"]

            });

            var platform = new Actor<Platform, SpriteRenderer>("Platform");
            platform.GetComponent<SpriteRenderer>().Material = _defaultSpriteMaterial;
            platform.Layer = LayerMask.NameToLayer(GameLayers.PLATFORM);


            // Debug.Log(ItemsDatabase.GetDatabaseSchemaCsv());

            // GamePrefabs.Enemies.InstantiatePigStandard(Player.Transform.LocalPosition + vec3.Right * 2, -1);

            // LoadTilemap();
            Canvas();


            Portal();
            Portal().Transform.LocalPosition = new vec3(33, -9.1f);
            Portal().Transform.LocalPosition = new vec3(43, -1);

            WaterTest();
            ParticleSystem();

        }

        private static Material _tilemapMaterial;

        private void LoadTilemap()
        {
            var testPathNow = "Tilemap";
            var tilemapTexture = Assets.GetTexture(testPathNow + "/SunnyLand_by_Ansimuz-extended.png");

            TextureAtlasUtils.SliceTiles(tilemapTexture.Atlas, 16, 16, tilemapTexture.Width, tilemapTexture.Height);

            var tilemapSprite = new Sprite(tilemapTexture);

            //var filepath = rootPathTest + "\\Tilemap\\World.ldtk";

            var filepath = testPathNow + "/WorldTilemap.ldtk";
            //var filepath = testPathNow + "/Test.ldtk";
            string json = Assets.GetText(filepath).Text;
            string json2 = Assets.GetText(testPathNow + "/Test_Grass.ldtk").Text;

            if (_tilemapMaterial == null)
            {
                _tilemapMaterial = new Material(new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text));
                _tilemapMaterial.Name = "Tilemap material";
            }

            var project = ldtk.LdtkJson.FromJson(json);
            var color = project.BgColor;

            vec3 ConvertToWorld(long[] px, Level level, float pixelPerUnit, LayerInstance layer)
            {
                return new vec3(MathF.Floor((level.WorldX + px[0] + layer.PxOffsetX) / pixelPerUnit), MathF.Ceiling((-level.WorldY + -px[1] + -layer.PxOffsetY) / pixelPerUnit), 0);
            }

            foreach (var level in project.Levels)
            {
                foreach (var layer in level.LayerInstances)
                {
                    foreach (var entity in layer.EntityInstances)
                    {
                        var position = ConvertToWorld(entity.Px, level, tilemapTexture.PixelPerUnit, layer);
                        //Debug.Log("Entity: " + entity.Identifier);
                        if (entity.Identifier.Equals("Player"))
                        {
                            _playerStartPosTest = position;
                        }

                        switch (entity.Identifier)
                        {
                            case "Enemy1":
                                GamePrefabs.Enemies.InstantiateKingPig(position, -1);
                                break;
                            case "Coin":
                                GamePrefabs.Collectibles.InstantiateCoin(position);
                                break;
                            default:
                                break;
                        }

                        foreach (var field in entity.FieldInstances)
                        {
                            //Debug.Log("Name: " + field.Identifier + ", Type: " + field.Type + ", Value: " + field.Value);
                        }
                    }
                }
            }

            //cam.BackgroundColor = new Color32(project.BackgroundColor.R, project.BackgroundColor.G, project.BackgroundColor.B, project.BackgroundColor.A);
            //cam.BackgroundColor = new Color32(23, 28, 57, project.BackgroundColor.A);

            var tilemapActor = new Actor<TilemapRenderer>("Foreground tilemap");
            var tilemap = tilemapActor.GetComponent<TilemapRenderer>();
            tilemap.Material = _tilemapMaterial;
            tilemap.Sprite = tilemapSprite;

            var tilemapActor2 = new Actor<TilemapRenderer>("Background tilemap");
            var tilemap2 = tilemapActor2.GetComponent<TilemapRenderer>();
            tilemap2.Material = _tilemapMaterial;
            tilemap2.Sprite = tilemapSprite;

            var tilemapActor3 = new Actor<TilemapRenderer>("Grass tilemap");
            var tilemap3 = tilemapActor3.GetComponent<TilemapRenderer>();
            tilemap3.Material = _tilemapMaterial;
            tilemap3.Sprite = tilemapSprite;

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

            var mainShader = new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text);
            var mat1 = new Material(mainShader);

            var sprites = TextureAtlasUtils.SliceSprites(Assets.GetTexture("KingsAndPigsSprites/12-Live and Coins/Small Heart Idle (18x14).png"), 8, 7);

            UIImage Image(string name, vec2 position, vec2 size, Sprite sprite, Transform parent)
            {
                var image = new Actor(name).AddComponent<UIImage>();
                image.Transform.Parent = parent.Transform;
                image.RectTransform.Pivot = new vec2(0.0f, 0.0f);
                image.RectTransform.Size = size;
                image.Material = mat1;
                image.Sprite = sprite;
                image.PreserveAspect = true;
                image.Transform.LocalPosition = position;
                // image.AddComponent<Button>();
                return image;
            }

            float uiMult = 3;
            var tex = Assets.GetTexture("KingsAndPigsSprites/12-Live and Coins/Live Bar_atlas(143x34).png");
            var lifebarSprites = TextureAtlasUtils.SliceSprites(tex, 143, 34);
            var lifeBar = Image("Life bar", new vec2(10, 10), new vec2(143, 34) * uiMult, lifebarSprites[2], canvas.Transform);




            Image("Heart1", new vec2(56, 40), new vec2(8, 7) * uiMult, sprites[0], lifeBar.Transform);
            Image("Heart2", new vec2(88, 40), new vec2(8, 7) * uiMult, sprites[0], lifeBar.Transform);
            Image("Heart3", new vec2(120, 40), new vec2(8, 7) * uiMult, sprites[0], lifeBar.Transform);
        }

        private Actor Portal()
        {
            var screenGrabTest = new Actor<SpriteRenderer, Rotate>();
            var renderer = screenGrabTest.GetComponent<SpriteRenderer>();
            renderer.SortOrder = 14;

            var screenShader = new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/Portal.frag").Text);
            renderer.Material = new Material(screenShader);

            var pass = renderer.Material.Passes.ElementAt(0);
            pass.IsScreenGrabPass = true;

            screenGrabTest.Transform.LocalScale = new vec3(6, 6);
            screenGrabTest.Transform.LocalPosition = new vec3(-9, -7);
            renderer.Material.AddTexture("uStarsTex", Assets.GetTexture("stars.png"));

            return screenGrabTest;
        }

        public override void OnUpdate()
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
                SceneManager.Test_LoadScene(new Scene());
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

            var pass = renderer.Material.Passes.ElementAt(0);
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