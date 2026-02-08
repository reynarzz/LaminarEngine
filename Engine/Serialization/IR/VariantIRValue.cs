using Engine.Utils;
using GlmNet;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.Serialization
{
    internal struct EnumIRValue
    {
        [SerializedField] public Guid TypeId;
        [SerializedField] public string EnumInternalType;
        [SerializedField] public long EnumValue;
    }

    internal struct VariantIRValue
    {
        [SerializedField] public SerializedType Kind;
        [SerializedField] public ValuePayload Payload;
        [SerializedField] public string String;
        [SerializedField] public EnumIRValue Enum;

        internal object GetValueAsObject()
        {
            switch (Kind)
            {
                case SerializedType.None:
                    return null;
                case SerializedType.Enum:
                    return Enum;
                case SerializedType.Char:
                    return Payload.Char;
                case SerializedType.String:
                    return String;
                case SerializedType.Bool:
                    return Payload.Bool;
                case SerializedType.Byte:
                    return Payload.Byte;
                case SerializedType.Short:
                    return Payload.Short;
                case SerializedType.UShort:
                    return Payload.UShort;
                case SerializedType.Int:
                    return Payload.Int;
                case SerializedType.Uint:
                    return Payload.Uint;
                case SerializedType.Float:
                    return Payload.Float;
                case SerializedType.Double:
                    return Payload.Double;
                case SerializedType.Long:
                    return Payload.Long;
                case SerializedType.Ulong:
                    return Payload.Ulong;
                case SerializedType.Vec2:
                    return Payload.Vec2;
                case SerializedType.Vec3:
                    return Payload.Vec3;
                case SerializedType.Vec4:
                    return Payload.Vec4;
                case SerializedType.Ivec2:
                    return Payload.Ivec2;
                case SerializedType.Ivec3:
                    return Payload.Ivec3;
                case SerializedType.Ivec4:
                    return Payload.Ivec4;
                case SerializedType.Quat:
                    return Payload.Quat;
                case SerializedType.Mat2:
                    return Payload.Mat2;
                case SerializedType.Mat3:
                    return Payload.Mat3;
                case SerializedType.Mat4:
                    return Payload.Mat4;
                case SerializedType.Color:
                    return Payload.Color;
                case SerializedType.Color32:
                    return Payload.Color32;
                default:
                    throw new Exception($"value kind '{Kind}' Not implements");
            }
        }
        internal static VariantIRValue FromChar(char value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Char,
                Payload = ValuePayload.FromChar(value),
                String = null
            };
        }

        internal static VariantIRValue FromBool(bool value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Bool,
                Payload = ValuePayload.FromBool(value),
                String = null
            };
        }

        internal static VariantIRValue FromByte(byte value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Byte,
                Payload = ValuePayload.FromByte(value),
                String = null
            };
        }

        internal static VariantIRValue FromShort(short value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Short,
                Payload = ValuePayload.FromShort(value),
                String = null
            };
        }

        internal static VariantIRValue FromUShort(ushort value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.UShort,
                Payload = ValuePayload.FromUShort(value),
                String = null
            };
        }

        internal static VariantIRValue FromInt(int value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Int,
                Payload = ValuePayload.FromInt(value),
                String = null
            };
        }

        internal static VariantIRValue FromUInt(uint value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Uint,
                Payload = ValuePayload.FromUInt(value),
                String = null
            };
        }

        internal static VariantIRValue FromFloat(float value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Float,
                Payload = ValuePayload.FromFloat(value),
                String = null
            };
        }

        internal static VariantIRValue FromDouble(double value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Double,
                Payload = ValuePayload.FromDouble(value),
                String = null
            };
        }

        internal static VariantIRValue FromLong(long value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Long,
                Payload = ValuePayload.FromLong(value),
                String = null
            };
        }

        internal static VariantIRValue FromULong(ulong value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Ulong,
                Payload = ValuePayload.FromULong(value),
                String = null
            };
        }

        internal static VariantIRValue FromEnum(Enum value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Enum,
                Enum = new EnumIRValue()
                {
                    TypeId = ReflectionUtils.GetStableGuid(value.GetType()),
                    EnumInternalType = ReflectionUtils.GetFullTypeName(value.GetType()),
                    EnumValue = Convert.ToInt64(value)
                },
                String = null
            };
        }
        internal static VariantIRValue FromVec2(vec2 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Vec2,
                Payload = ValuePayload.FromVec2(value),
                String = null
            };
        }

        internal static VariantIRValue FromVec3(vec3 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Vec3,
                Payload = ValuePayload.FromVec3(value),
                String = null
            };
        }

        internal static VariantIRValue FromVec4(vec4 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Vec4,
                Payload = ValuePayload.FromVec4(value),
                String = null
            };
        }

        internal static VariantIRValue FromIVec2(ivec2 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Ivec2,
                Payload = ValuePayload.FromIVec2(value),
                String = null
            };
        }

        internal static VariantIRValue FromIVec3(ivec3 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Ivec3,
                Payload = ValuePayload.FromIVec3(value),
                String = null
            };
        }

        internal static VariantIRValue FromIVec4(ivec4 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Ivec4,
                Payload = ValuePayload.FromIVec4(value),
                String = null
            };
        }

        internal static VariantIRValue FromQuat(quat value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Quat,
                Payload = ValuePayload.FromQuat(value),
                String = null
            };
        }

        internal static VariantIRValue FromMat2(mat2 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Mat2,
                Payload = ValuePayload.FromMat2(value),
                String = null
            };
        }

        internal static VariantIRValue FromMat3(mat3 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Mat3,
                Payload = ValuePayload.FromMat3(value),
                String = null
            };
        }

        internal static VariantIRValue FromMat4(mat4 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Mat4,
                Payload = ValuePayload.FromMat4(value),
                String = null
            };
        }

        internal static VariantIRValue FromColor(Color value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Color,
                Payload = ValuePayload.FromColor(value),
                String = null
            };
        }

        internal static VariantIRValue FromColor32(Color32 value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.Color32,
                Payload = ValuePayload.FromColor32(value),
                String = null
            };
        }

        internal static VariantIRValue FromString(string value)
        {
            return new VariantIRValue
            {
                Kind = SerializedType.String,
                Payload = default,
                String = value
            };
        }
    }


    [StructLayout(LayoutKind.Explicit)]
    internal struct ValuePayload
    {
        [FieldOffset(0), SerializedField] internal char Char;
        [FieldOffset(0), SerializedField] internal bool Bool;
        [FieldOffset(0), SerializedField] internal byte Byte;
        [FieldOffset(0), SerializedField] internal short Short;
        [FieldOffset(0), SerializedField] internal ushort UShort;
        [FieldOffset(0), SerializedField] internal int Int;
        [FieldOffset(0), SerializedField] internal uint Uint;
        [FieldOffset(0), SerializedField] internal float Float;
        [FieldOffset(0), SerializedField] internal double Double;
        [FieldOffset(0), SerializedField] internal long Long;
        [FieldOffset(0), SerializedField] internal ulong Ulong;
        [FieldOffset(0), SerializedField] internal vec2 Vec2;
        [FieldOffset(0), SerializedField] internal vec3 Vec3;
        [FieldOffset(0), SerializedField] internal vec4 Vec4;
        [FieldOffset(0), SerializedField] internal ivec2 Ivec2;
        [FieldOffset(0), SerializedField] internal ivec3 Ivec3;
        [FieldOffset(0), SerializedField] internal ivec4 Ivec4;
        [FieldOffset(0), SerializedField] internal quat Quat;
        [FieldOffset(0), SerializedField] internal mat2 Mat2;
        [FieldOffset(0), SerializedField] internal mat3 Mat3;
        [FieldOffset(0), SerializedField] internal mat4 Mat4;
        [FieldOffset(0), SerializedField] internal Color Color;
        [FieldOffset(0), SerializedField] internal Color32 Color32;

        internal static ValuePayload FromChar(char value)
        {
            return new ValuePayload { Char = value };
        }

        internal static ValuePayload FromBool(bool value)
        {
            return new ValuePayload { Bool = value };
        }

        internal static ValuePayload FromByte(byte value)
        {
            return new ValuePayload { Byte = value };
        }

        internal static ValuePayload FromShort(short value)
        {
            return new ValuePayload { Short = value };
        }

        internal static ValuePayload FromUShort(ushort value)
        {
            return new ValuePayload { UShort = value };
        }

        internal static ValuePayload FromInt(int value)
        {
            return new ValuePayload { Int = value };
        }

        internal static ValuePayload FromUInt(uint value)
        {
            return new ValuePayload { Uint = value };
        }

        internal static ValuePayload FromFloat(float value)
        {
            return new ValuePayload { Float = value };
        }

        internal static ValuePayload FromDouble(double value)
        {
            return new ValuePayload { Double = value };
        }

        internal static ValuePayload FromLong(long value)
        {
            return new ValuePayload { Long = value };
        }

        internal static ValuePayload FromULong(ulong value)
        {
            return new ValuePayload { Ulong = value };
        }

        internal static ValuePayload FromVec2(vec2 value)
        {
            return new ValuePayload { Vec2 = value };
        }

        internal static ValuePayload FromVec3(vec3 value)
        {
            return new ValuePayload { Vec3 = value };
        }

        internal static ValuePayload FromVec4(vec4 value)
        {
            return new ValuePayload { Vec4 = value };
        }

        internal static ValuePayload FromIVec2(ivec2 value)
        {
            return new ValuePayload { Ivec2 = value };
        }

        internal static ValuePayload FromIVec3(ivec3 value)
        {
            return new ValuePayload { Ivec3 = value };
        }

        internal static ValuePayload FromIVec4(ivec4 value)
        {
            return new ValuePayload { Ivec4 = value };
        }

        internal static ValuePayload FromQuat(quat value)
        {
            return new ValuePayload { Quat = value };
        }

        internal static ValuePayload FromMat2(mat2 value)
        {
            return new ValuePayload { Mat2 = value };
        }

        internal static ValuePayload FromMat3(mat3 value)
        {
            return new ValuePayload { Mat3 = value };
        }

        internal static ValuePayload FromMat4(mat4 value)
        {
            return new ValuePayload { Mat4 = value };
        }

        internal static ValuePayload FromColor(Color value)
        {
            return new ValuePayload { Color = value };
        }

        internal static ValuePayload FromColor32(Color32 value)
        {
            return new ValuePayload { Color32 = value };
        }
    }
}
