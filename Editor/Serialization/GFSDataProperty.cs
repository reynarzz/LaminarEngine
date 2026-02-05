using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Editor.Serialization
{
    internal class GFSDataProperty : JsonConverter
    {
        private const string _typeTag = "$type";
        private const string _valueTag = "$value";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var type = value.GetType();

            var wrapper = new JObject()
            {
                [_typeTag] = ReflectionUtils.GetFullTypeName(type),
                [_valueTag] = JToken.FromObject(value, serializer)
            };

            wrapper.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                        object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.StartObject)
            {
                return serializer.Deserialize(reader);
            }

            JObject wrapper = JObject.Load(reader);

            if (!wrapper.TryGetValue(_typeTag, out var typeToken))
            {
                return wrapper.ToObject(objectType, serializer);
            }

            string typeName = typeToken.ToString();

            if (ReflectionUtils.ResolveType(typeName, out var type))
            {
                //return new SerializedPropertyData()
                //{
                //    TypeName = type.FullName,
                //    Value = wrapper[_valueTag].ToObject(type, serializer)
                //};
                var val = wrapper[_valueTag];

                var obj = default(object);

                try
                {
                    obj = val.ToObject(type, serializer);
                }
                catch (Exception e)
                {
                    Debug.Warn("Cannot convert to type, it changed.");
                }
                return obj;

            }

            Debug.Error($"Type name: '{typeName}' cannot be found, can't deserialize data");
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }

    }
}

