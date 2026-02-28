using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Editor.Serialization
{
    public class TwoDimensionalArrayConverter<T> : JsonConverter<T[,]>
    {
        public override void WriteJson(JsonWriter writer, T[,] value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            writer.WriteStartArray();

            for (int i = 0; i < rows; i++)
            {
                writer.WriteStartArray();

                for (int j = 0; j < cols; j++)
                {
                    serializer.Serialize(writer, value[i, j]);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }

        public override T[,] ReadJson(JsonReader reader, Type objectType, T[,] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JArray outer = JArray.Load(reader);

            int rows = outer.Count;
            int cols = rows > 0 ? ((JArray)outer[0]).Count : 0;

            T[,] result = new T[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                JArray row = (JArray)outer[i];

                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = row[j].ToObject<T>(serializer);
                }
            }

            return result;
        }
    }
}
