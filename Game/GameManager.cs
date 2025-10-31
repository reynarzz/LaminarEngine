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

        public static ulong GROUND_MASK { get; } = LayerMask.NameToBit(FLOOR) |
                                                   LayerMask.NameToBit(PLATFORM) |
                                                   LayerMask.NameToBit(Default);
    }

    [Serializable]
    public class PlayerBag
    {
        public int Coins { get; set; }
        public int MaxLife { get; set; }
    }

    internal class GameManager : ScriptBehavior
    {
        public static Camera Camera { get; private set; }
        private static Material _defaultSpriteMaterial;
        private Material _playerSpriteMaterial;
        private vec3 _playerStartPosTest;
        private static UIText _coinCounterTest;
        private ItemsDatabase _itemsDatabase;
        private PauseMenu _pauseMenu;

        public static PlayerBag PlayerBag { get; } = new();
        public static Player Player { get; private set; }
        public static Material DefaultMaterial => _defaultSpriteMaterial;

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
            GetMaterial("DefaultSpriteMaterial", ref _defaultSpriteMaterial,
                        "Shaders/SpriteVert.vert",
                        "Shaders/SpriteFrag.frag");

            GetMaterial("PlayerMaterial", ref _playerSpriteMaterial,
                        "Shaders/SpriteVert.vert",
                        "Shaders/SpriteFrag.frag");


            var PlayerMatPass = _playerSpriteMaterial.Passes.ElementAt(0);
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
            //LayerMask.TurnOn(GameLayers.PLAYER, GameLayers.PLATFORM);
            //LayerMask.TurnOn(GameLayers.PLAYER, GameLayers.Default);
        }

        private void GetMaterial(string name, ref Material material, string vertexCode, string shaderCode)
        {
            material = new Material(new Shader(Assets.GetText(vertexCode).Text, Assets.GetText(shaderCode).Text));
            material.Name = name;
        }

        private void InitializeWorld()
        {
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
                Material = _playerSpriteMaterial,
                StartingLife = 5,
                SpriteLookDir = 1,
                Ground = new GroundDetectionOptions()
                {
                    Enabled = true,
                    MinX = -0.44f,
                    MaxX = 0.44f,
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

        }

        private void LoadTilemap()
        {
            var testPathNow = "Tilemap";
            var tilemapTexture = Assets.GetTexture(testPathNow + "/SunnyLand_by_Ansimuz-extended.png");

            TextureAtlasUtils.SliceTiles(tilemapTexture.Atlas, 16, 16, tilemapTexture.Width, tilemapTexture.Height);

            var tilemapSprite = new Sprite();

            tilemapSprite.Texture = tilemapTexture;

            //var filepath = rootPathTest + "\\Tilemap\\World.ldtk";

            var filepath = testPathNow + "/WorldTilemap.ldtk";
            //var filepath = testPathNow + "/Test.ldtk";
            string json = Assets.GetText(filepath).Text;
            string json2 = Assets.GetText(testPathNow + "/Test_Grass.ldtk").Text;

            var tilemapMaterial = new Material(new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text));
            tilemapMaterial.Name = "Tilemap material";

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
            tilemap.Material = tilemapMaterial;
            tilemap.Sprite = tilemapSprite;

            var tilemapActor2 = new Actor<TilemapRenderer>("Background tilemap");
            var tilemap2 = tilemapActor2.GetComponent<TilemapRenderer>();
            tilemap2.Material = tilemapMaterial;
            tilemap2.Sprite = tilemapSprite;

            var tilemapActor3 = new Actor<TilemapRenderer>("Grass tilemap");
            var tilemap3 = tilemapActor3.GetComponent<TilemapRenderer>();
            tilemap3.Material = tilemapMaterial;
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

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Physics2D.DrawColliders = !Physics2D.DrawColliders;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.Enter))
            {
                Window.FullScreen(!Window.IsFullScreen);
                Window.MouseVisible = !Window.IsFullScreen;
            }
        }

        internal static void UpdateCoinUI()
        {
            if (!_coinCounterTest)
            {
                _coinCounterTest = new Actor("CounterText").AddComponent<UIText>();
                _coinCounterTest.Font = DefaultFont; 
            }

            _coinCounterTest.SetText(PlayerBag.Coins.ToString()); // Remove from here, just for testing
            _coinCounterTest.Material = _defaultSpriteMaterial;
            _coinCounterTest.Transform.WorldPosition = new vec3(20, 20);

        }
    }
}