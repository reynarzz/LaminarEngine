using Engine;
using Engine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    // Note: This is a very naive serializaton, renaming types, changing to another assembly will cause references to break.
    //       But for this development stage, this is fine, and will not be difficult to change later.
    internal static class SceneSerializer
    {
        internal static List<ActorDataSceneAsset> SerializeScene(Scene scene)
        {
            int actorIndex = 0;
            var actors = new List<ActorDataSceneAsset>();

            void SerializeActors(Actor actor, int parentIndex)
            {
                if (!actor)
                    return;

                actors.Add(GetActor(actor, actorIndex, parentIndex));
                parentIndex = actorIndex;
                actorIndex++;

                foreach (var child in actor.Transform.Children)
                {
                    SerializeActors(child.Actor, parentIndex);
                }
            }

            foreach (var actor in scene.RootActors)
            {
                SerializeActors(actor, 0);
            }

            return actors;
        }

        internal static ActorDataSceneAsset GetActor(Actor actor, int index, int parentIndex)
        {
            return new ActorDataSceneAsset()
            {
                Name = actor.Name,
                IsActiveSelf = actor.IsActiveSelf,
                Layer = actor.Layer,
                ID = actor.GetID(),
                ParentID = actor.Transform?.Parent?.Actor?.GetID() ?? Guid.Empty,
                Components = GetAllComponentsData(actor),
            };
        }

        internal static List<ComponentDataSceneAsset> GetAllComponentsData(Actor actor)
        {
            var componentsData = new List<ComponentDataSceneAsset>();

            for (int i = 0; i < actor.Components.Count; i++)
            {
                componentsData.Add(GetComponentData(actor.Components[i]));
            }
            return componentsData;
        }

        internal static ComponentDataSceneAsset GetComponentData(Component component)
        {
            return new ComponentDataSceneAsset()
            {
                ID = component.GetID(),
                IsEnabled = component.IsEnabled,
                TypeName = GetTypeName(component.GetType()),
                SerializedProperties = GetSerializedProperties(component)
            };
        }

        public static List<SerializedPropertyData> GetSerializedProperties(object obj)
        {
            var serializedMembers = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(obj.GetType());
            var properties = new List<SerializedPropertyData>();

            foreach (var member in serializedMembers)
            {
                var value = ReflectionUtils.GetMemberValue(obj, member);
                var memberType = ReflectionUtils.GetMemberType(member);
                var serializedType = GetSerializedType(memberType);
                var valueType = value?.GetType() ?? memberType;


                properties.Add(new SerializedPropertyData()
                {
                    Name = member.Name,
                    InternalType = GetTypeName(valueType),
                    Assembly = valueType.Assembly.GetName().Name,
                    Type = serializedType,
                    Data = GetPropertyData(member, serializedType, value)
                });
            }

            return properties;
        }

        private static string GetTypeName(Type type)
        {
            return $"{type.Namespace}.{type.Name}";
        }
        public static SerializedType GetSerializedType(Type type)
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

                    return SerializedType.Asset;
                }
                else
                {
                    return SerializedType.EObject;
                }
            }
            else if (ReflectionUtils.IsCollection(type, out var collectionType))
            {
                if (ReflectionUtils.IsCollectionOfInternalTypes(type) ||
                    !ReflectionUtils.HasAnySerializedMemberWithType(type, typeof(IObject)))
                {
                    return SerializedType.SimpleCollection;
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
                if (ReflectionUtils.HasAnySerializedMemberWithType(type, typeof(IObject)))
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
        private static bool IsPureReferenceDictionary(Type[] genericArgs)
        {
            if (genericArgs.Length != 2)
            {
                Debug.Error("Invalid number of generic arguments.");
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

        // This only returns reference ids and complex property data, simple data should be taken care of elsewhere.
        public static object GetPropertyData(MemberInfo member, SerializedType serializedMemberType, object value)
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

            if (type.IsAssignableTo(typeof(IObject)))
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

                        if (serializedMemberType == SerializedType.ReferenceCollection)
                        {
                            referenced.Collection.Add(new DictionaryData<object, object>()
                            {
                                Type = serializedType,
                                Key = k,
                                Value = v,
                                keyType = dKey != null ? GetSerializedType(dKey?.GetType()) : GetSerializedType(elementsType[0]),
                                ValueType = dValue != null ? GetSerializedType(dValue?.GetType()) : GetSerializedType(elementsType[1])
                            });
                        }
                        else
                        {
                            var complexKey = CreateComplexType(k?.GetType(), k, serializedMemberType);
                            var complexValue = CreateComplexType(v?.GetType(), v, serializedMemberType);
                            referenced.Collection.Add(new ComplexDictionaryData<ComplexTypeData, ComplexTypeData>()
                            {
                                Type = serializedType,
                                Key = complexKey,
                                Value = complexValue,
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

        public static ComplexTypeData CreateComplexType(Type complexType, object value, SerializedType serializedType)
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
                    InternalType = GetTypeName(valueType),
                    Assembly = valueType.Assembly.GetName().Name,
                    Data = GetPropertyData(currentType, serializedType, value),
                };
            }

            var complexClass = new ComplexTypeData()
            {
                ComplexType = GetSerializedType(complexType),
                TargetTypeName = GetTypeName(complexType),
                Assembly = complexType.Assembly.GetName().Name,
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