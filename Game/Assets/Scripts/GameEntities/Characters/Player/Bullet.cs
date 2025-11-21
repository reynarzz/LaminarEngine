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
        public override void OnAwake()
        {
            
        }

        public void Shoot(vec2 dir, float speed, ulong layerMask)
        {
            _shootDir = dir;
            _speed = speed;
            _layerMask = layerMask;
            _shoot = true;
            Transform.WorldScale = new vec3(0.6f, 0.2f);

            var sprite = GetComponent<SpriteRenderer>();
            sprite.Material = GameManager.DefaultMaterial;
        }

        public override void OnUpdate()
        {
            Transform.WorldPosition += _shootDir * Time.DeltaTime * _speed;
        }

        public override void OnFixedUpdate()
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
                    }
                    else
                    {
                        return;
                    }
                }
                Debug.Log("Hit");
                Actor.Destroy(Actor);
            }
        }

        public void PoolObject()
        {
            _shoot = false;
        }
    }
}
