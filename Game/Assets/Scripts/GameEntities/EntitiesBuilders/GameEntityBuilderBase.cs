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
    public abstract class GameEntityBuilderBase
    {
        public abstract GameEntity Build(TilemapEntity entityData, TilemapData worldData);

        protected bool GetEnumArray<T>(TilemapEntity target, string id, out T[] value) where T : unmanaged, Enum
        {
            value = default;
            if (!target.Properties.TryGetValue(id, out var field))
                return false;

            if (field.Value.EnumArray != null && field.Value.EnumArray.Length > 0)
            {
                var enums = new T[field.Value.EnumArray.Length];

                for (int i = 0; i < field.Value.EnumArray.Length; i++)
                {
                    if (!ParseEnum(field.Value.EnumArray[i].EnumValStr, out enums[i]))
                    {
                        Debug.Error($"Can't parse enum: {field.Value.EnumArray[i].EnumValStr}, is not part of enum type: {typeof(T).Name}");
                    }
                }
                value = enums;
                return true;
            }

            return false;
        }

        protected bool GetEnum<T>(TilemapEntity target, string id, out T value) where T : unmanaged, Enum
        {
            value = default;
            if (!target.Properties.TryGetValue(id, out var field))
                return false;

            return ParseEnum(field.Value.Enum.EnumValStr, out value);
        }

        private bool ParseEnum<T>(string str, out T value) where T : unmanaged, Enum
        {
            return Enum.TryParse(str, true, out value);
        }
        protected bool GetBool(TilemapEntity target, string id, out bool value)
        {
            value = default;
            if (!target.Properties.TryGetValue(id, out var property))
                return false;
            value = property.Value.Bool;
            return true;
        }

        protected bool GetEntityRef(TilemapEntity target, string id, TilemapData data, out TilemapEntity value)
        {
            value = default;
            if (!target.Properties.TryGetValue(id, out var property))
                return false;

            if (data.Levels.TryGetValue(property.Value.EntityRef.levelIid, out var level))
            {
                if (level.Layers.TryGetValue(property.Value.EntityRef.layerIid, out var layer))
                {
                    if (layer.Entities.TryGetValue(property.Value.EntityRef.entityIid, out value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected bool GetInt(TilemapEntity target, string id, out int value)
        {
            value = default;
            if (!target.Properties.TryGetValue(id, out var property))
                return false;
            value = property.Value.Int;
            return true;
        }

        protected Predicate<Player> PlayerHasItem_Condition(ItemId item, int amount)
        {
            return p =>
            {
                if (item == ItemId.none)
                {
                    return true;
                }

                return p.Inventory.GetItemCount(item) >= amount;
            };
        }

    }
}
