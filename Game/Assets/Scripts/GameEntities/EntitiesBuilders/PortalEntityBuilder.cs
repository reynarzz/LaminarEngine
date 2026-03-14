using Engine;
using GlmNet;
using System;
using ldtk;
using System.Collections.Generic;

namespace Game
{
    public class PortalEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(TilemapEntity entityData, TilemapData worldData)
        {
            var portal = Actor.Create<SpriteRenderer>("Portal").AddComponent<Portal>();
            var portalData = new PortalData();
            portal.Transform.WorldPosition = entityData.WorldPosition;

            if (GetEnum<ItemId>(entityData, "locked_by", out var item))
            {
                portalData.LockedBy = item;
            }

            if (GetInt(entityData, "locked_amount", out var value))
            {
                portalData.LockedByAmount = value;
            }

            if (GetEntityRef(entityData, "target", worldData, out var targetValue))
            {
                portalData.TargetPos = targetValue.WorldPosition;

                // Debug.Log("Current portalPos: " + entityData.WorldPosition + ", Target portal pos: " + targetEntity.WorldPosition);
            }
            else
            {
                portalData.IsArriveOnly = true;
            }

            portalData.InteractCondition = PlayerHasItem_Condition(portalData.LockedBy, portalData.LockedByAmount);

            portal.Init(portalData);

            return portal;
        }
    }
}
