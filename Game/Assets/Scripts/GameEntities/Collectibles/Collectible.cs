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
    [RequireComponent(typeof(SpriteRenderer), typeof(RigidBody2D), typeof(AudioSource), typeof(BoxCollider2D))]
    public class Collectible : GameEntity
    {
        private CollectibleConfig _config;
        private CircleCollider2D _worldCollider;

        [RequiredProperty] protected SpriteRenderer Renderer { get; private set; }
        [RequiredProperty] protected AudioSource AudioSource { get; private set; }
        [RequiredProperty] protected RigidBody2D RigidBody { get; private set; }
        [RequiredProperty] protected BoxCollider2D Collider { get; private set; }

        public class CollectibleConfig()
        {
            public int TargetLayer { get; set; }
            public ItemId Item { get; set; }
            public int Amount { get; set; }
            public Sprite Sprite { get; set; }
            public AudioClip CollectedAudioClip { get;  set; }
            public vec2 TriggerSize { get; set; }
            public vec2 ForceDir { get; set; }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            RigidBody.BodyType = Body2DType.Dynamic;
            RigidBody.Interpolate = false;
            RigidBody.GravityScale = 3;
            RigidBody.AngularDamping = 2;
            Renderer.Material = GameMaterials.Instance.SpriteMaterial;
            Renderer.SortOrder = 2;

            Collider.IsTrigger = true;

            Actor.Layer = LayerMask.NameToLayer(GameConsts.COLLECTIBLE);

            var worldColliderActor = new Actor("WorldCollider");
            worldColliderActor.Layer = LayerMask.NameToLayer(GameConsts.CHARACTER_IGNORE);
            worldColliderActor.Transform.Parent = Actor.Transform;
            worldColliderActor.Transform.LocalPosition = default;
            _worldCollider = worldColliderActor.AddComponent<CircleCollider2D>();
            _worldCollider.Bounciness = 0.5f;
            _worldCollider.Radius = 0.49f;
            _worldCollider.Friction = 0.3f;
            Renderer.Transform.LocalScale *= 1.7f;
            var circleRenderer = new Actor("Circle").AddComponent<SpriteRenderer>();
            circleRenderer.Transform.Parent = worldColliderActor.Transform;
            circleRenderer.Sprite = GameTextures.GetSprite("outlineCircle");
            circleRenderer.Material = GameMaterials.Instance.SpriteMaterial;
            circleRenderer.SortOrder = Renderer.SortOrder;
            circleRenderer.Transform.LocalScale = vec3.One * 0.6f;
        }

        public void Init(CollectibleConfig config)
        {
            _config = config;
            Renderer.Sprite = config.Sprite;
            Collider.Size = config.TriggerSize;
            RigidBody.AddForce(config.ForceDir, ForceMode2D.Impulse);
            Collider.IsEnabled = false;

            TimedExecute(() => Collider.IsEnabled = true, 0.8f);
        }

        public void Restore()
        {
            Actor.IsActiveSelf = true;
        }

        public void Disable()
        {
            Actor.IsActiveSelf = false;
        }

        protected sealed override void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.Actor.Layer == _config.TargetLayer)
            {
                var character = collider.GetComponent<Character>();

                if (character && character.IsCharacterAlive())
                {
                    character.Inventory.Add(_config.Item, _config.Amount);
                    AudioSource.PlayOneShot(_config.CollectedAudioClip, 0.1f);
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
