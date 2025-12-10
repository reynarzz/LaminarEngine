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
    public enum EnemyType
    {
        Invalid,
        KingPig
    }

    public class EnemyInstanceData
    {
        public EnemyType Type;
        public int LookDir;
        public vec2 Position;
        public ItemAmountPair[] StartInventoryItems;
    }

    internal partial class GamePrefabs
    {
        public static class Enemies
        {
            public static EnemyBase InstantiateEnemy(EnemyInstanceData data)
            {
                switch (data.Type)
                {
                    case EnemyType.Invalid:
                        break;
                    case EnemyType.KingPig:
                        return InstancePig<KingPigEnemy>(data);
                    default:
                        Debug.Error($"Enemy type to instantiate is not implemented: {data.Type}");
                        break;
                }

                return null;
            }

            private static EnemyBase InstancePig<T>(EnemyInstanceData data) where T : EnemyBase
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
                    StartPosition = data.Position,
                    Material = GameMaterials.Instance.SpriteMaterial,
                    StartingLife = 3,
                    MaxLife = 3,
                    StartLookDir = data.LookDir,
                    SpriteLookDirFlip = -1,
                    InventoryMaxSlots = data.StartInventoryItems != null ? data.StartInventoryItems.Length : 0,
                    StartInventoryValues = data.StartInventoryItems,
                    HitInvincibilityBlinks = 0,
                    HitRecoilTime = 0.2f,
                    HitRecoilStrengthScaling = 3,
                    DisappearOnDead = true,
                    Ground = new GroundDetectionOptions()
                    {
                        Enabled = true,
                        MinX = -0.45f,
                        MaxX = 0.45f,
                        RaysCount = 3,
                        SizeY = 0.7f,
                        YOffset = 0,
                        GroundMask = GameConsts.GROUND_MASK //| LayerMask.NameToBit(GameConsts.ENEMY)
                    },
                    HitSoundsVolume = 0.8f,
                    WalkSoundsVolume = 0.18f,
                    AttackSoundsVolume = 0.18f,
                    JumpSoundsVolume = 0.17f,
                    WalkSounds = ["Audio/HALFTONE/UI/2. Clicks/Click_4.wav",
                              "Audio/HALFTONE/UI/2. Clicks/Click_5.wav",
                              "Audio/HALFTONE/UI/2. Clicks/Click_10.wav"],
                    AttackSounds = ["Audio/HALFTONE/Gameplay/Slash_1.wav"],
                    JumpSounds = ["Audio/HALFTONE/Gameplay/Jump_3.wav"],
                    GroundSounds = ["Audio/HALFTONE/Gameplay/Hit_4.wav"],
                    HitSounds = ["Audio/MinifantasySfx/16_human_walk_stone_1.wav"]
                });

                return actor;
            }
        }
    }
}