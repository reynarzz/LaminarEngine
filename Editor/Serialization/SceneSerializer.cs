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
                componentsData.Add(GetComponentData(actor.Components[i], i));
            }
            return componentsData;
        }

        internal static ComponentDataSceneAsset GetComponentData(Component component, int index)
        {
            return new ComponentDataSceneAsset()
            {
                ID = component.GetID(),
                IsEnabled = component.IsEnabled,
                TypeName = component.GetType().FullName,
                ComponentIndex = index,
                SerializedProperties = GetSerializedProperties(component)
            };
        }

        public static List<ComponentSerializedProperty> GetSerializedProperties(object obj)
        {
            var serializedMembers = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(obj.GetType());
            var properties = new List<ComponentSerializedProperty>();

            foreach (var member in serializedMembers)
            {
                var value = ReflectionUtils.GetMemberValue(obj, member);
                var serializedType = GetSerializedType(ReflectionUtils.GetMemberType(member));

                object data = value;
                // TODO: use enum flags
                if (serializedType != SerializedType.Simple &&
                    serializedType != SerializedType.SimpleClass &&
                    serializedType != SerializedType.SimpleCollection)
                {
                    data = GetPropertyData(member, value);
                }

                properties.Add(new ComponentSerializedProperty()
                {
                    Name = member.Name,
                    Type = serializedType,
                    Data = data
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
                if (elementsTypes.Length == 1 && elementsTypes[0].IsAssignableTo(typeof(IObject)))
                {
                    return SerializedType.ReferenceCollection;
                }
                // TODO: maybe this is error prone?----
                if (collectionType == ReflectionUtils.CollectionType.Dictionary &&
                    elementsTypes[1].IsAssignableTo(typeof(IObject)))
                {
                    return SerializedType.ReferenceCollection;
                }
                //------------

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

        public static object GetPropertyData(MemberInfo member, object value)
        {
            // Note: For runtime-created resource assets such as Materials, Shaders, Textures etc... maybe should have a empty guid, so the serializer,
            //       does not point to a invalid physical asset.

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
                if (value == null)
                {
                    return null;
                }

                var collection = (ICollection)value;
                var elementsType = ReflectionUtils.GetCollectionElementsType(type);

                if (collectionType == ReflectionUtils.CollectionType.Dictionary)
                {
                    // TODO:
                }
                else
                {
                    var elementType = elementsType[0];
                    if (elementType.IsAssignableTo(typeof(IObject)))
                    {
                        var referenced = new List<SerializedItem<Guid>>();
                        foreach (var item in collection)
                        {
                            referenced.Add(new SerializedItem<Guid>()
                            {
                                Type = GetSerializedType(item?.GetType() ?? null),
                                Value = (item as IObject)?.GetID() ?? Guid.Empty
                            });
                        }
                        return referenced;
                    }
                    else
                    {
                        // TODO: Complex class.
                        // Maybe I should
                        Debug.Log("TODO: Complex class: " + type.FullName);
                    }
                    return null;
                }
            }

            return null;
        }
    }
}