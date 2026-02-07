using Engine;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class BinaryIRSerializer
    {
        internal static void Serialize(List<SerializedPropertyIR> properties, BinaryWriter writer)
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
            writer.Write(ir.Name);
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
            writer.Write(ir.Name);
            writer.Write(ir.TypeId.ToByteArray());
            writer.Write((int)serializedType);

            if (serializedType.IsSimple())
            {
                WriteSimpleProperty(writer, ir.Data, serializedType);
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
                writer.Write((int)SerializedType.None);
                return;
            }
            writer.Write((int)data.ComplexType);
            writer.Write(data.TypeId.ToByteArray());
            Serialize(data.Properties, writer);
        }

        private static void WriteComplexCollection(BinaryWriter writer, CollectionPropertyData data)
        {
            var count = data?.Collection?.Count ?? 0;

            writer.Write(count);

            if (count == 0)
            {
                return;
            }

            switch (data.CollectionType)
            {
                case CollectionType.None:
                    break;
                case CollectionType.Array:
                case CollectionType.List:
                case CollectionType.Stack:
                case CollectionType.Queue:
                case CollectionType.Hashset:
                    {
                        for (int i = 0; i < data.Collection.Count; i++)
                        {
                            var item = data.Collection[i] as CollectionData<ComplexTypeData>;
                            writer.Write((int)item.Type);
                            WriteComplexClass(writer, item.Value);
                        }
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        for (int i = 0; i < data.Collection.Count; i++)
                        {
                            var item = data.Collection[i] as ComplexDictionaryData;
                            writer.Write((int)item.Type);
                            WriteComplexClass(writer, item.Key);
                            WriteComplexClass(writer, item.Value);
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

            switch (data.CollectionType)
            {
                case CollectionType.None:
                    break;
                case CollectionType.Array:
                case CollectionType.List:
                case CollectionType.Stack:
                case CollectionType.Queue:
                case CollectionType.Hashset:
                    {
                        for (int i = 0; i < data.Collection.Count; i++)
                        {
                            var item = data.Collection[i] as CollectionData<ReferenceData>;
                            writer.Write((int)item.Type);
                            writer.Write(item.Value.Id.ToByteArray());
                        }
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        for (int i = 0; i < data.Collection.Count; i++)
                        {
                            var item = data.Collection[i] as DictionaryData;
                            writer.Write((int)item.Type);
                            writer.Write((int)item.KeyType);
                            writer.Write((int)item.ValueType);

                            void WriteArg(SerializedType type, object argData)
                            {
                                if (type.IsSimple())
                                {
                                    WriteSimpleProperty(writer, argData, type);
                                }
                                else
                                {
                                    WriteReferenceProperty(writer, argData as ReferenceData);
                                }
                            }

                            WriteArg(item.KeyType, item.Key);
                            WriteArg(item.ValueType, item.Value);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"Collection type '{data.CollectionType}' is not implemented.");
            }
        }

        private static void WriteSimpleProperty(BinaryWriter writer, object data, SerializedType simpleType)
        {
            writer.Write((int)simpleType);
            switch (simpleType)
            {
                case SerializedType.None:
                    break;
                case SerializedType.Char:
                    writer.Write((char)data);
                    break;
                case SerializedType.String:
                    writer.Write((string)data);
                    break;
                case SerializedType.Bool:
                    writer.Write((bool)data);
                    break;
                case SerializedType.Byte:
                    writer.Write((byte)data);
                    break;
                case SerializedType.Short:
                    writer.Write((short)data);
                    break;
                case SerializedType.UShort:
                    writer.Write((ushort)data);
                    break;
                case SerializedType.Enum:
                    {
                        writer.Write(ReflectionUtils.GetStableGuid(data.GetType()).ToByteArray());
                        writer.Write((int)data);
                    }
                    break;
                case SerializedType.Int:
                    writer.Write((int)data);
                    break;
                case SerializedType.Uint:
                    writer.Write((uint)data);
                    break;
                case SerializedType.Float:
                    writer.Write((float)data);
                    break;
                case SerializedType.Double:
                    writer.Write((double)data);
                    break;
                case SerializedType.Long:
                    writer.Write((long)data);
                    break;
                case SerializedType.Ulong:
                    writer.Write((ulong)data);
                    break;
                case SerializedType.Vec2:
                    WriteVec2(writer, (vec2)data);
                    break;
                case SerializedType.Vec3:
                    WriteVec3(writer, (vec3)data);
                    break;
                case SerializedType.Vec4:
                    WriteVec4(writer, (vec4)data);
                    break;
                case SerializedType.Ivec2:
                    WriteIVec2(writer, (ivec2)data);
                    break;
                case SerializedType.Ivec3:
                    WriteIVec3(writer, (ivec3)data);
                    break;
                case SerializedType.Ivec4:
                    WriteIVec4(writer, (ivec4)data);
                    break;
                case SerializedType.Quat:
                    WriteQuat(writer, (quat)data);
                    break;
                case SerializedType.Mat2:
                    {
                        var value = (mat2)data;
                        WriteVec2(writer, value.c0);
                        WriteVec2(writer, value.c1);
                    }
                    break;
                case SerializedType.Mat3:
                    {
                        var value = (mat3)data;
                        WriteVec3(writer, value.c0);
                        WriteVec3(writer, value.c1);
                        WriteVec3(writer, value.c2);
                    }
                    break;
                case SerializedType.Mat4:
                    {
                        var value = (mat4)data;
                        WriteVec4(writer, value.c0);
                        WriteVec4(writer, value.c1);
                        WriteVec4(writer, value.c2);
                        WriteVec4(writer, value.c3);
                    }
                    break;
                case SerializedType.Color:
                    {
                        WriteVec4(writer, (Color)data);
                    }
                    break;
                case SerializedType.Color32:
                    {
                        WriteIVec4(writer, (Color32)data);
                    }
                    break;
                default:
                    throw new NotImplementedException($"Writer not implemented for simple type: '{simpleType}'");
            }
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

        private static void WriteVec2(BinaryWriter writer, vec2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }
        private static void WriteVec3(BinaryWriter writer, vec3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }
        private static void WriteVec4(BinaryWriter writer, vec4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }
        private static void WriteQuat(BinaryWriter writer, quat value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }
        private static void WriteIVec2(BinaryWriter writer, ivec2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }
        private static void WriteIVec3(BinaryWriter writer, ivec3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }
        private static void WriteIVec4(BinaryWriter writer, ivec4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }
       
        private static Guid GetTypeId()
        {
            return Guid.NewGuid();
        }
    }
}
