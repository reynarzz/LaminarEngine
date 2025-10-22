using Engine;
using GlmNet;
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
        public const string PLAYER = "Player";
        public const string FLOOR = "Floor";
        public const string ENEMY = "Enemy";
        public const string PLATFORM = "Platform";
    }

    internal class GameManager : ScriptBehavior
    {
        public static Camera Camera { get; private set; }
        private Material _defaultSpriteMaterial;
        private Material _playerSpriteMaterial;

        public override void OnAwake()
        {
            InitializeMaterials();
            InitializeActorLayers(typeof(GameLayers));
            InitializeWorld();
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

        private void InitializeActorLayers(Type layers)
        {
            static string[] GetConstStringValues(Type type)
            {
                return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                           .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                           .Select(f => (string)f.GetRawConstantValue())
                           .ToArray();
            }

            var names = GetConstStringValues(layers);

            for (int i = 0; i < names.Length; i++)
            {
                LayerMask.AssignName(i, names[i]);
            }

            LayerMask.TurnOff(GameLayers.PLAYER, GameLayers.PLAYER);
            LayerMask.TurnOn(GameLayers.PLAYER, GameLayers.PLATFORM);
        }

        private void GetMaterial(string name, ref Material material, string vertexCode, string shaderCode)
        {
            material = new Material(new Shader(Assets.GetText(vertexCode).Text, Assets.GetText(shaderCode).Text));
            material.Name = name;
        }

        private void InitializeWorld()
        {
            var player = new Actor().AddComponent<Player>();
            player.Transform.WorldPosition = new vec3();

            var camActor = new Actor("MainCamera");

            Camera = camActor.AddComponent<Camera>();
            var follow = camActor.AddComponent<CameraFollow>();
            follow.Target = player.Transform;

            Camera.BackgroundColor = new Color(0.2f, 0.2f, 0.2f, 1);
            Camera.OrthographicSize = 288.0f / 2.0f / 16.0f;
            Camera.RenderTexture = new RenderTexture(512 * 2, 288 * 2);

            Camera.Transform.WorldPosition = new vec3(player.Transform.WorldPosition.x,
                                                      player.Transform.WorldPosition.y, -12);


            player.Init(new CharacterConfig()
            {
                JumpSpeed = 15,
                WalkSpeed = 5.35f,
                YGravityScale = 3.5f,
                ColliderConfig = new BodyColliderOptions() { Size = new vec2(1.0f, 1.7f), Offset = new vec2(0, 0.25f) },
                LayerName = GameLayers.PLAYER,
                SortOrder = 2,
                StartPosition = new vec2(-25.875f, -9.5625f),
                Material = _playerSpriteMaterial,
                StartingLife = 5,
                Ground = new GroundDetectionOptions()
                {
                    Enabled = true,
                    MinX = -0.45f,
                    MaxX = 0.45f,
                    RaysCount = 3,
                    SizeY = 0.7f,
                    YOffset = 0,
                    GroundMask = LayerMask.NameToBit("Floor") | LayerMask.NameToBit("Platform")
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
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Physics2D.DrawColliders = !Physics2D.DrawColliders;
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Enter))
            {
                Window.FullScreen(!Window.IsFullScreen);
                Window.MouseVisible = !Window.IsFullScreen;
            }
        }
    }
}