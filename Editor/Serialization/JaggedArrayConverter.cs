using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Editor.Serialization
{
    public class JaggedArrayConverter<T> : JsonConverter<T[][]>
    {
        public override void WriteJson(JsonWriter writer, T[][] value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            int size = value.Length;

            writer.WriteStartArray();

            for (int i = 0; i < size; i++)
            {
                writer.WriteStartArray();

                for (int j = 0; j < size; j++)
                {
                    writer.WriteValue(value[i][j]);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }

        public override T[][] ReadJson(JsonReader reader, Type objectType, T[][] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JArray outer = JArray.Load(reader);
            int size = outer.Count;

            T[][] matrix = new T[size][];

            for (int i = 0; i < size; i++)
            {
                JArray row = (JArray)outer[i];

                matrix[i] = new T[size];

                for (int j = 0; j < size; j++)
                {
                    matrix[i][j] = row[j].Value<T>();
                }
            }

            return matrix;
        }

    }
}
