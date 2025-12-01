using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Portal : InteractableEntityBase<PortalData>
    {
        private CircleCollider2D _circle;
        protected override void OnAwake()
        {
            base.OnAwake();
            SpriteRenderer.IsEnabled = false;
            BoxCollider.IsEnabled = false;

            _circle = AddComponent<CircleCollider2D>();
            _circle.IsTrigger = true;
        }

        public override void Init(PortalData data)
        {
            base.Init(data);

            var portal = new Actor<Rotate>("PortalSprite").AddComponent<SpriteRenderer>();
            var renderer = portal.GetComponent<SpriteRenderer>();
            renderer.SortOrder = 14;
            renderer.Material = MaterialUtils.PortalMaterial;
            portal.Transform.LocalScale = new vec3(6, 6);
            portal.Transform.WorldPosition = Transform.WorldPosition;

           // BoxCollider.Size = portal.Transform.LocalScale;
            _circle.Radius = 3;
            if (data != null)
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
            if (Data != null && CanInteract(player))
            {
                player.Transform.WorldPosition = Data.TargetPos;
                return true;
            }
            return false;
        }
    }
}
