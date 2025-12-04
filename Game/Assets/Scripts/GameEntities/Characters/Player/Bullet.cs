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
    [RequireComponent(typeof(SpriteRenderer))]
    internal class Bullet : GameEntity
    {
        private vec3 _shootDir;
        private ulong _layerMask;
        public bool _shoot = false;
        private float _speed = 0;
        private float _timeAlive = 1;
        [RequiredProperty] private SpriteRenderer _renderer;
        private ParticleSystem2D _particleSystem;
        public void Shoot(vec2 position, vec2 dir, float speed, ulong layerMask)
        {
            Transform.WorldPosition = position;
            _shootDir = dir;
            _speed = speed;
            _layerMask = layerMask;
            _shoot = true;
            Transform.WorldScale = new vec3(0.6f, 0.18f);

            _renderer.Material = MaterialUtils.SpriteMaterial;
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
            particleSystem.Material = MaterialUtils.SpriteMaterial;
            particleSystem.Transform.LocalPosition = default;
            particleSystem.Transform.WorldScale = vec3.One;
            return particleSystem;
        }

        protected override void OnFixedUpdate()
        {
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
