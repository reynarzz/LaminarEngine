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

        protected bool GetEnum<T>(FieldInstance field, string id, out T value) where T : unmanaged, Enum
        {
            value = default;
            if (!IsField(field, id))
                return false;

            if (Enum.TryParse<T>(field.Value?.ToString(), true, out var e))
            {
                value = e;
            }

            return true;
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

    }
}
