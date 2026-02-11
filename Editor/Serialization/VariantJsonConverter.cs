using Engine;
using Engine.Serialization;
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

        // A private serializer without VariantJsonConverter to avoid the self-referencing loop
        private static readonly JsonSerializer _internalSerializer = new JsonSerializer();

        public override void WriteJson(JsonWriter writer, Variant value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
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
                    obj[ENUM_ID] = JToken.FromObject(value.Enum, _internalSerializer);
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

            JObject obj = JObject.Load(reader);

            if (!obj.TryGetValue(KIND_ID, out var kindToken))
                throw new JsonSerializationException("Missing Kind property");

            SerializedType kind = Enum.Parse<SerializedType>(kindToken.Value<string>());

            switch (kind)
            {
                case SerializedType.None:
                    return default;

                case SerializedType.String:
                    return Variant.FromString(obj[VALUE_ID]?.Value<string>());

                case SerializedType.Enum:
                    return new Variant
                    {
                        Kind = SerializedType.Enum,
                        Enum = obj[ENUM_ID]!.ToObject<EnumIRValue>(_internalSerializer)
                    };

                case SerializedType.Char: return Variant.FromChar(obj[VALUE_ID]!.Value<char>());
                case SerializedType.Bool: return Variant.FromBool(obj[VALUE_ID]!.Value<bool>());
                case SerializedType.Byte: return Variant.FromByte(obj[VALUE_ID]!.Value<byte>());
                case SerializedType.Short: return Variant.FromShort(obj[VALUE_ID]!.Value<short>());
                case SerializedType.UShort: return Variant.FromUShort(obj[VALUE_ID]!.Value<ushort>());
                case SerializedType.Int: return Variant.FromInt(obj[VALUE_ID]!.Value<int>());
                case SerializedType.UInt: return Variant.FromUInt(obj[VALUE_ID]!.Value<uint>());
                case SerializedType.Float: return Variant.FromFloat(obj[VALUE_ID]!.Value<float>());
                case SerializedType.Double: return Variant.FromDouble(obj[VALUE_ID]!.Value<double>());
                case SerializedType.Long: return Variant.FromLong(obj[VALUE_ID]!.Value<long>());
                case SerializedType.ULong: return Variant.FromULong(ulong.Parse(obj[VALUE_ID]!.Value<string>()));

                case SerializedType.Vec2: return Variant.FromVec2(obj[VALUE_ID]!.ToObject<vec2>(_internalSerializer));
                case SerializedType.Vec3: return Variant.FromVec3(obj[VALUE_ID]!.ToObject<vec3>(_internalSerializer));
                case SerializedType.Vec4: return Variant.FromVec4(obj[VALUE_ID]!.ToObject<vec4>(_internalSerializer));
                case SerializedType.IVec2: return Variant.FromIVec2(obj[VALUE_ID]!.ToObject<ivec2>(_internalSerializer));
                case SerializedType.IVec3: return Variant.FromIVec3(obj[VALUE_ID]!.ToObject<ivec3>(_internalSerializer));
                case SerializedType.IVec4: return Variant.FromIVec4(obj[VALUE_ID]!.ToObject<ivec4>(_internalSerializer));
                case SerializedType.Quat: return Variant.FromQuat(obj[VALUE_ID]!.ToObject<quat>(_internalSerializer));
                case SerializedType.Mat2: return Variant.FromMat2(obj[VALUE_ID]!.ToObject<mat2>(_internalSerializer));
                case SerializedType.Mat3: return Variant.FromMat3(obj[VALUE_ID]!.ToObject<mat3>(_internalSerializer));
                case SerializedType.Mat4: return Variant.FromMat4(obj[VALUE_ID]!.ToObject<mat4>(_internalSerializer));
                case SerializedType.Color: return Variant.FromColor(obj[VALUE_ID]!.ToObject<Color>(_internalSerializer));
                case SerializedType.Color32: return Variant.FromColor32(obj[VALUE_ID]!.ToObject<Color32>(_internalSerializer));

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
                    => JToken.FromObject(value.GetValueAsObject(), _internalSerializer),

                _ => throw new JsonSerializationException($"Unsupported SerializedType {value.Kind}")
            };
        }
    }
}