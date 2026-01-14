using System;
using Engine;
using Engine.Utils;
using System.Collections;
using System.Reflection;

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
                var serializedType = GetSerializedType(memberType);
                var valueType = value?.GetType() ?? memberType;


                properties.Add(new SerializedPropertyData()
                {
                    Name = member.Name,
                    InternalType = ReflectionUtils.GetFullTypeName(valueType),
                    Type = serializedType,
                    Data = GetPropertyData(member, serializedType, value)
                });
            }

            return properties;
        }

        internal static SerializedType GetSerializedType(Type type)
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
                    return SerializedType.EObject;
                }
            }
            else if (ReflectionUtils.IsCollection(type, out var collectionType))
            {
                // NOTE: ugly 'if/else' to avoid unnecessary computations.
                if (ReflectionUtils.IsCollectionOfInternalTypes(type) ||
                   !ReflectionUtils.HasAnySerializedMemberWithType(type, typeof(IObject)))
                {
                    if (!ReflectionUtils.HasAnySerializedMemberWithType(type, typeof(Delegate), false))
                    {
                        return SerializedType.SimpleCollection;
                    }
                    else
                    {
                        return SerializedType.ComplexCollection;
                    }
                }
                else if (ReflectionUtils.HasAnySerializedMemberWithType(type, typeof(Delegate), false))
                {
                    return SerializedType.ComplexCollection;
                }

                var elementsTypes = ReflectionUtils.GetCollectionElementsType(type);
                var isSingleArgCollectionAEObject = elementsTypes.Length == 1 && elementsTypes[0].IsAssignableTo(typeof(IObject));

                if (isSingleArgCollectionAEObject ||
                    (collectionType == ReflectionUtils.CollectionType.Dictionary && IsPureReferenceDictionary(elementsTypes)))
                {
                    return SerializedType.ReferenceCollection;
                }

                return SerializedType.ComplexCollection;
            }
            else if (ReflectionUtils.IsInternalType(type))
            {
                return SerializedType.Simple;
            }
            else if (type.IsClass || ReflectionUtils.IsUserDefinedStruct(type))
            {
                if (ReflectionUtils.HasAnySerializedMemberWithType(type, typeof(IObject)) ||
                    ReflectionUtils.HasAnySerializedMemberWithType(type, typeof(Delegate), false))
                {
                    return SerializedType.ComplexClass;
                }

                return SerializedType.SimpleClass;
            }
            return SerializedType.None;
        }

        /// <summary>
        /// Checks if the dictionary has not IObject deep in the graph, but only on the key or value generic args.
        /// </summary>
        internal static bool IsPureReferenceDictionary(Type[] genericArgs)
        {
            if (genericArgs == null || genericArgs.Length != 2)
            {
                Debug.Error("Invalid generic arguments.");
                return false;
            }

            void Check(Type argType, out bool hasAny, out bool isTopLevel)
            {
                if (argType.IsAssignableTo(typeof(IObject)))
                {
                    hasAny = true;
                    isTopLevel = true;
                    return;
                }

                hasAny = ReflectionUtils.HasAnySerializedMemberWithType(argType, typeof(IObject));
                isTopLevel = false;
            }

            Check(genericArgs[0], out bool aHasAny, out bool aTop);
            Check(genericArgs[1], out bool bHasAny, out bool bTop);

            return (aTop && (!bHasAny || bTop)) || (bTop && (!aHasAny || aTop));
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

            if (serializedMemberType == SerializedType.Simple ||
                serializedMemberType == SerializedType.SimpleClass ||
                serializedMemberType == SerializedType.SimpleCollection)
            {
                return value;
            }

            var type = ReflectionUtils.GetMemberType(member);

            if (type.IsAssignableTo(typeof(Delegate)))
            {
                // TODO: handle delegates.
                Debug.Warn($"TODO: Can't serialize delegate: {type.Name}");
                return null;
            }
            else if (type.IsAssignableTo(typeof(IObject)))
            {
                if (value != null)
                {
                    return (value as IObject).GetID();
                }
                return Guid.Empty;
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

                        var k = isKeyEObject ? (dKey as IObject)?.GetID() ?? Guid.Empty : dKey;
                        var v = isValueEObject ? (dValue as IObject)?.GetID() ?? Guid.Empty : dValue;
                        var referenceType = isKeyEObject ? dKey?.GetType() : dValue?.GetType();
                        var serializedType = GetSerializedType(referenceType);
                        var keySerializedType = dKey != null ? GetSerializedType(dKey?.GetType()) : GetSerializedType(elementsType[0]);
                        var ValueSerializedType = dValue != null ? GetSerializedType(dValue?.GetType()) : GetSerializedType(elementsType[1]);

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
                                if (argSerializedType == SerializedType.Simple ||
                                    argSerializedType == SerializedType.SimpleClass ||
                                    argSerializedType == SerializedType.SimpleCollection)
                                {
                                    var internalType = ReflectionUtils.GetFullTypeName(argValue?.GetType());
                                    return new ComplexTypeData()
                                    {
                                        ComplexType = argSerializedType,
                                        TargetTypeName = internalType,
                                        Properties = new List<SerializedPropertyData>()
                                        {
                                            new SerializedPropertyData()
                                            {
                                               Data = argValue,
                                               InternalType =internalType,
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
                            referenced.Collection.Add(new CollectionData<Guid>()
                            {
                                Type = GetSerializedType(item?.GetType()),
                                Value = (item as IObject)?.GetID() ?? Guid.Empty
                            });
                        }
                        else
                        {
                            var complex = CreateComplexType(item?.GetType(), item, serializedMemberType);

                            referenced.Collection.Add(new CollectionData<ComplexTypeData>()
                            {
                                Type = GetSerializedType(item?.GetType()),
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

        internal static ComplexTypeData CreateComplexType(Type complexType, object value, SerializedType serializedType)
        {
            if (complexType == null)
                return null;

            // Get the concreate type, this makes sure to get the actual type even for inherit types.
            complexType = value?.GetType() ?? complexType;

            SerializedPropertyData GetPropertyGraph(MemberInfo currentType, object target)
            {
                var currentMemberType = ReflectionUtils.GetMemberType(currentType);
                var serializedType = GetSerializedType(currentMemberType);
                var value = ReflectionUtils.GetMemberValue(target, currentType);

                var valueType = value?.GetType() ?? currentMemberType;

                return new SerializedPropertyData()
                {
                    Name = currentType.Name,
                    Type = serializedType,
                    InternalType = ReflectionUtils.GetFullTypeName(valueType),
                    Data = GetPropertyData(currentType, serializedType, value),
                };
            }

            var complexClass = new ComplexTypeData()
            {
                ComplexType = GetSerializedType(complexType),
                TargetTypeName = ReflectionUtils.GetFullTypeName(complexType),
                Properties = new List<SerializedPropertyData>()
            };

            var rootSerializedFields = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(complexType);

            foreach (var field in rootSerializedFields)
            {
                complexClass.Properties.Add(GetPropertyGraph(field, value));
            }

            return complexClass;
        }
    }
}
