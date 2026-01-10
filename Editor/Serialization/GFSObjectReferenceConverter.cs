using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Newtonsoft.Json;

namespace Editor.Serialization
{
    public sealed class GFSObjectReferenceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IObject).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var obj = value as IObject;
            Debug.Log("Reference converted: " + value.GetType().Name);
            var serializedItem = new SerializedItem<Guid>()
            {
                Type = SceneSerializer.GetSerializedType(value?.GetType()),
                Data = obj?.GetID() ?? Guid.Empty
            };

            serializer.Serialize(writer, serializedItem);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            return reader.Value as SerializedItem<Guid>;
        }
    }

}
