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
            else if (serializedType == SerializedType.ComplexClass)
            {
                property.ComplexClass = ReadComplexClass(reader);
            }
            else if (serializedType == SerializedType.ComplexCollection)
            {
                property.Collection = ReadComplexCollection(reader);
            }
            else if (serializedType == SerializedType.SimpleCollection)
            {
                // property.Data = ReadSimpleCollection(reader);
            }

            return property;
        }

        private static CollectionData ReadComplexCollection(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            if (count == 0)
            {
                return null;
            }
            var collectionType = (CollectionType)reader.ReadInt64();
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

                        // var collectionData = new CollectionData();
                        //var result = new List<CollectionData<ComplexTypeData>>();
                        //CollectionsMarshal.SetCount(result, count);

                        //for (int i = 0; i < count; i++)
                        //{
                        //    var item = new CollectionData<ComplexTypeData>();
                        //    item.Type = (SerializedType)reader.ReadInt64();
                        //    item.Value = ReadComplexClass(reader);
                        //    result[i] = item;
                        //}
                        //return result;
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
                        // return result;
                    }
                    break;
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

            // collectionData.Collection = new List<object>();
            var collectionType = (CollectionType)reader.ReadInt64();
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
                        // var collectionData = new CollectionData();

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
                        var result = new DictionaryIRReferences(count);

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
                        return result;
                    }
                default:
                    throw new NotImplementedException($"Deserializer: Collection type '{collectionType}' is not implemented.");
            }

            return null;
        }

        private static ReferenceData ReadReferenceProperty(BinaryReader reader)
        {
            return new ReferenceData()
            {
                Id = new Guid(reader.ReadBytes(GUID_BYTES_SIZE)),
            };
        }
        private static ComplexClassData ReadComplexClass(BinaryReader reader)
        {
            /*
              SerializedType ComplexType 
              Guid TypeId 
              List<SerializedPropertyIR> Properties 
           */
            var complexTypeData = new ComplexClassData();

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
        private static VariantIRValue ReadSimpleProperty(BinaryReader reader, SerializedType simpleType)
        {
            switch (simpleType)
            {
                case SerializedType.None:
                    return default;
                case SerializedType.Char:
                    return VariantIRValue.FromChar(reader.ReadChar());
                case SerializedType.String:
                    return VariantIRValue.FromString(reader.ReadString());
                case SerializedType.Bool:
                    return VariantIRValue.FromBool(reader.ReadBoolean());
                case SerializedType.Byte:
                    return VariantIRValue.FromByte(reader.ReadByte());
                case SerializedType.Short:
                    return VariantIRValue.FromShort(reader.ReadInt16());
                case SerializedType.UShort:
                    return VariantIRValue.FromUShort(reader.ReadUInt16());
                case SerializedType.Enum:
                    {
                        var enumTypeId = new Guid(reader.ReadBytes(GUID_BYTES_SIZE));
                        var enumValue = reader.ReadInt64();
                        return VariantIRValue.FromEnum(enumTypeId, string.Empty, enumValue);
                    }
                case SerializedType.Int:
                    return VariantIRValue.FromInt(reader.ReadInt32());
                case SerializedType.UInt:
                    return VariantIRValue.FromUInt(reader.ReadUInt32());
                case SerializedType.Float:
                    return VariantIRValue.FromFloat(reader.ReadSingle());
                case SerializedType.Double:
                    return VariantIRValue.FromDouble(reader.ReadDouble());
                case SerializedType.Long:
                    return VariantIRValue.FromLong(reader.ReadInt64());
                case SerializedType.ULong:
                    return VariantIRValue.FromULong(reader.ReadUInt64());
                case SerializedType.Vec2:
                    return VariantIRValue.FromVec2(ReadStruct<vec2>(reader));
                case SerializedType.Vec3:
                    return VariantIRValue.FromVec3(ReadStruct<vec3>(reader));
                case SerializedType.Vec4:
                    return VariantIRValue.FromVec4(ReadStruct<vec4>(reader));
                case SerializedType.IVec2:
                    return VariantIRValue.FromIVec2(ReadStruct<ivec2>(reader));
                case SerializedType.IVec3:
                    return VariantIRValue.FromIVec3(ReadStruct<ivec3>(reader));
                case SerializedType.IVec4:
                    return VariantIRValue.FromIVec4(ReadStruct<ivec4>(reader));
                case SerializedType.Quat:
                    return VariantIRValue.FromQuat(ReadStruct<quat>(reader));
                case SerializedType.Mat2:
                    return VariantIRValue.FromMat2(ReadStruct<mat2>(reader));
                case SerializedType.Mat3:
                    return VariantIRValue.FromMat3(ReadStruct<mat3>(reader));
                case SerializedType.Mat4:
                    return VariantIRValue.FromMat4(ReadStruct<mat4>(reader));
                case SerializedType.Color:
                    return VariantIRValue.FromColor((Color)reader.ReadUInt32());
                case SerializedType.Color32:
                    return VariantIRValue.FromColor32((Color32)(ColorPacketRGBA)reader.ReadUInt32());
                default:
                    throw new NotImplementedException($"Reader not implemented for simple type: '{simpleType}'");
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
