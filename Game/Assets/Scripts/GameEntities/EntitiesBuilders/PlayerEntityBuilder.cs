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
        public override GameEntity Build(EntityInstanceData entityData, IReadOnlyDictionary<string, LayerData> layers, Func<vec2, bool, vec2> positionConverter)
        {
            if (!_player)
            {
                _player = new Actor("Player").AddComponent<Player>();
                _player.Actor.Layer = LayerMask.NameToLayer(GameConsts.PLAYER);

                // TODO: get player's config values from a config .csv file.

                _player.Init(new CharacterConfig()
                {
                    JumpForce = 15,
                    WalkSpeed = 5.35f,
                    YGravityScale = 3.5f,
                    ColliderConfig = new BodyColliderOptions() { Size = new vec2(1.0f, 1.7f), Offset = new vec2(0, 0.25f) },
                    LayerName = GameConsts.PLAYER,
                    SortOrder = 2,
                    StartPosition = entityData.WorldPosition,
                    Material = MaterialUtils.SpriteMaterial,
                    StartingLife = 4,
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
            }

            if (GetBool(entityData.Entity.FieldInstances, "look_to_right", out bool lookToRight))
            {
                _player.LookAt(lookToRight ? 1 : -1);
            }

            _player.Transform.WorldPosition = entityData.WorldPosition;

            return _player;
        }
    }
}
