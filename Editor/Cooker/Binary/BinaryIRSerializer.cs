using Engine;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Editor.Utils;
using Engine.Serialization;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Editor.Cooker
{
    internal class BinaryIRSerializer
    {
        private readonly static ReferenceBinaryWriterFactory _referenceWriter = new();
        // TODO: this is temporal
        static BinaryIRSerializer()
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new PlatformNotSupportedException("The serializer requires a little endian CPU.");
            }
        }
        private static void Serialize(BinaryWriter writer, params SerializedPropertyIR[] properties)
        {
            writer.Write(properties.Length);
            foreach (var property in properties)
            {
                WriteProperty(writer, property);
            }
        }

        internal static void Serialize(BinaryWriter writer, MaterialIR material)
        {
            writer.Write(material.Version);
            Serialize(writer, material.Properties);
        }

        internal static void Serialize(SceneIR scene, BinaryWriter writer)
        {
            writer.Write(scene.SceneVersion);
            writer.Write(scene.ActorsVersion);
            writer.Write(scene.ComponentsVersion);

            writer.Write(scene.Actors.Count);
            for (int i = 0; i < scene.Actors.Count; i++)
            {
                WriteActorIR(writer, scene.Actors[i]);
            }
        }

        internal static void Serialize(ShaderIR shader, BinaryWriter writer)
        {
            writer.Write(shader.Version);
            Serialize(writer, shader.Properties);
        }

        private static void WriteActorIR(BinaryWriter writer, ActorIR ir)
        {
            /*
                 string Name 
                 int Layer 
                 bool IsActiveSelf 
                 Guid ID 
                 Guid ParentID 
                 List<ComponentIR> Components 
            */
            WriteString(writer, ir.Name);
            writer.Write(ir.Layer);
            WriteBool(writer, ir.IsActiveSelf);
            EditorUtils.WriteGuidNoAlloc(writer, ir.ID);
            EditorUtils.WriteGuidNoAlloc(writer, ir.ParentID);
            writer.Write(ir.Components.Count);

            for (int i = 0; i < ir.Components.Count; i++)
            {
                WriteComponentIR(writer, ir.Components[i]);
            }
        }

        private static void WriteComponentIR(BinaryWriter writer, ComponentIR ir)
        {
            /*
                Guid TypeId 
                bool IsEnabled 
                Guid ID 
                List<SerializedPropertyIR> SerializedProperties 
             */
            EditorUtils.WriteGuidNoAlloc(writer, ir.TypeId);
            WriteBool(writer, ir.IsEnabled);
            EditorUtils.WriteGuidNoAlloc(writer, ir.ID);
            writer.Write(ir.Properties.Length);

            RegisterType(ir.InternalType);

            for (int i = 0; i < ir.Properties.Length; i++)
            {
                WriteProperty(writer, ir.Properties[i]);
            }
        }

        private static void WriteProperty(BinaryWriter writer, SerializedPropertyIR ir)
        {
            /*
             string Name 
             Guid TypeId 
             SerializedType Type 
             object Data 
             */
            var serializedType = ir.Type;
            WriteString(writer, ir.Name);
            EditorUtils.WriteGuidNoAlloc(writer, ir.TypeId);
            writer.Write((ulong)(serializedType));

            RegisterType(ir.InternalType);

            if (serializedType.IsSimple())
            {
                WriteSimpleProperty(writer, ir.Simple, serializedType);
            }
            else if (serializedType.IsEObject())
            {
                WriteReferenceProperty(writer, ir.Reference);
            }
            else if (serializedType == SerializedType.ReferenceCollection)
            {
                WriteReferenceCollection(writer, ir.Collection);
            }
            else if (serializedType.IsClass())
            {
                if (serializedType == SerializedType.ComplexClass)
                {
                    WriteComplexClass(writer, ir.Class as ComplexClass);
                }
                else
                {
                    throw new NotImplementedException($"Class type '{serializedType}' is not implemented.");
                }
            }
            else if (serializedType == SerializedType.ComplexCollection)
            {
                WriteComplexCollection(writer, ir.Collection);
            }
            else if (serializedType == SerializedType.SimpleCollection)
            {
                WriteSimpleCollection(writer, ir.Collection);
            }
            else
            {
                throw new NotImplementedException($"Can't serialize type, not implemented: '{serializedType}'");
            }
        }


        private static void WriteSimpleCollection(BinaryWriter writer, CollectionData collectionData)
        {
            var count = collectionData?.Count ?? 0;

            writer.Write(count);

            if (count == 0)
            {
                return;
            }
            writer.Write((int)(collectionData.CollectionType));
            if (collectionData.CollectionType == CollectionType.Dictionary)
            {
                var simpleDictionary = collectionData as DictionarySimple;
                writer.Write((ulong)(simpleDictionary.KeyType));
                writer.Write((ulong)(simpleDictionary.ValueType));
                WriteVariantArray(writer, simpleDictionary.KeyType, simpleDictionary.Keys);
                WriteVariantArray(writer, simpleDictionary.ValueType, simpleDictionary.Values);
            }
            else
            {
                var variantCollection = collectionData;
                var itemsType = (variantCollection as IItemType<SerializedType>).ItemsType;
                writer.Write((ulong)itemsType);
                WriteSimpleArray(writer, itemsType, variantCollection);
            }
        }

        private static void WriteSimpleArray(BinaryWriter writer, SerializedType itemsType, CollectionData array)
        {
            switch (itemsType)
            {
                case SerializedType.None:
                    break;
                case SerializedType.Enum:
                    {
                        foreach (var v in (array as CollectionDataEnum).Value)
                        {
                            WriteEnum(writer, in v);
                        }
                    }
                    break;
                case SerializedType.Char:
                    WriteSpan(writer, (array as CollectionDataChar).Value);
                    break;
                case SerializedType.String:
                    {
                        foreach (var v in (array as CollectionDataString).Value)
                        {
                            WriteString(writer, v);
                        }
                    }
                    break;
                case SerializedType.Bool:
                    WriteSpan(writer, (array as CollectionDataBool).Value);
                    break;
                case SerializedType.Byte:
                    WriteSpan(writer, (array as CollectionDataByte).Value);
                    break;
                case SerializedType.Short:
                    WriteSpan(writer, (array as CollectionDataShort).Value);
                    break;
                case SerializedType.UShort:
                    WriteSpan(writer, (array as CollectionDataUShort).Value);
                    break;
                case SerializedType.Int:
                    WriteSpan(writer, (array as CollectionDataInt).Value);
                    break;
                case SerializedType.UInt:
                    WriteSpan(writer, (array as CollectionDataUInt).Value);
                    break;
                case SerializedType.Float:
                    WriteSpan(writer, (array as CollectionDataFloat).Value);
                    break;
                case SerializedType.Double:
                    WriteSpan(writer, (array as CollectionDataDouble).Value);
                    break;
                case SerializedType.Long:
                    WriteSpan(writer, (array as CollectionDataLong).Value);
                    break;
                case SerializedType.ULong:
                    WriteSpan(writer, (array as CollectionDataULong).Value);
                    break;
                case SerializedType.Vec2:
                    WriteSpan(writer, (array as CollectionDataVec2).Value);
                    break;
                case SerializedType.Vec3:
                    WriteSpan(writer, (array as CollectionDataVec3).Value);
                    break;
                case SerializedType.Vec4:
                    WriteSpan(writer, (array as CollectionDataVec4).Value);
                    break;
                case SerializedType.IVec2:
                    WriteSpan(writer, (array as CollectionDataIvec2).Value);
                    break;
                case SerializedType.IVec3:
                    WriteSpan(writer, (array as CollectionDataIvec3).Value);
                    break;
                case SerializedType.IVec4:
                    WriteSpan(writer, (array as CollectionDataIvec4).Value);
                    break;
                case SerializedType.Quat:
                    WriteSpan(writer, (array as CollectionDataQuat).Value);
                    break;
                case SerializedType.Mat2:
                    WriteSpan(writer, (array as CollectionDataMat2).Value);
                    break;
                case SerializedType.Mat3:
                    WriteSpan(writer, (array as CollectionDataMat3).Value);
                    break;
                case SerializedType.Mat4:
                    WriteSpan(writer, (array as CollectionDataMat4).Value);
                    break;
                case SerializedType.Color:
                    WriteSpan(writer, (array as CollectionDataColor).Value);
                    break;
                case SerializedType.Color32:
                    WriteSpan(writer, (array as CollectionDataColor32).Value);
                    break;
                default:
                    break;
            }
        }
        private static void WriteVariantArray(BinaryWriter writer, SerializedType kind, Variant[] variants)
        {
            if (variants == null || variants.Length == 0 || kind == SerializedType.None)
                return;

            if (kind == SerializedType.String)
            {
                foreach (var v in variants)
                {
                    WriteString(writer, v.String);
                }
                return;
            }

            if (kind == SerializedType.Enum)
            {
                foreach (var v in variants)
                {
                    WriteEnum(writer, v.Enum);
                }
                return;
            }

            switch (kind)
            {
                case SerializedType.Char:
                    WritePayloadSpan<char>(writer, variants);
                    break;
                case SerializedType.Bool:
                    WriteBoolPayloadSpan(writer, variants);
                    break;
                case SerializedType.Byte:
                    WritePayloadSpan<byte>(writer, variants);
                    break;
                case SerializedType.Short:
                    WritePayloadSpan<short>(writer, variants);
                    break;
                case SerializedType.UShort:
                    WritePayloadSpan<ushort>(writer, variants);
                    break;
                case SerializedType.Int:
                    WritePayloadSpan<int>(writer, variants);
                    break;
                case SerializedType.UInt:
                    WritePayloadSpan<uint>(writer, variants);
                    break;
                case SerializedType.Long:
                    WritePayloadSpan<long>(writer, variants);
                    break;
                case SerializedType.ULong:
                    WritePayloadSpan<ulong>(writer, variants);
                    break;
                case SerializedType.Float:
                    WritePayloadSpan<float>(writer, variants);
                    break;
                case SerializedType.Double:
                    WritePayloadSpan<double>(writer, variants);
                    break;
                case SerializedType.Vec2:
                    WritePayloadSpan<vec2>(writer, variants);
                    break;
                case SerializedType.Vec3:
                    WritePayloadSpan<vec3>(writer, variants);
                    break;
                case SerializedType.Vec4:
                    WritePayloadSpan<vec4>(writer, variants);
                    break;
                case SerializedType.IVec2:
                    WritePayloadSpan<ivec2>(writer, variants);
                    break;
                case SerializedType.IVec3:
                    WritePayloadSpan<ivec3>(writer, variants);
                    break;
                case SerializedType.IVec4:
                    WritePayloadSpan<ivec4>(writer, variants);
                    break;
                case SerializedType.Quat:
                    WritePayloadSpan<quat>(writer, variants);
                    break;
                case SerializedType.Mat2:
                    WritePayloadSpan<mat2>(writer, variants);
                    break;
                case SerializedType.Mat3:
                    WritePayloadSpan<mat3>(writer, variants);
                    break;
                case SerializedType.Mat4:
                    WritePayloadSpan<mat4>(writer, variants);
                    break;
                case SerializedType.Color:
                    WritePayloadSpan<Color>(writer, variants);
                    break;
                case SerializedType.Color32:
                    WritePayloadSpan<Color32>(writer, variants);
                    break;
                default:
                    throw new Exception($"Unsupported SerializedType: {kind}");
            }
        }

        private static void WriteComplexClass(BinaryWriter writer, ComplexClass data)
        {
            /*
               SerializedType ComplexType 
               Guid TypeId 
               List<SerializedPropertyIR> Properties 
            */
            if (data == null)
            {
                writer.Write((ulong)(SerializedType.None));
                return;
            }
            RegisterType(data.InternalType);

            writer.Write((ulong)(data.ClassType));
            EditorUtils.WriteGuidNoAlloc(writer, data.TypeId);
            Serialize(writer, data.Properties);
        }

        private static void WriteComplexCollection(BinaryWriter writer, CollectionData collectionData)
        {
            var count = collectionData?.Count ?? 0;

            writer.Write(count);

            if (count == 0)
            {
                return;
            }

            writer.Write((int)(collectionData.CollectionType));
            switch (collectionData.CollectionType)
            {
                case CollectionType.None:
                    break;
                case CollectionType.Array:
                case CollectionType.List:
                case CollectionType.Stack:
                case CollectionType.Queue:
                case CollectionType.HashSet:
                    {
                        var col = collectionData as CollectionClasses;
                        writer.Write((ulong)col.ItemsType);

                        foreach (var item in col.Value)
                        {
                            WriteComplexClass(writer, item as ComplexClass);
                        }
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        var dictComplexClass = collectionData as DictionaryClass;
                        writer.Write((ulong)dictComplexClass.KeyType);
                        writer.Write((ulong)dictComplexClass.ValueType);

                        for (var i = 0; i < dictComplexClass.Count; i++)
                        {
                            WriteComplexClass(writer, dictComplexClass.Keys[i] as ComplexClass);
                            WriteComplexClass(writer, dictComplexClass.Values[i] as ComplexClass);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"Collection type '{collectionData.CollectionType}' is not implemented.");
            }
        }
        private static void WriteReferenceCollection(BinaryWriter writer, CollectionData data)
        {
            var count = data?.Count ?? 0;

            writer.Write(count);

            if (count == 0)
            {
                return;
            }

            writer.Write((int)(data.CollectionType));
            switch (data.CollectionType)
            {
                case CollectionType.None:
                    break;
                case CollectionType.Array:
                case CollectionType.List:
                case CollectionType.Stack:
                case CollectionType.Queue:
                case CollectionType.HashSet:
                    {
                        var col = data as CollectionReferences;
                        for (int i = 0; i < col.ItemsType.Length; i++)
                        {
                            writer.Write((ulong)col.ItemsType[i]);
                        }
                        foreach (var item in col.Value)
                        {
                            if (item == null)
                            {
                                writer.Write((ulong)(SerializedType.None));
                                continue;
                            }
                            writer.Write((ulong)(item.Type));
                            EditorUtils.WriteGuidNoAlloc(writer, item.RefId);
                        }
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        var referenceDictionary = data as DictionaryIRReferences;
                        for (int i = 0; i < referenceDictionary.Count; i++)
                        {
                            writer.Write((ulong)(referenceDictionary.KeyType[i]));
                            writer.Write((ulong)(referenceDictionary.ValueType[i]));
                        }

                        for (var i = 0; i < referenceDictionary.Count; i++)
                        {
                            void WriteArg(SerializedType type, object argData)
                            {
                                if (type.IsSimple())
                                {
                                    WriteSimpleProperty(writer, _ObjectToVariantSafe(type, argData), type);
                                }
                                else
                                {
                                    WriteReferenceProperty(writer, argData as ReferenceData);
                                }
                            }

                            WriteArg(referenceDictionary.KeyType[i], referenceDictionary.Keys[i]);
                            WriteArg(referenceDictionary.ValueType[i], referenceDictionary.Values[i]);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"Collection type '{data.CollectionType}' is not implemented.");
            }
        }

        private static Variant _ObjectToVariantSafe(SerializedType type, object obj)
        {
            if (obj == null)
            {
                if (type == SerializedType.String)
                {
                    return string.Empty;
                }

                return default;
            }

            return (Variant)obj;
        }

        private static byte BoolToByte(bool value)
        {
            return value ? (byte)1 : (byte)0;
        }
        private static void WriteBool(BinaryWriter writer, bool value)
        {
            writer.Write(BoolToByte(value));
        }
        private static void WriteSimpleProperty(BinaryWriter writer, in Variant data, SerializedType simpleType)
        {
            switch (simpleType)
            {
                case SerializedType.None:
                    break;
                case SerializedType.Char:
                    writer.Write(data.value.Char);
                    break;
                case SerializedType.String:
                    WriteString(writer, data.String);
                    break;
                case SerializedType.Bool:
                    WriteBool(writer, data.value.Bool);
                    break;
                case SerializedType.Byte:
                    writer.Write(data.value.Byte);
                    break;
                case SerializedType.Short:
                    writer.Write(data.value.Short);
                    break;
                case SerializedType.UShort:
                    writer.Write(data.value.UShort);
                    break;
                case SerializedType.Enum:
                    WriteEnum(writer, data.Enum);
                    break;
                case SerializedType.Int:
                    writer.Write(data.value.Int);
                    break;
                case SerializedType.UInt:
                    writer.Write(data.value.Uint);
                    break;
                case SerializedType.Float:
                    writer.Write(data.value.Float);
                    break;
                case SerializedType.Double:
                    writer.Write(data.value.Double);
                    break;
                case SerializedType.Long:
                    writer.Write(data.value.Long);
                    break;
                case SerializedType.ULong:
                    writer.Write(data.value.Ulong);
                    break;
                case SerializedType.Vec2:
                    WriteStruct(writer, data.value.Vec2);
                    break;
                case SerializedType.Vec3:
                    WriteStruct(writer, data.value.Vec3);
                    break;
                case SerializedType.Vec4:
                    WriteStruct(writer, data.value.Vec4);
                    break;
                case SerializedType.IVec2:
                    WriteStruct(writer, data.value.Ivec2);
                    break;
                case SerializedType.IVec3:
                    WriteStruct(writer, data.value.Ivec3);
                    break;
                case SerializedType.IVec4:
                    WriteStruct(writer, data.value.Ivec4);
                    break;
                case SerializedType.Quat:
                    WriteStruct(writer, data.value.Quat);
                    break;
                case SerializedType.Mat2:
                    WriteStruct(writer, data.value.Mat2);
                    break;
                case SerializedType.Mat3:
                    WriteStruct(writer, data.value.Mat3);
                    break;
                case SerializedType.Mat4:
                    WriteStruct(writer, data.value.Mat4);
                    break;
                case SerializedType.Color:
                    writer.Write((uint)data.value.Color);
                    break;
                case SerializedType.Color32:
                    writer.Write(((ColorPacketRGBA)data.value.Color32).Value);
                    break;
                default:
                    throw new NotImplementedException($"Writer not implemented for simple type: '{simpleType}'");
            }
        }

        private static void WriteReferenceProperty(BinaryWriter writer, ReferenceData value)
        {
            _referenceWriter.WriteReference(writer, value);
        }

        private static void WriteEnum(BinaryWriter writer, in EnumIRValue value)
        {
            if (!string.IsNullOrEmpty(value.EnumInternalType))
            {
                EditorUtils.WriteGuidNoAlloc(writer, value.TypeId);
                writer.Write(value.EnumValue);
                RegisterType(value.EnumInternalType);
            }
            else
            {
                EditorUtils.WriteGuidNoAlloc(writer, Guid.Empty);
                writer.Write((long)0);
            }
        }

        private static unsafe void WriteStruct<T>(BinaryWriter writer, T value) where T : unmanaged
        {
            Span<byte> span = new Span<byte>(&value, sizeof(T));
            writer.BaseStream.Write(span);
        }

        private static void WritePayloadSpan<T>(BinaryWriter writer, Variant[] variants) where T : unmanaged
        {
            var count = variants.Length;
            var size = Unsafe.SizeOf<T>();

            var buffer = ArrayPool<byte>.Shared.Rent(count * size);
            try
            {
                Span<byte> dst = buffer.AsSpan(0, count * size);

                for (int i = 0; i < count; i++)
                {
                    ref Variant.Value payload = ref variants[i].value;
                    T value = Unsafe.As<Variant.Value, T>(ref payload);
                    Unsafe.WriteUnaligned(ref dst[i * size], value);
                }

                writer.Write(dst);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private static void WriteSpan<T>(BinaryWriter writer, T[] values) where T : unmanaged
        {
            ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(values.AsSpan());
            writer.Write(bytes);
        }

        private static void WriteBoolPayloadSpan(BinaryWriter writer, Variant[] variants)
        {
            var count = variants.Length;
            var buffer = ArrayPool<byte>.Shared.Rent(count);
            try
            {
                var dst = buffer.AsSpan(0, count);
                for (int i = 0; i < count; i++)
                {
                    dst[i] = BoolToByte(variants[i].value.Bool);
                }
                writer.Write(dst);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        private static void WriteBoolSpan(BinaryWriter writer, bool[] values)
        {
            var count = values.Length;
            var buffer = ArrayPool<byte>.Shared.Rent(count);
            try
            {
                var dst = buffer.AsSpan(0, count);
                for (int i = 0; i < count; i++)
                {
                    dst[i] = BoolToByte(values[i]);
                }
                writer.Write(dst);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        private static void WriteString(BinaryWriter writer, string str, int chunkSize = 8192)
        {
            if (string.IsNullOrEmpty(str))
            {
                writer.Write(0);
                return;
            }

            int totalBytes = Encoding.UTF8.GetByteCount(str);
            writer.Write(totalBytes);

            if (totalBytes <= 1024) // a kb
            {
                Span<byte> buff = stackalloc byte[totalBytes];
                Encoding.UTF8.GetBytes(str, buff);
                writer.BaseStream.Write(buff);
            }
            else
            {
                var bufferSize = Encoding.UTF8.GetMaxByteCount(chunkSize);
                var byteBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    int offset = 0;
                    var sourceSpan = str.AsSpan();

                    while (offset < str.Length)
                    {
                        int charsToProcess = Math.Min(chunkSize, str.Length - offset);
                        int bytesWritten = Encoding.UTF8.GetBytes(sourceSpan.Slice(offset, charsToProcess), byteBuffer);
                        writer.BaseStream.Write(byteBuffer.AsSpan(0, bytesWritten));
                        offset += charsToProcess;
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(byteBuffer);
                }
            }
        }

        private static void RegisterType(string internalType)
        {
            if (ReflectionUtils.ResolveType(internalType, out var type))
            {
                if (TypeRegistryClassGenerator.AddType(type))
                {
                    Debug.Info("Registers type: " + internalType);
                }
            }

        }
    }
}