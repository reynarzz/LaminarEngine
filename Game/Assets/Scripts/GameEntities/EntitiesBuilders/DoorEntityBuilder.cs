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
        public override GameEntity Build(TilemapEntity entityData, TilemapData worldData)
        {
            var doorData = new DoorData();

			if (GetEnum<ItemId>(entityData, "locked_by", out var item))
			{
				doorData.LockedBy = item;
			}

			if (GetInt(entityData, "locked_amount", out var value))
			{
				doorData.LockedByAmount = value;
			}

			doorData.CurrentLevel = worldData.Levels[entityData.LevelIID].LevelIndex;

            if (GetEntityRef(entityData, "target", worldData, out var targetValue))
            {
                doorData.TargetPosition = targetValue.WorldPosition;
                doorData.TargetLevelIndex = worldData.Levels[targetValue.LevelIID].LevelIndex;
            }
            doorData.InteractCondition = PlayerHasItem_Condition(doorData.LockedBy, doorData.LockedByAmount);
            return GamePrefabs.World.InstantiateDoor(entityData.WorldPosition, doorData);
        }
    }
}
