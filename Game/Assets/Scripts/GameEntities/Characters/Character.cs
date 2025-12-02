using Box2D.NET;
using Engine;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public struct BodyColliderOptions
    {
        public vec2 Offset;
        public vec2 Size;
    }
    public struct GroundDetectionOptions
    {
        public bool Enabled;
        public float MinX;
        public float MaxX;
        public float YOffset;
        public int RaysCount;
        public ulong GroundMask;
        public float SizeY;
    }


    public struct CharacterConfig
    {
        public float JumpForce;
        public float WalkSpeed;
        public float YGravityScale;
        public int StartingLife;
        public int SpriteLookDir;
        public BodyColliderOptions ColliderConfig;
        public GroundDetectionOptions Ground;
        public string LayerName;
        public int SortOrder;
        public vec2 StartPosition;
        public int InventoryMaxSlots;
        public Material Material;
        public string[] JumpSounds;
        public string[] WalkSounds;
        public string[] AttackSounds;
        public string[] GroundSounds;
    }

    public abstract class Character : GameEntity
    {
        protected Animator Animator { get; private set; }
        protected SpriteRenderer Renderer { get; private set; }
        protected RigidBody2D Rigidbody { get; private set; }
        protected Collider2D Collider { get; private set; }
        protected AudioSource AudioSource { get; private set; }

        protected const string SPRITE_PROPERTY_NAME = "Sprite_Property";
        protected const string VEL_X_PROP_NAME = "VelocityX";
        protected const string VEL_Y_PROP_NAME = "VelocityY";
        protected const string ON_GROUND_PROPERTY_NAME = "IsOnGround";
        protected const string DEATH_PROPERTY_NAME = "IsDeath";
        protected const string LIFE_PROPERTY_NAME = "Life";
        protected const string HIT_DAMAGE_PROPERTY_NAME = "IsHitDamage";

        protected const string IDLE_ANIM_STATE = "Idle";
        protected const string WALK_ANIM_STATE = "Walk";
        protected const string JUMP_ANIM_STATE = "Jump";
        protected const string FALL_ANIM_STATE = "Fall";
        protected const string HIT_ANIM_STATE = "Hit";
        protected const string DEATH_ANIM_STATE = "Death";
        protected const string ATTACK_ANIM_STATE = "Attack";


        protected const int MAX_LIFE = 10;
        protected readonly string[] Attacks = ["Attack1", "Attack2", "Attack3", "Attack4", "Attack5", "Attack6"];

        private bool _isOnGround = false;
        protected bool IsOnGround
        {
            get => _isOnGround;
            private set
            {
                if (_isOnGround == value)
                    return;

                _isOnGround = value;
                OnGroundChanged(value);
            }
        }
        private float _maxFallYVelocity = -20;

        public CharacterInventory Inventory { get; protected set; }

        public int SpriteLookDir => _characterConfig.SpriteLookDir;
        public int LookDir { get; private set; }
        private CharacterConfig _characterConfig;
        private AnimationState _main;
        private AudioClip[] _groundSfx;
        private AudioClip[] _jumpSfx;
        private AudioClip[] _attackSfx;
        private AudioClip[] _walkFx;
        private bool _jumped = false;
        public virtual void Init(CharacterConfig config)
        {
            Animator = AddComponent<Animator>();
            Renderer = AddComponent<SpriteRenderer>();
            Rigidbody = AddComponent<RigidBody2D>();
            AudioSource = AddComponent<AudioSource>();

            _characterConfig = config;

            var collider = AddComponent<CapsuleCollider2D>();
            collider.Offset = config.ColliderConfig.Offset;
            collider.Size = config.ColliderConfig.Size;
            Collider = collider;
            Actor.Layer = LayerMask.NameToLayer(config.LayerName);
            Renderer.SortOrder = config.SortOrder;
            Rigidbody.GravityScale = config.YGravityScale;
            Rigidbody.LockZRotation = true;
            Rigidbody.Interpolate = true;
            Rigidbody.IsAutoMass = false;
            Renderer.Material = config.Material;
            Collider.Friction = 0;
            Transform.WorldPosition = config.StartPosition;
            Inventory.Life = _characterConfig.StartingLife;

            LookDir = _characterConfig.SpriteLookDir;
            InitAudio(config);
            
        }

        private void InitAudio(CharacterConfig config)
        {
            AudioClip[] GetClips(string[] soundsPath)
            {
                if (soundsPath == null || soundsPath.Length == 0)
                    return null;

                var clips = new AudioClip[soundsPath.Length];
                for (int i = 0; i < soundsPath.Length; i++)
                {
                    clips[i] = Assets.GetAudioClip(soundsPath[i]);
                }
                return clips;
            }

            _groundSfx = GetClips(config.GroundSounds);
            _jumpSfx = GetClips(config.JumpSounds);
            _attackSfx = GetClips(config.AttackSounds);
            _walkFx = GetClips(config.WalkSounds);
        }

        protected void InitAnimationStates(AnimationsStates statesConfig)
        {
            Animator.Clear();

            AnimatorTransition toJump = null;
            AnimatorTransition toFall = null;
            AnimatorTransition toIdle = null;
            AnimatorTransition toWalk = null;
            AnimatorTransition toAttack = null;
            AnimatorTransition toHit = null;
            AnimatorTransition toDeath = null;
            AnimatorTransition toDeathLife0 = null;

            if (statesConfig.Idle.IsEnabled)
            {
                toIdle = new AnimatorTransition(IDLE_ANIM_STATE, [new IntCondition(VEL_X_PROP_NAME, 0, IntOp.Equal),
                                                         new BoolCondition(ON_GROUND_PROPERTY_NAME, true),
                                                         new IntCondition(LIFE_PROPERTY_NAME, 0, IntOp.GreaterThan)]);
            }

            if (statesConfig.Walk.IsEnabled)
            {
                toWalk = new AnimatorTransition(WALK_ANIM_STATE, [new IntCondition(VEL_X_PROP_NAME, 0, IntOp.NotEqual),
                                                        new BoolCondition(ON_GROUND_PROPERTY_NAME, true),
                                                        new IntCondition(LIFE_PROPERTY_NAME, 0, IntOp.GreaterThan)]);
            }
            if (statesConfig.Jump.IsEnabled)
            {
                toJump = new AnimatorTransition(JUMP_ANIM_STATE, [new IntCondition(VEL_Y_PROP_NAME, 0, IntOp.GreaterThan),
                                                         new BoolCondition(ON_GROUND_PROPERTY_NAME, false),
                                                         new IntCondition(LIFE_PROPERTY_NAME, 0, IntOp.GreaterThan)]);
            }
            if (statesConfig.Fall.IsEnabled)
            {
                toFall = new AnimatorTransition(FALL_ANIM_STATE, [new IntCondition(VEL_Y_PROP_NAME, 0, IntOp.LessThan),
                                                         new BoolCondition(ON_GROUND_PROPERTY_NAME, false),
                                                         new IntCondition(LIFE_PROPERTY_NAME, 0, IntOp.GreaterThan)]);
            }
            if (statesConfig.Attack.IsEnabled)
            {
                toAttack = new AnimatorTransition(ATTACK_ANIM_STATE, new TriggerCondition(Attacks[0]));
            }
            if (statesConfig.Hit.IsEnabled)
            {
                toHit = new AnimatorTransition(HIT_ANIM_STATE, new TriggerCondition(HIT_DAMAGE_PROPERTY_NAME));
            }
            if (statesConfig.Death.IsEnabled)
            {
                toDeath = new AnimatorTransition(DEATH_ANIM_STATE, new TriggerCondition(DEATH_PROPERTY_NAME));
                toDeathLife0 = new AnimatorTransition(DEATH_ANIM_STATE, new IntCondition(LIFE_PROPERTY_NAME, 0, IntOp.LessThanOrEqual));
            }

            if (statesConfig.Idle.IsEnabled)
            {
                AddSpriteAnimState(IDLE_ANIM_STATE, true, true, false, [toWalk, toJump, toFall, toAttack, toDeath, toHit], statesConfig.Idle.SpriteAtlasId, statesConfig.Idle.Fps, statesConfig.Idle.Events);
            }
            if (statesConfig.Walk.IsEnabled)
            {
                AddSpriteAnimState(WALK_ANIM_STATE, false, true, false, [toIdle, toJump, toFall, toAttack, toDeath, toHit], statesConfig.Walk.SpriteAtlasId, statesConfig.Walk.Fps, statesConfig.Walk.Events);
            }
            if (statesConfig.Jump.IsEnabled)
            {
                AddSpriteAnimState(JUMP_ANIM_STATE, false, false, false, [toIdle, toFall, toAttack, toHit], statesConfig.Jump.SpriteAtlasId, statesConfig.Jump.Fps, statesConfig.Jump.Events);
            }
            if (statesConfig.Fall.IsEnabled)
            {
                AddSpriteAnimState(FALL_ANIM_STATE, false, false, false, [toIdle, toWalk, toAttack, toHit], statesConfig.Fall.SpriteAtlasId, statesConfig.Fall.Fps, statesConfig.Fall.Events);
            }
            if (statesConfig.Attack.IsEnabled)
            {
                AddSpriteAnimState(ATTACK_ANIM_STATE, false, false, true, [toIdle, toWalk, toFall, toDeath, toHit], statesConfig.Attack.SpriteAtlasId, statesConfig.Attack.Fps, statesConfig.Attack.Events);
            }
            if (statesConfig.Death.IsEnabled)
            {
                AddSpriteAnimState(DEATH_ANIM_STATE, false, false, true, null, statesConfig.Death.SpriteAtlasId, statesConfig.Death.Fps, statesConfig.Death.Events);
            }
            if (statesConfig.Hit.IsEnabled)
            {
                AddSpriteAnimState(HIT_ANIM_STATE, false, false, true, [toIdle, toWalk, toJump, toFall, toAttack, toDeathLife0], statesConfig.Hit.SpriteAtlasId, statesConfig.Hit.Fps, statesConfig.Hit.Events);
            }

            Renderer.Sprite = GameTextures.GetAtlas(statesConfig.Idle.SpriteAtlasId).FirstOrDefault();
        }

        protected void AddSpriteAnimState(string stateName, bool makeMain, bool loop, bool useClipBlendTime,
                                          AnimatorTransition[] transitions, string atlasId, float fps,
                                          AnimEvent[] events)
        {
            var animClip = new AnimationClip(stateName, loop);
            var state = new AnimationState(stateName, animClip);

            if (makeMain)
            {
                _main = state;
            }

            var sprites = GameTextures.GetAtlas(atlasId);
                
            animClip.AddCurve(SPRITE_PROPERTY_NAME, new SpriteCurve(fps, sprites));

            if (events != null)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    animClip.AddEvent(events[i].Time, events[i].Callback);
                }
            }

            if (transitions != null)
            {
                var duration = animClip.Duration;
                for (int i = 0; i < transitions.Length; i++)
                {
                    if (transitions[i] == null)
                        continue;

                    var copy = new AnimatorTransition(transitions[i]);
                    if (useClipBlendTime)
                    {
                        copy.BlendTime = duration;
                    }
                    state.AddTransition(copy);
                }
            }

            if (makeMain)
            {
                Animator.SetState(state);
            }
            else
            {
                Animator.AddState(state);
            }
        }

        protected override void OnLateUpdate()
        {
            Renderer.Sprite = Animator.GetSprite(SPRITE_PROPERTY_NAME);
            Animator.Parameters.SetFloat(VEL_X_PROP_NAME, Rigidbody.Velocity.x);
            Animator.Parameters.SetFloat(VEL_Y_PROP_NAME, Rigidbody.Velocity.y);
            Animator.Parameters.SetInt(VEL_X_PROP_NAME, MathF.Sign(Math.Abs(Rigidbody.Velocity.x) < 0.09 ? 0 : Rigidbody.Velocity.x));
            Animator.Parameters.SetInt(VEL_Y_PROP_NAME, MathF.Sign(Math.Abs(Rigidbody.Velocity.y) < 0.09 ? 0 : Rigidbody.Velocity.y));
            Animator.Parameters.SetInt(LIFE_PROPERTY_NAME, Inventory.Life);
            Animator.Parameters.SetBool(ON_GROUND_PROPERTY_NAME, IsOnGround);
        }

        public void Jump()
        {
            if (!IsCharacterAlive())
                return;

            if (IsOnGround)
            {
                _jumped = true;
                Rigidbody.GravityScale = _characterConfig.YGravityScale;
                Rigidbody.Velocity = new vec2(Rigidbody.Velocity.x, 0);
                Rigidbody.AddForce(vec2.Up * _characterConfig.JumpForce, ForceMode2D.Impulse);
            }
        }

        public void LookAt(int dir)
        {
            var scaleX = Math.Abs(Transform.LocalScale.x);
            Transform.LocalScale = new vec3(scaleX * dir * Math.Sign(_characterConfig.SpriteLookDir), Transform.LocalScale.y, Transform.LocalScale.z);
        }

        public void Walk(int dir)
        {
            if (!IsCharacterAlive())
                return;
            dir *= _characterConfig.SpriteLookDir;
            if (dir != 0)
            {
                LookAt(dir);
                LookDir = dir;

                float targetX = _characterConfig.WalkSpeed * dir;
                float accel = 100;


                if (!IsOnGround)
                    accel *= 0.7f;

                float vx = Rigidbody.Velocity.x;

                if (dir != 0 && MathF.Sign(vx) != dir && MathF.Abs(vx) > 0.1f)
                {
                    var flipAmount = 1.0f;
                    if (!IsOnGround)
                    {
                        flipAmount = 0.01f;
                    }
                    vx = -vx * flipAmount; // Carry same magnitude, flip direction instantly
                }

                float delta = targetX - vx;
                float maxChange = accel * Time.DeltaTime;
                vx += Math.Clamp(delta, -maxChange, maxChange);

                if (IsOnGround && dir == 0 && MathF.Abs(vx) < 0.05f)
                {
                    vx = 0f;
                }

                Rigidbody.Velocity = new vec2(vx, Rigidbody.Velocity.y);
            }
            else if (dir == 0)
            {
                Rigidbody.Velocity = new vec2(0, Rigidbody.Velocity.y);
            }
        }

        public virtual bool Attack(int index = 0)
        {
            if (!IsCharacterAlive() || Animator.Parameters.HasTrigger(Attacks[index]))
                return false;
            Animator.Parameters.SetTrigger(Attacks[index]);
            return true;
        }

        protected virtual void Death()
        {
            if (!IsCharacterAlive())
                return;

            Rigidbody.Velocity = new vec2(0, Rigidbody.Velocity.y > 0 ? 0 : Rigidbody.Velocity.y);
            HitDamage(MAX_LIFE);
        }

        public virtual bool HitDamage(int amount)
        {
            if (!IsCharacterAlive())
                return false;

            Inventory.Life = Math.Clamp(Inventory.Life - amount, 0, MAX_LIFE);
            Rigidbody.Velocity = new vec2(0, Rigidbody.Velocity.y);
            Animator.Parameters.SetTrigger(HIT_DAMAGE_PROPERTY_NAME);
            Animator.SetState(HIT_ANIM_STATE);

            CameraShake.Instance.BurstShake(30, 0.19f, 0.09f);

            return true;
        }

        public bool IsCharacterAlive()
        {
            return Inventory.Life > 0;
        }
        protected override void OnFixedUpdate()
        {
            if (_characterConfig.Ground.Enabled)
            {
                CheckGround();
            }

            if (IsCharacterAlive())
            {
                Rigidbody.Velocity = new vec2(Rigidbody.Velocity.x, Math.Clamp(Rigidbody.Velocity.y, _maxFallYVelocity, float.MaxValue));
            }
            else
            {
                Rigidbody.Velocity = new vec2(0, Rigidbody.Velocity.y > 0 ? 0 : Rigidbody.Velocity.y);

            }
        }

        private void CheckGround()
        {
            var origin1 = Transform.WorldPosition + new vec3(_characterConfig.Ground.MinX, _characterConfig.Ground.YOffset, 0);
            var origin2 = Transform.WorldPosition + new vec3(_characterConfig.Ground.MaxX, _characterConfig.Ground.YOffset, 0);
            var raysCount = _characterConfig.Ground.RaysCount;

            var dir = Transform.Up * -_characterConfig.Ground.SizeY;

            uint hitIndex = 0;
            CastHit2D hit = default;
            for (var i = 0; i < raysCount; i++)
            {
                var pos = Mathf.Lerp(origin1, origin2, i / (float)(raysCount - 1));
                var hits = Physics2D.RaycastAll(pos, dir, _characterConfig.Ground.GroundMask);
                for (var j = 0; j < CastHit2DArray.Capacity; j++)
                {
                    hit = hits[j];

                    if (hit.isHit && Mathf.Dot(vec2.Up, hit.Normal) > 0.001f)
                    {
                        hitIndex |= 1u << i;
                        break;
                    }
                }

                if (hit.isHit)
                    break;
            }
            if (Physics2D.DrawColliders)
            {
                for (var i = 0; i < raysCount; i++)
                {
                    var pos = Mathf.Lerp(origin1, origin2, i / (float)(raysCount - 1));
                    var color = Color.White;
                    //if ((hitIndex & (1 << i)) != 0)
                    //{
                    //    color = Color.Red;
                    //}
                    Debug.DrawRay(pos, dir, color);
                }
            }
            if (hit.isHit && !IsOnGround && Rigidbody.Velocity.y <= 0)
            {
                const float bias = 0.06f;
                var yPos = (hit.Point.y - Collider.AABB.Min.y) + bias;
                Transform.WorldPosition = new vec3(Transform.WorldPosition.x, yPos, Transform.WorldPosition.z); 
            }
            IsOnGround = hit.isHit;
        }
        private void OnGroundChanged(bool value)
        {
            Rigidbody.GravityScale = value ? 0 : _characterConfig.YGravityScale;

            if (value && Rigidbody.Velocity.y < 0)
            {
                Rigidbody.Velocity = new vec2(Rigidbody.Velocity.x, 0);
            }

            if (value)
            {

                _jumped = false;
            }
        }

        public void Restart()
        {
            if (!IsCharacterAlive())
            {
                Inventory.Life = _characterConfig.StartingLife;
            }
            Renderer.IsEnabled = true;
            Animator.Play(_main.Name);
        }

        protected void PlayJumpSoundFx()
        {
            if (_jumpSfx != null && _jumpSfx.Length > 0)
                AudioSource.PlayOneShot(_jumpSfx[0], 0.2f);
        }

        protected void PlayWalkSoundFx()
        {
            if (_walkFx != null && _walkFx.Length > 0)
                AudioSource.PlayOneShot(_walkFx[0], 0.2f);
        }

        protected void PlayAttackSoundFx()
        {
            if (_attackSfx != null && _attackSfx.Length > 0)
                AudioSource.PlayOneShot(_attackSfx[0], 0.2f);
        }

        protected void PlayGroundSoundFx()
        {
            if (_groundSfx != null && _groundSfx.Length > 0)
                AudioSource.PlayOneShot(_groundSfx[0], 0.2f);
        }

        protected struct AnimationsStates
        {
            public AnimationStateInfo Idle;
            public AnimationStateInfo Walk;
            public AnimationStateInfo Jump;
            public AnimationStateInfo Fall;
            public AnimationStateInfo Hit;
            public AnimationStateInfo Death;
            public AnimationStateInfo Attack;
            public const int Length = 7;
            public AnimationStateInfo this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return Idle;
                        case 1:
                            return Walk;
                        case 2:
                            return Jump;
                        case 3:
                            return Fall;
                        case 4:
                            return Hit;
                        case 5:
                            return Death;
                        case 6:
                            return Attack;
                        default:
                            return default;
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            Idle = value;
                            break;
                        case 1:
                            Walk = value;
                            break;
                        case 2:
                            Jump = value;
                            break;
                        case 3:
                            Fall = value;
                            break;
                        case 4:
                            Hit = value;
                            break;
                        case 5:
                            Death = value;
                            break;
                        case 6:
                            Attack = value;
                            break;
                    }
                }
            }
        }

        protected struct AnimationStateInfo
        {
            public bool IsEnabled;
            public string SpriteAtlasId;
            public float Fps;
            public AnimEvent[] Events;
        }

    }

    public struct AnimEvent
    {
        public float Time;
        public Action Callback;
    }
}
