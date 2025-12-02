using Engine;
using Engine.Utils;
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
            public static EnemyBase InstantiatePigStandard(vec2 position, int lookDir)
            {
                return InstancePig<PigEnemyStandard>(position, lookDir);
            }

            public static EnemyBase InstantiateKingPig(vec2 position, int lookDir)
            {
                return InstancePig<KingPigEnemy>(position, lookDir);
            }

            private static EnemyBase InstancePig<T>(vec2 position, int lookDir) where T: EnemyBase
            {
                var actor = new Actor("Pig Enemy").AddComponent<T>();
                actor.Init(new CharacterConfig()
                {
                    JumpForce = 10,
                    WalkSpeed = 3.35f,
                    YGravityScale = 3.5f,
                    ColliderConfig = new BodyColliderOptions() { Size = new vec2(1.0f, 1.0f), Offset = new vec2(0, -0.1f) },
                    LayerName = GameConsts.ENEMY,
                    SortOrder = 2,
                    StartPosition = position,
                    Material = MaterialUtils.SpriteMaterial,
                    StartingLife = 3,
                    SpriteLookDir = -1,
                    InventoryMaxSlots = 3,
                    Ground = new GroundDetectionOptions()
                    {
                        Enabled = true,
                        MinX = -0.45f,
                        MaxX = 0.45f,
                        RaysCount = 3,
                        SizeY = 0.7f,
                        YOffset = 0,
                        GroundMask = GameConsts.GROUND_MASK | LayerMask.NameToBit(GameConsts.ENEMY)
                    },
                    WalkSounds = ["Audio/HALFTONE/UI/2. Clicks/Click_4.wav",
                              "Audio/HALFTONE/UI/2. Clicks/Click_5.wav",
                              "Audio/HALFTONE/UI/2. Clicks/Click_10.wav"],
                    AttackSounds = ["Audio/HALFTONE/Gameplay/Slash_1.wav"],
                    JumpSounds = ["Audio/HALFTONE/Gameplay/Jump_3.wav"],
                    GroundSounds = ["Audio/HALFTONE/Gameplay/Hit_4.wav"],

                });
                actor.Transform.LocalScale = new vec3(1 * Math.Sign(lookDir), 1, 1);
                return actor;
            }
        }
    }
}