using System;
using Engine;
using Engine.Utils;
using System.Collections;
using System.Reflection;
using GlmNet;
using Engine.Serialization;

namespace Editor.Serialization
{
    internal static class Serializer
    {
        /// <summary>
        /// Gets the IR of all the properties marked with the 'SerializedField' attribute.
        /// </summary>

        internal static CollectionData Serialize(IList collection)
        {
            var type = collection.GetType();
            var serializedType = GetSerializedType(type);
            return GetPropertyData(type, serializedType, collection) as CollectionData;
        }

        internal static SerializedPropertyIR[] Serialize(object target)
        {
            if (target is IEnumerable && target is not string)
            {
                throw new ArgumentException("Collections are not allowed.", nameof(target));
            }

            var serializedMembers = ReflectionUtils.GetAllMembersWithAttributeArray<SerializedFieldAttribute>(target.GetType());
            var properties = new SerializedPropertyIR[serializedMembers.Length];

            for (int i = 0; i < serializedMembers.Length; ++i)
            {
                var member = serializedMembers[i];
                var value = ReflectionUtils.GetMemberValue(target, member);
                var valueType = value?.GetType() ?? ReflectionUtils.GetMemberType(member);
                var serializedType = GetSerializedType(valueType, value);

                var propData = GetPropertyData(valueType, serializedType, value);
                properties[i] = new SerializedPropertyIR()
                {
                    Name = member.Name,
                    InternalType = GetInternalType(valueType),
                    TypeId = GetTypeId(valueType),
                    Type = serializedType,
                    Simple = TryGetSimplePropertyData(propData, serializedType),
                    Reference = propData as ReferenceData,
                    Class = propData as ClassData,
                    Collection = propData as CollectionData
                };
            }

            return properties;
        }

        internal static SerializedType GetSerializedType(Type type, object value = null)
        {
            type = value?.GetType() ?? type;

            if (type == null)
            {
                return SerializedType.None;
            }

            if (type.IsAssignableTo(typeof(IObject)))
            {
                if (type.IsAssignableTo(typeof(IComponent)))
                {
                    return SerializedType.Component;
                }
                else if (type.IsAssignableTo(typeof(Actor)))
                {
                    return SerializedType.Actor;
                }
                else if (type.IsAssignableTo(typeof(AssetResourceBase)))
                {
                    if (type.IsAssignableTo(typeof(Texture)))
                    {
                        return SerializedType.TextureAsset;
                    }
                    else if (type.IsAssignableTo(typeof(AudioClip)))
                    {
                        return SerializedType.AudioClipAsset;
                    }
                    else if (type.IsAssignableTo(typeof(AnimationClip)))
                    {
                        return SerializedType.AnimationAsset;
                    }
                    else if (type.IsAssignableTo(typeof(Shader)))
                    {
                        return SerializedType.ShaderAsset;
                    }
                    else if (type.IsAssignableTo(typeof(Material)))
                    {
                        return SerializedType.MaterialAsset;
                    }
                    else if (type.IsAssignableTo(typeof(AnimatorController)))
                    {
                        return SerializedType.AnimatorControllerAsset;
                    }
                    else if (type.IsAssignableTo(typeof(TilemapAsset)))
                    {
                        return SerializedType.Tilemap;
                    }
                    else if (type.IsAssignableTo(typeof(Prefab)))
                    {
                        return SerializedType.Prefab;
                    }
                    else if (type.IsAssignableTo(typeof(SceneAsset)))
                    {
                        return SerializedType.Scene;
                    }
                    else if (type.IsAssignableTo(typeof(FontAsset)))
                    {
                        return SerializedType.Font;
                    }
                    Debug.Error($"Type: '{ReflectionUtils.GetFriendlyTypeName(type)}' is an asset but is not added to the serializer 'SerializedType', this could cause a crash if trying to save.");
                    return SerializedType.AssetFlag;
                }
                else
                {
                    if (type.IsAssignableTo(typeof(Sprite)))
                    {
                        return SerializedType.SpriteAsset;
                    }
                    return SerializedType.EObjectFlag;
                }
            }
            else if (ReflectionUtils.IsCollection(type, out var collectionType))
            {
                // TODO: get all the elements in the collection so they can be checked in case there is a reference.
                var elementsTypes = ReflectionUtils.GetCollectionElementsType(type);
                bool allInternal = true;
                for (var i = 0; i < elementsTypes.Length; i++)
                {
                    allInternal = ReflectionUtils.IsInternalType(elementsTypes[i]);
                    if (!allInternal)
                    {
                        break;
                    }
                }

                if (allInternal)
                {
                    return SerializedType.SimpleCollection;
                }

                var isSingleArgCollectionAEObject = elementsTypes.Length == 1 && elementsTypes.Any(x => x.IsAssignableTo(typeof(IObject)));

                if (isSingleArgCollectionAEObject ||
                    (collectionType == CollectionType.Dictionary && IsPureReferenceDictionary(elementsTypes, value)))
                {
                    return SerializedType.ReferenceCollection;
                }

                return SerializedType.ComplexCollection;
            }
            else if (type.IsAssignableTo(typeof(Delegate)))
            {
                return SerializedType.Delegate;
            }
            else if (ReflectionUtils.IsInternalType(type))
            {
                return GetSimpleType(type);
            }
            else if (type.IsClass || ReflectionUtils.IsUserDefinedStruct(type))
            {
                return SerializedType.ComplexClass;
            }
            return SerializedType.None;
        }

        /// <summary>
        /// Checks if a dictionary containing EObjects doesn't have a EObject deep in the graph, but only on the key or value generic args.
        /// Immediate in the key or value = pure, Deep into the object graph = not pure
        /// </summary>
        internal static bool IsPureReferenceDictionary(Type[] genericArgs, object value)
        {
            if (genericArgs == null || genericArgs.Length != 2)
            {
                Debug.Error("Invalid generic arguments.");
                return false;
            }
            //if (genericArgs.Length > 1 && genericArgs[1] == typeof(Component))
            //{

            //}
            if (value == null)
            {
                bool ContainsTop(Type argType, out bool hasAny)
                {
                    hasAny = false;
                    if (ReflectionUtils.IsEObject(argType))
                    {
                        hasAny = true;
                        return true;
                    }

                    hasAny = ReflectionUtils.HasAnySerializedMemberWithType(argType, typeof(IObject), null);

                    return false;
                }

                var hasKeyTop = ContainsTop(genericArgs[0], out var hasEObjectAsKey);
                if (!hasKeyTop && hasEObjectAsKey)
                {
                    return false;
                }

                var hasValueTop = ContainsTop(genericArgs[1], out var containsEObjectAsVal);
                if (!hasValueTop && containsEObjectAsVal)
                {
                    return false;
                }

                return (hasEObjectAsKey && hasKeyTop && (!containsEObjectAsVal || hasValueTop)) ||
                       (containsEObjectAsVal && hasValueTop && (!hasEObjectAsKey || hasKeyTop));
            }

            var dictionary = value as IDictionary;

            void CheckInstanceArg(ICollection args, Type argDefault, out bool hasAny, out bool isTopLevel)
            {
                hasAny = false;
                isTopLevel = false;

                foreach (var argInstance in args)
                {
                    var argType = argInstance?.GetType() ?? argDefault;

                    if (ReflectionUtils.IsEObject(argType))
                    {
                        hasAny = true;
                        isTopLevel = true;
                    }
                    else
                    {
                        var hasEObject = ReflectionUtils.HasAnySerializedMemberWithType(argType, typeof(IObject), argInstance);

                        if (hasEObject)
                        {
                            hasAny = true;
                            isTopLevel = false;
                            return;
                        }
                    }
                }
            }

            // Check in keys using the runtime type.
            CheckInstanceArg(dictionary.Keys, genericArgs[0], out var keyHasAny, out var keyTop);

            if (keyHasAny && !keyTop)
            {
                return false;
            }

            // Check in values using the runtime type
            CheckInstanceArg(dictionary.Values, genericArgs[1], out var valHasAny, out var valTop);

            if (valHasAny && !valTop)
            {
                return false;
            }

            return (keyHasAny && keyTop && (!valHasAny || valTop)) ||
                   (valHasAny && valTop && (!keyHasAny || keyTop));
        }

        // This only returns reference ids, simple and complex property data.
        internal static object GetPropertyData(Type type, SerializedType serializedMemberType, object value)
        {
            // Note: For runtime-created resource assets such as Materials, Shaders, Textures etc... maybe should have a empty guid, so the serializer,
            //       does not point to a invalid physical asset.

            if (value == null)
            {
                return null;
            }

            if (serializedMemberType.IsSimple())
            {
                return GetVariantValue(value, serializedMemberType);
            }

            if (value != null)
            {
                type = value?.GetType();
            }

            if (type.IsAssignableTo(typeof(Delegate)))
            {
                // TODO: handle delegates.
                Debug.Warn($"TODO: Can't serialize delegate: {type.Name}");

                if (value != null)
                {
                    var delegateObj = value as Delegate;
                    var data = new DelegateClassData();

                    foreach (var subscriber in delegateObj.GetInvocationList())
                    {
                        if (subscriber.Target is IObject targetEObject)
                        {

                        }
                    }
                }

                return null;
            }
            else if (type.IsAssignableTo(typeof(IObject)))
            {
                var id = (value as IObject)?.GetID() ?? Guid.Empty;

                return GetReferenceData(id, serializedMemberType, value);
            }
            else if (ReflectionUtils.IsCollection(type, out var collectionType))
            {
                CollectionData collectionPropData = null;

                if (collectionType == CollectionType.Dictionary)
                {
                    collectionPropData = GetDictionaryData(type, serializedMemberType, collectionType, value);
                }
                else
                {
                    collectionPropData = Get1DCollectionData(serializedMemberType, collectionType, value, out var itemsType);
                }

                return collectionPropData;
            }
            else if (serializedMemberType.IsClass())
            {
                if (serializedMemberType == SerializedType.ComplexClass)
                {
                    return CreateComplexType(type, value);
                }
                else
                {
                    throw new NotImplementedException($"Class type '{serializedMemberType}' is not implemented.");
                }
            }

            return null;
        }

        private static CollectionData Get1DCollectionSimpleData(ICollection collection, CollectionType collectionType,
            out SerializedType itemsType)
        {
            itemsType = SerializedType.None;
            foreach (var item in collection)
            {
                if (itemsType == SerializedType.None)
                {
                    itemsType = GetSimpleType(item?.GetType());
                    break;
                }
            }
            if (itemsType == SerializedType.None)
            {
                return null;
            }
            T[] GetCollectionValues<T>()
            {
                var values = new T[collection.Count];
                int index = 0;
                foreach (var item in collection)
                {
                    values[index++] = (T)item;
                }
                return values;
            }

            switch (itemsType)
            {
                case SerializedType.Enum:
                    {
                        var collectionDataEnum = new EnumIRValue[collection.Count];
                        int index = 0;
                        foreach (var item in collection)
                        {
                            collectionDataEnum[index++] = ((Variant)(Enum)item).Enum;
                        }
                        return new CollectionDataEnum(collectionDataEnum, collectionType);
                    }
                case SerializedType.Char:
                    return new CollectionDataChar(GetCollectionValues<char>(), collectionType);
                case SerializedType.String:
                    return new CollectionDataString(GetCollectionValues<string>(), collectionType);
                case SerializedType.Bool:
                    return new CollectionDataBool(GetCollectionValues<bool>(), collectionType);
                case SerializedType.Byte:
                    return new CollectionDataByte(GetCollectionValues<byte>(), collectionType);
                case SerializedType.Short:
                    return new CollectionDataShort(GetCollectionValues<short>(), collectionType);
                case SerializedType.UShort:
                    return new CollectionDataUShort(GetCollectionValues<ushort>(), collectionType);
                case SerializedType.Int:
                    return new CollectionDataInt(GetCollectionValues<int>(), collectionType);
                case SerializedType.UInt:
                    return new CollectionDataUInt(GetCollectionValues<uint>(), collectionType);
                case SerializedType.Float:
                    return new CollectionDataFloat(GetCollectionValues<float>(), collectionType);
                case SerializedType.Double:
                    return new CollectionDataDouble(GetCollectionValues<double>(), collectionType);
                case SerializedType.Long:
                    return new CollectionDataLong(GetCollectionValues<long>(), collectionType);
                case SerializedType.ULong:
                    return new CollectionDataULong(GetCollectionValues<ulong>(), collectionType);
                case SerializedType.Vec2:
                    return new CollectionDataVec2(GetCollectionValues<vec2>(), collectionType);
                case SerializedType.Vec3:
                    return new CollectionDataVec3(GetCollectionValues<vec3>(), collectionType);
                case SerializedType.Vec4:
                    return new CollectionDataVec4(GetCollectionValues<vec4>(), collectionType);
                case SerializedType.IVec2:
                    return new CollectionDataIvec2(GetCollectionValues<ivec2>(), collectionType);
                case SerializedType.IVec3:
                    return new CollectionDataIvec3(GetCollectionValues<ivec3>(), collectionType);
                case SerializedType.IVec4:
                    return new CollectionDataIvec4(GetCollectionValues<ivec4>(), collectionType);
                case SerializedType.Quat:
                    return new CollectionDataQuat(GetCollectionValues<quat>(), collectionType);
                case SerializedType.Mat2:
                    return new CollectionDataMat2(GetCollectionValues<mat2>(), collectionType);
                case SerializedType.Mat3:
                    return new CollectionDataMat3(GetCollectionValues<mat3>(), collectionType);
                case SerializedType.Mat4:
                    return new CollectionDataMat4(GetCollectionValues<mat4>(), collectionType);
                case SerializedType.Color:
                    return new CollectionDataColor(GetCollectionValues<Color>(), collectionType);
                case SerializedType.Color32:
                    return new CollectionDataColor32(GetCollectionValues<Color32>(), collectionType);
                default:
                    throw new NotImplementedException($"Simple type: '{itemsType}' is not implemented");
            }
        }
        private static CollectionData Get1DCollectionData(SerializedType serializedMemberType, CollectionType collectionType,
                                                          object value, out SerializedType itemsType)
        {
            itemsType = SerializedType.None;
            var collection = (ICollection)value;

            if (serializedMemberType == SerializedType.SimpleCollection)
            {
                return Get1DCollectionSimpleData(collection, collectionType, out itemsType);
            }
            else if (serializedMemberType == SerializedType.ReferenceCollection)
            {
                var references = new ReferenceData[collection.Count];
                var referencesTypes = new SerializedType[collection.Count];
                int index = 0;

                foreach (var item in collection)
                {
                    var itemSerializedType = GetSerializedType(item?.GetType(), null);

                    referencesTypes[index] = itemSerializedType;
                    references[index] = GetReferenceData((item as IObject)?.GetID() ?? Guid.Empty, itemSerializedType, item);
                    index++;
                }

                return new CollectionReferences(references, referencesTypes, collectionType);
            }
            else if (serializedMemberType == SerializedType.ComplexCollection)
            {
                var complexTypeArr = new ClassData[collection.Count];
                int index = 0;

                foreach (var item in collection)
                {
                    complexTypeArr[index++] = CreateComplexType(item?.GetType(), item);
                }
                return new CollectionClasses(complexTypeArr, collectionType);
            }

            throw new NotSupportedException($"Collection is not supported '{serializedMemberType}', maybe is an error, or is not implemented?");
        }

        private static CollectionData GetDictionaryData(Type type, SerializedType serializedMemberType,
                                                        CollectionType collectionType, object value)
        {
            var defaultElementsType = ReflectionUtils.GetCollectionElementsType(type);

            var dictionary = (IDictionary)value;

            CollectionData dictionaryCollection = null;

            if (serializedMemberType == SerializedType.SimpleCollection)
            {
                dictionaryCollection = new DictionarySimple(dictionary.Keys.Count, collectionType);
            }
            else if (serializedMemberType == SerializedType.ReferenceCollection)
            {
                dictionaryCollection = new DictionaryIRReferences(dictionary.Keys.Count, collectionType);
            }
            else if (serializedMemberType == SerializedType.ComplexCollection)
            {
                dictionaryCollection = new DictionaryClass(dictionary.Keys.Count, collectionType);
            }
            else
            {
                throw new NotImplementedException($"Collection type not implemented: {serializedMemberType}");
            }

            object GetArgData(object argData, Type argType, SerializedType argSerializedType)
            {
                var isArgEObject = (argData?.GetType() ?? argType).IsAssignableTo(typeof(IObject));
                return isArgEObject ? GetReferenceData((argData as IObject)?.GetID() ?? Guid.Empty, argSerializedType, argData) : argData;
            }


            void PopulateDictionaryCollection(ICollection argCollection, Type argDefaultType, Variant[] simpleArg,
                                               object[] referenceArg, ClassData[] complexArg, SerializedType[] refArgTypes,
                                               out SerializedType simpleArgType, string complexPropKey)
            {
                simpleArgType = SerializedType.None;

                int index = 0;
                foreach (var arg in argCollection)
                {
                    var argSerializedType = GetSerializedType(argDefaultType, arg);
                    var argData = GetArgData(arg, argDefaultType, argSerializedType);

                    if (serializedMemberType == SerializedType.SimpleCollection)
                    {
                        simpleArg[index] = GetVariantValue(argData, argSerializedType);
                        simpleArgType = argSerializedType;
                    }
                    else if (serializedMemberType == SerializedType.ReferenceCollection)
                    {
                        referenceArg[index] = argSerializedType.IsSimple() ? GetVariantValue(argData, argSerializedType) : argData;
                        refArgTypes[index] = argSerializedType;
                    }
                    else if (serializedMemberType == SerializedType.ComplexCollection)
                    {
                        complexArg[index] = GetComplexTypeData(argSerializedType, argData, complexPropKey);
                    }

                    index++;
                }
            }

            var simpleDictionary = dictionaryCollection as DictionarySimple;
            var referenceDictionary = dictionaryCollection as DictionaryIRReferences;
            var complexDictionary = dictionaryCollection as DictionaryClass;


            PopulateDictionaryCollection(dictionary.Keys, defaultElementsType[0], simpleDictionary?.Keys, referenceDictionary?.Keys,
                                         complexDictionary?.Keys, referenceDictionary?.KeyType, out var keySimpleType, "D_K");

            PopulateDictionaryCollection(dictionary.Values, defaultElementsType[1], simpleDictionary?.Values, referenceDictionary?.Values,
                                         complexDictionary?.Values, referenceDictionary?.ValueType, out var valueSimpleType, "D_V");

            if (serializedMemberType == SerializedType.SimpleCollection)
            {
                simpleDictionary.KeyType = keySimpleType;
                simpleDictionary.ValueType = valueSimpleType;
            }

            return dictionaryCollection;
        }

        // This is only used for dictionaries. The performance is not ideal for Simple types, but it balances the complexity.
        private static ClassData GetComplexTypeData(SerializedType argSerializedType, object argValue, string argName)
        {
            if (argSerializedType.IsSimple())
            {
                var internalType = GetInternalType(argValue?.GetType());
                var typeId = GetTypeId(argValue?.GetType());
                return new ComplexClass()
                {
                    ClassType = argSerializedType,
                    InternalType = internalType,
                    TypeId = typeId,
                    Properties =
                    [
                        new SerializedPropertyIR()
                        {
                           Simple = GetVariantValue(argValue, argSerializedType),
                           InternalType = internalType,
                           TypeId = typeId,
                           Type = argSerializedType,
                           Name = argName
                        }
                    ]
                };
            }

            return CreateComplexType(argValue?.GetType(), argValue);
        }

        private static Variant GetVariantValue(object val, SerializedType type)
        {
            switch (type)
            {
                case SerializedType.Enum:
                    return (Enum)val;
                case SerializedType.Char:
                    return (char)val;
                case SerializedType.String:
                    return val as string ?? string.Empty;
                case SerializedType.Bool:
                    return (bool)val;
                case SerializedType.Byte:
                    return (byte)val;
                case SerializedType.Short:
                    return (short)val;
                case SerializedType.UShort:
                    return (ushort)val;
                case SerializedType.Int:
                    return (int)val;
                case SerializedType.UInt:
                    return (uint)val;
                case SerializedType.Float:
                    return (float)val;
                case SerializedType.Double:
                    return (double)val;
                case SerializedType.Long:
                    return (long)val;
                case SerializedType.ULong:
                    return (ulong)val;
                case SerializedType.Vec2:
                    return (vec2)val;
                case SerializedType.Vec3:
                    return (vec3)val;
                case SerializedType.Vec4:
                    return (vec4)val;
                case SerializedType.IVec2:
                    return (ivec2)val;
                case SerializedType.IVec3:
                    return (ivec3)val;
                case SerializedType.IVec4:
                    return (ivec4)val;
                case SerializedType.Quat:
                    return (quat)val;
                case SerializedType.Mat2:
                    return (mat2)val;
                case SerializedType.Mat3:
                    return (mat3)val;
                case SerializedType.Mat4:
                    return (mat4)val;
                case SerializedType.Color:
                    return (Color)val;
                case SerializedType.Color32:
                    return (Color32)val;
                default:
                    throw new ArgumentException($"Probably not a simple type: {type}");
            }
        }

        private static ReferenceData GetReferenceData(Guid id, SerializedType serializedMemberType, object value)
        {
            if (serializedMemberType == SerializedType.None)
            {
                return null;
            }

            if (serializedMemberType == SerializedType.SpriteAsset)
            {
                return new SpriteReferenceData()
                {
                    Type = serializedMemberType,
                    RefId = id,
                    AtlasIndex = (value as Sprite).AtlasIndex,
                    TexRefId = (value as Sprite).Texture.GetID()
                };
            }
            else if (serializedMemberType.IsEObject())
            {
                return new ReferenceData()
                {
                    Type = serializedMemberType,
                    RefId = id
                };
            }

            Debug.EngineError($"Reference type '{serializedMemberType}' not supported");

            return new ReferenceData()
            {
                Type = serializedMemberType,
                RefId = id
            };
        }
        internal static ClassData CreateComplexType(Type complexType, object value)
        {
            if (complexType == null)
                return null;

            // Get the concreate type, this makes sure to get the actual type even for inherit types.
            complexType = value?.GetType() ?? complexType;

            SerializedPropertyIR GetPropertyGraph(MemberInfo currentType, object target)
            {
                var value = ReflectionUtils.GetMemberValue(target, currentType);
                var valueType = value?.GetType() ?? ReflectionUtils.GetMemberType(currentType);
                var serializedType = GetSerializedType(valueType, null);

                var data = GetPropertyData(valueType, serializedType, value);
                return new SerializedPropertyIR()
                {
                    Name = currentType.Name,
                    Type = serializedType,
                    InternalType = GetInternalType(valueType),
                    TypeId = GetTypeId(valueType),
                    Simple = TryGetSimplePropertyData(data, serializedType),
                    Reference = data as ReferenceData,
                    Class = data as ClassData,
                    Collection = data as CollectionData,
                };
            }

            var rootSerializedFields = ReflectionUtils.GetAllMembersWithAttributeArray<SerializedFieldAttribute>(complexType);

            var complexClass = new ComplexClass()
            {
                ClassType = GetSerializedType(complexType, value),
                InternalType = GetInternalType(complexType),
                TypeId = GetTypeId(complexType),
                Properties = new SerializedPropertyIR[rootSerializedFields.Length]
            };

            for (int i = 0; i < rootSerializedFields.Length; i++)
            {
                complexClass.Properties[i] = GetPropertyGraph(rootSerializedFields[i], value);
            }

            return complexClass;
        }

        private static Variant TryGetSimplePropertyData(object obj, SerializedType type)
        {
            if (type.IsSimple())
            {
                if (type == SerializedType.String && obj == null)
                {
                    return string.Empty;
                }

                return (Variant)obj;
            }

            return default;
        }

        private static string GetInternalType(Type type)
        {
            if (type.IsAssignableTo(typeof(Delegate)))
            {
                return "DelegateForwarder";// TODO: point to the real delegate forwarder type.
            }

            return ReflectionUtils.GetFullTypeName(type);
        }

        private static Guid GetTypeId(Type type)
        {
            return ReflectionUtils.GetStableGuid(type);
        }

        private static SerializedType GetSimpleType(Type type)
        {
            if (type == null)
                return SerializedType.None;

            if (type.IsEnum)
                return SerializedType.Enum;

            if (_simpleTypeMap.TryGetValue(type, out var serializedType))
            {
                return serializedType;
            }

            throw new NotImplementedException($"Type for '{type.Name}' is not handled by the binary serializer.");
        }

        private static readonly Dictionary<Type, SerializedType> _simpleTypeMap = new()
        {
            // Basic Primitives
            { typeof(char),    SerializedType.Char },
            { typeof(string),  SerializedType.String },
            { typeof(bool),    SerializedType.Bool },
            { typeof(byte),    SerializedType.Byte },
            { typeof(short),   SerializedType.Short },
            { typeof(ushort),  SerializedType.UShort },
            { typeof(int),     SerializedType.Int },
            { typeof(uint),    SerializedType.UInt },
            { typeof(long),    SerializedType.Long },
            { typeof(ulong),   SerializedType.ULong },
            { typeof(float),   SerializedType.Float },
            { typeof(double),  SerializedType.Double },

            { typeof(vec2),    SerializedType.Vec2 },
            { typeof(vec3),    SerializedType.Vec3 },
            { typeof(vec4),    SerializedType.Vec4 },
            { typeof(ivec2),   SerializedType.IVec2 },
            { typeof(ivec3),   SerializedType.IVec3 },
            { typeof(ivec4),   SerializedType.IVec4 },
            { typeof(quat),    SerializedType.Quat },
            { typeof(mat2),    SerializedType.Mat2 },
            { typeof(mat3),    SerializedType.Mat3 },
            { typeof(mat4),    SerializedType.Mat4 },

            { typeof(Color),   SerializedType.Color },
            { typeof(Color32), SerializedType.Color32 }
        };
    }
}
