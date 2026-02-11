using Engine;
using Engine.Serialization;
using Engine.Utils;
using GlmNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Editor.Serialization
{
    internal sealed class VariantJsonConverter : JsonConverter<Variant>
    {
        private const string KIND_ID = "Kind";
        private const string VALUE_ID = "Value";
        private const string ENUM_ID = "Enum";

        public override void WriteJson(JsonWriter writer, Variant value, JsonSerializer serializer)
        {
            if (value.Kind == SerializedType.None)
            {
                writer.WriteNull();
                return;
            }

            var obj = new JObject
            {
                //["$type"] = ReflectionUtils.GetFullTypeName(typeof(Variant)),
                [KIND_ID] = value.Kind.ToString()
            };

            switch (value.Kind)
            {
                case SerializedType.None:
                    obj[VALUE_ID] = JValue.CreateNull();
                    break;

                case SerializedType.String:
                    obj[VALUE_ID] = value.String;
                    break;

                case SerializedType.Enum:
                    obj[ENUM_ID] = JToken.FromObject(value.Enum);
                    break;

                default:
                    obj[VALUE_ID] = CreatePayloadToken(value);
                    break;
            }

            obj.WriteTo(writer);
        }

        public override Variant ReadJson(JsonReader reader, Type objectType, Variant existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return default;

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException($"Expected object for VariantIRValue, got {reader.TokenType}");

            JObject objLoad = JObject.Load(reader);

            if (!objLoad.TryGetValue("$value", out var valueToken))
            {
                valueToken = objLoad;
            }

            if (valueToken.Type == JTokenType.Null)
                return default;

            var kind = Enum.Parse<SerializedType>(valueToken[KIND_ID].Value<string>());

            var val = ParseValue(valueToken, kind);
         
            return val;
        }

        private Variant ParseValue(JToken obj, SerializedType kind)
        {
            if (obj == null || obj.Type == JTokenType.Null)
                return default;

            switch (kind)
            {
                case SerializedType.None:
                    return default;

                case SerializedType.String:
                    return (obj[VALUE_ID]?.ToObject<string>()) ?? string.Empty;

                case SerializedType.Enum:
                    return new Variant
                    {
                        Kind = SerializedType.Enum,
                        Enum = obj[ENUM_ID]?.ToObject<EnumIRValue>() ?? default
                    };

                case SerializedType.Char: return obj[VALUE_ID]?.ToObject<char>() ?? default;
                case SerializedType.Bool: return obj[VALUE_ID]?.ToObject<bool>() ?? default;
                case SerializedType.Byte: return obj[VALUE_ID]?.ToObject<byte>() ?? default;
                case SerializedType.Short: return obj[VALUE_ID]?.ToObject<short>() ?? default;
                case SerializedType.UShort: return obj[VALUE_ID]?.ToObject<ushort>() ?? default;
                case SerializedType.Int: return obj[VALUE_ID]?.ToObject<int>() ?? 0;
                case SerializedType.UInt: return obj[VALUE_ID]?.ToObject<uint>() ?? 0;
                case SerializedType.Float: return obj[VALUE_ID]?.ToObject<float>() ?? 0f;
                case SerializedType.Double: return obj[VALUE_ID]?.ToObject<double>() ?? 0.0;
                case SerializedType.Long: return obj[VALUE_ID]?.ToObject<long>() ?? 0L;
                case SerializedType.ULong: return obj[VALUE_ID]?.ToObject<ulong>() ?? 0UL;

                case SerializedType.Vec2: return obj[VALUE_ID]?.ToObject<vec2>() ?? default;
                case SerializedType.Vec3: return obj[VALUE_ID]?.ToObject<vec3>() ?? default;
                case SerializedType.Vec4: return obj[VALUE_ID]?.ToObject<vec4>() ?? default;
                case SerializedType.IVec2: return obj[VALUE_ID]?.ToObject<ivec2>() ?? default;
                case SerializedType.IVec3: return obj[VALUE_ID]?.ToObject<ivec3>() ?? default;
                case SerializedType.IVec4: return obj[VALUE_ID]?.ToObject<ivec4>() ?? default;
                case SerializedType.Quat: return obj[VALUE_ID]?.ToObject<quat>() ?? default;
                case SerializedType.Mat2: return obj[VALUE_ID]?.ToObject<mat2>() ?? default;
                case SerializedType.Mat3: return obj[VALUE_ID]?.ToObject<mat3>() ?? default;
                case SerializedType.Mat4: return obj[VALUE_ID]?.ToObject<mat4>() ?? default;
                case SerializedType.Color: return obj[VALUE_ID]?.ToObject<Color>() ?? default;
                case SerializedType.Color32: return obj[VALUE_ID]?.ToObject<Color32>() ?? default;

                default:
                    throw new JsonSerializationException($"Unsupported SerializedType {kind}");
            }
        }
        private static JToken CreatePayloadToken(Variant value)
        {
            return value.Kind switch
            {
                SerializedType.Char => value.value.Char,
                SerializedType.Bool => value.value.Bool,
                SerializedType.Byte => value.value.Byte,
                SerializedType.Short => value.value.Short,
                SerializedType.UShort => value.value.UShort,
                SerializedType.Int => value.value.Int,
                SerializedType.UInt => value.value.Uint,
                SerializedType.Float => value.value.Float,
                SerializedType.Double => value.value.Double,
                SerializedType.Long => value.value.Long,
                SerializedType.ULong => value.value.Ulong.ToString(),

                SerializedType.Vec2 or SerializedType.Vec3 or SerializedType.Vec4 or
                SerializedType.IVec2 or SerializedType.IVec3 or SerializedType.IVec4 or
                SerializedType.Quat or SerializedType.Mat2 or SerializedType.Mat3 or
                SerializedType.Mat4 or SerializedType.Color or SerializedType.Color32
                    => JToken.FromObject(value.GetValueAsObject()),

                _ => throw new JsonSerializationException($"Unsupported SerializedType {value.Kind}")
            };
        }
    }
}