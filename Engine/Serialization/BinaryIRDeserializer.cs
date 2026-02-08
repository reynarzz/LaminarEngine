using Engine.Utils;
using GlmNet;
using System;
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
        internal static List<SerializedPropertyIR> Deserialize(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            var properties = new List<SerializedPropertyIR>();
            CollectionsMarshal.SetCount(properties, count);

            for (int i = 0; i < count; i++)
            {
                properties[i] = ReadPropertyIR(reader);
            }

            return properties;
        }

        internal static SceneIR DeserializeScene(BinaryReader reader)
        {
            var scene = new SceneIR();
            scene.Version = reader.ReadInt32();
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
                 int Version 
                 string Name 
                 int Layer 
                 bool IsActiveSelf 
                 Guid ID 
                 Guid ParentID 
                 List<ComponentIR> Components 
            */

            var actor = new ActorIR();
            actor.Version = reader.ReadInt32();
            var strLength = reader.ReadInt32();
            actor.Name = Encoding.UTF8.GetString(reader.ReadBytes(strLength));
            actor.Layer = reader.ReadInt32();
            actor.IsActiveSelf = reader.ReadBoolean();
            actor.ID = new Guid(reader.ReadBytes(16));
            actor.ParentID = new Guid(reader.ReadBytes(16));
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
               int Version 
               Guid TypeId 
               bool IsEnabled 
               Guid ID 
               List<SerializedPropertyIR> SerializedProperties 
            */
            var component = new ComponentIR();
            component.Version = reader.ReadInt32();
            component.TypeId = new Guid(reader.ReadBytes(16));
            component.IsEnabled = reader.ReadBoolean();
            component.ID = new Guid(reader.ReadBytes(16));
            var propertiesCount = reader.ReadInt32();
            component.SerializedProperties = new List<SerializedPropertyIR>();

            CollectionsMarshal.SetCount(component.SerializedProperties, propertiesCount);

            for (int i = 0; i < propertiesCount; i++)
            {
                component.SerializedProperties[i] = ReadPropertyIR(reader);
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
            int strLength = reader.ReadInt32();
            property.Name = Encoding.UTF8.GetString(reader.ReadBytes(strLength));
            property.TypeId = new Guid(reader.ReadBytes(16));
            property.Type = (SerializedType)reader.ReadUInt64();
            var serializedType = property.Type;

            if (serializedType.IsSimple())
            {
                property.Data = ReadSimpleProperty(reader, serializedType);
            }
            //else if (serializedType.IsEObject())
            //{
            //    ReadReferenceProperty(reader, ir.Data as ReferenceData);
            //}
            //else if (serializedType == SerializedType.ReferenceCollection)
            //{
            //    ReadReferenceCollection(reader, ir.Data as CollectionPropertyData);
            //}
            else if (serializedType == SerializedType.ComplexClass)
            {
                property.Data = ReadComplexClass(reader);
            }
            //else if (serializedType == SerializedType.ComplexCollection)
            //{
            //    ReadComplexCollection(reader, ir.Data as CollectionPropertyData);
            //}

            return property;
        }

        private static ComplexTypeData ReadComplexClass(BinaryReader reader)
        {
            /*
              SerializedType ComplexType 
              Guid TypeId 
              List<SerializedPropertyIR> Properties 
           */
            var complexTypeData = new ComplexTypeData();

            complexTypeData.ComplexType = (SerializedType)reader.ReadUInt64();

            if (complexTypeData.ComplexType == SerializedType.None)
            {
                return complexTypeData;
            }

            complexTypeData.TypeId = new Guid(reader.ReadBytes(16));
            complexTypeData.Properties = Deserialize(reader);

            return complexTypeData;
        }

        private static object ReadSimpleProperty(BinaryReader reader, SerializedType simpleType)
        {
            switch (simpleType)
            {
                case SerializedType.None:
                    break;
                case SerializedType.Char:
                    return reader.ReadChar();
                case SerializedType.String:
                    return reader.ReadString();
                case SerializedType.Bool:
                    return reader.ReadBoolean();
                case SerializedType.Byte:
                    return reader.ReadByte();
                case SerializedType.Short:
                    return reader.ReadInt16();
                case SerializedType.UShort:
                    return reader.ReadUInt16();
                case SerializedType.Enum:
                    {
                        var enumTypeId = new Guid(reader.ReadBytes(16));
                        var enumValue = reader.ReadInt64();

                        // TODO: read enum type from TypeRegistry, and return real value.

                        return null;
                    }
                case SerializedType.Int:
                    return reader.ReadInt32();
                case SerializedType.Uint:
                    return reader.ReadUInt32();
                case SerializedType.Float:
                    return reader.ReadSingle();
                case SerializedType.Double:
                    return reader.ReadDouble();
                case SerializedType.Long:
                    return reader.ReadInt64();
                case SerializedType.Ulong:
                    return reader.ReadUInt64();
                case SerializedType.Vec2:
                    return ReadStruct<vec2>(reader);
                case SerializedType.Vec3:
                    return ReadStruct<vec3>(reader);
                case SerializedType.Vec4:
                    return ReadStruct<vec4>(reader);
                case SerializedType.Ivec2:
                    return ReadStruct<ivec2>(reader);
                case SerializedType.Ivec3:
                    return ReadStruct<ivec3>(reader);
                case SerializedType.Ivec4:
                    return ReadStruct<ivec4>(reader);
                case SerializedType.Quat:
                    return ReadStruct<quat>(reader);
                case SerializedType.Mat2:
                    return ReadStruct<mat2>(reader);
                case SerializedType.Mat3:
                    return ReadStruct<mat3>(reader);
                case SerializedType.Mat4:
                    return ReadStruct<mat4>(reader);
                case SerializedType.Color:
                    return (Color)reader.ReadUInt32();
                case SerializedType.Color32:
                    return (Color32)(ColorPacketRGBA)reader.ReadUInt32();
                default:
                    throw new NotImplementedException($"Reader not implemented for simple type: '{simpleType}'");
            }

            return null;
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

        public static T ReadStruct<T>(BinaryReader reader) where T : unmanaged
        {
            int size = Unsafe.SizeOf<T>();

            var buffer = reader.ReadBytes(size);

            if (buffer.Length < size)
            {
                throw new EndOfStreamException($"Expected {size} bytes, but reached end of stream.");
            }

            return MemoryMarshal.Read<T>(buffer);
        }
    }
}
