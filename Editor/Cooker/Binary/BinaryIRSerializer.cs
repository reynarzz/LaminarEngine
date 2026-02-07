using Engine;
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
                WriteSimpleProperty(writer, ir);
            }
            else if (ir.Type.HasFlag(SerializedType.EObject))
            {
                WriteReferenceProperty(writer, ir);
            }
            else if (ir.Type.HasFlag(SerializedType.ReferenceCollection))
            {
                // TODO:
            }
            else if (ir.Type.HasFlag(SerializedType.ComplexClass))
            {
                // TODO:
            }
            else if (ir.Type.HasFlag(SerializedType.ComplexCollection))
            {
                // TODO:
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
                    WriteVec2(writer, data);
                    break;
                case SerializedSimpleType.Vec3:
                    WriteVec3(writer, data);
                    break;
                case SerializedSimpleType.Vec4:
                    WriteVec4(writer, data);
                    break;
                case SerializedSimpleType.Ivec2:
                    WriteIVec2(writer, data);
                    break;
                case SerializedSimpleType.Ivec3:
                    WriteIVec3(writer, data);
                    break;
                case SerializedSimpleType.Ivec4:
                    WriteIVec4(writer, data);
                    break;
                case SerializedSimpleType.Quat:
                    WriteQuat(writer, data);
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
                default:
                    break;
            }
        }

        private static void WriteReferenceProperty(BinaryWriter writer, object data)
        {
            if (data != null)
            {
                var value = data as ReferenceData;
                writer.Write(value.Id.ToByteArray());
            }
            else
            {
                writer.Write(Guid.Empty.ToByteArray());
            }
        }

        private static void WriteVec2(BinaryWriter writer, object data)
        {
            var value = (vec2)data;
            writer.Write(value.x);
            writer.Write(value.y);
        }
        private static void WriteVec3(BinaryWriter writer, object data)
        {
            var value = (vec3)data;
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }
        private static void WriteVec4(BinaryWriter writer, object data)
        {
            var value = (vec4)data;
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }
        private static void WriteQuat(BinaryWriter writer, object data)
        {
            var value = (quat)data;
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }
        private static void WriteIVec2(BinaryWriter writer, object data)
        {
            var value = (ivec2)data;
            writer.Write(value.x);
            writer.Write(value.y);
        }
        private static void WriteIVec3(BinaryWriter writer, object data)
        {
            var value = (ivec3)data;
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }
        private static void WriteIVec4(BinaryWriter writer, object data)
        {
            //var value = (ivec4)data;
            //writer.Write(value.x);
            //writer.Write(value.y);
            //writer.Write(value.z);
            //writer.Write(value.w);
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
                // case ivec4 _: return SerializedSimpleType.Ivec4;
                case quat _: return SerializedSimpleType.Quat;
                case mat2 _: return SerializedSimpleType.Mat2;
                case mat3 _: return SerializedSimpleType.Mat3;
                case mat4 _: return SerializedSimpleType.Mat4;

                default:
                    throw new NotImplementedException($"Type for '{data.GetType().Name}' is not handled by binary serializer.");
            }
        }

        private static Guid GetTypeId()
        {
            return Guid.NewGuid();
        }
    }
}
