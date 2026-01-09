using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Editor
{
    public class GFSDataProperty : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            // Serialize value to a token (can be JObject, JValue, JArray, etc.)
            JToken token = JToken.FromObject(value, serializer);

            // Wrap everything in an object with $type + $value
            JObject wrapper = new JObject
            {
                ["$type"] = value.GetType().AssemblyQualifiedName,
                ["$value"] = token
            };

            wrapper.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                        object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject wrapper = JObject.Load(reader);

            string typeName = wrapper["$type"]?.ToString();
            if (typeName == null)
                throw new JsonSerializationException("Missing $type");

            Type type = Type.GetType(typeName, throwOnError: true);

            JToken valueToken = wrapper["$value"];
            return valueToken.ToObject(type, serializer);
        }
    }

}
