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
        public override GameEntity Build(vec2 position, FieldInstance[] fields, Func<vec2, bool, vec2> positionConverter)
        {
            var doorData = new DoorData();

            foreach (var field in fields)
            {
                if (GetEnum<ItemId>(field, "locked_by", out var item))
                {
                    doorData.LockedBy = item;
                }

                if (GetInt(field, "value", out var value))
                {
                    doorData.Amount = value;
                }
            }

            doorData.InteractCondition = p =>
            {
                if (doorData.LockedBy == ItemId.none || doorData.Amount <= 0)
                {
                    return true;
                }

                return p.Inventory.GetItemCount(doorData.LockedBy) >= doorData.Amount;
            };

            return GamePrefabs.World.InstantiateDoor(position, doorData);
        }
    }
}
