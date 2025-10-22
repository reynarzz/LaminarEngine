using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal partial class GamePrefabs
    {
        public static class Enemies
        {
            private static Material _defaultEnemyMat;

            static Enemies()
            {
                _defaultEnemyMat = new Material(new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text));
                _defaultEnemyMat.Name = "Enemy Default";

                var PlayerMatPass = _defaultEnemyMat.Passes.ElementAt(0);
                PlayerMatPass.Stencil.Enabled = true;
                PlayerMatPass.Stencil.Func = StencilFunc.Always;
                PlayerMatPass.Stencil.Ref = 3;
                PlayerMatPass.Stencil.ZPassOp = StencilOp.Replace;
            }

            public static EnemyBase InstancePigStandard(vec2 position, int lookDir)
            {
                var actor = new Actor("Pig Enemy").AddComponent<PigEnemyStandard>();
                actor.Init(new CharacterConfig()
                {
                    JumpSpeed = 15,
                    WalkSpeed = 5.35f,
                    YGravityScale = 3.5f,
                    ColliderConfig = new BodyColliderOptions() { Size = new vec2(1.0f, 1.7f), Offset = new vec2(0, 0.25f) },
                    LayerName = GameLayers.ENEMY,
                    SortOrder = 2,
                    StartPosition = position,
                    Material = _defaultEnemyMat,
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
                actor.Transform.LocalScale = new vec3(1 * Math.Sign(lookDir), 1, 1);
                return actor;
            }
        }
    }
}
