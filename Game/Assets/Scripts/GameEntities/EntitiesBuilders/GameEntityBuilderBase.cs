using Engine;
using GlmNet;
using ldtk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class GameEntityBuilderBase
    {
        public abstract GameEntity Build(vec2 position, FieldInstance[] fields, Func<vec2, bool, vec2> positionConverter);

        private bool IsField(FieldInstance field, string id)
        {
            return field.Identifier.Equals(id, StringComparison.OrdinalIgnoreCase);
        }

        protected bool Deserialize<T>(FieldInstance field, string id, out T value)
        {
            value = default;
            if (!IsField(field, id) || field.Value == null)
                return false;
            value = JsonConvert.DeserializeObject<T>(field.Value.ToString());
            return true;
        }

        protected bool GetEnumArray<T>(FieldInstance field, string id, out T[] value) where T : unmanaged, Enum
        {
            value = default;
            if (Deserialize<string[]>(field, id, out var enumsStr))
            {
                var enums = new T[enumsStr.Length];

                for (int i = 0; i < enumsStr.Length; i++)
                {
                    if(!ParseEnum(enumsStr[i], out enums[i]))
                    {
                        Debug.Error($"Can't parse enum: {enumsStr[i]}, is not part of enum type: {typeof(T).Name}");
                    }
                }
                value = enums;
                return true;
            }

            return false;
        }

        protected bool GetEnum<T>(FieldInstance field, string id, out T value) where T : unmanaged, Enum
        {
            value = default;
            if (!IsField(field, id))
                return false;

            return ParseEnum(field.Value?.ToString(), out value);
        }

        private bool ParseEnum<T>(string str, out T value) where T : unmanaged, Enum
        {
            return Enum.TryParse(str, true, out value);
        }
        protected bool GetBool(FieldInstance field, string id, out bool value)
        {
            value = default;
            if (!IsField(field, id))
                return false;

            value = bool.Parse(field.Value?.ToString());

            return true;
        }

        protected bool GetInt(FieldInstance field, string id, out int value)
        {
            value = default;
            if (!IsField(field, id))
                return false;

            value = int.Parse(field.Value?.ToString());

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
