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
        internal CollectionDataChar(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataChar(char[] value, CollectionType collectionType) : base(value, SerializedType.Char, collectionType) { }
    }

    internal sealed class CollectionDataBool : CollectionData<bool>
    {
        protected CollectionDataBool() { }
        internal CollectionDataBool(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataBool(bool[] value, CollectionType collectionType) : base(value, SerializedType.Bool, collectionType) { }
    }

    internal sealed class CollectionDataByte : CollectionData<byte>
    {
        protected CollectionDataByte() { }
        internal CollectionDataByte(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataByte(byte[] value, CollectionType collectionType) : base(value, SerializedType.Byte, collectionType) { }
    }

    internal sealed class CollectionDataShort : CollectionData<short>
    {
        protected CollectionDataShort() { }
        internal CollectionDataShort(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataShort(short[] value, CollectionType collectionType) : base(value, SerializedType.Short, collectionType) { }
    }

    internal sealed class CollectionDataUShort : CollectionData<ushort>
    {
        protected CollectionDataUShort() { }
        internal CollectionDataUShort(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataUShort(ushort[] value, CollectionType collectionType) : base(value, SerializedType.UShort, collectionType) { }
    }

    internal sealed class CollectionDataInt : CollectionData<int>
    {
        protected CollectionDataInt() { }
        internal CollectionDataInt(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataInt(int[] value, CollectionType collectionType) : base(value, SerializedType.Int, collectionType) { }
    }

    internal sealed class CollectionDataUint : CollectionData<uint>
    {
        protected CollectionDataUint() { }
        internal CollectionDataUint(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataUint(uint[] value, CollectionType collectionType) : base(value, SerializedType.UInt, collectionType) { }
    }

    internal sealed class CollectionDataFloat : CollectionData<float>
    {
        protected CollectionDataFloat() { }
        internal CollectionDataFloat(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataFloat(float[] value, CollectionType collectionType) : base(value, SerializedType.Float, collectionType) { }
    }

    internal sealed class CollectionDataDouble : CollectionData<double>
    {
        protected CollectionDataDouble() { }
        internal CollectionDataDouble(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataDouble(double[] value, CollectionType collectionType) : base(value, SerializedType.Double, collectionType) { }
    }

    internal sealed class CollectionDataLong : CollectionData<long>
    {
        protected CollectionDataLong() { }
        internal CollectionDataLong(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataLong(long[] value, CollectionType collectionType) : base(value, SerializedType.Long, collectionType) { }
    }

    internal sealed class CollectionDataUlong : CollectionData<ulong>
    {
        protected CollectionDataUlong() { }
        internal CollectionDataUlong(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataUlong(ulong[] value, CollectionType collectionType) : base(value, SerializedType.ULong, collectionType) { }
    }

    internal sealed class CollectionDataVec2 : CollectionData<vec2>
    {
        protected CollectionDataVec2() { }
        internal CollectionDataVec2(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataVec2(vec2[] value, CollectionType collectionType) : base(value, SerializedType.Vec2, collectionType) { }
    }

    internal sealed class CollectionDataVec3 : CollectionData<vec3>
    {
        protected CollectionDataVec3() { }
        internal CollectionDataVec3(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataVec3(vec3[] value, CollectionType collectionType) : base(value, SerializedType.Vec3, collectionType) { }
    }

    internal sealed class CollectionDataVec4 : CollectionData<vec4>
    {
        protected CollectionDataVec4() { }
        internal CollectionDataVec4(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataVec4(vec4[] value, CollectionType collectionType) : base(value, SerializedType.Vec4, collectionType) { }
    }

    internal sealed class CollectionDataIvec2 : CollectionData<ivec2>
    {
        protected CollectionDataIvec2() { }
        internal CollectionDataIvec2(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataIvec2(ivec2[] value, CollectionType collectionType) : base(value, SerializedType.IVec2, collectionType) { }
    }

    internal sealed class CollectionDataIvec3 : CollectionData<ivec3>
    {
        protected CollectionDataIvec3() { }
        internal CollectionDataIvec3(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataIvec3(ivec3[] value, CollectionType collectionType) : base(value, SerializedType.IVec3, collectionType) { }
    }

    internal sealed class CollectionDataIvec4 : CollectionData<ivec4>
    {
        protected CollectionDataIvec4() { }
        internal CollectionDataIvec4(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataIvec4(ivec4[] value, CollectionType collectionType) : base(value, SerializedType.IVec4, collectionType) { }
    }

    internal sealed class CollectionDataQuat : CollectionData<quat>
    {
        protected CollectionDataQuat() { }
        internal CollectionDataQuat(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataQuat(quat[] value, CollectionType collectionType) : base(value, SerializedType.Quat, collectionType) { }
    }

    internal sealed class CollectionDataMat2 : CollectionData<mat2>
    {
        protected CollectionDataMat2() { }
        internal CollectionDataMat2(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataMat2(mat2[] value, CollectionType collectionType) : base(value, SerializedType.Mat2, collectionType) { }
    }

    internal sealed class CollectionDataMat3 : CollectionData<mat3>
    {
        protected CollectionDataMat3() { }
        internal CollectionDataMat3(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataMat3(mat3[] value, CollectionType collectionType) : base(value, SerializedType.Mat3, collectionType) { }
    }

    internal sealed class CollectionDataMat4 : CollectionData<mat4>
    {
        protected CollectionDataMat4() { }
        internal CollectionDataMat4(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataMat4(mat4[] value, CollectionType collectionType) : base(value, SerializedType.Mat4, collectionType) { }
    }

    internal sealed class CollectionDataColor : CollectionData<Color>
    {
        protected CollectionDataColor() { }
        internal CollectionDataColor(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataColor(Color[] value, CollectionType collectionType) : base(value, SerializedType.Color, collectionType) { }
    }

    internal sealed class CollectionDataColor32 : CollectionData<Color32>
    {
        protected CollectionDataColor32() { }
        internal CollectionDataColor32(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataColor32(Color32[] value, CollectionType collectionType) : base(value, SerializedType.Color32, collectionType) { }
    }

    internal sealed class CollectionDataEnum : CollectionData<EnumIRValue>
    {
        protected CollectionDataEnum() { }
        internal CollectionDataEnum(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataEnum(EnumIRValue[] value, CollectionType collectionType) : base(value, SerializedType.Enum, collectionType) { }
    }

    internal sealed class CollectionDataString : CollectionData<string>
    {
        protected CollectionDataString() { }
        internal CollectionDataString(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionDataString(string[] value, CollectionType collectionType) : base(value, SerializedType.String, collectionType) { }
    }

}
