using Engine;
using Engine.Types;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer), typeof(AudioSource))]
    public abstract class InteractableEntityBase : GameEntity
    {
        [RequiredProperty] protected BoxCollider2D BoxCollider { get; private set; }
        [RequiredProperty] protected SpriteRenderer SpriteRenderer { get; private set; }
        [RequiredProperty] protected SpriteRenderer InteractableRenderer { get; private set; }
        [RequiredProperty] protected AudioSource AudioSource { get; private set; }

        private bool _isPlayerInZone = false;
        private Coroutine _coroutine;
        protected bool WasInteracted { get; set; }
        protected override void OnAwake()
        {
            Actor.Layer = LayerMask.NameToLayer(GameConsts.Interactable);

            BoxCollider.IsTrigger = true;
            SpriteRenderer.Material = GameMaterials.Instance.SpriteMaterial;
            SpriteRenderer.SortOrder = -1;

            InteractableRenderer = new Actor("InteractableIcon").AddComponent<SpriteRenderer>();
            InteractableRenderer.Transform.Parent = Transform;
            InteractableRenderer.Transform.LocalPosition = default;
            InteractableRenderer.Material = GameMaterials.Instance.SpriteMaterial;
            InteractableRenderer.IsEnabled = true;
            InteractableRenderer.SortOrder = 6;
            InteractableRenderer.Color = Color.Transparent;
            InteractableRenderer.Sprite = GameTextures.GetSprite("e_interactable3");
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (IsPlayerLayer(collider) && !_isPlayerInZone)
            {
                _isPlayerInZone = true;
                OnPlayerInteractZone(true, collider.GetComponent<Player>());
            }
        }

        protected override void OnTriggerExit2D(Collider2D collider)
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
        public override void OnLateUpdate()
        {
            base.OnLateUpdate();

            if (InteractableRenderer)
            {
                InteractableRenderer.Transform.LocalPosition += vec3.Up * MathF.Sin(Time.TimeCurrent * 5) * Time.DeltaTime;
            }
        }

        public abstract bool TryInteract(Player player);
        public abstract bool CanInteract(Player player);
        protected virtual void OnPlayerInteractZone(bool enter, Player player)
        {
            InteractableRenderVisible(enter && CanInteract(player));
        }
        protected void InteractableRenderVisible(bool isVisible)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(InteractableRendererAnimation(InteractableRenderer, isVisible));
        }
        protected IEnumerator InteractableRendererAnimation(SpriteRenderer renderer, bool show, float speed = 6)
        {
            float t = renderer.Color.A;
            while (show ? (t < 1.0f) : (t > 0.0f))
            {
                t += (show ? Time.DeltaTime : -Time.DeltaTime) * speed;
                t = Mathf.Clamp01(t);

                var c = renderer.Color;
                c.A = t;
                renderer.Color = c;
                yield return null;
            }
        }
    }

    public abstract class InteractableEntityBase<T> : InteractableEntityBase where T : InteractableData
    {
        protected SpriteRenderer LockedByRenderer { get; private set; }
        protected T Data { get; private set; }
        protected vec2 LockedByItemPos { get; set; }
        private Coroutine _coroutine;

        public virtual void Init(T data)
        {
            Data = data;

            if (data != null && data.LockedBy != ItemId.none)
            {
                // NOTE: Right now i'm using just one renderer, but in the future interactables will be locked by more than one item,
                //       For than reason I'm not reusing the interactable renderer,
                //       however, ideally I would just pool those icons and change the positon on demand.
                LockedByRenderer = new Actor("LockedByRequested").AddComponent<SpriteRenderer>();
                LockedByRenderer.Transform.Parent = Transform;
                LockedByRenderer.Sprite = GameTextures.GetSprite(data.LockedBy.ToString());
                //LockedByRenderer.Actor.IsActiveSelf = false;
                LockedByRenderer.Material = GameMaterials.Instance.SpriteMaterial;
                LockedByRenderer.Transform.LocalPosition = LockedByItemPos;
                LockedByRenderer.Transform.WorldScale = vec3.One * 2;
                LockedByRenderer.SortOrder = 6;
                LockedByRenderer.Color = Color.Transparent;
            }
        }
        protected void LockedByRenderVisible(bool isVisible)
        {
            if (!LockedByRenderer)
                return;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(InteractableRendererAnimation(LockedByRenderer, isVisible));
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            if (LockedByRenderer)
                LockedByRenderer.Transform.LocalPosition += vec3.Up * MathF.Sin(Time.TimeCurrent * 5) * Time.DeltaTime;

        }
        public sealed override bool CanInteract(Player player)
        {
            return Data?.InteractCondition?.Invoke(player) ?? true;
        }

        protected sealed override void OnTriggerEnter2D(Collider2D collider)
        {
            base.OnTriggerEnter2D(collider);

            if (collider.Actor.Layer == LayerMask.NameToLayer(GameConsts.PLAYER))
            {
                var character = collider.GetComponent<Character>();
                if (!character)
                    return;

                if ((!character.Inventory.Contains(Data.LockedBy, Data.LockedByAmount)) && LockedByRenderer
                    && !WasInteracted)
                {
                    // LockedByRenderer.Actor.IsActiveSelf = true;
                    LockedByRenderVisible(true);
                }
            }
        }

        protected sealed override void OnTriggerExit2D(Collider2D collider)
        {
            base.OnTriggerExit2D(collider);

            if (collider.Actor.Layer == LayerMask.NameToLayer(GameConsts.PLAYER) && LockedByRenderer)
            {
                // LockedByRenderer.Actor.IsActiveSelf = false;

                LockedByRenderVisible(false);
            }
        }
    }
}
