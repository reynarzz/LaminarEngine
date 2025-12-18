using Box2D.NET;
using Engine;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections;
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
        public float NormalJumpHeightThreshold;
        public float MaxJumpHeight;
        public float WalkSpeed;
        public float YGravityScale;
        public int StartingLife;
        public int MaxLife;
        public int SpriteLookDirFlip;
        public BodyColliderOptions ColliderConfig;
        public GroundDetectionOptions Ground;
        public string LayerName;
        public int SortOrder;
        public vec2 StartPosition;
        public int InventoryMaxSlots;
        public ItemAmountPair[] StartInventoryValues;
        public Material Material;
        public string[] JumpSounds;
        public string[] WalkSounds;
        public string[] AttackSounds;
        public string[] GroundSounds;
        public string[] HitSounds;

        public float WalkSoundsVolume;
        public float AttackSoundsVolume;
        public float HitSoundsVolume;
        public float JumpSoundsVolume;
        public float HitRecoilTime;
        public float HitRecoilStrengthScaling;
        public int HitInvincibilityBlinks;
        public bool DisappearOnDead;
        public bool PushAwayFromWalls;
        public int StartLookDir;

    }

    public abstract class Character : GameEntity
    {
        protected Animator Animator { get; private set; }
        protected SpriteRenderer Renderer { get; private set; }
        protected RigidBody2D Rigidbody { get; private set; }
        protected Collider2D Collider { get; private set; }
        protected AudioSource AudioSource { get; private set; }
        public event Action OnCharacterDead;

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
        protected float _currentHitRecoilTime = 0;
        private bool _isOnGround = false;
        public bool IsOnGround
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
        private bool _isStopBoxHit = false;
        private float _maxFallYVelocity = -20;

        public CharacterInventory Inventory { get; protected set; }

        public int SpriteLookDir => _characterConfig.SpriteLookDirFlip;
        public int LookDir { get; private set; }
        private CharacterConfig _characterConfig;
        private AnimationState _main;
        private AudioClip[] _groundSfx;
        private AudioClip[] _jumpSfx;
        private AudioClip[] _attackSfx;
        private AudioClip[] _walkFx;
        private AudioClip[] _hitSfx;
        private bool _jumped = false;
        private AnimationState _deadState;
        private bool _jumping = false;
        private float _jumpStartY;

        public bool IsInvencible { get; protected set; }
        public bool IsEnteringThroughDoor { get; protected set; }
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
            Inventory.MaxLife = _characterConfig.MaxLife;
            Inventory.Life = _characterConfig.StartingLife;
            InitAudio(config);
            LookAt(_characterConfig.StartLookDir);
        }

        private void InitAudio(CharacterConfig config)
        {
            AudioClip[] GetClips(string[] soundsPath)
            {
                if (soundsPath == null || soundsPath.Length == 0)
                    return null;

                var clips = new List<AudioClip>();
                for (int i = 0; i < soundsPath.Length; i++)
                {
                    if (!string.IsNullOrEmpty(soundsPath[i]))
                        clips.Add(Assets.GetAudioClip(soundsPath[i]));
                }
                return clips.ToArray();
            }

            _groundSfx = GetClips(config.GroundSounds);
            _jumpSfx = GetClips(config.JumpSounds);
            _attackSfx = GetClips(config.AttackSounds);
            _walkFx = GetClips(config.WalkSounds);
            _hitSfx = GetClips(config.HitSounds);
        }

        protected void InitAnimationStates(AnimationsStates statesConfig)
        {
            Animator.Clear();

            AnimatorTransition toJump = null;
            AnimatorTransition toFall = null;
            AnimatorTransition toIdle = null;
            AnimatorTransition toWalk = null;
            AnimatorTransition toWalkYVeloDown = null;
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

                toWalkYVeloDown = new AnimatorTransition(WALK_ANIM_STATE, [new IntCondition(VEL_X_PROP_NAME, 0, IntOp.NotEqual),
                                                        new BoolCondition(ON_GROUND_PROPERTY_NAME, true),
                                                        new IntCondition(VEL_Y_PROP_NAME, 0, IntOp.LessThanOrEqual),
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
                toAttack = new AnimatorTransition(ATTACK_ANIM_STATE, [new TriggerCondition(Attacks[0]),
                                                         new IntCondition(LIFE_PROPERTY_NAME, 0, IntOp.GreaterThan)]);
            }
            if (statesConfig.Hit.IsEnabled)
            {
                toHit = new AnimatorTransition(HIT_ANIM_STATE, new TriggerCondition(HIT_DAMAGE_PROPERTY_NAME));
            }
            if (statesConfig.Death.IsEnabled)
            {
                toDeath = new AnimatorTransition(DEATH_ANIM_STATE, new TriggerCondition(DEATH_PROPERTY_NAME));
                toDeathLife0 = new AnimatorTransition(DEATH_ANIM_STATE, [new IntCondition(LIFE_PROPERTY_NAME, 0, IntOp.LessThanOrEqual),
                                                                        new BoolCondition(ON_GROUND_PROPERTY_NAME, true)]);
            }

            if (statesConfig.Idle.IsEnabled)
            {
                AddSpriteAnimState(IDLE_ANIM_STATE, true, true, false, [toWalk, toJump, toFall, toAttack, toHit], statesConfig.Idle.SpriteAtlasId, statesConfig.Idle.Fps, statesConfig.Idle.Events);
            }
            if (statesConfig.Walk.IsEnabled)
            {
                AddSpriteAnimState(WALK_ANIM_STATE, false, true, false, [toIdle, toJump, toFall, toAttack, toHit], statesConfig.Walk.SpriteAtlasId, statesConfig.Walk.Fps, statesConfig.Walk.Events);
            }
            if (statesConfig.Jump.IsEnabled)
            {
                AddSpriteAnimState(JUMP_ANIM_STATE, false, false, false, [toIdle, toFall, toWalkYVeloDown, toAttack, toHit], statesConfig.Jump.SpriteAtlasId, statesConfig.Jump.Fps, statesConfig.Jump.Events);
            }
            if (statesConfig.Fall.IsEnabled)
            {
                AddSpriteAnimState(FALL_ANIM_STATE, false, false, false, [toIdle, toWalk, toAttack, toHit], statesConfig.Fall.SpriteAtlasId, statesConfig.Fall.Fps, statesConfig.Fall.Events);
            }
            if (statesConfig.Attack.IsEnabled)
            {
                AddSpriteAnimState(ATTACK_ANIM_STATE, false, false, true, [toIdle, toWalk, toFall, toHit], statesConfig.Attack.SpriteAtlasId, statesConfig.Attack.Fps, statesConfig.Attack.Events);
            }
            if (statesConfig.Death.IsEnabled)
            {
                _deadState = AddSpriteAnimState(DEATH_ANIM_STATE, false, false, true, null, statesConfig.Death.SpriteAtlasId, statesConfig.Death.Fps, statesConfig.Death.Events);
                _deadState.Clip.AddEvent(0, () =>
                {
                    Inventory?.DropAll(Transform.WorldPosition);
                });
            }
            if (statesConfig.Hit.IsEnabled)
            {
                AddSpriteAnimState(HIT_ANIM_STATE, false, false, true, [toIdle, toWalk, toJump, toFall, toAttack, toDeathLife0], statesConfig.Hit.SpriteAtlasId, statesConfig.Hit.Fps, statesConfig.Hit.Events);
            }

            Renderer.Sprite = GameTextures.GetAtlas(statesConfig.Idle.SpriteAtlasId).FirstOrDefault();
        }

        protected AnimationState AddSpriteAnimState(string stateName, bool makeMain, bool loop, bool useClipBlendTime,
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

            return state;
        }

        protected sealed override void OnLateUpdate()
        {
            Renderer.Sprite = Animator.GetSprite(SPRITE_PROPERTY_NAME);
            Animator.Parameters.SetFloat(VEL_X_PROP_NAME, Rigidbody.Velocity.x);
            Animator.Parameters.SetFloat(VEL_Y_PROP_NAME, Rigidbody.Velocity.y);
            Animator.Parameters.SetInt(VEL_X_PROP_NAME, MathF.Sign(Math.Abs(Rigidbody.Velocity.x) < 0.09 ? 0 : Rigidbody.Velocity.x));
            Animator.Parameters.SetInt(VEL_Y_PROP_NAME, MathF.Sign(Math.Abs(Rigidbody.Velocity.y) < 0.09 ? 0 : Rigidbody.Velocity.y));
            Animator.Parameters.SetInt(LIFE_PROPERTY_NAME, Inventory.Life);
            Animator.Parameters.SetBool(ON_GROUND_PROPERTY_NAME, IsOnGround);

            _currentHitRecoilTime = Math.Clamp(_currentHitRecoilTime - Time.DeltaTime, -1, _currentHitRecoilTime);

            if (!IsCharacterAlive() && Animator.CurrentState != _deadState && IsOnGround)
            {
                PlayDeadAnim();
            }
        }

        private void PlayDeadAnim()
        {
            Animator.Play(DEATH_ANIM_STATE);
            // Actor.Layer = LayerMask.NameToLayer(GameConsts.CHARACTER_DEAD);

            if (_characterConfig.DisappearOnDead)
            {
                IEnumerator Disappear()
                {
                    float t = Renderer.Color.A;
                    yield return new WaitForSeconds(1f);
                    while (t > 0)
                    {
                        t -= Time.DeltaTime;

                        Renderer.Color = new Color(Renderer.Color.R, Renderer.Color.G, Renderer.Color.B, t);
                        yield return null;
                    }

                    Renderer.Color = new Color(Renderer.Color.R, Renderer.Color.G, Renderer.Color.B, 0);
                    Actor.IsActiveSelf = false;
                }

                StartCoroutine(Disappear());
            }
        }

        public void Jump()
        {
            if (!CanCharacterMove())
                return;

            if (IsOnGround)
            {
                _jumped = true;
                Rigidbody.GravityScale = _characterConfig.YGravityScale;
                Rigidbody.Velocity = new vec2(Rigidbody.Velocity.x, 0);
                Rigidbody.AddForce(vec2.Up * _characterConfig.JumpForce, ForceMode2D.Impulse);
            }
        }

        public void BeginJump()
        {
            if (!CanCharacterMove() || !IsOnGround || _jumping)
                return;

            _jumping = true;
            _jumpStartY = Transform.WorldPosition.y;

            Rigidbody.GravityScale = _characterConfig.YGravityScale;
            float gravity = MathF.Abs(Physics2D.Gravity.y * Rigidbody.GravityScale);
            float jumpVelocity = MathF.Sqrt(2 * gravity * _characterConfig.MaxJumpHeight);

            Rigidbody.Velocity = new vec2(Rigidbody.Velocity.x, jumpVelocity);

            PlayJumpSFX();
        }

        public void EndJump()
        {
            var diff = Transform.WorldPosition.y - _jumpStartY;
            //Debug.Log(diff + ", " + _characterConfig.NormalJumpHeightThreshold);
            var isNormalJump = diff >= _characterConfig.NormalJumpHeightThreshold;
            if (Rigidbody.Velocity.y > 0 && !isNormalJump)
            {
                Rigidbody.Velocity = new vec2(Rigidbody.Velocity.x, Rigidbody.Velocity.y * 0.5f);
            }

            _jumping = false;
        }

        protected override void OnFixedUpdate()
        {
            if (_characterConfig.Ground.Enabled)
            {
                CheckGround();
            }

            if (_currentHitRecoilTime <= 0)
            {
                if (IsCharacterAlive())
                {
                    Rigidbody.Velocity = new vec2(Rigidbody.Velocity.x, Math.Clamp(Rigidbody.Velocity.y, _maxFallYVelocity, float.MaxValue));
                }
                else
                {
                    Rigidbody.Velocity = new vec2(0, Rigidbody.Velocity.y > 0 ? 0 : Rigidbody.Velocity.y);

                }
            }

            //CheckPushWall();
        }

        // Push the character back when is touching a wall, this prevents the collider to slide when jumping.
        private bool CheckPushWall()
        {
            if (!_characterConfig.PushAwayFromWalls)
                return false;

            vec3 origin = Transform.WorldPosition + new vec3(Transform.WorldScale.x * _characterConfig.SpriteLookDirFlip * 0.5f, 0.22f);
            vec3 size = new vec3(0.3f, 1.5f);
            var boxWallkhit = Physics2D.BoxCast(origin, size, GameConsts.GROUND_MASK);

            if (boxWallkhit.isHit)
            {
                Rigidbody.Velocity = new vec2(-(Transform.WorldScale.x * _characterConfig.SpriteLookDirFlip) * 0.55f, Rigidbody.Velocity.y);

                if (Physics2D.DrawColliders)
                {
                    Debug.DrawBox(boxWallkhit.Point, vec3.One * 0.1f, Color.Green);
                }

                var dist = origin - new vec3(boxWallkhit.Point, 0);
               // Transform.WorldPosition += new vec3(dist.x, 0, 0);
            }
            if (Physics2D.DrawColliders)
            {
                Debug.DrawBox(origin, size, Color.Red);
            }

            return boxWallkhit.isHit;
        }
        public void LookAt(int dir)
        {
            if (dir == 0)
            {
                dir = 1;
                Debug.Error("Look dir is 0, it should be either 1 or -1");
            }

            LookDir = dir;
            var scaleX = Math.Abs(Transform.WorldScale.x);
            Transform.WorldScale = new vec3(scaleX * dir * Math.Sign(_characterConfig.SpriteLookDirFlip), Transform.WorldScale.y, Transform.WorldScale.z);
        }


        protected bool CanCharacterMove()
        {
            return IsCharacterAlive() && _currentHitRecoilTime <= 0;
        }
        public void Walk(int dir)
        {
            if (!CanCharacterMove())
                return;
            dir *= _characterConfig.SpriteLookDirFlip;
            
            if (dir != 0)
            {

                //if (CheckPushWall())
                //{
                //    return;
                //}

                LookAt(dir);
                
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
            if (!CanCharacterMove() || Animator.Parameters.HasTrigger(Attacks[index]))
                return false;
            Animator.Parameters.SetTrigger(Attacks[index]);
            return true;
        }

        protected virtual void Death()
        {
            if (!IsCharacterAlive())
                return;

            Rigidbody.Velocity = new vec2(0, Rigidbody.Velocity.y > 0 ? 0 : Rigidbody.Velocity.y);
            HitDamage(this, MAX_LIFE);
        }

        private IEnumerator HitEffect(int blinks)
        {
            float endAngle = (float)Mathf.PI / 2f + 2f * (float)Mathf.PI * blinks;
            float angle = 0f;
            var color = Renderer.Color;

            while (angle < endAngle)
            {
                const float freq = 30;
                angle += Time.DeltaTime * freq;
                float alpha = MathF.Sin(angle) * 0.5f + 0.5f;
                Renderer.Color = new Color(Renderer.Color.R, Renderer.Color.G, Renderer.Color.B, alpha);

                yield return null;
            }
            if (color.A < 0.9)
            {
                Debug.Error("Fix: character color is not being restarted correctly.");
            }
            color.A = 1;
            Renderer.Color = color;
            IsInvencible = false;
        }

        public virtual bool HitDamage(GameEntity who, int amount)
        {
            if (!IsCharacterAlive() || IsInvencible || IsEnteringThroughDoor)
                return false;

            Inventory.Life = Math.Clamp(Inventory.Life - amount, 0, MAX_LIFE);
            Rigidbody.GravityScale = _characterConfig.YGravityScale;

            var damageDir = (Transform.WorldPosition - who.Transform.WorldPosition).Normalized;
            float max = 50;
            if (IsOnGround)
            {
                damageDir.y = (float)(max / 100.0f) * _characterConfig.HitRecoilStrengthScaling;
            }
            else
            {
                damageDir.y = 0;
            }
            Rigidbody.Velocity = damageDir * _characterConfig.HitRecoilStrengthScaling;
            _currentHitRecoilTime = _characterConfig.HitRecoilTime;

            if (Inventory.Life > 0)
            {
                Animator.Parameters.SetTrigger(HIT_DAMAGE_PROPERTY_NAME);
                Animator.SetState(HIT_ANIM_STATE);

                if (_characterConfig.HitInvincibilityBlinks > 0 && !IsInvencible)
                {
                    IsInvencible = true;


                    StartCoroutine(HitEffect(_characterConfig.HitInvincibilityBlinks));
                }
            }
            else
            {
                Animator.SetState(HIT_ANIM_STATE);
                Animator.Parameters.SetTrigger(DEATH_ANIM_STATE);
                OnCharacterDead?.Invoke();
            }

            PlayHitSFX();
            return true;
        }



        public bool IsCharacterAlive()
        {
            return Inventory.Life > 0;
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
                _currentHitRecoilTime = 0;
                _jumped = false;
                _jumping = false;
                if (!IsCharacterAlive())
                {
                    CameraShake.Instance.BurstShake(20, 0.1f, 0.2f);
                }
            }
        }

        public void Restart()
        {
            if (!IsCharacterAlive())
            {
                Inventory.Life = _characterConfig.StartingLife;
            }

#if DEBUG
            Renderer.IsEnabled = true;
            Animator.Play(_main.Name);
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            OnCharacterDead = null;
        }

        protected void PlayHitSFX()
        {
            if (_hitSfx != null && _hitSfx.Length > 0)
            {
                var index = Random.Shared.Next(0, _hitSfx.Length);
                AudioSource.PlayOneShot(_hitSfx[index], _characterConfig.HitSoundsVolume);
            }
        }
        protected void PlayJumpSFX()
        {
            if (_jumpSfx != null && _jumpSfx.Length > 0)
            {
                AudioSource.PlayOneShot(_jumpSfx[0], _characterConfig.JumpSoundsVolume);
            }
        }

        protected void PlayWalkSFX()
        {
            if (_walkFx != null && _walkFx.Length > 0)
            {
                var index = Random.Shared.Next(0, _hitSfx.Length);
                AudioSource.PlayOneShot(_walkFx[index], _characterConfig.WalkSoundsVolume);
            }
        }

        protected void PlayAttackSFX()
        {
            if (_attackSfx != null && _attackSfx.Length > 0)
            {
                AudioSource.PlayOneShot(_attackSfx[0], _characterConfig.AttackSoundsVolume);
            }
        }

        protected void PlayGroundSFX()
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
