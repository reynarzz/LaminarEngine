using Engine;
using Engine.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    // Note: This is a very naive serializaton, renaming types, changing to another assembly will cause references to break.
    //       But for this development stage, this is fine, and will not be difficult to change later.
    internal static class SceneEditorSerializer
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

                properties.Add(new ComponentSerializedProperty()
                {
                    Name = member.Name,
                    Type = GetSerializedType(member),
                    Data = GetPropertyData(member, value)
                });
            }

            return properties;
        }

        public static SerializableType GetSerializedType(MemberInfo member)
        {
            var type = ReflectionUtils.GetMemberType(member);

            if (type.IsAssignableTo(typeof(IObject)))
            {
                if (type.IsAssignableTo(typeof(IComponent)))
                {
                    return SerializableType.Component;
                }
                else if (type.IsAssignableTo(typeof(Actor)))
                {
                    return SerializableType.Actor;
                }
                else if (type.IsAssignableTo(typeof(AssetResourceBase)))
                {
                    if (type.IsAssignableTo(typeof(Texture)))
                    {
                        return SerializableType.TextureAsset;
                    }
                    else if (type.IsAssignableTo(typeof(AudioClip)))
                    {
                        return SerializableType.AudioClipAsset;
                    }
                    else if (type.IsAssignableTo(typeof(AnimationClip)))
                    {
                        return SerializableType.AnimationAsset;
                    }
                }
                else
                {
                    return SerializableType.EObject;
                }
            }
            else if (ReflectionUtils.IsCollection(type))
            {
                return SerializableType.Collection;
            }
            else if (ReflectionUtils.IsInternalValueType(member) || type == typeof(string))
            {
                return SerializableType.Simple;
            }
            else if (type.IsClass) // Serializable class
            {
                return SerializableType.Class;
            }
            return SerializableType.None;
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
            else if (ReflectionUtils.IsCollection(type))
            {

            }
            else if (ReflectionUtils.IsInternalValueType(member) || type == typeof(string))
            {
                return value;
            }
            else if (type.IsClass)
            {

            }

            return null;
        }
    }
}
