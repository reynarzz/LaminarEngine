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

namespace Editor.Cooker
{
    internal class BinaryIRSerializer
    {
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
            writer.Write(ir.Name.Length);
            writer.Write(Encoding.UTF8.GetBytes(ir.Name), 0, ir.Name.Length);
            writer.Write(ir.Layer);
            writer.Write(ir.IsActiveSelf);
            writer.Write(ir.ID.ToByteArray());
            writer.Write(ir.ParentID.ToByteArray());
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
            writer.Write(ir.TypeId.ToByteArray());
            writer.Write(ir.IsEnabled);
            writer.Write(ir.ID.ToByteArray());
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
            writer.Write(ir.Name.Length);
            writer.Write(Encoding.UTF8.GetBytes(ir.Name), 0, ir.Name.Length);
            writer.Write(ir.TypeId.ToByteArray());
            writer.Write(Convert.ToInt64(serializedType));

            if (serializedType.IsSimple())
            {
                WriteSimpleProperty(writer, ObjectToVariantSafe(serializedType, ir.Data), serializedType);
            }
            else if (serializedType.IsEObject())
            {
                WriteReferenceProperty(writer, ir.Data as ReferenceData);
            }
            else if (serializedType == SerializedType.ReferenceCollection)
            {
                WriteReferenceCollection(writer, ir.Data as CollectionPropertyData);
            }
            else if (serializedType == SerializedType.ComplexClass)
            {
                WriteComplexClass(writer, ir.Data as ComplexTypeData);
            }
            else if (serializedType == SerializedType.ComplexCollection)
            {
                WriteComplexCollection(writer, ir.Data as CollectionPropertyData);
            }
            else if (serializedType == SerializedType.SimpleCollection)
            {
                WriteSimpleCollection(writer, ir.Data as CollectionPropertyData);
            }
        }

        private static void WriteSimpleCollection(BinaryWriter writer, CollectionPropertyData data)
        {
            var count = data?.Collection?.Count ?? 0;

            writer.Write(count);

            if (count == 0)
            {
                return;
            }
            writer.Write(Convert.ToInt64(data.CollectionType));
            // TODO: improve writing, it should be a byte array, and no one by one.
            if (data.CollectionType == CollectionType.Dictionary)
            {
                foreach (var item in data.Collection)
                {
                    var itemData = item as DictionaryDataSimple;
                    writer.Write(Convert.ToInt64(itemData.KeyType));
                    writer.Write(Convert.ToInt64(itemData.ValueType));
                    WriteSimpleProperty(writer, itemData.Key, itemData.KeyType);
                    WriteSimpleProperty(writer, itemData.Value, itemData.ValueType);
                }
            }
            else
            {
                var variatCollection = data.Collection as VariantIRValue[];
                for (int i = 0; i < variatCollection.Length; i++)
                {
                    ref var value = ref variatCollection[i];
                    WriteSimpleProperty(writer, in value, data.ItemsType);
                }
            }
        }

        private static void WriteComplexClass(BinaryWriter writer, ComplexTypeData data)
        {
            /*
               SerializedType ComplexType 
               Guid TypeId 
               List<SerializedPropertyIR> Properties 
            */
            if (data == null)
            {
                writer.Write(Convert.ToInt64(SerializedType.None));
                return;
            }
            writer.Write(Convert.ToInt64(data.ComplexType));
            writer.Write(data.TypeId.ToByteArray());
            Serialize(writer, data.Properties);
        }

        private static void WriteComplexCollection(BinaryWriter writer, CollectionPropertyData data)
        {
            var count = data?.Collection?.Count ?? 0;

            writer.Write(count);

            if (count == 0)
            {
                return;
            }

            writer.Write(Convert.ToInt64(data.CollectionType));
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
                        foreach (var item in data.Collection)
                        {
                            var itemData = item as CollectionData<ComplexTypeData>;
                            writer.Write(Convert.ToInt64(itemData.Type));
                            WriteComplexClass(writer, itemData.Value);
                        }
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        foreach (var item in data.Collection)
                        {
                            var itemData = item as ComplexDictionaryData;
                            writer.Write(Convert.ToInt64(itemData.Type));
                            WriteComplexClass(writer, itemData.Key);
                            WriteComplexClass(writer, itemData.Value);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"Collection type '{data.CollectionType}' is not implemented.");
            }
        }
        private static void WriteReferenceCollection(BinaryWriter writer, CollectionPropertyData data)
        {
            var count = data?.Collection?.Count ?? 0;

            writer.Write(count);

            if (count == 0)
            {
                return;
            }

            writer.Write(Convert.ToInt64(data.CollectionType));
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
                        foreach (var item in data.Collection)
                        {
                            var itemData = item as CollectionData<ReferenceData>;
                            writer.Write(Convert.ToInt64(itemData.Type));
                            writer.Write(itemData.Value.Id.ToByteArray());
                        }
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        foreach (var item in data.Collection)
                        {
                            var itemData = item as DictionaryData;
                            writer.Write(Convert.ToInt64(itemData.Type));
                            writer.Write(Convert.ToInt64(itemData.KeyType));
                            writer.Write(Convert.ToInt64(itemData.ValueType));

                            void WriteArg(SerializedType type, object argData)
                            {
                                if (type.IsSimple())
                                {
                                    WriteSimpleProperty(writer, ObjectToVariantSafe(type, argData), type);
                                }
                                else
                                {
                                    WriteReferenceProperty(writer, argData as ReferenceData);
                                }
                            }

                            WriteArg(itemData.KeyType, itemData.Key);
                            WriteArg(itemData.ValueType, itemData.Value);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"Collection type '{data.CollectionType}' is not implemented.");
            }
        }

        private static VariantIRValue ObjectToVariantSafe(SerializedType type, object obj)
        {
            if (obj == null)
            {
                if (type == SerializedType.String)
                {
                    return VariantIRValue.FromString(string.Empty);
                }

                return default;
            }

            return (VariantIRValue)obj;
        }

        private static void WriteSimpleProperty(BinaryWriter writer, in VariantIRValue data, SerializedType simpleType)
        {
            switch (simpleType)
            {
                case SerializedType.None:
                    break;
                case SerializedType.Char:
                    writer.Write(data.Payload.Char);
                    break;
                case SerializedType.String:
                    writer.Write(data.String);
                    break;
                case SerializedType.Bool:
                    writer.Write(data.Payload.Bool);
                    break;
                case SerializedType.Byte:
                    writer.Write(data.Payload.Byte);
                    break;
                case SerializedType.Short:
                    writer.Write(data.Payload.Short);
                    break;
                case SerializedType.UShort:
                    writer.Write(data.Payload.UShort);
                    break;
                case SerializedType.Enum:
                    {
                        if (!string.IsNullOrEmpty(data.Enum.EnumInternalType))
                        {
                            writer.Write(data.Enum.TypeId.ToByteArray());
                            writer.Write(data.Enum.EnumValue);
                        }
                        else
                        {
                            writer.Write(Guid.Empty.ToByteArray());
                            writer.Write(0UL);
                        }
                    }
                    break;
                case SerializedType.Int:
                    writer.Write(data.Payload.Int);
                    break;
                case SerializedType.UInt:
                    writer.Write(data.Payload.Uint);
                    break;
                case SerializedType.Float:
                    writer.Write(data.Payload.Float);
                    break;
                case SerializedType.Double:
                    writer.Write(data.Payload.Double);
                    break;
                case SerializedType.Long:
                    writer.Write(data.Payload.Long);
                    break;
                case SerializedType.ULong:
                    writer.Write(data.Payload.Ulong);
                    break;
                case SerializedType.Vec2:
                    WriteStruct(writer, data.Payload.Vec2);
                    break;
                case SerializedType.Vec3:
                    WriteStruct(writer, data.Payload.Vec3);
                    break;
                case SerializedType.Vec4:
                    WriteStruct(writer, data.Payload.Vec4);
                    break;
                case SerializedType.IVec2:
                    WriteStruct(writer, data.Payload.Ivec2);
                    break;
                case SerializedType.IVec3:
                    WriteStruct(writer, data.Payload.Ivec3);
                    break;
                case SerializedType.IVec4:
                    WriteStruct(writer, data.Payload.Ivec4);
                    break;
                case SerializedType.Quat:
                    WriteStruct(writer, data.Payload.Quat);
                    break;
                case SerializedType.Mat2:
                    WriteStruct(writer, data.Payload.Mat2);
                    break;
                case SerializedType.Mat3:
                    WriteStruct(writer, data.Payload.Mat3);
                    break;
                case SerializedType.Mat4:
                    WriteStruct(writer, data.Payload.Mat4);
                    break;
                case SerializedType.Color:
                    writer.Write((uint)data.Payload.Color);
                    break;
                case SerializedType.Color32:
                    writer.Write(((ColorPacketRGBA)data.Payload.Color32).Value);
                    break;
                default:
                    throw new NotImplementedException($"Writer not implemented for simple type: '{simpleType}'");
            }
        }

        private static T GetSimpleValueSafe<T>(object data)
        {
            if (data != null)
            {
                return (T)data;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)string.Empty;
            }

            return default;
        }

        private static void WriteReferenceProperty(BinaryWriter writer, ReferenceData value)
        {
            if (value != null)
            {
                writer.Write(value.Id.ToByteArray());
            }
            else
            {
                writer.Write(Guid.Empty.ToByteArray());
            }
        }

        private static void WriteStruct<T>(BinaryWriter writer, T value) where T : unmanaged
        {
            ReadOnlySpan<T> span = stackalloc T[] { value };
            writer.Write(MemoryMarshal.AsBytes(span));
        }
    }
}
