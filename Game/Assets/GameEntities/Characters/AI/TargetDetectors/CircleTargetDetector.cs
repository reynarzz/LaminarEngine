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
                var hit = Physics2D.Raycast(Transform.WorldPosition, dir, LayerMask.LayerToBits(collider.Actor.Layer) | GameLayers.GROUND_MASK);
                Debug.DrawRay(Transform.WorldPosition, dir, Color.Red);
                IsTargetDetected = hit.isHit && hit.Collider.Actor.Layer == collider.Actor.Layer;
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
