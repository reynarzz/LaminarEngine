using Engine;
using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class DoorEntityBuilder : GameEntityBuilderBase
    {
        public override GameEntity Build(EntityInstanceData entityData, WorldData worldData, Func<vec2, bool, vec2> positionConverter)
        {
            var doorData = new DoorData();

            foreach (var field in entityData.Entity.FieldInstances)
            {
                if (GetEnum<ItemId>(field, "locked_by", out var item))
                {
                    doorData.LockedBy = item;
                }

                if (GetInt(field, "locked_amount", out var value))
                {
                    doorData.LockedAmount = value;
                }
            }
            
            doorData.CurrentLevel = worldData.Levels[entityData.LevelIID].LevelIndex;

            if (GetEntityRef(entityData.Entity.FieldInstances, "target", worldData, out var targetValue))
            {
                doorData.TargetPosition = targetValue.WorldPosition;
                doorData.TargetLevelIndex = worldData.Levels[targetValue.LevelIID].LevelIndex;
            }
            doorData.InteractCondition = PlayerHasItem_Condition(doorData.LockedBy, doorData.LockedAmount);
            return GamePrefabs.World.InstantiateDoor(entityData.WorldPosition, doorData);
        }
    }
}
