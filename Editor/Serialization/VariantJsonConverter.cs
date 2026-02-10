using Engine;
using Engine.Serialization;
using GlmNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Editor.Serialization
{
    internal sealed class VariantJsonConverter : JsonConverter<VariantIRValue>
    {
        private const string KIND_ID = "Kind";
        private const string VALUE_ID = "Value";
        private const string ENUM_ID = "Enum";

        // A private serializer without VariantJsonConverter to avoid the self-referencing loop
        private static readonly JsonSerializer _internalSerializer = new JsonSerializer();

        public override void WriteJson(JsonWriter writer, VariantIRValue value, JsonSerializer serializer)
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

        public override VariantIRValue ReadJson(JsonReader reader, Type objectType, VariantIRValue existingValue, bool hasExistingValue, JsonSerializer serializer)
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
                    return VariantIRValue.FromString(obj[VALUE_ID]?.Value<string>());

                case SerializedType.Enum:
                    return new VariantIRValue
                    {
                        Kind = SerializedType.Enum,
                        Enum = obj[ENUM_ID]!.ToObject<EnumIRValue>(_internalSerializer)
                    };

                case SerializedType.Char: return VariantIRValue.FromChar(obj[VALUE_ID]!.Value<char>());
                case SerializedType.Bool: return VariantIRValue.FromBool(obj[VALUE_ID]!.Value<bool>());
                case SerializedType.Byte: return VariantIRValue.FromByte(obj[VALUE_ID]!.Value<byte>());
                case SerializedType.Short: return VariantIRValue.FromShort(obj[VALUE_ID]!.Value<short>());
                case SerializedType.UShort: return VariantIRValue.FromUShort(obj[VALUE_ID]!.Value<ushort>());
                case SerializedType.Int: return VariantIRValue.FromInt(obj[VALUE_ID]!.Value<int>());
                case SerializedType.UInt: return VariantIRValue.FromUInt(obj[VALUE_ID]!.Value<uint>());
                case SerializedType.Float: return VariantIRValue.FromFloat(obj[VALUE_ID]!.Value<float>());
                case SerializedType.Double: return VariantIRValue.FromDouble(obj[VALUE_ID]!.Value<double>());
                case SerializedType.Long: return VariantIRValue.FromLong(obj[VALUE_ID]!.Value<long>());
                case SerializedType.ULong: return VariantIRValue.FromULong(ulong.Parse(obj[VALUE_ID]!.Value<string>()));

                case SerializedType.Vec2: return VariantIRValue.FromVec2(obj[VALUE_ID]!.ToObject<vec2>(_internalSerializer));
                case SerializedType.Vec3: return VariantIRValue.FromVec3(obj[VALUE_ID]!.ToObject<vec3>(_internalSerializer));
                case SerializedType.Vec4: return VariantIRValue.FromVec4(obj[VALUE_ID]!.ToObject<vec4>(_internalSerializer));
                case SerializedType.IVec2: return VariantIRValue.FromIVec2(obj[VALUE_ID]!.ToObject<ivec2>(_internalSerializer));
                case SerializedType.IVec3: return VariantIRValue.FromIVec3(obj[VALUE_ID]!.ToObject<ivec3>(_internalSerializer));
                case SerializedType.IVec4: return VariantIRValue.FromIVec4(obj[VALUE_ID]!.ToObject<ivec4>(_internalSerializer));
                case SerializedType.Quat: return VariantIRValue.FromQuat(obj[VALUE_ID]!.ToObject<quat>(_internalSerializer));
                case SerializedType.Mat2: return VariantIRValue.FromMat2(obj[VALUE_ID]!.ToObject<mat2>(_internalSerializer));
                case SerializedType.Mat3: return VariantIRValue.FromMat3(obj[VALUE_ID]!.ToObject<mat3>(_internalSerializer));
                case SerializedType.Mat4: return VariantIRValue.FromMat4(obj[VALUE_ID]!.ToObject<mat4>(_internalSerializer));
                case SerializedType.Color: return VariantIRValue.FromColor(obj[VALUE_ID]!.ToObject<Color>(_internalSerializer));
                case SerializedType.Color32: return VariantIRValue.FromColor32(obj[VALUE_ID]!.ToObject<Color32>(_internalSerializer));

                default:
                    throw new JsonSerializationException($"Unsupported SerializedType {kind}");
            }
        }

        private static JToken CreatePayloadToken(VariantIRValue value)
        {
            return value.Kind switch
            {
                SerializedType.Char => value.Payload.Char,
                SerializedType.Bool => value.Payload.Bool,
                SerializedType.Byte => value.Payload.Byte,
                SerializedType.Short => value.Payload.Short,
                SerializedType.UShort => value.Payload.UShort,
                SerializedType.Int => value.Payload.Int,
                SerializedType.UInt => value.Payload.Uint,
                SerializedType.Float => value.Payload.Float,
                SerializedType.Double => value.Payload.Double,
                SerializedType.Long => value.Payload.Long,
                SerializedType.ULong => value.Payload.Ulong.ToString(),

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