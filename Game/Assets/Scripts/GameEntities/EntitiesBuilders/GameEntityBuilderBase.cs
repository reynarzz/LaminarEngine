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
        public abstract GameEntity Build(EntityInstanceData entityData, IReadOnlyDictionary<string, LayerData> layers, Func<vec2, bool, vec2> positionConverter);

        protected bool Deserialize<T>(FieldInstance[] fields, string id, out T value)
        {
            value = default;
            if (!TryGetField(fields, id, out var field, true))
                return false;
            value = JsonConvert.DeserializeObject<T>(field.Value.ToString());
            return true;
        }

        protected bool GetEnumArray<T>(FieldInstance[] fields, string id, out T[] value) where T : unmanaged, Enum
        {
            value = default;
            if (Deserialize<string[]>(fields, id, out var enumsStr))
            {
                var enums = new T[enumsStr.Length];

                for (int i = 0; i < enumsStr.Length; i++)
                {
                    if (!ParseEnum(enumsStr[i], out enums[i]))
                    {
                        Debug.Error($"Can't parse enum: {enumsStr[i]}, is not part of enum type: {typeof(T).Name}");
                    }
                }
                value = enums;
                return true;
            }

            return false;
        }

        protected bool GetEnum<T>(FieldInstance[] fields, string id, out T value) where T : unmanaged, Enum
        {
            value = default;
            if (!TryGetField(fields, id, out var field))
                return false;

            return ParseEnum(field.Value?.ToString(), out value);
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
        protected bool GetBool(FieldInstance[] fields, string id, out bool value)
        {
            value = default;
            if (!TryGetField(fields, id, out var field))
                return false;

            value = bool.Parse(field.Value?.ToString());

            return true;
        }

        protected bool GetDictionary(FieldInstance[] fields, string id, out Dictionary<string, string> value)
        {
            value = default;
            if (!TryGetField(fields, id, out var field))
                return false;
            value = JsonConvert.DeserializeObject<Dictionary<string, string>>(field.Value.ToString());
            return true;
        }

        protected bool GetInt(FieldInstance[] fields, string id, out int value)
        {
            value = default;
            if (!TryGetField(fields, id, out var field))
                return false;

            value = int.Parse(field.Value?.ToString());

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

        private bool TryGetField(FieldInstance[] fields, string id, out FieldInstance fieldOut, bool failIfValueIsNull = false)
        {
            fieldOut = null;
            foreach (var field in fields)
            {
                if (IsField(field, id))
                {
                    if (failIfValueIsNull && field.Value == null)
                    {
                        break;
                    }
                    fieldOut = field;
                }
            }

            return fieldOut != null;
        }

        private bool IsField(FieldInstance field, string id)
        {
            return field.Identifier.Equals(id, StringComparison.OrdinalIgnoreCase);
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
