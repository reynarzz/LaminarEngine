using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Editor.Serialization
{
    public class GFSDataProperty : JsonConverter
    {
        private readonly string _typeTag = "$type";
        private readonly string _valueTag = "$value";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var type = value.GetType();
            var typeId = $"{type.FullName}, {type.Assembly.GetName().Name}";

            var wrapper = new JObject()
            {
                [_typeTag] = typeId,
                [_valueTag] = JToken.FromObject(value, serializer)
            };

            wrapper.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                        object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var wrapper = JObject.Load(reader);

            var typeName = wrapper[_typeTag]?.ToString();
            if (typeName == null)
            {
                throw new JsonSerializationException("Missing $type");
            }

            // Try fast path first
            var type = Type.GetType(typeName);

            // Fallback: search loaded assemblies
            if (type == null)
            {
                var shortName = typeName.Split(',')[0];
                type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(shortName)).FirstOrDefault(t => t != null);
            }

            if (type == null)
            {
                throw new JsonSerializationException($"Type not found: {typeName}");
            }

            //return new SerializedPropertyData()
            //{
            //    TypeName = type.FullName,
            //    Value = wrapper[_valueTag].ToObject(type, serializer)
            //};

            return wrapper[_valueTag].ToObject(type, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }

    }
}

