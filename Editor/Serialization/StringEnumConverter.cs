using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace Editor.Serialization
{
    public sealed class StringEnumConverter<T> : StringEnumConverter where T: Enum
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }
}