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

        protected bool IsFieldId<T>(FieldInstance field, string id, out T value)
        {
            value = default;
            var isField = field.Identifier.Equals(id, StringComparison.OrdinalIgnoreCase);

            if (isField)
            {
                if (typeof(T).IsArray)
                {
                    value = JsonConvert.DeserializeObject<T>(field.Value.ToString());
                }
                else if (typeof(T).IsClass)
                {
                    value = (T)field.Value;
                }
                else if (typeof(T) == typeof(int))
                {
                    throw new Exception("TODO: implement int");
                }

            }

            return isField;
        }
    }
}
