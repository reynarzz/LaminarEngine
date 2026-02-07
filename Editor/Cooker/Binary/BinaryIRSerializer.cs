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
                string InternalType 
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
             SerializedType Type 
             string InternalType 
             Guid TypeId 
             object Data 
             */

            writer.Write(ir.Name);
            writer.Write((int)ir.Type);
            writer.Write(ir.TypeId.ToByteArray());

            if (ir.Type == SerializedType.Simple)
            {
                WriteSimpleProperty(writer, ir.Data);
            }
            else if (ir.Type.HasFlag(SerializedType.EObject))
            {
                WriteReferenceProperty(writer, ir.Data as ReferenceData);
            }
            else if (ir.Type.HasFlag(SerializedType.ReferenceCollection))
            {
                WriteReferenceCollection(writer, ir.Data as CollectionPropertyData);
            }
            else if (ir.Type.HasFlag(SerializedType.ComplexClass))
            {
                Debug.Log("complex class");
                // TODO:
            }
            else if (ir.Type.HasFlag(SerializedType.ComplexCollection))
            {
                // TODO:
                Debug.Log("complex collection");
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
                            var item = data.Collection[i] as DictionaryData<object, object>;
                            writer.Write((int)item.Type);
                            writer.Write((int)item.keyType);
                            writer.Write((int)item.ValueType);

                            if (item.keyType.HasFlag(SerializedType.Simple))
                            {
                                WriteSimpleProperty(writer, item.Key);
                            }
                            else
                            {
                                WriteReferenceProperty(writer, item.Key as ReferenceData);
                            }

                            if (item.ValueType.HasFlag(SerializedType.Simple))
                            {
                                WriteSimpleProperty(writer, item.Value);
                            }
                            else
                            {
                                WriteReferenceProperty(writer, item.Value as ReferenceData);
                            }
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException($"Collection type '{data.CollectionType}' is not implemented.");
            }
        }
        private static void WriteSimpleProperty(BinaryWriter writer, object data)
        {
            var simpleType = GetSimpleType(data);
            writer.Write((int)simpleType);
            switch (simpleType)
            {
                case SerializedSimpleType.None:
                    break;
                case SerializedSimpleType.Char:
                    writer.Write((char)data);
                    break;
                case SerializedSimpleType.String:
                    writer.Write((string)data);
                    break;
                case SerializedSimpleType.Bool:
                    writer.Write((bool)data);
                    break;
                case SerializedSimpleType.Byte:
                    writer.Write((byte)data);
                    break;
                case SerializedSimpleType.Short:
                    writer.Write((short)data);
                    break;
                case SerializedSimpleType.UShort:
                    writer.Write((ushort)data);
                    break;
                case SerializedSimpleType.Enum:
                    {
                        writer.Write(ReflectionUtils.GetStableGuid(data.GetType()).ToByteArray());
                        writer.Write((int)data);
                    }
                    break;
                case SerializedSimpleType.Int:
                    writer.Write((int)data);
                    break;
                case SerializedSimpleType.Uint:
                    writer.Write((uint)data);
                    break;
                case SerializedSimpleType.Float:
                    writer.Write((float)data);
                    break;
                case SerializedSimpleType.Double:
                    writer.Write((double)data);
                    break;
                case SerializedSimpleType.Long:
                    writer.Write((long)data);
                    break;
                case SerializedSimpleType.Ulong:
                    writer.Write((ulong)data);
                    break;
                case SerializedSimpleType.Vec2:
                    WriteVec2(writer, (vec2)data);
                    break;
                case SerializedSimpleType.Vec3:
                    WriteVec3(writer, (vec3)data);
                    break;
                case SerializedSimpleType.Vec4:
                    WriteVec4(writer, (vec4)data);
                    break;
                case SerializedSimpleType.Ivec2:
                    WriteIVec2(writer, (ivec2)data);
                    break;
                case SerializedSimpleType.Ivec3:
                    WriteIVec3(writer, (ivec3)data);
                    break;
                case SerializedSimpleType.Ivec4:
                    WriteIVec4(writer, (ivec4)data);
                    break;
                case SerializedSimpleType.Quat:
                    WriteQuat(writer, (quat)data);
                    break;
                case SerializedSimpleType.Mat2:
                    {
                        var value = (mat2)data;
                        WriteVec2(writer, value.c0);
                        WriteVec2(writer, value.c1);
                    }
                    break;
                case SerializedSimpleType.Mat3:
                    {
                        var value = (mat3)data;
                        WriteVec3(writer, value.c0);
                        WriteVec3(writer, value.c1);
                        WriteVec3(writer, value.c2);
                    }
                    break;
                case SerializedSimpleType.Mat4:
                    {
                        var value = (mat4)data;
                        WriteVec4(writer, value.c0);
                        WriteVec4(writer, value.c1);
                        WriteVec4(writer, value.c2);
                        WriteVec4(writer, value.c3);
                    }
                    break;
                case SerializedSimpleType.Color:
                    {
                        WriteVec4(writer, (Color)data);
                    }
                    break;
                case SerializedSimpleType.Color32:
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
        private static SerializedSimpleType GetSimpleType(object data)
        {
            switch (data)
            {
                case null: return SerializedSimpleType.None;
                case char _: return SerializedSimpleType.Char;
                case string _: return SerializedSimpleType.String;
                case bool _: return SerializedSimpleType.Bool;
                case byte _: return SerializedSimpleType.Byte;
                case short _: return SerializedSimpleType.Short;
                case ushort _: return SerializedSimpleType.UShort;
                case Enum _: return SerializedSimpleType.Enum;
                case int _: return SerializedSimpleType.Int;
                case uint _: return SerializedSimpleType.Uint;
                case float _: return SerializedSimpleType.Float;
                case double _: return SerializedSimpleType.Double;
                case long _: return SerializedSimpleType.Long;
                case ulong _: return SerializedSimpleType.Ulong;
                case vec2 _: return SerializedSimpleType.Vec2;
                case vec3 _: return SerializedSimpleType.Vec3;
                case vec4 _: return SerializedSimpleType.Vec4;
                case ivec2 _: return SerializedSimpleType.Ivec2;
                case ivec3 _: return SerializedSimpleType.Ivec3;
                case ivec4 _: return SerializedSimpleType.Ivec4;
                case quat _: return SerializedSimpleType.Quat;
                case mat2 _: return SerializedSimpleType.Mat2;
                case mat3 _: return SerializedSimpleType.Mat3;
                case mat4 _: return SerializedSimpleType.Mat4;
                case Color _: return SerializedSimpleType.Color;
                case Color32 _: return SerializedSimpleType.Color32;
                default:
                    throw new NotImplementedException($"Type for '{data.GetType().Name}' is not handled by the binary serializer.");
            }
        }

        private static Guid GetTypeId()
        {
            return Guid.NewGuid();
        }
    }
}
