using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class CircleTargetDetector : ScriptBehavior, ITargetDetector
    {
        public bool IsTargetDetected { get; private set; }
        public float Size { get => _collider.Radius; set => _collider.Radius = value; }

        private CircleCollider2D _collider;
        public override void OnAwake()
        {
            _collider = AddComponent<CircleCollider2D>();
            _collider.IsTrigger = true;
            _collider.Radius = 5;
        }


        public override void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.Actor.Layer == LayerMask.NameToLayer(GameLayers.PLAYER))
            {
                var dir = collider.Transform.WorldPosition - Transform.WorldPosition;

                // TODO: decrease the freq raycast is called.
                var hits = Physics2D.RaycastAll(Transform.WorldPosition, dir.Normalized * 10, LayerMask.LayerToBits(collider.Actor.Layer) | GameLayers.GROUND_MASK);
                Debug.DrawRay(Transform.WorldPosition, dir.Normalized * 10, Color.Red);
                IsTargetDetected = false;
                for (int i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];
                    IsTargetDetected = hit.isHit && hit.Collider.Actor.Layer == collider.Actor.Layer;
                    if (IsTargetDetected)
                    { 
                        break; 
                    }

                }
            }
        }

        public override void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.Actor.Layer == LayerMask.NameToLayer(GameLayers.PLAYER))
            {
                IsTargetDetected = false;
            }
        }
    }
}
