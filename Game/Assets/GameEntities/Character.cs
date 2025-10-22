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
        public vec2 Offset { get; set; }
        public vec2 Size { get; set; }
    }

    public struct CharacterConfig
    {
        public float JumpSpeed { get; set; }
        public float WalkSpeed { get; set; }
        public float YGravityScale { get; set; }
        public int StartingLife { get; set; }
        public BodyColliderOptions ColliderConfig { get; set; }
        public string LayerName { get; set; }
        public int SortOrder { get; set; }
        public vec2 StartPosition { get; set; }
        public Material Material { get; set; }

        public string[] JumpSounds { get; set; }
        public string[] WalkSounds { get; set; }
        public string[] AttackSounds { get; set; }
        public string[] GroundSounds { get; set; }
    }

    internal abstract class Character : ScriptBehavior
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
        protected const int MAX_LIFE = 10;
        protected readonly string[] Attacks = ["Attack1", "Attack2", "Attack3", "Attack4", "Attack5", "Attack6"];

        private bool _isOnGround = false;
        protected bool IsOnGround
        {
            get => _isOnGround;
            set
            {
                if (_isOnGround == value)
                    return;

                _isOnGround = value;
                OnGroundChanged(value);
            }
        }
        private float _maxFallYVelocity = -20;

        private int _life;
        public int Life { get => _life; private set { _life = value; Animator.Parameters.SetInt(LIFE_PROPERTY_NAME, _life); } }

        private CharacterConfig _characterConfig;
        private AnimationState _main;
        private int _startingLife;
        private AudioClip[] _groundSfx;
        private AudioClip[] _jumpSfx;
        private AudioClip[] _attackSfx;
        private AudioClip[] _walkFx;
        private float _gravityScale;
        public void Init(CharacterConfig config)
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
            _startingLife = config.StartingLife;
            _gravityScale = Rigidbody.GravityScale;
            Life = _startingLife;

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


        protected void AddSpriteAnimState(string stateName, bool makeMain, bool loop, bool useClipBlendTime, AnimatorTransition[] transitions, string spritePath, float fps, vec2 size, vec2 pivot)
        {
            var animClip = new AnimationClip(stateName, loop);
            var state = new AnimationState(stateName, animClip);

            if (makeMain)
            {
                _main = state;
            }
            var texture = Assets.GetTexture(spritePath);
            texture.PixelPerUnit = 16;
            var sprites = TextureAtlasUtils.SliceSprites(texture, (int)size.x, (int)size.y, pivot);

            var spriteCurve = new SpriteCurve();

            var unit = 1.0f / fps;
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteCurve.AddKeyFrame(unit * i, sprites[i]);
            }

            animClip.AddCurve(SPRITE_PROPERTY_NAME, spriteCurve);

            if (transitions != null)
            {
                var duration = animClip.Duration;
                for (int i = 0; i < transitions.Length; i++)
                {
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

        public override void OnLateUpdate()
        {
            Renderer.Sprite = Animator.GetSprite(SPRITE_PROPERTY_NAME);
            Animator.Parameters.SetFloat(VEL_X_PROP_NAME, Rigidbody.Velocity.x);
            Animator.Parameters.SetFloat(VEL_Y_PROP_NAME, Rigidbody.Velocity.y);

            Animator.Parameters.SetInt(VEL_X_PROP_NAME, (int)MathF.Round(Rigidbody.Velocity.x));
            Animator.Parameters.SetInt(VEL_Y_PROP_NAME, (int)MathF.Round(Rigidbody.Velocity.y));
            Animator.Parameters.SetBool(ON_GROUND_PROPERTY_NAME, IsOnGround);
        }

        protected void Jump()
        {
            if (!IsCharacterAlive())
                return;

            if (IsOnGround)
            {
                Rigidbody.GravityScale = _gravityScale;
                Rigidbody.AddForce(vec2.Up * _characterConfig.JumpSpeed, ForceMode2D.Impulse);
            }
        }

        protected void Walk(int dir)
        {
            if (!IsCharacterAlive())
                return;

            Rigidbody.Velocity = new vec2(_characterConfig.WalkSpeed * dir, Rigidbody.Velocity.y);

            if (dir != 0)
            {
                var scaleX = Math.Abs(Transform.LocalScale.x);
                Transform.LocalScale = new vec3(scaleX * dir, 1, 1);
            }
        }

        public virtual void Attack(int index = 0)
        {
            if (!IsCharacterAlive() || Animator.Parameters.HasTrigger(Attacks[index]))
                return;
            Animator.Parameters.SetTrigger(Attacks[index]);
        }

        protected virtual void Death()
        {
            if (!IsCharacterAlive())
                return;

            Rigidbody.Velocity = new vec2(0, Rigidbody.Velocity.y > 0 ? 0 : Rigidbody.Velocity.y);
            HitDamage(MAX_LIFE);
        }

        public virtual void HitDamage(int amount)
        {
            if (!IsCharacterAlive())
                return;

            var life = Math.Clamp(Life - amount, 0, MAX_LIFE);
            Rigidbody.Velocity = new vec2(0, Rigidbody.Velocity.y);
            Animator.Parameters.SetTrigger(HIT_DAMAGE_PROPERTY_NAME);
            Life = life;
        }

        protected bool IsCharacterAlive()
        {
            return Life > 0;
        }
        public override void OnFixedUpdate()
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
        private void OnGroundChanged(bool value)
        {
            Rigidbody.GravityScale = value ? 0 : _gravityScale;

            if (value && Rigidbody.Velocity.y <= 0)
            {
                Rigidbody.Velocity = new vec2(Rigidbody.Velocity.x, 0);
            }
        }

        public void Restart()
        {
            if (!IsCharacterAlive())
            {
                Life = _startingLife;
                Animator.SetState(_main);
            }
        }
    }
}
