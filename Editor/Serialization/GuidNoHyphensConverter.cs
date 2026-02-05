using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    public class GuidNoHyphensConverter : JsonConverter<Guid>
    {
        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("N"));
        }

        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = (string)reader.Value;
            return value.Length == 32 ? Guid.ParseExact(value, "N") : Guid.Parse(value);
        }
    }
}
