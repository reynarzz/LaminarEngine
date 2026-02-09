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
        private const int GUID_BYTES_SIZE = 16;
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
            actor.ID = new Guid(reader.ReadBytes(GUID_BYTES_SIZE));
            actor.ParentID = new Guid(reader.ReadBytes(GUID_BYTES_SIZE));
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
            component.TypeId = new Guid(reader.ReadBytes(GUID_BYTES_SIZE));
            component.IsEnabled = reader.ReadBoolean();
            component.ID = new Guid(reader.ReadBytes(GUID_BYTES_SIZE));
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
            property.TypeId = new Guid(reader.ReadBytes(GUID_BYTES_SIZE));
            property.Type = (SerializedType)reader.ReadInt64();
            var serializedType = property.Type;

            if (serializedType.IsSimple())
            {
                property.Data = ReadSimpleProperty(reader, serializedType);
            }
            else if (serializedType.IsEObject())
            {
                property.Data = ReadReferenceProperty(reader);
            }
            else if (serializedType == SerializedType.ReferenceCollection)
            {
                property.Data = ReadReferenceCollection(reader);
            }
            else if (serializedType == SerializedType.ComplexClass)
            {
                property.Data = ReadComplexClass(reader);
            }
            else if (serializedType == SerializedType.ComplexCollection)
            {
                property.Data = ReadComplexCollection(reader);
            }
            else if (serializedType == SerializedType.SimpleCollection)
            {
                // property.Data = ReadSimpleCollection(reader);
            }

            return property;
        }

        private static CollectionPropertyData ReadComplexCollection(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            var collectionData = new CollectionPropertyData();
            if (count == 0)
            {
                return collectionData;
            }
            collectionData.CollectionType = (CollectionType)reader.ReadInt64();
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
                        //var result = new List<CollectionData<ComplexTypeData>>();
                        //CollectionsMarshal.SetCount(result, count);

                        //for (int i = 0; i < count; i++)
                        //{
                        //    var item = new CollectionData<ComplexTypeData>();
                        //    item.Type = (SerializedType)reader.ReadInt64();
                        //    item.Value = ReadComplexClass(reader);
                        //    result[i] = item;
                        //}
                        //collectionData.Collection = result;
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        //var result = new ComplexDictionaryData(count);

                        //for (int i = 0; i < count; i++)
                        //{
                        //    //result.Type = (SerializedType)reader.ReadInt64();
                        //    result.Keys[i] = ReadComplexClass(reader);
                        //    result.Values[i] = ReadComplexClass(reader);
                        //}
                        //collectionData.Collection = result;
                    }
                    break;
                default:
                    throw new NotImplementedException($"Collection type '{collectionData.CollectionType}' is not implemented.");
            }

            return collectionData;
        }

        private static CollectionPropertyData ReadReferenceCollection(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            var collectionData = new CollectionPropertyData();
            if (count == 0)
            {
                return collectionData;
            }
            // collectionData.Collection = new List<object>();
            collectionData.CollectionType = (CollectionType)reader.ReadInt64();
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
                        //var result = new List<CollectionData<ReferenceData>>();
                        //CollectionsMarshal.SetCount(result, count);

                        //for (int i = 0; i < count; i++)
                        //{
                        //    var item = new CollectionData<ReferenceData>();
                        //    item.Type = (SerializedType)reader.ReadInt64();
                        //    item.Value = new ReferenceData()
                        //    {
                        //        Id = new Guid(reader.ReadBytes(GUID_BYTES_SIZE))
                        //    };
                        //    result[i] = item;
                        //}

                        //collectionData.Collection = result;
                    }
                    break;
                case CollectionType.Dictionary:
                    {
                        var result = new DictionaryData(count);

                        //for (int i = 0; i < count; i++)
                        //{
                        //    var item = new DictionaryData();
                        //    result.Types = (SerializedType)reader.ReadInt64();
                        //    result.KeyType = (SerializedType)reader.ReadInt64();
                        //    result.ValueType = (SerializedType)reader.ReadInt64();

                        //    object ReadArg(SerializedType type)
                        //    {
                        //        if (type.IsSimple())
                        //        {
                        //            return ReadSimpleProperty(reader, type);
                        //        }

                        //        return ReadReferenceProperty(reader);
                        //    }

                        //    item.Key = ReadArg(item.KeyType);
                        //    item.Value = ReadArg(item.ValueType);

                        //    result[i] = item;
                        //}
                        collectionData.Collection = result;
                    }
                    break;
                default:
                    throw new NotImplementedException($"Deserializer: Collection type '{collectionData.CollectionType}' is not implemented.");
            }

            return collectionData;
        }

        private static ReferenceData ReadReferenceProperty(BinaryReader reader)
        {
            return new ReferenceData()
            {
                Id = new Guid(reader.ReadBytes(GUID_BYTES_SIZE)),
            };
        }
        private static ComplexTypeData ReadComplexClass(BinaryReader reader)
        {
            /*
              SerializedType ComplexType 
              Guid TypeId 
              List<SerializedPropertyIR> Properties 
           */
            var complexTypeData = new ComplexTypeData();

            complexTypeData.ComplexType = (SerializedType)reader.ReadInt64();

            if (complexTypeData.ComplexType == SerializedType.None)
            {
                return complexTypeData;
            }

            complexTypeData.TypeId = new Guid(reader.ReadBytes(GUID_BYTES_SIZE));
            complexTypeData.Properties = Deserialize(reader);

            return complexTypeData;
        }

        // TODO: use Variant
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
                        var enumTypeId = new Guid(reader.ReadBytes(GUID_BYTES_SIZE));
                        var enumValue = reader.ReadInt64();

                        // TODO: read enum type from TypeRegistry, and return real value.

                        return null;
                    }
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
                    return reader.ReadInt64();
                case SerializedType.Vec2:
                    return ReadStruct<vec2>(reader);
                case SerializedType.Vec3:
                    return ReadStruct<vec3>(reader);
                case SerializedType.Vec4:
                    return ReadStruct<vec4>(reader);
                case SerializedType.IVec2:
                    return ReadStruct<ivec2>(reader);
                case SerializedType.IVec3:
                    return ReadStruct<ivec3>(reader);
                case SerializedType.IVec4:
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
