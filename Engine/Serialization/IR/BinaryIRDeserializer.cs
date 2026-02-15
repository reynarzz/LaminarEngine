using Engine.Utils;
using GlmNet;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal class BinaryIRDeserializer
    {
        private readonly static ReferenceBinaryReaderFactory _referenceReader = new();
        internal static SerializedPropertyIR[] Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            var properties = new SerializedPropertyIR[count];

            for (int i = 0; i < count; i++)
            {
                properties[i] = ReadPropertyIR(reader);
            }

            return properties;
        }
        internal static MaterialIR DeserializeMaterial(BinaryReader reader)
        {
            var material = new MaterialIR();
            material.Version = reader.ReadInt32();
            var propCount = reader.ReadInt32();

            material.Properties = new SerializedPropertyIR[propCount];
            for (int i = 0; i < propCount; i++)
            {
                material.Properties[i] = ReadPropertyIR(reader);
            }

            return material;
        }
        internal static ShaderIR DeserializeShader(BinaryReader reader)
        {
            var shader = new ShaderIR();
            shader.Version = reader.ReadInt32();
            shader.Properties = Deserialize(reader);
            return shader;
        }
        internal static SceneIR DeserializeScene(BinaryReader reader)
        {
            var scene = new SceneIR();
            scene.SceneVersion = reader.ReadInt32();
            scene.ActorsVersion = reader.ReadInt32();
            scene.ComponentsVersion = reader.ReadInt32();

            var count = reader.ReadInt32();

            scene.Actors = new List<ActorIR>();
            CollectionsMarshal.SetCount(scene.Actors, count);

            for (int i = 0; i < count; i++)
            {
                scene.Actors[i] = ReadActorIR(reader);
            }

            return scene;
        }

        private static ActorIR ReadActorIR(BinaryReader reader)
        {
            /*
                 string Name 
                 int Layer 
                 bool IsActiveSelf 
                 Guid ID 
                 Guid ParentID 
                 List<ComponentIR> Components 
            */

            var actor = new ActorIR();
            actor.Name = ReadString(reader);
            actor.Layer = reader.ReadInt32();
            actor.IsActiveSelf = ReadBool(reader);
            actor.ID = FileUtils.ReadGuidNoAlloc(reader);
            actor.ParentID = FileUtils.ReadGuidNoAlloc(reader);
            var componentsCount = reader.ReadInt32();

            actor.Components = new List<ComponentIR>();
            CollectionsMarshal.SetCount(actor.Components, componentsCount);

            for (int i = 0; i < componentsCount; i++)
            {
                actor.Components[i] = ReadComponentIR(reader);
            }

            return actor;
        }

        private static ComponentIR ReadComponentIR(BinaryReader reader)
        {
            /*
               Guid TypeId 
               bool IsEnabled 
               Guid ID 
               List<SerializedPropertyIR> SerializedProperties 
            */
            var component = new ComponentIR();
            component.TypeId = FileUtils.ReadGuidNoAlloc(reader);
            component.IsEnabled = ReadBool(reader);
            component.ID = FileUtils.ReadGuidNoAlloc(reader);
            var propertiesCount = reader.ReadInt32();
            component.Properties = new SerializedPropertyIR[propertiesCount];

            for (int i = 0; i < propertiesCount; i++)
            {
                component.Properties[i] = ReadPropertyIR(reader);
            }

            return component;
        }

        private static SerializedPropertyIR ReadPropertyIR(BinaryReader reader)
        {
            /*
              string Name 
              Guid TypeId 
              SerializedType Type 
              object Data 
          */
            var property = new SerializedPropertyIR();
            property.Name = ReadString(reader);
            property.TypeId = FileUtils.ReadGuidNoAlloc(reader);
            property.Type = (SerializedType)reader.ReadUInt64();
            var serializedType = property.Type;

            if (serializedType.IsSimple())
            {
                property.Simple = ReadSimpleProperty(reader, serializedType);
            }
            else if (serializedType.IsEObject())
            {
                property.Reference = ReadReferenceProperty(reader);
            }
            else if (serializedType == SerializedType.ReferenceCollection)
            {
                property.Collection = ReadReferenceCollection(reader);
            }
            else if (serializedType.IsClass())
            {
                if (serializedType == SerializedType.ComplexClass)
                {
                    property.Class = ReadComplexClass(reader);
                }
                else
                {
                    throw new NotImplementedException($"Class type '{serializedType}' is not implemented.");
                }
            }
            else if (serializedType == SerializedType.ComplexCollection)
            {
                property.Collection = ReadComplexCollection(reader);
            }
            else if (serializedType == SerializedType.SimpleCollection)
            {
                property.Collection = ReadSimpleCollection(reader);
            }
            else
            {
                throw new NotImplementedException($"Can't deserialize type, not implemented, {serializedType}");
            }
            return property;
        }

        private static string ReadString(BinaryReader reader)
        {
            var totalBytes = reader.ReadInt32();

            if (totalBytes <= 0)
            {
                return string.Empty;
            }

            if (totalBytes <= 1024) // a kb
            {
                Span<byte> buffer = stackalloc byte[totalBytes];
                reader.BaseStream.ReadExactly(buffer);
                return Encoding.UTF8.GetString(buffer);
            }
            else
            {
                var buffer = ArrayPool<byte>.Shared.Rent(totalBytes);
                try
                {
                    var span = buffer.AsSpan(0, totalBytes);
                    reader.BaseStream.ReadExactly(span);
                    return Encoding.UTF8.GetString(span);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        private static CollectionData ReadSimpleCollection(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            if (count == 0)
            {
                return null;
            }
            var collectionType = (CollectionType)reader.ReadInt32();
            if (collectionType == CollectionType.Dictionary)
            {
                var simpleDictionary = new DictionarySimple(count, collectionType);
                simpleDictionary.KeyType = (SerializedType)reader.ReadUInt64();
                simpleDictionary.ValueType = (SerializedType)reader.ReadUInt64();
                simpleDictionary.Keys = ReadVariantArray(reader, simpleDictionary.KeyType, simpleDictionary.Count);
                simpleDictionary.Values = ReadVariantArray(reader, simpleDictionary.ValueType, simpleDictionary.Count);
                return simpleDictionary;
            }

            var itemsType = (SerializedType)reader.ReadUInt64();
            return ReadSimpleArray(reader, itemsType, count, collectionType);
        }

        private static CollectionData ReadSimpleArray(BinaryReader reader, SerializedType itemsType, int count, CollectionType collectionType)
        {
            switch (itemsType)
            {
                case SerializedType.None:
                    return null;
                case SerializedType.Enum:
                    {
                        var values = new EnumIRValue[count];
                        for (int i = 0; i < count; i++)
                        {
                            values[i] = ReadEnum(reader);
                        }
                        return new CollectionDataEnum(values, collectionType);
                    }
                case SerializedType.Char:
                    return new CollectionDataChar(ReadArray<char>(reader, count), collectionType);
                case SerializedType.String:
                    {
                        var values = new string[count];

                        for (int i = 0; i < count; i++)
                        {
                            values[i] = ReadString(reader);
                        }
                        return new CollectionDataString(values, collectionType);
                    }
                case SerializedType.Bool:
                    return new CollectionDataBool(ReadArray<bool>(reader, count), collectionType);
                case SerializedType.Byte:
                    return new CollectionDataByte(ReadArray<byte>(reader, count), collectionType);
                case SerializedType.Short:
                    return new CollectionDataShort(ReadArray<short>(reader, count), collectionType);
                case SerializedType.UShort:
                    return new CollectionDataUShort(ReadArray<ushort>(reader, count), collectionType);
                case SerializedType.Int:
                    return new CollectionDataInt(ReadArray<int>(reader, count), collectionType);
                case SerializedType.UInt:
                    return new CollectionDataUInt(ReadArray<uint>(reader, count), collectionType);
                case SerializedType.Float:
                    return new CollectionDataFloat(ReadArray<float>(reader, count), collectionType);
                case SerializedType.Double:
                    return new CollectionDataDouble(ReadArray<double>(reader, count), collectionType);
                case SerializedType.Long:
                    return new CollectionDataLong(ReadArray<long>(reader, count), collectionType);
                case SerializedType.ULong:
                    return new CollectionDataULong(ReadArray<ulong>(reader, count), collectionType);
                case SerializedType.Vec2:
                    return new CollectionDataVec2(ReadArray<vec2>(reader, count), collectionType);
                case SerializedType.Vec3:
                    return new CollectionDataVec3(ReadArray<vec3>(reader, count), collectionType);
                case SerializedType.Vec4:
                    return new CollectionDataVec4(ReadArray<vec4>(reader, count), collectionType);
                case SerializedType.IVec2:
                    return new CollectionDataIvec2(ReadArray<ivec2>(reader, count), collectionType);
                case SerializedType.IVec3:
                    return new CollectionDataIvec3(ReadArray<ivec3>(reader, count), collectionType);
                case SerializedType.IVec4:
                    return new CollectionDataIvec4(ReadArray<ivec4>(reader, count), collectionType);
                case SerializedType.Quat:
                    return new CollectionDataQuat(ReadArray<quat>(reader, count), collectionType);
                case SerializedType.Mat2:
                    return new CollectionDataMat2(ReadArray<mat2>(reader, count), collectionType);
                case SerializedType.Mat3:
                    return new CollectionDataMat3(ReadArray<mat3>(reader, count), collectionType);
                case SerializedType.Mat4:
                    return new CollectionDataMat4(ReadArray<mat4>(reader, count), collectionType);
                case SerializedType.Color:
                    return new CollectionDataColor(ReadArray<Color>(reader, count), collectionType);
                case SerializedType.Color32:
                    return new CollectionDataColor32(ReadArray<Color32>(reader, count), collectionType);
                default:
                    throw new NotImplementedException($"Item not implemented: {itemsType}");
            }
        }

        private static Variant[] ReadVariantArray(BinaryReader reader, SerializedType kind, int count)
        {
            if (kind == SerializedType.None)
                return new Variant[count];

            if (kind == SerializedType.String)
            {
                var strVariants = new Variant[count];
                for (int i = 0; i < strVariants.Length; i++)
                {
                    strVariants[i] = ReadString(reader);
                }
                return strVariants;
            }

            if (kind == SerializedType.Enum)
            {
                var enumVariants = new Variant[count];
                for (int i = 0; i < enumVariants.Length; i++)
                {
                    enumVariants[i] = Variant.FromEnum(ReadEnum(reader));
                }
                return enumVariants;
            }

            switch (kind)
            {
                case SerializedType.Char:
                    return ReadPayloadSpan<char>(reader, count, kind);
                case SerializedType.Bool:
                    return ReadBoolPayloadSpan(reader, count);
                case SerializedType.Byte:
                    return ReadPayloadSpan<byte>(reader, count, kind);
                case SerializedType.Short:
                    return ReadPayloadSpan<short>(reader, count, kind);
                case SerializedType.UShort:
                    return ReadPayloadSpan<ushort>(reader, count, kind);
                case SerializedType.Int:
                    return ReadPayloadSpan<int>(reader, count, kind);
                case SerializedType.UInt:
                    return ReadPayloadSpan<uint>(reader, count, kind);
                case SerializedType.Long:
                    return ReadPayloadSpan<long>(reader, count, kind);
                case SerializedType.ULong:
                    return ReadPayloadSpan<ulong>(reader, count, kind);
                case SerializedType.Float:
                    return ReadPayloadSpan<float>(reader, count, kind);
                case SerializedType.Double:
                    return ReadPayloadSpan<double>(reader, count, kind);
                case SerializedType.Vec2:
                    return ReadPayloadSpan<vec2>(reader, count, kind);
                case SerializedType.Vec3:
                    return ReadPayloadSpan<vec3>(reader, count, kind);
                case SerializedType.Vec4:
                    return ReadPayloadSpan<vec4>(reader, count, kind);
                case SerializedType.IVec2:
                    return ReadPayloadSpan<ivec2>(reader, count, kind);
                case SerializedType.IVec3:
                    return ReadPayloadSpan<ivec3>(reader, count, kind);
                case SerializedType.IVec4:
                    return ReadPayloadSpan<ivec4>(reader, count, kind);
                case SerializedType.Quat:
                    return ReadPayloadSpan<quat>(reader, count, kind);
                case SerializedType.Mat2:
                    return ReadPayloadSpan<mat2>(reader, count, kind);
                case SerializedType.Mat3:
                    return ReadPayloadSpan<mat3>(reader, count, kind);
                case SerializedType.Mat4:
                    return ReadPayloadSpan<mat4>(reader, count, kind);
                case SerializedType.Color:
                    return ReadPayloadSpan<Color>(reader, count, kind);
                case SerializedType.Color32:
                    return ReadPayloadSpan<Color32>(reader, count, kind);
                default:
                    throw new Exception($"Unsupported SerializedType: {kind}");
            }
        }

        private static unsafe Variant[] ReadPayloadSpan<T>(BinaryReader reader, int count, SerializedType kind)
            where T : unmanaged
        {
            var variants = new Variant[count];

            int totalBytes = count * sizeof(T);
            var buffer = ArrayPool<byte>.Shared.Rent(totalBytes);

            try
            {
                reader.BaseStream.ReadExactly(buffer.AsSpan(0, totalBytes));

                for (int i = 0; i < count; i++)
                {
                    ref byte b = ref buffer[i * sizeof(T)];
                    T tmp = Unsafe.ReadUnaligned<T>(ref b);
                    variants[i] = new Variant
                    {
                        Kind = kind,
                        value = Unsafe.As<T, Variant.Value>(ref tmp),
                        String = null,
                        Enum = default
                    };
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return variants;
        }

        private static T[] ReadArray<T>(BinaryReader reader, int count) where T : unmanaged
        {
            var values = new T[count];
            Span<byte> bytes = MemoryMarshal.AsBytes(values.AsSpan());
            reader.BaseStream.ReadExactly(bytes);
            return values;
        }

        private static Variant[] ReadBoolPayloadSpan(BinaryReader reader, int count)
        {
            var variants = new Variant[count];

            if (count <= 512) // half a kb
            {
                // Small array, use stackalloc to avoid heap allocation
                Span<byte> buffer = stackalloc byte[count];
                reader.BaseStream.ReadExactly(buffer);

                for (int i = 0; i < count; i++)
                {
                    variants[i] = ByteToBool(buffer[i]);
                }
            }
            else
            {
                // large array, rent from ArrayPool to avoid GC pressure
                var buffer = ArrayPool<byte>.Shared.Rent(count);
                try
                {
                    reader.BaseStream.ReadExactly(buffer.AsSpan(0, count));

                    for (int i = 0; i < count; i++)
                    {
                        variants[i] = ByteToBool(buffer[i]);
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer, clearArray: false);
                }
            }

            return variants;
        }


        private static bool ByteToBool(byte value)
        {
            return value != 0;
        }
        private static bool ReadBool(BinaryReader reader)
        {
            return ByteToBool(reader.ReadByte());
        }

        private static EnumIRValue ReadEnum(BinaryReader reader)
        {
            var id = FileUtils.ReadGuidNoAlloc(reader);
            var enumVal = reader.ReadInt64();
            return new EnumIRValue()
            {
                TypeId = id,
                EnumInternalType = null,
                EnumValue = enumVal
            };
        }

        private static CollectionData ReadComplexCollection(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            if (count == 0)
            {
                return null;
            }
            var collectionType = (CollectionType)reader.ReadInt32();
            switch (collectionType)
            {
                case CollectionType.None:
                    break;
                case CollectionType.Array:
                case CollectionType.List:
                case CollectionType.Stack:
                case CollectionType.Queue:
                case CollectionType.HashSet:
                    {
                        var collectionData = new CollectionClasses(count, collectionType);
                        collectionData.ItemsType = (SerializedType)reader.ReadUInt64();
                        for (int i = 0; i < count; i++)
                        {
                            collectionData.Value[i] = ReadComplexClass(reader);
                        }
                        return collectionData;
                    }
                case CollectionType.Dictionary:
                    {
                        var result = new DictionaryClass(count, collectionType);
                        result.KeyType = (SerializedType)reader.ReadUInt64();
                        result.ValueType = (SerializedType)reader.ReadUInt64();

                        for (int i = 0; i < result.Count; i++)
                        {
                            result.Keys[i] = ReadComplexClass(reader);
                            result.Values[i] = ReadComplexClass(reader);
                        }
                        return result;
                    }
                default:
                    throw new NotImplementedException($"Collection type '{collectionType}' is not implemented.");
            }

            return null;
        }

        private static CollectionData ReadReferenceCollection(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            if (count == 0)
            {
                return null;
            }

            var collectionType = (CollectionType)reader.ReadInt32();
            switch (collectionType)
            {
                case CollectionType.None:
                    break;
                case CollectionType.Array:
                case CollectionType.List:
                case CollectionType.Stack:
                case CollectionType.Queue:
                case CollectionType.HashSet:
                    {
                        var result = new CollectionReferences(count, collectionType);
                        for (int i = 0; i < result.Count; i++)
                        {
                            result.ItemsType[i] = (SerializedType)reader.ReadUInt64();
                        }
                        for (int i = 0; i < result.Count; i++)
                        {
                            result.Value[i] = ReadReferenceProperty(reader);
                        }
                        return result;
                    }
                case CollectionType.Dictionary:
                    {
                        var result = new DictionaryIRReferences(count, collectionType);

                        for (int i = 0; i < result.Count; i++)
                        {
                            result.KeyType[i] = (SerializedType)reader.ReadUInt64();
                            result.ValueType[i] = (SerializedType)reader.ReadUInt64();
                        }
                        for (int i = 0; i < result.Count; i++)
                        {
                            object ReadArg(SerializedType type)
                            {
                                if (type.IsSimple())
                                {
                                    return ReadSimpleProperty(reader, type);
                                }

                                return ReadReferenceProperty(reader);
                            }

                            result.Keys[i] = ReadArg(result.KeyType[i]);
                            result.Values[i] = ReadArg(result.ValueType[i]);
                        }
                        return result;
                    }
                default:
                    throw new NotImplementedException($"Deserializer: Collection type '{collectionType}' is not implemented.");
            }

            return null;
        }

        private static ReferenceData ReadReferenceProperty(BinaryReader reader)
        {
            return _referenceReader.ReadReference(reader);
        }

        private static ClassData ReadComplexClass(BinaryReader reader)
        {
            /*
              SerializedType ComplexType 
              Guid TypeId 
              List<SerializedPropertyIR> Properties 
           */
            var complexTypeData = new ComplexClass();

            complexTypeData.ClassType = (SerializedType)reader.ReadUInt64();

            if (complexTypeData.ClassType == SerializedType.None)
            {
                return complexTypeData;
            }

            complexTypeData.TypeId = FileUtils.ReadGuidNoAlloc(reader);
            complexTypeData.Properties = Deserialize(reader);

            return complexTypeData;
        }

        private static Variant ReadSimpleProperty(BinaryReader reader, SerializedType simpleType)
        {
            switch (simpleType)
            {
                case SerializedType.None:
                    return default;
                case SerializedType.Char:
                    return reader.ReadChar();
                case SerializedType.String:
                    return ReadString(reader);
                case SerializedType.Bool:
                    return ReadBool(reader);
                case SerializedType.Byte:
                    return reader.ReadByte();
                case SerializedType.Short:
                    return reader.ReadInt16();
                case SerializedType.UShort:
                    return reader.ReadUInt16();
                case SerializedType.Enum:
                    return Variant.FromEnum(ReadEnum(reader));
                case SerializedType.Int:
                    return reader.ReadInt32();
                case SerializedType.UInt:
                    return reader.ReadUInt32();
                case SerializedType.Float:
                    return reader.ReadSingle();
                case SerializedType.Double:
                    return reader.ReadDouble();
                case SerializedType.Long:
                    return reader.ReadInt64();
                case SerializedType.ULong:
                    return reader.ReadUInt64();
                case SerializedType.Vec2:
                    return ReadStructNoAlloc<vec2>(reader);
                case SerializedType.Vec3:
                    return ReadStructNoAlloc<vec3>(reader);
                case SerializedType.Vec4:
                    return ReadStructNoAlloc<vec4>(reader);
                case SerializedType.IVec2:
                    return ReadStructNoAlloc<ivec2>(reader);
                case SerializedType.IVec3:
                    return ReadStructNoAlloc<ivec3>(reader);
                case SerializedType.IVec4:
                    return ReadStructNoAlloc<ivec4>(reader);
                case SerializedType.Quat:
                    return ReadStructNoAlloc<quat>(reader);
                case SerializedType.Mat2:
                    return ReadStructNoAlloc<mat2>(reader);
                case SerializedType.Mat3:
                    return ReadStructNoAlloc<mat3>(reader);
                case SerializedType.Mat4:
                    return ReadStructNoAlloc<mat4>(reader);
                case SerializedType.Color:
                    return (Color)reader.ReadUInt32();
                case SerializedType.Color32:
                    return (Color32)(ColorPacketRGBA)reader.ReadUInt32();
                default:
                    throw new NotImplementedException($"Reader not implemented for simple type: '{simpleType}'");
            }
        }

        public static unsafe T ReadStructNoAlloc<T>(BinaryReader reader) where T : unmanaged
        {
            Span<byte> buff = stackalloc byte[sizeof(T)];
            reader.BaseStream.ReadExactly(buff);
            return MemoryMarshal.Read<T>(buff);
        }
    }
}
