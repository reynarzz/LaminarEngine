using Engine;
using Engine.Utils;
using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class PlayerEntityBuilder : GameEntityBuilderBase
    {
        private static Player _player;
        public override GameEntity Build(EntityInstanceData entityData, WorldData worldData, Func<vec2, bool, vec2> positionConverter)
        {
            GetBool(entityData, "look_to_right", out bool lookToRight);

            if (!_player)
            {
                _player = new Actor("Player").AddComponent<Player>();
                _player.Actor.Layer = LayerMask.NameToLayer(GameConsts.PLAYER);

                // TODO: get player's config values from a config .csv file.

                _player.Init(new CharacterConfig()
                {
                    JumpForce = 15,
                    NormalJumpHeightThreshold = 2.1f,
                    MaxJumpHeight = 3,
                    WalkSpeed = 5.35f,
                    YGravityScale = 3.5f,
                    ColliderConfig = new BodyColliderOptions() { Size = new vec2(1.0f, 1.7f), Offset = new vec2(0, 0.25f) },
                    LayerName = GameConsts.PLAYER,
                    SortOrder = 2,
                    StartPosition = entityData.WorldPosition,
                    Material = GameMaterials.Instance.SpriteMaterial,
                    StartingLife = 4,
                    MaxLife = 4,
                    SpriteLookDirFlip = 1,
                    InventoryMaxSlots = 6,
                    HitInvincibilityBlinks = 5,
                    StartLookDir = lookToRight ? 1 : -1,
                    HitRecoilTime = 0.5f,
                    HitRecoilStrengthScaling = 3.5f,
                    Ground = new GroundDetectionOptions()
                    {
                        Enabled = true,
                        MinX = -0.5f,
                        MaxX = 0.5f,
                        RaysCount = 3,
                        SizeY = 0.7f,
                        YOffset = 0,
                        GroundMask = GameConsts.GROUND_MASK
                    },
                    HitSoundsVolume = 0.08f * 1.3f,
                    WalkSoundsVolume = 0.13f * 1.3f,
                    AttackSoundsVolume = 0.12f * 1.3f,
                    JumpSoundsVolume = 0.1f * 1.3f,
                    WalkSounds = ["Audio/HALFTONE/UI/2. Clicks/Click_4.wav",
                                  "Audio/HALFTONE/UI/2. Clicks/Click_5.wav",
                                  "Audio/HALFTONE/UI/2. Clicks/Click_10.wav"],
                    AttackSounds = ["Audio/HALFTONE/Gameplay/Bullet_1.wav"],
                    JumpSounds = ["Audio/HALFTONE/Gameplay/Jump_3.wav"],
                    GroundSounds = ["Audio/HALFTONE/Gameplay/Hit_4.wav"],
                    HitSounds = ["Audio/HALFTONE/Gameplay/Hit_2.wav"]
                });
            }
            else
            {
                _player.Transform.WorldPosition = entityData.WorldPosition;
                _player.LookAt(lookToRight ? 1 : -1);
            }

            return _player;
        }
    }
}
