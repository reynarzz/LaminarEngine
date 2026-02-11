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
        // TODO: this is temporal
        static BinaryIRSerializer()
        {
            if (!BitConverter.IsLittleEndian)
            {
                throw new PlatformNotSupportedException("The serializer requires a little endian CPU.");
            }
        }
        internal static void Serialize(BinaryWriter writer, params List<SerializedPropertyIR> properties)
        {
            writer.Write(properties.Count);
            foreach (var property in properties)
            {
                WriteProperty(writer, property);
            }
        }

        internal static void Serialize(SceneIR scene, BinaryWriter writer)
        {
            writer.Write(scene.Version);
            writer.Write(scene.Actors.Count);
            for (int i = 0; i < scene.Actors.Count; i++)
            {
                WriteActorIR(writer, scene.Actors[i]);
            }
        }

        internal static void Serialize(ShaderIR shader, BinaryWriter writer)
        {
            writer.Write(ShaderIR.Version);
            Serialize(writer, shader.SourcesCollection);
        }

        private static void WriteActorIR(BinaryWriter writer, ActorIR ir)
        {
            /*
                 int Version 
                 string Name 
                 int Layer 
                 bool IsActiveSelf 
                 Guid ID 
                 Guid ParentID 
                 List<ComponentIR> Components 
            */
            writer.Write(ir.Version);
            WriteString(writer, ir.Name);
            writer.Write(ir.Layer);
            WriteBool(writer, ir.IsActiveSelf);
            WriteGUID(writer, ir.ID);
            WriteGUID(writer, ir.ParentID);
            writer.Write(ir.Components.Count);

            for (int i = 0; i < ir.Components.Count; i++)
            {
                WriteComponentIR(writer, ir.Components[i]);
            }
        }

        private static void WriteComponentIR(BinaryWriter writer, ComponentIR ir)
        {
            /*
                int Version 
                Guid TypeId 
                bool IsEnabled 
                Guid ID 
                List<SerializedPropertyIR> SerializedProperties 
             */
            writer.Write(ir.Version);
            WriteGUID(writer, ir.TypeId);
            WriteBool(writer, ir.IsEnabled);
            WriteGUID(writer, ir.ID);
            writer.Write(ir.SerializedProperties.Count);

            for (int i = 0; i < ir.SerializedProperties.Count; i++)
            {
                WriteProperty(writer, ir.SerializedProperties[i]);
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
            WriteGUID(writer, ir.TypeId);
            writer.Write((long)(serializedType));

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
            else if (serializedType == SerializedType.ComplexClass)
            {
                WriteComplexClass(writer, ir.Complex);
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

        private static void WriteGUID(BinaryWriter writer, in Guid id)
        {
            const int GUID_SIZE = 16;
            Span<byte> bytes = stackalloc byte[GUID_SIZE];
            if (id.TryWriteBytes(bytes))
            {
                writer.Write(bytes);
            }
            else
            {
                writer.Write(id.ToByteArray());
                Debug.Warn("Default guid writer");
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
            writer.Write((long)(collectionData.CollectionType));
            if (collectionData.CollectionType == CollectionType.Dictionary)
            {
                var simpleDictionary = collectionData as DictionaryIRVariants;
                writer.Write((long)(simpleDictionary.KeyType));
                writer.Write((long)(simpleDictionary.ValueType));
                WriteVariantArray(writer, simpleDictionary.KeyType, simpleDictionary.Keys);
                WriteVariantArray(writer, simpleDictionary.ValueType, simpleDictionary.Values);
            }
            else
            {
                var variantCollection = collectionData as CollectionIRVariants;
                writer.Write((long)(variantCollection.ItemsType));
                WriteVariantArray(writer, variantCollection.ItemsType, variantCollection.Value);
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

        private static void WriteComplexClass(BinaryWriter writer, ComplexData data)
        {
            /*
               SerializedType ComplexType 
               Guid TypeId 
               List<SerializedPropertyIR> Properties 
            */
            if (data == null)
            {
                writer.Write((long)(SerializedType.None));
                return;
            }
            writer.Write((long)(data.ComplexType));
            WriteGUID(writer, data.TypeId);
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

            writer.Write((long)(collectionData.CollectionType));
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
                        var col = collectionData as CollectionIRComplexTypes;
                        foreach (var item in col.Value)
                        {
                            WriteComplexClass(writer, item);
                        }
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        var dictComplexClass = collectionData as DictionaryIRComplexTypes;
                        for (var i = 0; i < dictComplexClass.Count; i++)
                        {
                            WriteComplexClass(writer, dictComplexClass.Keys[i]);
                            WriteComplexClass(writer, dictComplexClass.Values[i]);
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

            writer.Write((long)(data.CollectionType));
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
                        var col = data as CollectionIRReferences;
                        foreach (var item in col.Value)
                        {
                            if (item == null)
                            {
                                writer.Write((long)(SerializedType.None));
                                continue;
                            }
                            writer.Write((long)(item.Type));
                            writer.Write(item.Id.ToByteArray());
                        }
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        var referenceDictionary = data as DictionaryIRReferences;
                        for (int i = 0; i < referenceDictionary.Count; i++)
                        {
                            writer.Write((long)(referenceDictionary.KeyType[i]));
                            writer.Write((long)(referenceDictionary.ValueType[i]));
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
                    return Variant.FromString(string.Empty);
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
            if (value != null)
            {
                writer.Write((long)(value.Type));
                writer.Write(value.Id.ToByteArray());
            }
            else
            {
                writer.Write((long)(SerializedType.None));
            }
        }

        private static void WriteEnum(BinaryWriter writer, in EnumIRValue value)
        {
            if (!string.IsNullOrEmpty(value.EnumInternalType))
            {
                WriteGUID(writer, value.TypeId);
                writer.Write(value.EnumValue);
            }
            else
            {
                WriteGUID(writer, Guid.Empty);
                writer.Write((long)0);
            }
        }

        private static void WriteStruct<T>(BinaryWriter writer, T value) where T : unmanaged
        {
            ReadOnlySpan<T> span = stackalloc T[] { value };
            writer.Write(MemoryMarshal.AsBytes(span));
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

        // Makes sure bool is exactly 1 byte.
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

        private static void WriteString(BinaryWriter writer, string str, int chunkSize = 8192)
        {
            if (string.IsNullOrEmpty(str))
            {
                writer.Write(0);
                return;
            }

            var totalBytes = Encoding.UTF8.GetByteCount(str);
            writer.Write(totalBytes);

            // Use a single rented buffer for everything to avoid heap allocations
            // We rent based on the total bytes (if small) or chunkSize (if large)
            var bufferSize = (str.Length < 1024) ? totalBytes : Encoding.UTF8.GetMaxByteCount(chunkSize);
            var byteBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);

            try
            {
                if (str.Length < 1024)
                {
                    int bytesWritten = Encoding.UTF8.GetBytes(str.AsSpan(), byteBuffer);
                    writer.Write(byteBuffer, 0, bytesWritten);
                }
                else
                {
                    int offset = 0;
                    var sourceSpan = str.AsSpan();
                    while (offset < str.Length)
                    {
                        int charsToProcess = Math.Min(chunkSize, str.Length - offset);
                        int bytesWritten = Encoding.UTF8.GetBytes(sourceSpan.Slice(offset, charsToProcess), byteBuffer);
                        writer.Write(byteBuffer, 0, bytesWritten);
                        offset += charsToProcess;
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(byteBuffer);
            }
        }
    }
}