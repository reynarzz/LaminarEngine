using Engine.Utils;
using System;
using System.Collections;
using System.Reflection;

namespace Engine.Serialization
{
    internal class DeserializerData
    {
        internal Dictionary<Guid, (Actor value, ActorDataSceneAsset data)> ActorsByID = new();
        internal Dictionary<Guid, (Component value, ComponentDataSceneAsset data)> ComponentsByID = new();
    }

    internal static class Deserializer
    {
        internal static T Deserialize<T>(IReadOnlyList<SerializedPropertyData> properties)
        {
            var target = Activator.CreateInstance(typeof(T), true);
            Deserialize(target, properties);
            if (target == default)
            {
                return default;
            }
            return (T)target;
        }
        internal static void Deserialize(object targetInstance, IReadOnlyList<SerializedPropertyData> properties)
        {
            DeserializeTarget(targetInstance, properties, null);
        }
        internal static void DeserializeTarget(object targetInstance, IReadOnlyList<SerializedPropertyData> properties,
                                               DeserializerData deserializerData)
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
                        DeserializeReferencedProperty(targetInstance, property, deserializerData);
                        break;
                    case SerializedType.Simple:
                    case SerializedType.SimpleCollection:
                    case SerializedType.SimpleClass:
                        DeserializeSimpleProperty(targetInstance, property);
                        break;
                    case SerializedType.ReferenceCollection:
                        DeserializeReferenceCollectionProperty(targetInstance, property, deserializerData);
                        break;
                    case SerializedType.ComplexClass:
                        DeserializeComplexClass(targetInstance, property, deserializerData);
                        break;
                    case SerializedType.ComplexCollection:
                        DeserializeComplexCollection(targetInstance, property, deserializerData);
                        break;
                    default:
                        // Debug.Error($"Cannot deserialize property of type: {property.Type}, please implement it.");
                        break;
                }
            }
        }

        private static void DeserializeReferencedProperty(object target, SerializedPropertyData property, DeserializerData deserializerData)
        {
            if (property.Data == null)
            {
                // Debug.Warn($"Deserialization error: property '{property.Name}' data is null.");
                return;
            }
            //if(Guid.TryParse((string)property.Data, out var guid))
            var guid = GetGuidSafe(property.Data);

            var referenceValue = GetReferenceValue(property.Type, guid, deserializerData);

            if (referenceValue != null)
            {
                ReflectionUtils.SetMemberValue(target, referenceValue, property.Name);
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

        private static void DeserializeReferenceCollectionProperty(object target, SerializedPropertyData property, DeserializerData deserializerData)
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

            if (collectionPropertyType == null || collectionData == null || collectionData.Collection == null)
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

                    dictionary.Add(serializedItem.Key, GetReferenceValue(serializedItem.Type, guid, deserializerData));
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

                return GetReferenceValue(referenceElement.Type, referenceElement.Value, deserializerData);
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

        internal static void DeserializeComplexClass(object target, SerializedPropertyData property, DeserializerData deserializerData)
        {
            if (target == null || property == null || property.Data == null)
                return;

            var complexData = property.Data as ComplexTypeData;
            if (ReflectionUtils.ResolveType(complexData.TargetTypeName, out Type type))
            {
                var inst = Activator.CreateInstance(type);
                DeserializeTarget(inst, complexData.Properties, deserializerData);
                ReflectionUtils.SetMemberValue(target, inst, property.Name);
            }
        }

        internal static void DeserializeComplexCollection(object target, SerializedPropertyData property, DeserializerData deserializerData)
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
                    if (itemInstance == null)
                    {
                        throw new Exception($"Can't create an instance of type: {itemType}, check if a default constructor is available.");
                    }
                    DeserializeTarget(itemInstance, complexItem.Properties, deserializerData);
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
                                // if (complexArg.Properties != null && complexArg.Properties.Count > 0)
                                {
                                    deserializedArgValue = complexArg.Properties?[0].Data ?? null;
                                }
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

        private static object GetReferenceValue(SerializedType type, Guid guid, DeserializerData deserializerData)
        {
            if (type == SerializedType.None || guid == Guid.Empty)
                return null;

            switch (type)
            {
                case SerializedType.Component:
                    return GetReferenceValue(deserializerData?.ComponentsByID, guid);
                case SerializedType.Actor:
                    return GetReferenceValue(deserializerData?.ActorsByID, guid);
                case SerializedType.Asset:
                case SerializedType.TextureAsset:
                case SerializedType.RenderTextureAsset:
                case SerializedType.AudioClipAsset:
                case SerializedType.MaterialAsset:
                case SerializedType.AnimationAsset:
                case SerializedType.AnimatorControllerAsset:
                case SerializedType.ScriptableObject:
                    return Assets.GetAssetFromGuid(guid);
                default:
                    // Debug.Error($"Can't deserialize reference: '{type}' is not implemented.");
                    break;
            }

            return null;
        }
        private static object GetReferenceValue<V, D>(Dictionary<Guid, (V value, D data)> ids, Guid guid)
        {
            if (guid == Guid.Empty)
            {
                return null;
            }

            if (ids != null && ids.TryGetValue(guid, out var data))
            {
                return data.value;
            }

            return null;
        }
    }
}
