using Engine;
using Engine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal static class SceneDeserializer
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

                    if (ReflectionUtils.ResolveType(componentData.TypeName, out var componentType))
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
                DeserializeTarget(componentValue.value, componentValue.data.SerializedProperties);
            }
        }

        private static void DeserializeTarget(object target, IReadOnlyList<SerializedPropertyData> properties)
        {
            foreach (var property in properties)
            {
                switch (property.Type)
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
                    case SerializedType.MaterialAsset:
                    case SerializedType.AnimationAsset:
                    case SerializedType.AnimatorControllerAsset:
                    case SerializedType.ScriptableObject:
                        DeserializeReferencedProperty(property.Type, target, property);
                        break;
                    case SerializedType.Simple:
                    case SerializedType.SimpleCollection:
                    case SerializedType.SimpleClass:
                        DeserializeSimpleProperty(target, property);
                        break;
                    //case SerializedType.ComplexCollection:
                    //    break;
                    case SerializedType.ReferenceCollection:
                        DeserializeReferenceCollectionProperty(target, property);
                        break;
                    //case SerializedType.ComplexClass:
                    //    break;
                    default:
                        Debug.Error($"Cannot deserialize property of type: {property.Type}, please implement it.");
                        break;
                }
            }
        }

        private static void DeserializeReferencedProperty(SerializedType serializableType, object target,
                                                          SerializedPropertyData property)
        {
            if (property.Data == null)
            {
                Debug.EngineError($"Deserialization error: property '{property.Name}' data is null.");
                return;
            }
            //if(Guid.TryParse((string)property.Data, out var guid))
            var guid = (Guid)property.Data;

            {
                var referenceValue = GetReferenceValue(serializableType, guid);

                if (referenceValue != null)
                {
                    ReflectionUtils.SetMemberValue(target, property.Name, referenceValue);
                }
            }
        }

        private static void DeserializeSimpleProperty(object target, SerializedPropertyData property)
        {
            if (property.Data == null)
            {
                return;
            }

            ReflectionUtils.SetMemberValue(target, property.Name, property.Data);
        }

        private static void DeserializeReferenceCollectionProperty(object target, SerializedPropertyData property)
        {
            var collectionData = property.Data as CollectionPropertyData;

            if (collectionData == null || collectionData.Collection == null)
                return;

            var collectionPropertyType = ReflectionUtils.GetMemberType(target.GetType(), property.Name);

            if (ReflectionUtils.IsCollection(collectionPropertyType, out var collectionType))
            {
                if (collectionType == ReflectionUtils.CollectionType.Dictionary)
                {
                    var args = collectionPropertyType.GetGenericArguments();
                    var dictType = typeof(Dictionary<,>).MakeGenericType(args[0], args[1]);

                    var dictionary = (IDictionary)Activator.CreateInstance(dictType);
                    foreach (var item in collectionData.Collection)
                    {
                        var serializedItem = (SerializedItem<KeyValuePair<object, object>>)item;
                        var guid = serializedItem.Data.Value != null? (Guid)serializedItem.Data.Value: Guid.Empty;
                        dictionary.Add(serializedItem.Data.Key, GetReferenceValue(serializedItem.Type, guid));
                    }

                    ReflectionUtils.SetMemberValue(target, property.Name, dictionary);
                }
                else if (collectionType == ReflectionUtils.CollectionType.List)
                {
                    var list = (IList)ReflectionUtils.GetDefaultValue(collectionPropertyType);

                    SetValueToProperty(list, (item, _) =>
                    {
                        list.Add(GetItemReferenceValue(item));
                    });
                }
                else if (collectionType == ReflectionUtils.CollectionType.Array)
                {
                    var array = Array.CreateInstance(collectionPropertyType.GetElementType(), collectionData.Collection.Count);

                    SetValueToProperty(array, (item, index) =>
                    {
                        array.SetValue(GetItemReferenceValue(item), index);
                    });
                }
            }

            object GetItemReferenceValue(object item)
            {
                var referenceElement = item as SerializedItem<Guid>;

                if (referenceElement == null)
                    return null;

                return GetReferenceValue(referenceElement.Type, referenceElement.Data);
            }

            void SetValueToProperty(object collectionInstance, Action<object, int> setCollectionValueCallback)
            {
                int index = 0;
                foreach (var item in collectionData.Collection)
                {
                    setCollectionValueCallback(item, index);
                    index++;
                }

                ReflectionUtils.SetMemberValue(target, property.Name, collectionInstance);
            }
        }

        private static object GetReferenceValue(SerializedType type, Guid guid)
        {
            if(type == SerializedType.None || guid == Guid.Empty) 
                return null;

            switch (type)
            {
                case SerializedType.Component:
                    return GetReferenceValue(_componentsByID, guid);
                case SerializedType.Actor:
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
                case SerializedType.EObject:
                    break;
                default:
                    Debug.Error($"Can't deserialize reference: '{type}' is not implemented.");
                    break;
            }

            return null;
        }
        private static object GetReferenceValue<V, D>(Dictionary<Guid, (V value, D data)> ids, Guid guid)
        {
            if (ids.TryGetValue(guid, out var data))
            {
                return data.value;
            }

            return null;
        }
        private static void InstantiateActor()
        {

        }
    }
}
