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

        protected override void OnAwake()
        {
            Actor.Layer = LayerMask.NameToLayer(GameConsts.Interactable);

            BoxCollider.IsTrigger = true;
            SpriteRenderer.Material = GameMaterials.Instance.SpriteMaterial;
            SpriteRenderer.SortOrder = -1;

            InteractableRenderer = new Actor("InteractableIcon").AddComponent<SpriteRenderer>();
            InteractableRenderer.Transform.Parent = Transform;
            InteractableRenderer.Material = GameMaterials.Instance.SpriteMaterial;
            InteractableRenderer.IsEnabled = true;
            InteractableRenderer.SortOrder = 6;
            InteractableRenderer.Color = Color.Transparent;
            InteractableRenderer.Sprite = GameTextures.GetSprite("e_interactable3");
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
        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            InteractableRenderer.Transform.LocalPosition += vec3.Up * MathF.Sin(Time.TimeCurrent * 5) * Time.DeltaTime;
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
            _coroutine = StartCoroutine(InteractableRendererAnimation(isVisible));
        }

        private IEnumerator InteractableRendererAnimation(bool show, float speed = 4)
        {
            float t = InteractableRenderer.Color.A;
            while (show?(t < 1.0f):(t > 0.0f))
            {
                t += (show ? Time.DeltaTime : -Time.DeltaTime) * speed;
                t = Mathf.Clamp01(t);

                var c = InteractableRenderer.Color;
                c.A = t;
                InteractableRenderer.Color = c;
                yield return null;
            }
        }
    }

    public abstract class InteractableEntityBase<T> : InteractableEntityBase where T : InteractableData
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
