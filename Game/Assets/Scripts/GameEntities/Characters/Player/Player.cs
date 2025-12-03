using Engine;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Player : Character
    {
        private float _shootCooldown = 0.21f;
        private float _shootCooldownTime = 0;
        private float _bulletSpeed = 23;
        private bool _canMove = false;

        private readonly List<InteractableEntityBase> _nearInteractables = new();
        public override void Init(CharacterConfig config)
        {
            Inventory = new PlayerInventory(config.InventoryMaxSlots, 4);
            GameUIManager.Inventory.InitInventory(Inventory);

            base.Init(config);
            var box = AddComponent<BoxCollider2D>();
            box.Size = new vec2(0.95f, 0.6f);
            box.Offset = new vec2(0, 0.8f);
            box.Friction = 0;

            const float fps = 11.5f;

            string[] atlasid = ["player_idle",
                                "player_run",
                                "player_jump",
                                "player_fall",
                                "player_hit",
                                "player_dead",
                                "player_attack",
                                ];

            var states = new AnimationsStates();
            for (int i = 0; i < AnimationsStates.Length; i++)
            {
                states[i] = new AnimationStateInfo()
                {
                    IsEnabled = true,
                    Fps = fps,
                    SpriteAtlasId = atlasid[i],
                };
            }

            states.Attack.Events = [new AnimEvent { Time = 0, Callback = PlayAttackSoundFx }];
            states.Walk.Events = [new AnimEvent { Time = 0, Callback = PlayWalkSoundFx }, new AnimEvent { Time = (1.0f / fps) * 4, Callback = PlayWalkSoundFx }];
            states.Jump.Events = [new AnimEvent { Time = 0, Callback = PlayJumpSoundFx }];

            InitAnimationStates(states);

            var doorInState = AnimatorUtils.AddState(Animator, "DoorIn", false);
            doorInState.Clip.AddCurve(SPRITE_PROPERTY_NAME, new SpriteCurve(fps, GameTextures.GetAtlas("player_door_in")));

            var doorOutState = AnimatorUtils.AddState(Animator, "DoorOut", false);
            doorOutState.Clip.AddCurve(SPRITE_PROPERTY_NAME, new SpriteCurve(fps, GameTextures.GetAtlas("player_door_out")));

            Inventory.OnLifeChanged += Inventory_OnLifeChanged;
            GameUIManager.PlayerHealth.InitHealth(Inventory.Life);

            InitLevel();
        }

        private void Inventory_OnLifeChanged(int life)
        {
            GameUIManager.PlayerHealth.UpdatePlayerHealth(life);
        }

        public void InitLevel(bool startWithDoor = true)
        {
            if (startWithDoor)
            {
                _canMove = false;
                Renderer.IsEnabled = false;
                TimedExecute(() => ExitFromDoor(_nearInteractables.FirstOrDefault(x => x as Door) as Door), 0.7f);
            }
        }

        public void ExitFromDoor(Door door)
        {
            if (door == null)
                return;
            Renderer.IsEnabled = false;
            _canMove = false;
            
            IEnumerator ExitFromDoor()
            {
                Debug.Log("Open");
                door.Open();
                yield return new WaitForSeconds(0.3f);
                Walk(1);
                Animator.Play("DoorOut");
                Renderer.IsEnabled = true;
                yield return new WaitForSeconds(0.4f);
                door.Close();
                Animator.Play(IDLE_ANIM_STATE);
                _canMove = true;
            }

            StartCoroutine(ExitFromDoor());
        }

        private void MoveToDoor(Door door)
        {
            _canMove = false;
            IEnumerator WalkToDoor()
            {
                var walkDir = door.Transform.WorldPosition.x - Transform.WorldPosition.x;
                while (Math.Abs(walkDir) > 0.1f)
                {
                    walkDir = door.Transform.WorldPosition.x - Transform.WorldPosition.x;
                    Walk(Math.Sign(walkDir));
                    yield return null;
                }

                if (door.TryInteract(this))
                {
                    Debug.Log("Open");
                    Walk(0);
                    yield return new WaitForSeconds(0.3f);
                    Animator.Play("DoorIn");
                    yield return new WaitForSeconds(0.5f);
                    Renderer.IsEnabled = false;
                    door.Close();
                }
            }

            StartCoroutine(WalkToDoor());
        }

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (_nearInteractables.Count > 0)
                {
                    var interactable = _nearInteractables[^1];
                    if (interactable.CanInteract(this))
                    {
                        _nearInteractables.RemoveAt(_nearInteractables.Count - 1);

                        if (interactable is Door door)
                        {
                            MoveToDoor(door);
                        }
                        else
                        {
                            interactable.TryInteract(this);
                        }
                    }
                }
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

            if (_canMove)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Jump();
                }

                if (Input.GetKey(KeyCode.F) && _shootCooldownTime <= 0)
                {
                    _shootCooldownTime = _shootCooldown;
                    Attack();
                    CameraShake.Instance.BurstShake(20, 0.09f, 0.09f);
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
        }

        public override bool HitDamage(int amount)
        {
            var isHit = base.HitDamage(amount);

            if (isHit)
            {
                //CameraShake.Instance.BurstShake(30, 0.19f, 0.09f);
            }
            return isHit;
        }
        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
        }

        public override bool Attack(int index = 0)
        {
            if (!IsCharacterAlive())
                return false;
            PlayAttackSoundFx();

            var origin = Transform.WorldPosition + new vec3(Transform.LocalScale.x + Math.Sign(Transform.LocalScale.x) * 0.3f, -0.1f);

            // TODO: use object pool
            var bullet = new Actor<SpriteRenderer>("Bullet").AddComponent<Bullet>();

            var mask = LayerMask.NameToBit(GameConsts.Default) | LayerMask.NameToBit(GameConsts.ENEMY) |
                                           LayerMask.NameToBit(GameConsts.PLATFORM);
            bullet.Shoot(origin, vec2.Right * LookDir, _bulletSpeed, mask);
            return true;
        }
        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            var interactable = collider.GetComponent<InteractableEntityBase>();
            if (interactable)
            {
                if (!_nearInteractables.Contains(interactable))
                    _nearInteractables.Add(interactable);
            }
        }

        protected override void OnTriggerExit2D(Collider2D collider)
        {
            var interactable = collider.GetComponent<InteractableEntityBase>();
            if (interactable)
            {
                _nearInteractables.Remove(interactable);
            }
        }
    }
}
