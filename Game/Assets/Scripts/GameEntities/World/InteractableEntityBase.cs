using Engine;
using Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequiredComponent(typeof(RigidBody2D), typeof(BoxCollider2D))]
    public abstract class InteractableEntityBase : ScriptBehavior
    {
        public RigidBody2D Rigidbody { get; private set; }
        public BoxCollider2D BoxCollider { get; private set; }
        public override void OnAwake()
        {
            Rigidbody = GetComponent<RigidBody2D>();
            BoxCollider = GetComponent<BoxCollider2D>();
        }

        protected sealed override void OnTriggerEnter2D(Collider2D collider)
        {
            if (IsPlayerLayer(collider))
            {
                OnPlayerInteractZone(true, collider.GetComponent<Player>());
            }
        }

        protected sealed override void OnTriggerExit2D(Collider2D collider)
        {
            if (IsPlayerLayer(collider))
            {
                OnPlayerInteractZone(false, collider.GetComponent<Player>());
            }
        }

        protected abstract void OnPlayerInteractZone(bool enter, Player player);

        private bool IsPlayerLayer(Collider2D collider)
        {
            return collider.Actor.Layer == LayerMask.NameToLayer(GameLayers.PLAYER);
        }
    }
}
