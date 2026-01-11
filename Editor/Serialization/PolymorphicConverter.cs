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
        readonly Func<int, TBase> factory;

        public override bool CanConvert(Type objectType)
        {
            return typeof(TBase).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
                                        object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var typeName = jo["$type"].Value<string>();
            object instance = null;// factory(type);
            serializer.Populate(jo["$data"].CreateReader(), instance);
            return instance;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Clone serializer WITHOUT this converter
            var innerSerializer = new JsonSerializer();
            foreach (var c in serializer.Converters)
            {
                if (c != this)
                    innerSerializer.Converters.Add(c);
            }

            var data = JObject.FromObject(value, innerSerializer);

            var jo = new JObject
            {
                ["$type"] = $"{value?.GetType().FullName}, {value?.GetType().Assembly.GetName().Name}",
                ["$data"] = data
            };

            jo.WriteTo(writer);
        }
    }
}
