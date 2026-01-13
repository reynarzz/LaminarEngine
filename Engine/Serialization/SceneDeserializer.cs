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
        private static List<Component> _initializationComponents = new();
        public static void DeserializeScene(IReadOnlyList<ActorDataSceneAsset> actors, WeakReference<Scene> scene)
        {
            _actorsByID.Clear();
            _componentsByID.Clear();
            _initializationComponents.Clear();
            if (actors == null || actors.Count == 0)
                return;

            // Instantiate all the actors.
            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];
                var actor = new Actor(actorData.Name, actorData.ID, scene);
                actor.Layer = actorData.Layer;
                actor.IsActiveSelf = actorData.IsActiveSelf;
                _actorsByID.Add(actor.GetID(), (actor, actorData));
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

            // Note: loops through the actors again after the hierarchy is completed to add the components.
            //      This is important in playmode since components could be added to a "enable later" queue, if actors are disabled
            //      by itself or in the hierarchy.
            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];
                var actor = _actorsByID[actorData.ID].value;

                // Add components, but no deserialize yet.
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (ReflectionUtils.ResolveType(componentData.TypeName, out var componentType))
                    {
                        // TODO: fix the component initialization for the ones that auto add other required components.
                        // When 'Application.IsInPlayMode' is on, it will auto add components, making this not usable.

                        var component = actor.AddComponent(componentType, componentData.ID, false,
                                                           componentData.IsEnabled, true, out var isPendingToInitialize);

                        if (!isPendingToInitialize)
                        {
                            if (Application.IsInPlayMode)
                            {
                                _initializationComponents.Add(component);
                            }
                            else
                            {
                               // Debug.Log(component.GetType() + " Not adding, pending for later");
                            }
                        }

                        _componentsByID.Add(componentData.ID, (component, componentData));
                    }
                }

                // Resolve required properties
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (_componentsByID.TryGetValue(componentData.ID, out var comp))
                    {
                        var component = comp.value;

                        var reqProperties = ReflectionUtils.GetAllMembersWithAttribute<RequiredPropertyAttribute>(component.GetType())?.ToList();

                        if (reqProperties != null && reqProperties.Count > 0)
                        {
                            for (int k = 0; k < reqProperties.Count; k++)
                            {
                                var prop = reqProperties[k];
                                var reqComponent = component.GetComponent(ReflectionUtils.GetMemberType(prop));
                                ReflectionUtils.SetMemberValue(component, prop, reqComponent);
                            }
                        }
                    }
                }
            }


            // Deserialize components data, and resolve references.
            foreach (var (id, componentValue) in _componentsByID)
            {
                DeserializeTarget(componentValue.value, componentValue.data.SerializedProperties);
            }

            if (Application.IsInPlayMode)
            {
                foreach (var component in _initializationComponents)
                {
                    try
                    {
                        (component as IAwakeableComponent).OnAwake();
                    }
                    catch (Exception e)
                    {
                        Debug.Error(e);
                    }

                    try
                    {
                        if (component.IsEnabled)
                        {
                            (component as IEnabledComponent).OnEnabled();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Error(e);
                    }
                }
                _initializationComponents.Clear();
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
                        DeserializeReferencedProperty(target, property);
                        break;
                    case SerializedType.Simple:
                    case SerializedType.SimpleCollection:
                    case SerializedType.SimpleClass:
                        DeserializeSimpleProperty(target, property);
                        break;
                    case SerializedType.ReferenceCollection:
                        DeserializeReferenceCollectionProperty(target, property);
                        break;
                    case SerializedType.ComplexClass:
                        DeserializeComplexClass(target, property);
                        break;
                    case SerializedType.ComplexCollection:
                        DeserializeComplexCollection(target, property);
                        break;
                    default:
                       // Debug.Error($"Cannot deserialize property of type: {property.Type}, please implement it.");
                        break;
                }
            }
        }

        private static void DeserializeReferencedProperty(object target, SerializedPropertyData property)
        {
            if (property.Data == null)
            {
                // Debug.Warn($"Deserialization error: property '{property.Name}' data is null.");
                return;
            }
            //if(Guid.TryParse((string)property.Data, out var guid))
            var guid = GetGuidSafe(property.Data);

            {
                var referenceValue = GetReferenceValue(property.Type, guid);

                if (referenceValue != null)
                {
                    ReflectionUtils.SetMemberValue(target, referenceValue, property.Name);
                }
            }
        }

        private static void DeserializeSimpleProperty(object target, SerializedPropertyData property)
        {
            if (property.Data == null)
            {
                return;
            }

            ReflectionUtils.SetMemberValue(target, property.Data, property.Name);
        }

        private static void DeserializeReferenceCollectionProperty(object target, SerializedPropertyData property)
        {
            if (property.Data == null)
                return;

            Type collectionPropertyType = null;
            CollectionPropertyData collectionData = null;

            if (ReflectionUtils.IsCollection(target?.GetType(), out var colType))
            {
                // Note: Nested collections
                if (colType == ReflectionUtils.CollectionType.Dictionary)
                {
                    // NOTE: This assumes that the reference is in the value side, so keys that are references will not be supported.
                    collectionPropertyType = ReflectionUtils.GetMemberType(target.GetType().GetGenericArguments()[1], property.Name);
                }
                else if (colType == ReflectionUtils.CollectionType.Array)
                {
                    collectionPropertyType = ReflectionUtils.GetMemberType(target.GetType().GetElementType(), property.Name);
                }
                else
                {
                    collectionPropertyType = ReflectionUtils.GetMemberType(target.GetType().GetGenericArguments()[0], property.Name);
                }

                // throw new Exception("Implement: Get value of collectionData.Collection");
                collectionData = property.Data as CollectionPropertyData;
            }
            else
            {
                collectionPropertyType = ReflectionUtils.GetMemberType(target.GetType(), property.Name);
                collectionData = property.Data as CollectionPropertyData;
            }

            if (collectionData == null || collectionData.Collection == null)
            {
                return;
            }

            if (collectionData.CollectionType == ReflectionUtils.CollectionType.Dictionary)
            {
                var args = collectionPropertyType.GetGenericArguments();
                var dictType = typeof(Dictionary<,>).MakeGenericType(args[0], args[1]);

                var dictionary = (IDictionary)Activator.CreateInstance(dictType);
                foreach (var item in collectionData.Collection)
                {
                    var serializedItem = (DictionaryData<object, object>)item;
                    var guid = GetGuidSafe(serializedItem.Value);
                    if ((serializedItem.keyType == SerializedType.Simple ||
                        serializedItem.keyType == SerializedType.SimpleClass ||
                        serializedItem.keyType == SerializedType.SimpleCollection) && serializedItem.Key != null
                        && serializedItem.Key.GetType() != args[0].GetType())
                    {
                        serializedItem.Key = Convert.ChangeType(serializedItem.Key, args[0]);
                    }

                    dictionary.Add(serializedItem.Key, GetReferenceValue(serializedItem.Type, guid));
                }

                ReflectionUtils.SetMemberValue(target, dictionary, property.Name);

            }
            else if (collectionData.CollectionType == ReflectionUtils.CollectionType.List)
            {
                var list = (IList)ReflectionUtils.GetDefaultValueInstance(collectionPropertyType);

                SetValueToProperty(list, (item, _) =>
                {
                    list.Add(GetItemReferenceValue(item));
                });
            }
            else if (collectionData.CollectionType == ReflectionUtils.CollectionType.Array)
            {
                var array = Array.CreateInstance(collectionPropertyType.GetElementType(), collectionData.Collection.Count);

                SetValueToProperty(array, (item, index) =>
                {
                    array.SetValue(GetItemReferenceValue(item), index);
                });
            }

            object GetItemReferenceValue(object item)
            {
                var referenceElement = item as CollectionData<Guid>;

                if (referenceElement == null)
                    return null;

                return GetReferenceValue(referenceElement.Type, referenceElement.Value);
            }

            void SetValueToProperty(object collectionInstance, Action<object, int> setCollectionValueCallback)
            {
                int collIndex = 0;
                foreach (var item in collectionData.Collection)
                {
                    setCollectionValueCallback(item, collIndex);
                    collIndex++;
                }

                ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
            }
        }

        private static Guid GetGuidSafe(object guid)
        {
            if (guid == null)
            {
                return Guid.Empty;
            }

            if (guid.GetType() == typeof(string))
            {
                Guid.TryParse((string)guid, out var guidValue);
                return guidValue;
            }

            return (Guid)guid;
        }

        private static void DeserializeComplexClass(object target, SerializedPropertyData property)
        {
            if (target == null || property == null || property.Data == null)
                return;

            var complexData = property.Data as ComplexTypeData;
            if (ReflectionUtils.ResolveType(complexData.TargetTypeName, out Type type))
            {
                var inst = Activator.CreateInstance(type);
                DeserializeTarget(inst, complexData.Properties);
                ReflectionUtils.SetMemberValue(target, inst, property.Name);
            }
        }

        private static void DeserializeComplexCollection(object target, SerializedPropertyData property)
        {

            if (target == null || property == null || property.Data == null)
                return;

            var collectionData = property.Data as CollectionPropertyData;

            if (collectionData == null)
            {
                Debug.EngineError("FATAL: Not a collection to deserialize!");
                return;
            }

            void DeserializeItem(ComplexTypeData complexItem, Action<object> setValueCallback)
            {
                if (complexItem == null)
                {
                    setValueCallback(null);
                    return;
                }
                if (ReflectionUtils.ResolveType(complexItem.TargetTypeName, out Type itemType))
                {
                    var itemInstance = ReflectionUtils.GetDefaultValueInstance(itemType);
                    DeserializeTarget(itemInstance, complexItem.Properties);
                    setValueCallback(itemInstance);
                }
            }

            if (ReflectionUtils.ResolveType(property.InternalType, out Type type))
            {
                var collectionInstance = ReflectionUtils.GetDefaultValueInstance(type, collectionData.Collection.Count);

                if (collectionData.CollectionType == ReflectionUtils.CollectionType.Dictionary)
                {
                    var dictionary = collectionInstance as IDictionary;
                    for (int i = 0; i < collectionData.Collection.Count; i++)
                    {
                        object DeserializeArgValue(ComplexTypeData complexArg)
                        {
                            object deserializedArgValue = null;

                            if (complexArg.ComplexType == SerializedType.Simple ||
                                complexArg.ComplexType == SerializedType.SimpleClass ||
                                complexArg.ComplexType == SerializedType.SimpleCollection)
                            {
                                deserializedArgValue = complexArg.Properties?[0].Data ?? null;
                            }
                            else
                            {
                                DeserializeItem(complexArg, item =>
                                {
                                    deserializedArgValue = item;
                                });
                            }

                            return deserializedArgValue;
                        }

                        var complexItem = collectionData.Collection[i] as ComplexDictionaryData<ComplexTypeData, ComplexTypeData>;

                        var key = DeserializeArgValue(complexItem.Key);
                        var value = DeserializeArgValue(complexItem.Value);

                        if (key != null && !dictionary.Contains(key))
                        {
                            dictionary.Add(key, value);
                        }
                    }

                    ReflectionUtils.SetMemberValue(target, dictionary, property.Name);
                }
                else
                {
                    collectionInstance = ReflectionUtils.EnsureCount(collectionInstance, collectionData.Collection.Count);
                    for (int i = 0; i < collectionData.Collection.Count; i++)
                    {
                        var complexItem = collectionData.Collection[i] as CollectionData<ComplexTypeData>;

                        DeserializeItem(complexItem.Value, item =>
                        {
                            ReflectionUtils.SetMemberValueSafe(collectionInstance, item, default(MemberInfo), i);
                        });
                    }

                    ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
                }
            }
        }

        private static object GetReferenceValue(SerializedType type, Guid guid)
        {
            if (type == SerializedType.None || guid == Guid.Empty)
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
                case SerializedType.AudioClipAsset:
                    return Assets.GetAssetFromGuid(guid) as AudioClip;
                //case SerializableType.MaterialAsset:
                //    break;
                //case SerializableType.AnimationAsset:
                //    break;
                //case SerializableType.AnimatorAsset:
                //    break;
                //case SerializableType.ScriptableObject:
                //    break;
                default:
                    // Debug.Error($"Can't deserialize reference: '{type}' is not implemented.");
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
