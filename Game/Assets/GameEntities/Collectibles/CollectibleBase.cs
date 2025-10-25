using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public enum GameItem
    {
        None,
        Coin,
        Health
    }

    public abstract class CollectibleBase : GameEntity
    {
        private Animator _animator;
        private CollectibleConfig _config;
        private SpriteRenderer _renderer;
        private BoxCollider2D _collider;
        protected AudioSource AudioSource { get; private set; }
        private AnimationState _idleState;
        protected struct CollectibleConfig()
        {
            public int TargetLayer { get; set; }
            public GameItem Item { get; set; }
            public int Amount { get; set; }
            public vec2 TriggerSize { get; set; }
            public float AnimFPS { get; set; }
            public Sprite[] IdleSprites { get; set; }
            public Sprite[] CollectedSprites { get; set; }
            // public AnimEvent MyProperty { get; set; }
        }

        public sealed override void OnAwake()
        {
            Actor.Layer = LayerMask.NameToLayer(GameLayers.COLLECTIBLE);
            _animator = AddComponent<Animator>();
            _renderer = AddComponent<SpriteRenderer>();
            var rigid = AddComponent<RigidBody2D>();
            AudioSource = AddComponent<AudioSource>();

            rigid.BodyType = Body2DType.Kinematic;
            _renderer.Material = GameManager.DefaultMaterial;
            _renderer.SortOrder = 0;
            _collider = AddComponent<BoxCollider2D>();

            _collider.IsTrigger = true;

            _config = Init();
            _collider.Size = _config.TriggerSize;

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
            _renderer.IsEnabled = true;
            _animator.IsEnabled = false;

            if (_idleState != null)
            {
                _animator.SetState(_idleState);
            }
        }

        public void Disable()
        {
            _collider.IsEnabled = false;
            _renderer.IsEnabled = false;
            _animator.IsEnabled = false;
           
        }

        public override void OnUpdate()
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
            var spritesCurve = new SpriteCurve();

            clip.AddCurve("Sprite", spritesCurve);

            for (var i = 0; i < sprites.Length; i++)
            {
                spritesCurve.AddKeyFrame((1.0f / _config.AnimFPS) * i, sprites[i]);
            }

            return new AnimationState(name, clip);
        }

        protected abstract CollectibleConfig Init();

        protected void Collect()
        {
            switch (_config.Item)
            {
                case GameItem.None:
                    break;
                case GameItem.Coin:
                    GameManager.PlayerBag.Coins += _config.Amount;
                    break;
                case GameItem.Health:
                    GameManager.Player.Life += _config.Amount;
                    break;
            }
        }

        public sealed override void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.Actor.Layer == _config.TargetLayer)
            {
                OnTargetCollided(true);
            }
        }

        public sealed override void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.Actor.Layer == _config.TargetLayer)
            {
                OnTargetCollided(false);
            }
        }

        public abstract void OnTargetCollided(bool collision);
    }
}
