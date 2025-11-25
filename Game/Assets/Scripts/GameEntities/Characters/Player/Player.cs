using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Player : Character
    {
        private float _shootCooldown = 0.25f;
        private float _shootCooldownTime = 0;
        private float _bulletSpeed = 23;

        public override void Init(CharacterConfig config)
        {
            Inventory = new PlayerInventory(config.InventoryMaxSlots, 4);

            base.Init(config);
            var box = AddComponent<BoxCollider2D>();
            box.Size = new vec2(0.95f, 0.6f);
            box.Offset = new vec2(0, 0.8f);
            box.Friction = 0;

            const float fps = 11.5f;
            var size = new vec2(78, 58);
            var pivot = new vec2(0.4f, 0.4f);

            string[] pathSprites = ["KingsAndPigsSprites/01-King Human/Idle (78x58)S.png",
                                    "KingsAndPigsSprites/01-King Human/Run (78x58)S.png",
                                    "KingsAndPigsSprites/01-King Human/Jump (78x58)S.png",
                                    "KingsAndPigsSprites/01-King Human/Fall (78x58)S.png",
                                    "KingsAndPigsSprites/01-King Human/Hit (78x58).png",
                                    "KingsAndPigsSprites/01-King Human/Dead (78x58).png",
                                    "KingsAndPigsSprites/01-King Human/Attack (78x58).png"];

            var states = new AnimationsStates();
            for (int i = 0; i < AnimationsStates.Length; i++)
            {
                states[i] = new AnimationStateInfo()
                {
                    IsEnabled = true,
                    Fps = fps,
                    Pivot = pivot,
                    Size = size,
                    SpriteAtlasPath = pathSprites[i],
                };
            }

            states.Attack.Events = [new AnimEvent { Time = 0, Callback = PlayAttackSoundFx }];
            states.Walk.Events = [new AnimEvent { Time = 0, Callback = PlayWalkSoundFx }, new AnimEvent { Time = (1.0f / fps) * 4, Callback = PlayWalkSoundFx }];
            states.Jump.Events = [new AnimEvent { Time = 0, Callback = PlayJumpSoundFx }];

            InitAnimationStates(states);
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }


            if (Input.GetKey(KeyCode.F) && _shootCooldownTime <= 0)
            {
                _shootCooldownTime = _shootCooldown;
                Attack();
            }

            _shootCooldownTime -= Time.DeltaTime;

            if (Input.GetKeyDown(KeyCode.X))
            {
                //Death();
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                HitDamage(1);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Restart();
            }

            if (Input.GetKey(KeyCode.A))
            {
                Walk(-1);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Walk(1);
            }
            else
            {
                Walk(0);
            }

        }
        public override bool Attack(int index = 0)
        {
            if (!IsCharacterAlive())
                return false;
            PlayAttackSoundFx();

            var origin = Transform.WorldPosition + new vec3(Transform.LocalScale.x + Math.Sign(Transform.LocalScale.x) * 0.3f, -0.1f);

            // TODO: use object pool
            var bullet = new Actor<SpriteRenderer>("Bullet").AddComponent<Bullet>();

            var mask = LayerMask.NameToBit(GameLayers.Default) | LayerMask.NameToBit(GameLayers.ENEMY) |
                                           LayerMask.NameToBit(GameLayers.PLATFORM);
            bullet.Shoot(origin, vec2.Right * LookDir, _bulletSpeed, mask);
            return true;
        }
        public override void OnFixedUpdate()
        {

            base.OnFixedUpdate();
        }
    }
}
