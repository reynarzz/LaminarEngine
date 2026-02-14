using Engine.Utils;
using GlmNet;
using System.Runtime.InteropServices;

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
        internal static Variant GetDefault(SerializedType type)
        {
            switch (type)
            {
                case SerializedType.None:
                case SerializedType.Enum:
                    return default;
                case SerializedType.Char:
                    return '\0';
                case SerializedType.String:
                    return string.Empty;
                case SerializedType.Bool:
                    return false;
                case SerializedType.Byte:
                    return (byte)0;
                case SerializedType.Short:
                    return (short)0;
                case SerializedType.UShort:
                    return (ushort)0;
                case SerializedType.Int:
                    return (int)0;
                case SerializedType.UInt:
                    return (uint)0;
                case SerializedType.Float:
                    return (float)0;
                case SerializedType.Double:
                    return (double)0;
                case SerializedType.Long:
                    return (long)0;
                case SerializedType.ULong:
                    return (ulong)0;
                case SerializedType.Vec2:
                    return new vec2();
                case SerializedType.Vec3:
                    return new vec3();
                case SerializedType.Vec4:
                    return new vec4();
                case SerializedType.IVec2:
                    return new ivec2();
                case SerializedType.IVec3:
                    return new ivec3();
                case SerializedType.IVec4:
                    return new ivec4();
                case SerializedType.Quat:
                    return new quat();
                case SerializedType.Mat2:
                    return new mat2();
                case SerializedType.Mat3:
                    return new mat3();
                case SerializedType.Mat4:
                    return new mat4();
                case SerializedType.Color:
                    return new Color(1, 1, 1, 1);
                case SerializedType.Color32:
                    return new Color32(255, 255, 255, 255);
                default:
                    throw new Exception($"value kind '{type}' Not implements");
            }
        }
        public static implicit operator Variant(char value)
        {
            return new Variant
            {
                Kind = SerializedType.Char,
                value = new Value { Char = value },
                String = null
            };
        }

        public static implicit operator Variant(bool value)
        {
            return new Variant
            {
                Kind = SerializedType.Bool,
                value = new Value { Bool = value },
                String = null
            };
        }

        public static implicit operator Variant(byte value)
        {
            return new Variant
            {
                Kind = SerializedType.Byte,
                value = new Value { Byte = value },
                String = null
            };
        }

        public static implicit operator Variant(short value)
        {
            return new Variant
            {
                Kind = SerializedType.Short,
                value = new Value { Short = value },
                String = null
            };
        }

        public static implicit operator Variant(ushort value)
        {
            return new Variant
            {
                Kind = SerializedType.UShort,
                value = new Value { UShort = value },
                String = null
            };
        }

        public static implicit operator Variant(int value)
        {
            return new Variant
            {
                Kind = SerializedType.Int,
                value = new Value { Int = value },
                String = null
            };
        }

        public static implicit operator Variant(uint value)
        {
            return new Variant
            {
                Kind = SerializedType.UInt,
                value = new Value { Uint = value },
                String = null
            };
        }

        public static implicit operator Variant(float value)
        {
            return new Variant
            {
                Kind = SerializedType.Float,
                value = new Value { Float = value },
                String = null
            };
        }

        public static implicit operator Variant(double value)
        {
            return new Variant
            {
                Kind = SerializedType.Double,
                value = new Value { Double = value },
                String = null
            };
        }

        public static implicit operator Variant(long value)
        {
            return new Variant
            {
                Kind = SerializedType.Long,
                value = new Value { Long = value },
                String = null
            };
        }

        public static implicit operator Variant(ulong value)
        {
            return new Variant
            {
                Kind = SerializedType.ULong,
                value = new Value { Ulong = value },
                String = null
            };
        }

        public static implicit operator Variant(Enum value)
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
                String = null,
                value = default
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
                String = null,
                value = default
            };
        }
        internal static Variant FromEnum(EnumIRValue value)
        {
            return new Variant
            {
                Kind = SerializedType.Enum,
                Enum = value,
                String = null,
                value = default
            };
        }
        public static implicit operator Variant(vec2 value)
        {
            return new Variant
            {
                Kind = SerializedType.Vec2,
                value = new Value { Vec2 = value },
                String = null
            };
        }

        public static implicit operator Variant(vec3 value)
        {
            return new Variant
            {
                Kind = SerializedType.Vec3,
                value = new Value { Vec3 = value },
                String = null
            };
        }

        public static implicit operator Variant(vec4 value)
        {
            return new Variant
            {
                Kind = SerializedType.Vec4,
                value = new Value { Vec4 = value },
                String = null
            };
        }

        public static implicit operator Variant(ivec2 value)
        {
            return new Variant
            {
                Kind = SerializedType.IVec2,
                value = new Value { Ivec2 = value },
                String = null
            };
        }

        public static implicit operator Variant(ivec3 value)
        {
            return new Variant
            {
                Kind = SerializedType.IVec3,
                value = new Value { Ivec3 = value },
                String = null
            };
        }

        public static implicit operator Variant(ivec4 value)
        {
            return new Variant
            {
                Kind = SerializedType.IVec4,
                value = new Value { Ivec4 = value },
                String = null
            };
        }

        public static implicit operator Variant(quat value)
        {
            return new Variant
            {
                Kind = SerializedType.Quat,
                value = new Value { Quat = value },
                String = null
            };
        }

        public static implicit operator Variant(mat2 value)
        {
            return new Variant
            {
                Kind = SerializedType.Mat2,
                value = new Value { Mat2 = value },
                String = null
            };
        }

        public static implicit operator Variant(mat3 value)
        {
            return new Variant
            {
                Kind = SerializedType.Mat3,
                value = new Value { Mat3 = value },
                String = null
            };
        }

        public static implicit operator Variant(mat4 value)
        {
            return new Variant
            {
                Kind = SerializedType.Mat4,
                value = new Value { Mat4 = value },
                String = null
            };
        }

        public static implicit operator Variant(Color value)
        {
            return new Variant
            {
                Kind = SerializedType.Color,
                value = new Value { Color = value },
                String = null
            };
        }

        public static implicit operator Variant(Color32 value)
        {
            return new Variant
            {
                Kind = SerializedType.Color32,
                value = new Value { Color32 = value },
                String = null
            };
        }

        public static implicit operator Variant(string value)
        {
            return new Variant
            {
                Kind = SerializedType.String,
                value = default,
                String = value
            };
        }

        public override string ToString()
        {
            object obj = GetValueAsObject();
            string valueStr;

            if (obj == null)
            {
                valueStr = "null";
            }
            else if (obj is EnumIRValue e)
            {
                valueStr = $"(id: {e.TypeId}, internal: {e.EnumInternalType}, val: {e.EnumValue})";
            }
            else if (obj is string s)
            {
                valueStr = $"\"{s}\"";
            }
            else
            {
                valueStr = obj.ToString();
            }

            return $"Kind: {Kind}, Value: {valueStr}";
        }
    }
}
