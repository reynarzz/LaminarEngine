using Engine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    sealed class PolymorphicConverter<TBase> : JsonConverter
    {
        private JsonSerializer _innerSerializer;

        public override bool CanConvert(Type objectType)
        {
            return typeof(TBase).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObj = JObject.Load(reader);
            var typeName = jObj["$type"].Value<string>();
            object instance = null;
            if (ReflectionUtils.ResolveType(typeName, out var type))
            {
                instance = Activator.CreateInstance(type);
                serializer.Populate(jObj["$data"].CreateReader(), instance);
            }
            return instance;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Clone serializer without this converter
            if (_innerSerializer == null)
            {
                _innerSerializer = new JsonSerializer();
                foreach (var converter in serializer.Converters)
                {
                    if (converter != this)
                    {
                        _innerSerializer.Converters.Add(converter);
                    }
                }
            }

            var data = JObject.FromObject(value, _innerSerializer);

            var jObj = new JObject
            {
                ["$type"] = $"{value?.GetType().FullName}, {value?.GetType().Assembly.GetName().Name}",
                ["$data"] = data
            };

            jObj.WriteTo(writer);
        }
    }
}
