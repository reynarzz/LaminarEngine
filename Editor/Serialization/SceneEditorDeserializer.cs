using Engine;
using Engine.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    internal static class SceneEditorDeserializer
    {
        private readonly static Dictionary<Guid, (Actor value, ActorDataSceneAsset data)> _actorsByID = new();
        private readonly static Dictionary<Guid, (Component value, ComponentDataSceneAsset data)> _componentsByID = new();

        public static void DeserializeScene(IReadOnlyList<ActorDataSceneAsset> actors, WeakReference<Scene> scene)
        {
            _actorsByID.Clear();
            _componentsByID.Clear();

            if (actors == null || actors.Count == 0)
                return;

            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];

                var actor = new Actor(actorData.Name, actorData.ID, scene);
                actor.Layer = actorData.Layer;
                actor.IsActiveSelf = actorData.IsActiveSelf;

                // actor.AddComponent(typeof());
                _actorsByID.Add(actor.GetID(), (actor, actorData));

                // Add components, but no deserialize yet.
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (ReflectionUtils.TryGetTypeFromName(componentData.TypeName, out var componentType))
                    {
                        var component = actor.AddComponent(componentType, componentData.ID, false);
                        component.IsEnabled = componentData.IsEnabled;

                        _componentsByID.Add(componentData.ID, (component, componentData));
                    }
                }
            }

            // Resolve parent-child relationship
            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];

                if (actorData.ParentID != Guid.Empty)
                {
                    _actorsByID[actorData.ID].value.Transform.Parent = _actorsByID[actorData.ParentID].value.Transform;
                }
            }

            // Deserialize components data, and resolve references.
            foreach (var (id, componentValue) in _componentsByID)
            {
                DeserializeComponent(componentValue.value, componentValue.data);
            }
        }

        private static void DeserializeComponent(Component component, ComponentDataSceneAsset data)
        {
            foreach (var property in data.SerializedProperties)
            {
                if (property.Type == SerializableType.Simple ||
                    property.Type == SerializableType.SimpleCollection)
                {
                    DeserializeSimpleProperty(component, property);
                }
                else if (property.Type == SerializableType.Component)
                {
                    DeserializeReferencedProperty(_componentsByID, component, property);
                }
                else if (property.Type == SerializableType.Actor)
                {
                    DeserializeReferencedProperty(_actorsByID, component, property);
                }
                else if (property.Type == SerializableType.ReferenceCollection)
                {
                    DeserializeReferenceCollectionProperty(component, property);
                }
            }
        }

        private static void DeserializeReferencedProperty<V, D>(Dictionary<Guid, (V value, D data)> ids,
                                                                object target, ComponentSerializedProperty property)
        {
            if (property.Data == null)
            {
                Debug.EngineError("Serialization error: property data is null.");
                return;
            }
            var guid = (Guid)property.Data;
            var referenceValue = GetReferenceValue(ids, guid);

            if (referenceValue != null)
            {
                ReflectionUtils.SetMemberValue(target, property.Name, referenceValue);
            }
            else
            {
                Debug.Error($"Could not deserialize value for component: {target.GetType().Name}, Property: {property.Name}");
            }
        }
        private static object GetReferenceValue<V, D>(Dictionary<Guid, (V value, D data)> ids, Guid guid)
        {
            if (ids.TryGetValue(guid, out var data))
            {
                return data.value;
            }

            return null;
        }
        private static void DeserializeSimpleProperty(object target, ComponentSerializedProperty property)
        {
            if (property.Data == null)
            {
                return;
            }

            ReflectionUtils.SetMemberValue(target, property.Name, property.Data);
        }

        private static void DeserializeReferenceCollectionProperty(object target, ComponentSerializedProperty property)
        {
            if (property.Data == null)
            {
                return;
            }

            var collection = property.Data as IEnumerable;

            if (collection == null)
                return;

            var collectionPropertyType = ReflectionUtils.GetMemberType(target.GetType(), property.Name);

            object GetItemReferenceValue(object item)
            {
                var referenceElement = item as SerializedCollectionElement<Guid>;

                if (referenceElement == null)
                    return null;

                return GetReferenceValue(referenceElement.Type, referenceElement.Value);
            }

            object propertyValue = null;
            if (ReflectionUtils.IsCollection(collectionPropertyType, out var collectionType))
            {
                if (collectionType == ReflectionUtils.CollectionType.Dictionary)
                {

                }
                else if (collectionType == ReflectionUtils.CollectionType.List)
                {
                    var list = (IList)ReflectionUtils.GetDefaultValue(collectionPropertyType);

                    foreach (var item in collection)
                    {
                        list.Add(GetItemReferenceValue(item));
                    }
                    propertyValue = list;
                }
                else if (collectionType == ReflectionUtils.CollectionType.Array)
                {
                    int arraySize = 0;
                    foreach (var item in collection)
                    {
                        arraySize++;
                    }

                    var emptyArray = Array.CreateInstance(collectionPropertyType.GetElementType(), arraySize);
                    int index = 0;
                    foreach (var item in collection)
                    {
                        emptyArray.SetValue(GetItemReferenceValue(item), index++);
                    }
                    propertyValue = emptyArray;
                }
            }

            ReflectionUtils.SetMemberValue(target, property.Name, propertyValue);
        }

        private static object GetReferenceValue(SerializableType type, Guid guid)
        {
            switch (type)
            {
                case SerializableType.Component:
                    return GetReferenceValue(_componentsByID, guid);
                case SerializableType.Actor:
                    return GetReferenceValue(_actorsByID, guid);
                //case SerializableType.Asset:
                //    break;
                //case SerializableType.TextureAsset:
                //    break;
                //case SerializableType.RenderTextureAsset:
                //    break;
                //case SerializableType.AudioClipAsset:
                //    break;
                //case SerializableType.MaterialAsset:
                //    break;
                //case SerializableType.AnimationAsset:
                //    break;
                //case SerializableType.AnimatorAsset:
                //    break;
                //case SerializableType.ScriptableObject:
                //    break;
                case SerializableType.EObject:
                    break;
                default:
                    Debug.Error($"Can't deserialize reference: '{type}' is not implemented.");
                    break;
            }

            return null;
        }

        private static void InstantiateActor()
        {

        }
    }
}
