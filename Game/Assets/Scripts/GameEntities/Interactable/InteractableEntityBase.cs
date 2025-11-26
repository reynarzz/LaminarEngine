using Engine;
using Engine.Types;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequiredComponent(typeof(RigidBody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
    public abstract class InteractableEntityBase : GameEntity 
    {
        protected RigidBody2D Rigidbody { get; private set; }
        protected BoxCollider2D BoxCollider { get; private set; }
        protected SpriteRenderer SpriteRenderer { get; private set; }
        private bool _isPlayerInZone = false;
        public override void OnAwake()
        {
            Rigidbody = GetComponent<RigidBody2D>();
            Rigidbody.BodyType = Body2DType.Static;

            BoxCollider = GetComponent<BoxCollider2D>();
            BoxCollider.IsTrigger = true;

            SpriteRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer.Material = MaterialUtils.SpriteMaterial;
            SpriteRenderer.SortOrder = -1;
        }

        protected sealed override void OnTriggerEnter2D(Collider2D collider)
        {
            if (IsPlayerLayer(collider) && !_isPlayerInZone)
            {
                _isPlayerInZone = true;
                OnPlayerInteractZone(true, collider.GetComponent<Player>());
            }
        }

        protected sealed override void OnTriggerExit2D(Collider2D collider)
        {
            if (IsPlayerLayer(collider) && _isPlayerInZone)
            {
                _isPlayerInZone = false;
                OnPlayerInteractZone(false, collider.GetComponent<Player>());
            }
        }

        private bool IsPlayerLayer(Collider2D collider)
        {
            return collider.Actor.Layer == LayerMask.NameToLayer(GameConsts.PLAYER);
        }

        public abstract bool TryInteract(Player player);
        public abstract bool CanInteract(Player player);
        protected virtual void OnPlayerInteractZone(bool enter, Player player) { }
    }

    public abstract class InteractableEntityBase<T> : InteractableEntityBase where T: InteractableData
    {
        protected T Data { get; private set; }
        public virtual void Init(T data)
        {
            Data = data;
        }

        public sealed override bool CanInteract(Player player)
        {
            return Data?.InteractCondition?.Invoke(player) ?? true;
        }
    }
}
