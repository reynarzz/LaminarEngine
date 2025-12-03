using Engine;
using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Portal : InteractableEntityBase<PortalData>
    {
        private CircleCollider2D _circle;
        private bool _shouldDisappear = false;
        private SpriteRenderer _renderer;
        protected override void OnAwake()
        {
            base.OnAwake();
            SpriteRenderer.IsEnabled = false;
            BoxCollider.IsEnabled = false;

            _circle = AddComponent<CircleCollider2D>();
            _circle.IsTrigger = true;
            InteractableRenderer.SortOrder = 15;
            // InteractableRenderer.Transform.LocalPosition += vec3.Up * 4;
        }

        public override void Init(PortalData data)
        {
            base.Init(data);

            var portal = new Actor<Rotate>("PortalSprite").AddComponent<SpriteRenderer>();
            _renderer = portal.GetComponent<SpriteRenderer>();
            _renderer.SortOrder = 14;
            _renderer.Material = MaterialUtils.PortalMaterial;

            portal.Transform.LocalScale = new vec3(6, 6);
            portal.Transform.WorldPosition = Transform.WorldPosition;
            portal.Transform.Parent = Transform;

            // BoxCollider.Size = portal.Transform.LocalScale;
            _circle.Radius = 3;
            if (!data.IsArriveOnly)
            {
                // read data
            }
            else
            {
                // This is a landing - one way portal.
            }
        }

        public override bool TryInteract(Player player)
        {
            if (!Data.IsArriveOnly && CanInteract(player))
            {
                player.Transform.WorldPosition = Data.TargetPos;
                return true;
            }
            return false;
        }

        protected override void OnUpdate()
        {
            if (_shouldDisappear)
            {
                _renderer.Color = Color.Lerp(_renderer.Color, Color.Transparent, Time.DeltaTime * 2);
            }
        }

        protected override void OnPlayerInteractZone(bool enter, Player player)
        {
            if (!Data.IsArriveOnly)
            {
                base.OnPlayerInteractZone(enter, player);
            }
            if (enter && Data.IsArriveOnly && !_shouldDisappear)
            {
                TimedExecute(() => _shouldDisappear = true, 0.5f);
            }
        }
    }
}