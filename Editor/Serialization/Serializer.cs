using System;
using Engine;
using Engine.Utils;
using System.Collections;
using System.Reflection;
using Editor.Utils;
using GlmNet;
using Engine.Serialization;
using System.Xml.Schema;

namespace Editor.Serialization
{
    internal static class Serializer
    {
        /// <summary>
        /// Gets the IR of all the properties marked with the 'SerializedField' attribute.
        /// </summary>

        private struct CollectionWrapper
        {
            [SerializedField] public object Collection;
        }
        internal static List<SerializedPropertyIR> Serialize(object target)
        {
            if (ReflectionUtils.IsCollection(target.GetType()))
            {
                target = new CollectionWrapper() { Collection = target };
            }
            var serializedMembers = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(target.GetType());
            var properties = new List<SerializedPropertyIR>();

            foreach (var member in serializedMembers)
            {
                var value = ReflectionUtils.GetMemberValue(target, member);
                var valueType = value?.GetType() ?? ReflectionUtils.GetMemberType(member);
                var serializedType = GetSerializedType(valueType, value);

                var propData = GetPropertyData(member, serializedType, value);
                properties.Add(new SerializedPropertyIR()
                {
                    Name = member.Name,
                    InternalType = GetInternalType(valueType),
                    TypeId = GetTypeId(valueType),
                    Type = serializedType,
                    Simple = TryGetSimplePropertyData(propData, serializedType),
                    Reference = TryGetReferencePropertyData(propData, serializedType),
                    ComplexClass = TryGetComplexClassPropertyData(propData, serializedType),
                    Collection = TryGetCollectionPropertyData(propData, serializedType)
                });
            }

            return properties;
        }

        internal static SerializedType GetSerializedType(Type type, object value)
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
        internal static object GetPropertyData(MemberInfo member, SerializedType serializedMemberType, object value)
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

            var type = value?.GetType() ?? ReflectionUtils.GetMemberType(member);

            if (type.IsAssignableTo(typeof(Delegate)))
            {
                // TODO: handle delegates.
                Debug.Warn($"TODO: Can't serialize delegate: {type.Name}");

                if (value != null)
                {
                    var delegateObj = value as Delegate;
                    var data = new DelegateData();

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
                    collectionPropData = GetDictionaryData(type, serializedMemberType, value);
                }
                else
                {
                    collectionPropData = Get1DCollectionData(serializedMemberType, value, out var itemsType);
                }

                if (collectionPropData != null)
                {
                    collectionPropData.CollectionType = collectionType;
                }
                return collectionPropData;
            }
            else if (serializedMemberType == SerializedType.ComplexClass)
            {
                return CreateComplexType(type, value);
            }
            return null;
        }

        private static CollectionData Get1DCollectionData(SerializedType serializedMemberType, object value, out SerializedType itemsType)
        {
            itemsType = SerializedType.None;
            var collection = (ICollection)value;

            if (serializedMemberType == SerializedType.SimpleCollection)
            {
                var valueCollection = new VariantIRValue[collection.Count];
                int index = 0;
                foreach (var item in collection)
                {
                    if (itemsType == SerializedType.None)
                    {
                        itemsType = GetSimpleType(item?.GetType());
                    }
                    valueCollection[index++] = GetVariantValue(item, itemsType);
                }

                return new CollectionIRVariants(valueCollection, itemsType);
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

                return new CollectionIRReferences(references, referencesTypes);
            }
            else if (serializedMemberType == SerializedType.ComplexCollection)
            {
                var complexTypeArr = new ComplexClassData[collection.Count];
                int index = 0;

                foreach (var item in collection)
                {
                    complexTypeArr[index++] = CreateComplexType(item?.GetType(), item);
                }
                return new CollectionIRComplexTypes(complexTypeArr);
            }

            throw new NotSupportedException($"Collection is not supported '{serializedMemberType}', maybe is an error, or is not implemented?");
        }

        private static CollectionData GetDictionaryData(Type type, SerializedType serializedMemberType, object value)
        {
            var defaultElementsType = ReflectionUtils.GetCollectionElementsType(type);

            var dictionary = (IDictionary)value;

            CollectionData dictionaryCollection = null;

            if (serializedMemberType == SerializedType.SimpleCollection)
            {
                dictionaryCollection = new DictionaryIRVariants(dictionary.Keys.Count);
            }
            else if (serializedMemberType == SerializedType.ReferenceCollection)
            {
                dictionaryCollection = new DictionaryIRReferences(dictionary.Keys.Count);
            }
            else if (serializedMemberType == SerializedType.ComplexCollection)
            {
                dictionaryCollection = new DictionaryIRComplexTypes(dictionary.Keys.Count);
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


            void PopulateDictionaryCollection(ICollection argCollection, Type argDefaultType, VariantIRValue[] simpleArg,
                                               object[] referenceArg, ComplexClassData[] complexArg, SerializedType[] refArgTypes,
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

            var simpleDictionary = dictionaryCollection as DictionaryIRVariants;
            var referenceDictionary = dictionaryCollection as DictionaryIRReferences;
            var complexDictionary = dictionaryCollection as DictionaryIRComplexTypes;


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

        private static ComplexClassData GetComplexTypeData(SerializedType argSerializedType, object argValue, string argName)
        {
            if (argSerializedType.IsSimple())
            {
                var internalType = GetInternalType(argValue?.GetType());
                var typeId = GetTypeId(argValue?.GetType());
                return new ComplexClassData()
                {
                    ComplexType = argSerializedType,
                    InternalType = internalType,
                    TypeId = typeId,
                    Properties = new List<SerializedPropertyIR>()
                    {
                        new SerializedPropertyIR()
                        {
                           Simple = GetVariantValue(argValue, argSerializedType),
                           InternalType = internalType,
                           TypeId = typeId,
                           Type = argSerializedType,
                           Name = argName
                        }
                    }
                };
            }

            return CreateComplexType(argValue?.GetType(), argValue);
        }

        private static VariantIRValue GetVariantValue(object val, SerializedType type)
        {
            switch (type)
            {
                case SerializedType.Enum:
                    return VariantIRValue.FromEnum((Enum)val);
                case SerializedType.Char:
                    return VariantIRValue.FromChar((char)val);
                case SerializedType.String:
                    return VariantIRValue.FromString(val != null ? (string)val : string.Empty);
                case SerializedType.Bool:
                    return VariantIRValue.FromBool((bool)val);
                case SerializedType.Byte:
                    return VariantIRValue.FromByte((byte)val);
                case SerializedType.Short:
                    return VariantIRValue.FromShort((short)val);
                case SerializedType.UShort:
                    return VariantIRValue.FromUShort((ushort)val);
                case SerializedType.Int:
                    return VariantIRValue.FromInt((int)val);
                case SerializedType.UInt:
                    return VariantIRValue.FromUInt((uint)val);
                case SerializedType.Float:
                    return VariantIRValue.FromFloat((float)val);
                case SerializedType.Double:
                    return VariantIRValue.FromDouble((double)val);
                case SerializedType.Long:
                    return VariantIRValue.FromLong((long)val);
                case SerializedType.ULong:
                    return VariantIRValue.FromULong((ulong)val);
                case SerializedType.Vec2:
                    return VariantIRValue.FromVec2((vec2)val);
                case SerializedType.Vec3:
                    return VariantIRValue.FromVec3((vec3)val);
                case SerializedType.Vec4:
                    return VariantIRValue.FromVec4((vec4)val);
                case SerializedType.IVec2:
                    return VariantIRValue.FromIVec2((ivec2)val);
                case SerializedType.IVec3:
                    return VariantIRValue.FromIVec3((ivec3)val);
                case SerializedType.IVec4:
                    return VariantIRValue.FromIVec4((ivec4)val);
                case SerializedType.Quat:
                    return VariantIRValue.FromQuat((quat)val);
                case SerializedType.Mat2:
                    return VariantIRValue.FromMat2((mat2)val);
                case SerializedType.Mat3:
                    return VariantIRValue.FromMat3((mat3)val);
                case SerializedType.Mat4:
                    return VariantIRValue.FromMat4((mat4)val);
                case SerializedType.Color:
                    return VariantIRValue.FromColor((Color)val);
                case SerializedType.Color32:
                    return VariantIRValue.FromColor32((Color32)val);
                default:
                    throw new ArgumentException($"Probably not a simple type: {type}");
            }
        }

        private static ReferenceData GetReferenceData(Guid id, SerializedType serializedMemberType, object value)
        {
            if (serializedMemberType == SerializedType.SpriteAsset)
            {
                return new SpriteReferenceData()
                {
                    Type = serializedMemberType,
                    Id = id,
                    AtlasIndex = (value as Sprite).AtlasIndex,
                    TextureId = (value as Sprite).Texture.GetID()
                };
            }
            else if (serializedMemberType.IsEObject())
            {
                return new ReferenceData()
                {
                    Type = serializedMemberType,
                    Id = id
                };
            }

            Debug.EngineError($"Reference type '{serializedMemberType}' not supported");

            return new ReferenceData()
            {
                Type = serializedMemberType,
                Id = id
            };
        }
        internal static ComplexClassData CreateComplexType(Type complexType, object value)
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

                var data = GetPropertyData(currentType, serializedType, value);
                return new SerializedPropertyIR()
                {
                    Name = currentType.Name,
                    Type = serializedType,
                    InternalType = GetInternalType(valueType),
                    TypeId = GetTypeId(valueType),
                    Simple = TryGetSimplePropertyData(data, serializedType),
                    Reference = TryGetReferencePropertyData(data, serializedType),
                    ComplexClass = TryGetComplexClassPropertyData(data, serializedType),
                    Collection = TryGetCollectionPropertyData(data, serializedType),
                };
            }

            var complexClass = new ComplexClassData()
            {
                ComplexType = GetSerializedType(complexType, value),
                InternalType = GetInternalType(complexType),
                TypeId = GetTypeId(complexType),
                Properties = new List<SerializedPropertyIR>()
            };

            var rootSerializedFields = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(complexType);

            foreach (var field in rootSerializedFields)
            {
                complexClass.Properties.Add(GetPropertyGraph(field, value));
            }

            return complexClass;
        }

        private static VariantIRValue TryGetSimplePropertyData(object obj, SerializedType type)
        {
            if (type.IsSimple())
            {
                if(type == SerializedType.String && obj == null)
                {
                    return VariantIRValue.FromString(string.Empty);
                }

                return (VariantIRValue)obj;
            }

            return default;
        }

        private static ReferenceData TryGetReferencePropertyData(object obj, SerializedType type)
        {
            if (type.IsEObject())
            {
                return obj as ReferenceData;
            }

            return null;
        }
        private static ComplexClassData TryGetComplexClassPropertyData(object obj, SerializedType type)
        {
            if (type.IsComplexClass())
            {
                return obj as ComplexClassData;
            }

            return null;
        }

        private static CollectionData TryGetCollectionPropertyData(object obj, SerializedType type)
        {
            if (type.IsCollection())
            {
                return obj as CollectionData;
            }

            return null;
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
