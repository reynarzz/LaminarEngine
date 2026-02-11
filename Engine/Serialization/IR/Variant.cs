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

    [StructLayout(LayoutKind.Sequential)]
    internal struct Variant
    {
        [SerializedField] public SerializedType Kind;
        [SerializedField] public Value value;
        [SerializedField] public string String;
        [SerializedField] public EnumIRValue Enum;

        [StructLayout(LayoutKind.Explicit)]
        internal struct Value
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

            internal static Value FromChar(char value)
            {
                return new Value { Char = value };
            }

            internal static Value FromBool(bool value)
            {
                return new Value { Bool = value };
            }

            internal static Value FromByte(byte value)
            {
                return new Value { Byte = value };
            }

            internal static Value FromShort(short value)
            {
                return new Value { Short = value };
            }

            internal static Value FromUShort(ushort value)
            {
                return new Value { UShort = value };
            }

            internal static Value FromInt(int value)
            {
                return new Value { Int = value };
            }

            internal static Value FromUInt(uint value)
            {
                return new Value { Uint = value };
            }

            internal static Value FromFloat(float value)
            {
                return new Value { Float = value };
            }

            internal static Value FromDouble(double value)
            {
                return new Value { Double = value };
            }

            internal static Value FromLong(long value)
            {
                return new Value { Long = value };
            }

            internal static Value FromULong(ulong value)
            {
                return new Value { Ulong = value };
            }

            internal static Value FromVec2(vec2 value)
            {
                return new Value { Vec2 = value };
            }

            internal static Value FromVec3(vec3 value)
            {
                return new Value { Vec3 = value };
            }

            internal static Value FromVec4(vec4 value)
            {
                return new Value { Vec4 = value };
            }

            internal static Value FromIVec2(ivec2 value)
            {
                return new Value { Ivec2 = value };
            }

            internal static Value FromIVec3(ivec3 value)
            {
                return new Value { Ivec3 = value };
            }

            internal static Value FromIVec4(ivec4 value)
            {
                return new Value { Ivec4 = value };
            }

            internal static Value FromQuat(quat value)
            {
                return new Value { Quat = value };
            }

            internal static Value FromMat2(mat2 value)
            {
                return new Value { Mat2 = value };
            }

            internal static Value FromMat3(mat3 value)
            {
                return new Value { Mat3 = value };
            }

            internal static Value FromMat4(mat4 value)
            {
                return new Value { Mat4 = value };
            }

            internal static Value FromColor(Color value)
            {
                return new Value { Color = value };
            }

            internal static Value FromColor32(Color32 value)
            {
                return new Value { Color32 = value };
            }
        }
        internal object GetValueAsObject()
        {
            switch (Kind)
            {
                case SerializedType.None:
                    return null;
                case SerializedType.Enum:
                    return Enum;
                case SerializedType.Char:
                    return value.Char;
                case SerializedType.String:
                    return String;
                case SerializedType.Bool:
                    return value.Bool;
                case SerializedType.Byte:
                    return value.Byte;
                case SerializedType.Short:
                    return value.Short;
                case SerializedType.UShort:
                    return value.UShort;
                case SerializedType.Int:
                    return value.Int;
                case SerializedType.UInt:
                    return value.Uint;
                case SerializedType.Float:
                    return value.Float;
                case SerializedType.Double:
                    return value.Double;
                case SerializedType.Long:
                    return value.Long;
                case SerializedType.ULong:
                    return value.Ulong;
                case SerializedType.Vec2:
                    return value.Vec2;
                case SerializedType.Vec3:
                    return value.Vec3;
                case SerializedType.Vec4:
                    return value.Vec4;
                case SerializedType.IVec2:
                    return value.Ivec2;
                case SerializedType.IVec3:
                    return value.Ivec3;
                case SerializedType.IVec4:
                    return value.Ivec4;
                case SerializedType.Quat:
                    return value.Quat;
                case SerializedType.Mat2:
                    return value.Mat2;
                case SerializedType.Mat3:
                    return value.Mat3;
                case SerializedType.Mat4:
                    return value.Mat4;
                case SerializedType.Color:
                    return value.Color;
                case SerializedType.Color32:
                    return value.Color32;
                default:
                    throw new Exception($"value kind '{Kind}' Not implements");
            }
        }
        internal static Variant FromChar(char value)
        {
            return new Variant
            {
                Kind = SerializedType.Char,
                value = Value.FromChar(value),
                String = null
            };
        }

        internal static Variant FromBool(bool value)
        {
            return new Variant
            {
                Kind = SerializedType.Bool,
                value = Value.FromBool(value),
                String = null
            };
        }

        internal static Variant FromByte(byte value)
        {
            return new Variant
            {
                Kind = SerializedType.Byte,
                value = Value.FromByte(value),
                String = null
            };
        }

        internal static Variant FromShort(short value)
        {
            return new Variant
            {
                Kind = SerializedType.Short,
                value = Value.FromShort(value),
                String = null
            };
        }

        internal static Variant FromUShort(ushort value)
        {
            return new Variant
            {
                Kind = SerializedType.UShort,
                value = Value.FromUShort(value),
                String = null
            };
        }

        internal static Variant FromInt(int value)
        {
            return new Variant
            {
                Kind = SerializedType.Int,
                value = Value.FromInt(value),
                String = null
            };
        }

        internal static Variant FromUInt(uint value)
        {
            return new Variant
            {
                Kind = SerializedType.UInt,
                value = Value.FromUInt(value),
                String = null
            };
        }

        internal static Variant FromFloat(float value)
        {
            return new Variant
            {
                Kind = SerializedType.Float,
                value = Value.FromFloat(value),
                String = null
            };
        }

        internal static Variant FromDouble(double value)
        {
            return new Variant
            {
                Kind = SerializedType.Double,
                value = Value.FromDouble(value),
                String = null
            };
        }

        internal static Variant FromLong(long value)
        {
            return new Variant
            {
                Kind = SerializedType.Long,
                value = Value.FromLong(value),
                String = null
            };
        }

        internal static Variant FromULong(ulong value)
        {
            return new Variant
            {
                Kind = SerializedType.ULong,
                value = Value.FromULong(value),
                String = null
            };
        }

        internal static Variant FromEnum(Enum value)
        {
            return new Variant
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
        internal static Variant FromEnum(Guid id, string internalType, long value)
        {
            return new Variant
            {
                Kind = SerializedType.Enum,
                Enum = new EnumIRValue()
                {
                    TypeId = id,
                    EnumInternalType = internalType,
                    EnumValue = value
                },
                String = null
            };
        }

        internal static Variant FromVec2(vec2 value)
        {
            return new Variant
            {
                Kind = SerializedType.Vec2,
                value = Value.FromVec2(value),
                String = null
            };
        }

        internal static Variant FromVec3(vec3 value)
        {
            return new Variant
            {
                Kind = SerializedType.Vec3,
                value = Value.FromVec3(value),
                String = null
            };
        }

        internal static Variant FromVec4(vec4 value)
        {
            return new Variant
            {
                Kind = SerializedType.Vec4,
                value = Value.FromVec4(value),
                String = null
            };
        }

        internal static Variant FromIVec2(ivec2 value)
        {
            return new Variant
            {
                Kind = SerializedType.IVec2,
                value = Value.FromIVec2(value),
                String = null
            };
        }

        internal static Variant FromIVec3(ivec3 value)
        {
            return new Variant
            {
                Kind = SerializedType.IVec3,
                value = Value.FromIVec3(value),
                String = null
            };
        }

        internal static Variant FromIVec4(ivec4 value)
        {
            return new Variant
            {
                Kind = SerializedType.IVec4,
                value = Value.FromIVec4(value),
                String = null
            };
        }

        internal static Variant FromQuat(quat value)
        {
            return new Variant
            {
                Kind = SerializedType.Quat,
                value = Value.FromQuat(value),
                String = null
            };
        }

        internal static Variant FromMat2(mat2 value)
        {
            return new Variant
            {
                Kind = SerializedType.Mat2,
                value = Value.FromMat2(value),
                String = null
            };
        }

        internal static Variant FromMat3(mat3 value)
        {
            return new Variant
            {
                Kind = SerializedType.Mat3,
                value = Value.FromMat3(value),
                String = null
            };
        }

        internal static Variant FromMat4(mat4 value)
        {
            return new Variant
            {
                Kind = SerializedType.Mat4,
                value = Value.FromMat4(value),
                String = null
            };
        }

        internal static Variant FromColor(Color value)
        {
            return new Variant
            {
                Kind = SerializedType.Color,
                value = Value.FromColor(value),
                String = null
            };
        }

        internal static Variant FromColor32(Color32 value)
        {
            return new Variant
            {
                Kind = SerializedType.Color32,
                value = Value.FromColor32(value),
                String = null
            };
        }

        internal static Variant FromString(string value)
        {
            return new Variant
            {
                Kind = SerializedType.String,
                value = default,
                String = value
            };
        }
    }

}
