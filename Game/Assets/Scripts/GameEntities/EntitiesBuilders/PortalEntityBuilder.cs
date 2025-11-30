using Engine;
using GlmNet;
using System;
using ldtk;
using System.Collections.Generic;

namespace Game
{
    public class PortalEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(EntityInstanceData entityData, IReadOnlyDictionary<string, LayerData> layers, Func<vec2, bool, vec2> positionConverter)
        {
            var portal = new Actor<SpriteRenderer>("Portal").AddComponent<Portal>();
            PortalData portalData = null;
            portal.Transform.LocalScale = new vec3(6, 6);
            portal.Transform.WorldPosition = entityData.WorldPosition;

            if (GetDictionary(entityData.Entity.FieldInstances, "Target", out var targetValue))
            {
                /* -Keys
                 entityIid
                 layerIid
                 levelIid
                 worldIid
                 */
                var targetEntity = layers[targetValue["layerIid"]].EntitiesData[targetValue["entityIid"]];
                portalData = new PortalData()
                {
                    TargetPos = targetEntity.WorldPosition,
                };

                Debug.Log("Current portalPos: " + entityData.WorldPosition + ", Target portal pos: " + targetEntity.WorldPosition);
            }

            portal.Init(portalData);

            return portal;
        }
    }
}
