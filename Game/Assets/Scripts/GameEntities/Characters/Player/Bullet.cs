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
    internal class Bullet : ScriptBehavior
    {
        private vec3 _shootDir;
        private ulong _layerMask;
        public bool _shoot = false;
        private float _speed = 0;
        private float _timeAlive = 1;

        public void Shoot(vec2 position, vec2 dir, float speed, ulong layerMask)
        {
            Transform.WorldPosition = position;
            _shootDir = dir;
            _speed = speed;
            _layerMask = layerMask;
            _shoot = true;
            Transform.WorldScale = new vec3(0.6f, 0.2f);

            var sprite = GetComponent<SpriteRenderer>();
            sprite.Material = GameManager.DefaultMaterial;
            _timeAlive = 1;

            CheckCollision();
        }

        protected override void OnUpdate()
        {
            Transform.WorldPosition += _shootDir * Time.DeltaTime * _speed;

            // TODO: make it distance based instead.
            if((_timeAlive -= Time.DeltaTime) <= 0)
            {
                PoolObject();
            }
        }

        protected override void OnFixedUpdate()
        {
            CheckCollision();
        }

        private void CheckCollision()
        {
            var hit = Physics2D.BoxCast(Transform.WorldPosition, Transform.LocalScale, _layerMask);
            if (Physics2D.DrawColliders)
            {
                Debug.DrawBox(Transform.WorldPosition, Transform.LocalScale, Color.Red);
            }
            if (hit.isHit && Actor)
            {
                var character = hit.Collider.GetComponent<Character>();

                if (character)
                {
                    if (character.IsCharacterAlive())
                    {
                        character?.HitDamage(1);
                        // character.GetComponent<RigidBody2D>().AddForce(_shootDir * 12, ForceMode2D.Impulse);
                    }
                    else
                    {
                        return;
                    }
                }
                PoolObject();
            }
        }

        public void PoolObject()
        {
            Actor.Destroy(Actor);

            _shoot = false;
            Actor.IsActiveSelf = false;
        }
    }
}
