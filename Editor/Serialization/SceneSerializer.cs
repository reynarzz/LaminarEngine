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
                TypeName = component.GetType().FullName,
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
                var serializedType = GetSerializedType(ReflectionUtils.GetMemberType(member));

                properties.Add(new SerializedPropertyData()
                {
                    Name = member.Name,
                    Type = serializedType,
                    Data = GetPropertyData(member, serializedType, value)
                });
            }

            return properties;
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

                if (isSingleArgCollectionAEObject)
                {
                    return SerializedType.ReferenceCollection;
                }
                var isPureRefDictionary = collectionType == ReflectionUtils.CollectionType.Dictionary && IsPureReferenceDictionary(elementsTypes);

                if (isPureRefDictionary)
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

                if (collectionType == ReflectionUtils.CollectionType.Dictionary)
                {
                    var referenced = new CollectionPropertyData();
                    var dictionary = (IDictionary)value;

                    if (serializedMemberType == SerializedType.ReferenceCollection)
                    {
                        var isKeyEObject = elementsType[0].IsAssignableTo(typeof(IObject));
                        var isValueEObject = elementsType[1].IsAssignableTo(typeof(IObject));

                        foreach (var dKey in dictionary.Keys)
                        {
                            var dValue = dictionary[dKey];

                            var k = isKeyEObject ? (dKey as IObject)?.GetID() ?? Guid.Empty : dKey;
                            var v = isValueEObject ? (dValue as IObject)?.GetID() ?? Guid.Empty : dValue;
                            var referenceType = isKeyEObject ? dKey?.GetType() : dValue?.GetType();

                            referenced.Collection.Add(new SerializedItem<KeyValuePair<object, object>>()
                            {
                                Type = GetSerializedType(referenceType),
                                Value = new KeyValuePair<object, object>(k, v)
                            });
                        }
                    }
                    else
                    {
                        Debug.Log("TODO: Complex dictionary: " + type.FullName);
                    }

                    return referenced;
                }
                else
                {
                    var referenced = new CollectionPropertyData();
                    var collection = (ICollection)value;

                    if (serializedMemberType == SerializedType.ReferenceCollection)
                    {
                        foreach (var item in collection)
                        {
                            referenced.Collection.Add(new SerializedItem<Guid>()
                            {
                                Type = GetSerializedType(item?.GetType()), // Note: Get the type of the item reference.
                                Value = (item as IObject)?.GetID() ?? Guid.Empty
                            });
                        }
                    }
                    else
                    {
                        // TODO: Complex collection, create complete object graph.
                        // Maybe I should
                        Debug.Log("TODO: Complex collection: " + type.FullName);
                    }
                    return referenced;
                }
            }
            else if (serializedMemberType == SerializedType.ComplexClass)
            {
                // TODO: Complex class, create complete object graph.
                var complex = CreateComplexType(type, value, serializedMemberType);

                return complex;
            }
            return null;
        }

        public static ComplexTypeData CreateComplexType(Type complexType, object value, SerializedType serializedType)
        {
            var complexClass = new ComplexTypeData();
            complexClass.ComplexType = serializedType;
            complexClass.TargetTypeName = complexType.FullName;
            complexClass.Properties = new List<SerializedPropertyData>();


            SerializedPropertyData GetPropertyGraph(MemberInfo currentType, object target)
            {
                var currentMemberType = ReflectionUtils.GetMemberType(currentType);
                var serializedType = GetSerializedType(currentMemberType);
                return new SerializedPropertyData()
                {
                    Name = currentType.Name,
                    Type = serializedType,
                    Data = GetPropertyData(currentType, serializedType, ReflectionUtils.GetMemberValue(target, currentType)),
                };
            }

            var rootSerializedFields = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(complexType);

            foreach (var field in rootSerializedFields)
            {
                complexClass.Properties.Add(GetPropertyGraph(field, value));
            }

            return complexClass;
        }
    }
}