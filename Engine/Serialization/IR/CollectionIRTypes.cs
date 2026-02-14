using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal sealed class CollectionDataChar : CollectionData<char>
    {
        protected CollectionDataChar() { }
        public CollectionDataChar(char[] value, CollectionType collectionType) : base(value, SerializedType.Char, collectionType) { }
    }

    internal sealed class CollectionDataBool : CollectionData<bool>
    {
        protected CollectionDataBool() { }
        public CollectionDataBool(bool[] value, CollectionType collectionType) : base(value, SerializedType.Bool, collectionType) { }
    }

    internal sealed class CollectionDataByte : CollectionData<byte>
    {
        protected CollectionDataByte() { }
        public CollectionDataByte(byte[] value, CollectionType collectionType) : base(value, SerializedType.Byte, collectionType) { }
    }

    internal sealed class CollectionDataShort : CollectionData<short>
    {
        protected CollectionDataShort() { }
        public CollectionDataShort(short[] value, CollectionType collectionType) : base(value, SerializedType.Short, collectionType) { }
    }

    internal sealed class CollectionDataUShort : CollectionData<ushort>
    {
        protected CollectionDataUShort() { }
        public CollectionDataUShort(ushort[] value, CollectionType collectionType) : base(value, SerializedType.UShort, collectionType) { }
    }

    internal sealed class CollectionDataInt : CollectionData<int>
    {
        protected CollectionDataInt() { }
        public CollectionDataInt(int[] value, CollectionType collectionType) : base(value, SerializedType.Int, collectionType) { }
    }

    internal sealed class CollectionDataUInt : CollectionData<uint>
    {
        protected CollectionDataUInt() { }
        public CollectionDataUInt(uint[] value, CollectionType collectionType) : base(value, SerializedType.UInt, collectionType) { }
    }

    internal sealed class CollectionDataFloat : CollectionData<float>
    {
        protected CollectionDataFloat() { }
        public CollectionDataFloat(float[] value, CollectionType collectionType) : base(value, SerializedType.Float, collectionType) { }
    }

    internal sealed class CollectionDataDouble : CollectionData<double>
    {
        protected CollectionDataDouble() { }
        public CollectionDataDouble(double[] value, CollectionType collectionType) : base(value, SerializedType.Double, collectionType) { }
    }

    internal sealed class CollectionDataLong : CollectionData<long>
    {
        protected CollectionDataLong() { }
        public CollectionDataLong(long[] value, CollectionType collectionType) : base(value, SerializedType.Long, collectionType) { }
    }

    internal sealed class CollectionDataULong : CollectionData<ulong>
    {
        protected CollectionDataULong() { }
        public CollectionDataULong(ulong[] value, CollectionType collectionType) : base(value, SerializedType.ULong, collectionType) { }
    }

    internal sealed class CollectionDataVec2 : CollectionData<vec2>
    {
        protected CollectionDataVec2() { }
        public CollectionDataVec2(vec2[] value, CollectionType collectionType) : base(value, SerializedType.Vec2, collectionType) { }
    }

    internal sealed class CollectionDataVec3 : CollectionData<vec3>
    {
        protected CollectionDataVec3() { }
        public CollectionDataVec3(vec3[] value, CollectionType collectionType) : base(value, SerializedType.Vec3, collectionType) { }
    }

    internal sealed class CollectionDataVec4 : CollectionData<vec4>
    {
        protected CollectionDataVec4() { }
        public CollectionDataVec4(vec4[] value, CollectionType collectionType) : base(value, SerializedType.Vec4, collectionType) { }
    }

    internal sealed class CollectionDataIvec2 : CollectionData<ivec2>
    {
        protected CollectionDataIvec2() { }
        public CollectionDataIvec2(ivec2[] value, CollectionType collectionType) : base(value, SerializedType.IVec2, collectionType) { }
    }

    internal sealed class CollectionDataIvec3 : CollectionData<ivec3>
    {
        protected CollectionDataIvec3() { }
        public CollectionDataIvec3(ivec3[] value, CollectionType collectionType) : base(value, SerializedType.IVec3, collectionType) { }
    }

    internal sealed class CollectionDataIvec4 : CollectionData<ivec4>
    {
        protected CollectionDataIvec4() { }
        public CollectionDataIvec4(ivec4[] value, CollectionType collectionType) : base(value, SerializedType.IVec4, collectionType) { }
    }

    internal sealed class CollectionDataQuat : CollectionData<quat>
    {
        protected CollectionDataQuat() { }
        public CollectionDataQuat(quat[] value, CollectionType collectionType) : base(value, SerializedType.Quat, collectionType) { }
    }

    internal sealed class CollectionDataMat2 : CollectionData<mat2>
    {
        protected CollectionDataMat2() { }
        public CollectionDataMat2(mat2[] value, CollectionType collectionType) : base(value, SerializedType.Mat2, collectionType) { }
    }

    internal sealed class CollectionDataMat3 : CollectionData<mat3>
    {
        protected CollectionDataMat3() { }
        public CollectionDataMat3(mat3[] value, CollectionType collectionType) : base(value, SerializedType.Mat3, collectionType) { }
    }

    internal sealed class CollectionDataMat4 : CollectionData<mat4>
    {
        protected CollectionDataMat4() { }
        public CollectionDataMat4(mat4[] value, CollectionType collectionType) : base(value, SerializedType.Mat4, collectionType) { }
    }

    internal sealed class CollectionDataColor : CollectionData<Color>
    {
        protected CollectionDataColor() { }
        public CollectionDataColor(Color[] value, CollectionType collectionType) : base(value, SerializedType.Color, collectionType) { }
    }

    internal sealed class CollectionDataColor32 : CollectionData<Color32>
    {
        protected CollectionDataColor32() { }
        public CollectionDataColor32(Color32[] value, CollectionType collectionType) : base(value, SerializedType.Color32, collectionType) { }
    }

    internal sealed class CollectionDataEnum : CollectionData<EnumIRValue>
    {
        protected CollectionDataEnum() { }
        public CollectionDataEnum(EnumIRValue[] value, CollectionType collectionType) : base(value, SerializedType.Enum, collectionType) { }
    }

    internal sealed class CollectionDataString : CollectionData<string>
    {
        protected CollectionDataString() { }
        public CollectionDataString(string[] value, CollectionType collectionType) : base(value, SerializedType.String, collectionType) { }
    }

}
