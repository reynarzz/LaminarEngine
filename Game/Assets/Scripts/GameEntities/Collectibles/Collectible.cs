using Engine;
using Engine.Types;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequireComponent(typeof(Animator))]
    public class Collectible : GameEntity
    {
        private Animator _animator;
        private CollectibleConfig _config;
        private SpriteRenderer _renderer;
        private BoxCollider2D _collider;
        private CircleCollider2D _worldCollider;
        protected AudioSource AudioSource { get; private set; }
        private AnimationState _idleState;
        private bool _initializedComponents = false;

        public struct CollectibleConfig()
        {
            public int TargetLayer { get; set; }
            public ItemId Item { get; set; }
            public int Amount { get; set; }
            public vec2 TriggerSize { get; set; }
            public float AnimFPS { get; set; }
            public Sprite[] IdleSprites { get; set; }
            public Sprite[] CollectedSprites { get; set; }
            public AudioClip CollectedAudioClip { get; set; }
            // public AnimEvent MyProperty { get; set; }
        }
        public void Init(CollectibleConfig config)
        {
            _config = config;

            if (!_initializedComponents)
            {
                _initializedComponents = true;
                Actor.Layer = LayerMask.NameToLayer(GameConsts.COLLECTIBLE);
                _animator = GetComponent<Animator>();
                _renderer = AddComponent<SpriteRenderer>();
                var rigid = AddComponent<RigidBody2D>();
                AudioSource = AddComponent<AudioSource>();

                rigid.BodyType = Body2DType.Dynamic;
                rigid.LockZRotation = true;
                rigid.Interpolate = true;

                _renderer.Material = GameManager.DefaultMaterial;
                _renderer.SortOrder = 0;
                _collider = AddComponent<BoxCollider2D>();

                var worldColliderActor = new Actor("WorldCollider");
                worldColliderActor.Layer = LayerMask.NameToLayer(GameConsts.CHARACTER_IGNORE);
                worldColliderActor.Transform.Parent = Actor.Transform;
                worldColliderActor.Transform.LocalPosition = default;
                _worldCollider = worldColliderActor.AddComponent<CircleCollider2D>();

                _collider.IsTrigger = true;
            }
            else
            {
                _animator.Clear();
            }

            _collider.Size = _config.TriggerSize;
            _worldCollider.Radius = 0.2f;
            _worldCollider.Bounciness = 0.3f;
            var idle = GetState("Idle", _config.AnimFPS, _config.IdleSprites);
            var collected = GetState("Collected", _config.AnimFPS, _config.CollectedSprites);

            if (idle != null)
            {
                _animator.AddState(idle);
            }

            if (collected != null)
            {
                _animator.AddState(collected);
            }
        }

        public void Restore()
        {
            _collider.IsEnabled = true;
            _worldCollider.IsEnabled = true;
            _renderer.IsEnabled = true;
            _animator.IsEnabled = false;

            if (_idleState != null)
            {
                _animator.SetState(_idleState);
            }
        }

        public void Disable()
        {
            _worldCollider.IsEnabled = false;
            _collider.IsEnabled = false;
            _renderer.IsEnabled = false;
            _animator.IsEnabled = false;
        }

        protected override void OnUpdate()
        {
            var sprite = _animator.GetSprite("Sprite");

            if (sprite)
            {
                _renderer.Sprite = sprite;
            }
        }

        private AnimationState GetState(string name, float fps, Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0)
                return null;

            var clip = new AnimationClip(name);
            var spritesCurve = new SpriteCurve(_config.AnimFPS, sprites);

            clip.AddCurve("Sprite", spritesCurve);

            return new AnimationState(name, clip);
        }

        protected sealed override void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.Actor.Layer == _config.TargetLayer)
            {
                var character = collider.GetComponent<Character>();

                if (character)
                {
                    character.Inventory.Add(_config.Item, _config.Amount);
                    AudioSource.PlayOneShot(_config.CollectedAudioClip, 0.2f);
                    Disable();
                }
                OnTargetCollided(true);
            }
        }

        protected sealed override void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.Actor.Layer == _config.TargetLayer)
            {
                OnTargetCollided(false);
            }
        }

        public virtual void OnTargetCollided(bool collision)
        {
        }
    }
}
