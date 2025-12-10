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
    [RequireComponent(typeof(SpriteRenderer), typeof(RigidBody2D), typeof(BoxCollider2D))]
    internal class Bullet : GameEntity
    {
        private vec3 _shootDir;
        private ulong _layerMask;
        public bool _shoot = false;
        private float _speed = 0;
        private float _timeAlive = 1;
        [RequiredProperty] private SpriteRenderer _renderer;
        [RequiredProperty] private RigidBody2D _rigid;
        [RequiredProperty] private BoxCollider2D _collider;
        private ParticleSystem2D _particleSystem;

        protected override void OnAwake()
        {
            base.OnAwake();

            _rigid.BodyType = Body2DType.Static;
            Actor.Layer = LayerMask.NameToLayer(GameConsts.BULLET);
            _collider.Size = new vec2(1, 0.3f);
        }

        public void Shoot(vec2 position, vec2 dir, float speed, ulong layerMask)
        {
            Transform.WorldPosition = position;
            _shootDir = dir;
            _speed = speed;
            _layerMask = layerMask;
            _shoot = true;
            Transform.WorldScale = new vec3(0.6f, 0.18f);

            _renderer.Material = GameMaterials.Instance.SpriteMaterial;
            _timeAlive = 1;
            _particleSystem = GetParticles();
            CheckCollision();
        }

        protected override void OnUpdate()
        {
            Transform.WorldPosition += _shootDir * Time.DeltaTime * _speed;

            CheckCollision();

            // TODO: make it distance based instead.
            if ((_timeAlive -= Time.DeltaTime) <= 0)
            {
                PoolObject();
            }

            if(_particleSystem)
            _particleSystem.Transform.WorldPosition = Transform.WorldPosition;
        }

        private ParticleSystem2D GetParticles()
        {
            var particleSystem = new Actor("ParticleSystem").AddComponent<ParticleSystem2D>();
            particleSystem.EmitRate = 50;
            particleSystem.ParticleLife = 0.2f;
            particleSystem.SortOrder = _renderer.SortOrder;
            particleSystem.StartColor = new Color(1, 1, 1, 0.5f);
            particleSystem.EndColor = Color.Transparent;// new Color(0, 0, 0, 0);
            particleSystem.EndSize = new vec2(0, 0);
            particleSystem.Spread = new vec2(0.0f, 0);
            particleSystem.SimulationSpeed = 1;
            particleSystem.StartSize = new vec2(0.3f);
            particleSystem.IsWorldSpace = true;
            particleSystem.Gravity = default;
            particleSystem.VelocityMax = default;
            particleSystem.VelocityMin = default;
            particleSystem.AngularVelocity = 40;
            particleSystem.Material = GameMaterials.Instance.SpriteMaterial;
            particleSystem.Transform.LocalPosition = default;
            particleSystem.Transform.WorldScale = vec3.One;
            return particleSystem;
        }

        private void CheckCollision()
        {
            var hits = Physics2D.BoxCastAll(Transform.WorldPosition, Transform.LocalScale, _layerMask);
            if (Physics2D.DrawColliders)
            {
                Debug.DrawBox(Transform.WorldPosition, Transform.LocalScale, Color.Red);
            }

            for (int i = 0; i < hits.Length; i++)
            {
                ref var hit = ref hits[i];
                if (hit.isHit && Actor)
                {
                    var character = hit.Collider.GetComponent<Character>();
                    bool characterWasHit = false;
                    if (character)
                    {
                        if (character.IsCharacterAlive())
                        {
                            if(character.HitDamage(this, 1))
                            {
                                characterWasHit = true;
                            }
                            // character.GetComponent<RigidBody2D>().AddForce(_shootDir * 12, ForceMode2D.Impulse);
                        }
                        else
                        {
                            return;
                        }
                    }
                    if (characterWasHit)
                    {
                        CameraShake.Instance.BurstShake(30, 0.19f, 0.09f);
                    }
                    else
                    {
                        CameraShake.Instance.BurstShake(10, 0.09f, 0.09f);
                    }

                    PoolObject();
                    break;
                }
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            collider.AttachedRigidbody.AddForce(_shootDir * 5, ForceMode2D.Impulse);
        }
        public void PoolObject()
        {
            Actor.Destroy(Actor);
            _particleSystem.Pause();

            _particleSystem.OnEmitFinished += () =>
            {
                Actor.Destroy(_particleSystem);
            };


            _shoot = false;
            Actor.IsActiveSelf = false;
        }
    }
}
