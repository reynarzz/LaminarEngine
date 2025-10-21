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
        public BodyColliderOptions ColliderConfig { get; set; }
        public string LayerName { get; set; }
        public int SortOrder { get; set; }
        public vec2 StartPosition { get; set; }
        public Material Material { get; set; }
    }

    internal abstract class Character : ScriptBehavior
    {
        protected Animator Animator { get; private set; }
        protected SpriteRenderer Renderer { get; private set; }
        protected RigidBody2D Rigidbody { get; private set; }
        protected Collider2D Collider { get; private set; }
        protected AudioSource AudioSource { get; private set; }

        protected const string SPRITE_PROPERTY_NAME = "Sprite_Property";
        protected const string VELOCITY_X_PROPERTY_NAME = "VelocityX";
        protected const string VELOCITY_Y_PROPERTY_NAME = "VelocityY";
        protected const string ON_GROUND_PROPERTY_NAME = "IsOnGround";
        private readonly string[] _attacks = ["Attack1", "Attack2", "Attack3", "Attack4", "Attack5", "Attack6"];
        protected bool IsOnGround { get; set; }

        private CharacterConfig _characterConfig;

        public void Init(CharacterConfig config)
        {
            Animator = AddComponent<Animator>();
            Renderer = AddComponent<SpriteRenderer>();
            Rigidbody = AddComponent<RigidBody2D>();

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
        }

        protected void AddSpriteAnimState(string stateName, bool makeMain, AnimatorTransition[] transitions, string spritePath, float fps, vec2 size, vec2 pivot)
        {
            var animClip = new AnimationClip(stateName);
            var state = new AnimationState(stateName, animClip);

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
                for (int i = 0; i < transitions.Length; i++)
                {
                    state.AddTransition(transitions[i]);
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

        public override void OnUpdate()
        {
            Renderer.Sprite = Animator.GetSprite(SPRITE_PROPERTY_NAME);
            Animator.Parameters.SetFloat(VELOCITY_X_PROPERTY_NAME, Rigidbody.Velocity.x);
            Animator.Parameters.SetFloat(VELOCITY_Y_PROPERTY_NAME, Rigidbody.Velocity.y);
            Animator.Parameters.SetBool(ON_GROUND_PROPERTY_NAME, IsOnGround);
        }

        protected void Jump()
        {
            Rigidbody.AddForce(vec2.Up * _characterConfig.JumpSpeed, ForceMode2D.Impulse);
        }

        protected void Walk(int dir)
        {
            Rigidbody.Velocity = new vec2(_characterConfig.WalkSpeed * dir, Rigidbody.Velocity.y);

            if (dir != 0)
            {
                Transform.LocalScale = new vec3(dir, 1, 1);
            }
        }

        public virtual void Attack(int index = 0)
        {
            Animator.Parameters.SetTrigger(_attacks[index]);
        }
    }
}
