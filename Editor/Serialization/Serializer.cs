using System;
using Engine;
using Engine.Utils;
using System.Collections;
using System.Reflection;
using Microsoft.VisualBasic.FileIO;

namespace Editor.Serialization
{
    internal static class Serializer
    {
        /// <summary>
        /// Gets the IR of all the properties marked with the 'SerializedField' attribute.
        /// </summary>
        internal static List<SerializedPropertyData> Serialize(object target)
        {
            var serializedMembers = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(target.GetType());
            var properties = new List<SerializedPropertyData>();

            foreach (var member in serializedMembers)
            {
                var value = ReflectionUtils.GetMemberValue(target, member);
                var memberType = ReflectionUtils.GetMemberType(member);
                var valueType = value?.GetType() ?? memberType;
                var serializedType = GetSerializedType(valueType, value);


                properties.Add(new SerializedPropertyData()
                {
                    Name = member.Name,
                    InternalType = GetInternalType(valueType),
                    Type = serializedType,
                    Data = GetPropertyData(member, serializedType, value)
                });
            }

            return properties;
        }

        internal static SerializedType GetSerializedType(Type type, object value)
        {
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

                    return SerializedType.Asset;
                }
                else
                {
                    if (type.IsAssignableTo(typeof(Sprite)))
                    {
                        return SerializedType.SpriteAsset;
                    }
                    return SerializedType.EObject;
                }
            }
            else if (ReflectionUtils.IsCollection(type, out var collectionType))
            {
                // TODO: get all the elements in the collection so they can be checked in case there is a reference.
                var elementsTypes = ReflectionUtils.GetCollectionElementsType(type);

                var isSingleArgCollectionAEObject = elementsTypes.Length == 1 && elementsTypes.Any(x => x.IsAssignableTo(typeof(IObject)));

                if (isSingleArgCollectionAEObject ||
                    (collectionType == ReflectionUtils.CollectionType.Dictionary && IsPureReferenceDictionary(elementsTypes, value)))
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
                return SerializedType.Simple;
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

            var nameTest = member.Name;

            if (serializedMemberType == SerializedType.Simple)
            {
                return value;
            }

            var type = ReflectionUtils.GetMemberType(member);

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
                var elementsType = ReflectionUtils.GetCollectionElementsType(type);
                var referenced = new CollectionPropertyData()
                {
                    CollectionType = collectionType
                };

                if (collectionType == ReflectionUtils.CollectionType.Dictionary)
                {
                    var isKeyEObject = elementsType[0].IsAssignableTo(typeof(IObject));
                    var isValueEObject = elementsType[1].IsAssignableTo(typeof(IObject));

                    var dictionary = (IDictionary)value;

                    foreach (var dKey in dictionary.Keys)
                    {
                        var dValue = dictionary[dKey];

                        var keySerializedType = dKey != null ? GetSerializedType(dKey?.GetType(), null) : GetSerializedType(elementsType[0], null);
                        var ValueSerializedType = dValue != null ? GetSerializedType(dValue?.GetType(), null) : GetSerializedType(elementsType[1], null);

                        var k = isKeyEObject ? GetReferenceData((dKey as IObject)?.GetID() ?? Guid.Empty, keySerializedType, dKey) : dKey;
                        var v = isValueEObject ? GetReferenceData((dValue as IObject)?.GetID() ?? Guid.Empty, ValueSerializedType, dValue) : dValue;
                        var referenceType = isKeyEObject ? dKey?.GetType() : dValue?.GetType();
                        var serializedType = GetSerializedType(referenceType, null);

                        if (serializedMemberType == SerializedType.ReferenceCollection)
                        {
                            referenced.Collection.Add(new DictionaryData<object, object>()
                            {
                                Type = serializedType,
                                Key = k,
                                Value = v,
                                keyType = keySerializedType,
                                ValueType = ValueSerializedType
                            });
                        }
                        else
                        {
                            ComplexTypeData GetComplexTypeData(SerializedType argSerializedType, object argValue, string argName)
                            {
                                if (argSerializedType == SerializedType.Simple)
                                {
                                    var internalType = GetInternalType(argValue?.GetType());
                                    return new ComplexTypeData()
                                    {
                                        ComplexType = argSerializedType,
                                        TargetTypeName = internalType,
                                        Properties = new List<SerializedPropertyData>()
                                        {
                                            new SerializedPropertyData()
                                            {
                                               Data = argValue,
                                               InternalType = internalType,
                                               Type = argSerializedType,
                                               Name = argName
                                            }
                                        }
                                    };
                                }

                                return CreateComplexType(argValue?.GetType(), argValue, serializedMemberType);
                            }

                            referenced.Collection.Add(new ComplexDictionaryData<ComplexTypeData, ComplexTypeData>()
                            {
                                Type = serializedType,
                                Key = GetComplexTypeData(keySerializedType, k, "DictionaryKey"),
                                Value = GetComplexTypeData(ValueSerializedType, v, "DictionaryValue"),
                            });
                        }
                    }

                    return referenced;
                }
                else
                {
                    var collection = (ICollection)value;

                    foreach (var item in collection)
                    {
                        if (serializedMemberType == SerializedType.ReferenceCollection)
                        {
                            var itemSerializedType = GetSerializedType(item?.GetType(), null);
                            referenced.Collection.Add(new CollectionData<ReferenceData>()
                            {
                                Type = itemSerializedType,
                                Value = GetReferenceData((item as IObject)?.GetID() ?? Guid.Empty, itemSerializedType, item),
                            });
                        }
                        else
                        {
                            var complex = CreateComplexType(item?.GetType(), item, serializedMemberType);

                            referenced.Collection.Add(new CollectionData<ComplexTypeData>()
                            {
                                Type = GetSerializedType(item?.GetType(), null),
                                Value = complex
                            });
                        }
                    }

                    return referenced;
                }
            }
            else if (serializedMemberType == SerializedType.ComplexClass)
            {
                return CreateComplexType(type, value, serializedMemberType);
            }
            return null;
        }

        private static ReferenceData GetReferenceData(Guid id, SerializedType serializedMemberType, object value)
        {
            switch (serializedMemberType)
            {
                case SerializedType.None:
                    break;
                case SerializedType.EObject:
                case SerializedType.Component:
                case SerializedType.Actor:
                case SerializedType.Asset:
                case SerializedType.TextureAsset:
                case SerializedType.RenderTextureAsset:
                case SerializedType.AudioClipAsset:
                case SerializedType.ShaderAsset:
                case SerializedType.MaterialAsset:
                case SerializedType.AnimationAsset:
                case SerializedType.AnimatorControllerAsset:
                case SerializedType.ScriptableObject:
                    return new ReferenceData() { Id = id };
                case SerializedType.SpriteAsset:
                    return new SpriteReferenceData()
                    {
                        Id = id,
                        AtlasIndex = (value as Sprite).AtlasIndex,
                        TextureId = (value as Sprite).Texture.GetID()
                    };
                default:
                    break;
            }

            return new ReferenceData() { Id = id };
        }
        internal static ComplexTypeData CreateComplexType(Type complexType, object value, SerializedType serializedType)
        {
            if (complexType == null)
                return null;

            // Get the concreate type, this makes sure to get the actual type even for inherit types.
            complexType = value?.GetType() ?? complexType;

            SerializedPropertyData GetPropertyGraph(MemberInfo currentType, object target)
            {
                var currentMemberType = ReflectionUtils.GetMemberType(currentType);
                var serializedType = GetSerializedType(currentMemberType, null);
                var value = ReflectionUtils.GetMemberValue(target, currentType);

                var valueType = value?.GetType() ?? currentMemberType;

                return new SerializedPropertyData()
                {
                    Name = currentType.Name,
                    Type = serializedType,
                    InternalType = GetInternalType(valueType),
                    Data = GetPropertyData(currentType, serializedType, value),
                };
            }

            var complexClass = new ComplexTypeData()
            {
                ComplexType = GetSerializedType(complexType, null),
                TargetTypeName = GetInternalType(complexType),
                Properties = new List<SerializedPropertyData>()
            };

            var rootSerializedFields = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(complexType);

            foreach (var field in rootSerializedFields)
            {
                complexClass.Properties.Add(GetPropertyGraph(field, value));
            }

            return complexClass;
        }

        private static string GetInternalType(Type type)
        {
            if (type.IsAssignableTo(typeof(Delegate)))
            {
                return "DelegateForwarder";// TODO: point to the real delegate forwarder type.
            }

            return ReflectionUtils.GetFullTypeName(type);
        }
    }
}
