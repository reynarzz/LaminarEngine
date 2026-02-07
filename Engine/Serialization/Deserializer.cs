using Engine.Utils;
using Engine;
using System;
using System.Collections;
using System.Reflection;

namespace Engine.Serialization
{

    internal class DeserializerData
    {
        internal Dictionary<Guid, (Actor value, ActorIR data)> ActorsByID = new();
        internal Dictionary<Guid, (Component value, ComponentIR data)> ComponentsByID = new();
    }
    internal class Deserializer : Deserializer<TypeResolver> { }
    internal class Deserializer<Tr> where Tr : ITypeResolver
    {
        internal static T Deserialize<T>(IReadOnlyList<SerializedPropertyIR> properties)
        {
            if (properties == null)
                return default;

            var target = Activator.CreateInstance(typeof(T), true);
            Deserialize(target, properties);
            if (target == default)
            {
                return default;
            }
            return (T)target;
        }
        internal static void Deserialize(object targetInstance, IReadOnlyList<SerializedPropertyIR> properties)
        {
            DeserializeTarget(targetInstance, properties, null);
        }
        internal static void DeserializeTarget(object targetInstance, IReadOnlyList<SerializedPropertyIR> properties,
                                               DeserializerData deserializerData)
        {
            if (targetInstance == null || properties == null)
                return;

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
                    case SerializedType.SpriteAsset:
                    case SerializedType.RenderTextureAsset:
                    case SerializedType.AudioClipAsset:
                    case SerializedType.MaterialAsset:
                    case SerializedType.ShaderAsset:
                    case SerializedType.AnimationAsset:
                    case SerializedType.AnimatorControllerAsset:
                    case SerializedType.ScriptableObject:
                        DeserializeReferencedProperty(targetInstance, property, deserializerData);
                        break;
                    case SerializedType.Simple:
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

        private static void DeserializeReferencedProperty(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (property.Data == null)
            {
                // Debug.Warn($"Deserialization error: property '{property.Name}' data is null.");
                return;
            }
            //if(Guid.TryParse((string)property.Data, out var guid))

            var refData = property.Data as ReferenceData;

            var referenceValue = GetReferenceValue(property.Type, deserializerData, refData);

            if (referenceValue != null)
            {
                ReflectionUtils.SetMemberValue(target, referenceValue, property.Name);
            }
        }

        private static void DeserializeSimpleProperty(object target, SerializedPropertyIR property)
        {
            if (property.Data == null)
            {
                return;
            }

            ReflectionUtils.SetMemberValue(target, property.Data, property.Name);
        }

        private static void DeserializeReferenceCollectionProperty(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (property.Data == null)
                return;

            Type collectionPropertyType = null;
            CollectionPropertyData collectionData = null;

            if (ReflectionUtils.IsCollection(target?.GetType(), out var colType))
            {
                // Note: Nested collections
                if (colType == CollectionType.Dictionary)
                {
                    // NOTE: This assumes that the reference is in the value side, so keys that are references will not be supported.
                    collectionPropertyType = ReflectionUtils.GetMemberType(target.GetType().GetGenericArguments()[1], property.Name);
                }
                else if (colType == CollectionType.Array)
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

            if (collectionData.CollectionType == CollectionType.Dictionary)
            {
                var args = collectionPropertyType.GetGenericArguments();
                var dictType = typeof(Dictionary<,>).MakeGenericType(args[0], args[1]);

                var dictionary = (IDictionary)Activator.CreateInstance(dictType);
                foreach (var item in collectionData.Collection)
                {
                    var serializedItem = (DictionaryData<object, object>)item;
                    if (serializedItem.keyType == SerializedType.Simple && serializedItem.Key != null
                        && serializedItem.Key.GetType() != args[0].GetType())
                    {
                        serializedItem.Key = Convert.ChangeType(serializedItem.Key, args[0]);
                    }

                    var refVal = GetReferenceValue(serializedItem.Type, deserializerData, serializedItem.Value as ReferenceData);
                    dictionary.Add(serializedItem.Key, refVal);
                }

                ReflectionUtils.SetMemberValue(target, dictionary, property.Name);

            }
            else if (collectionData.CollectionType == CollectionType.List)
            {
                var list = (IList)ReflectionUtils.GetDefaultValueInstance(collectionPropertyType);

                SetValueToProperty(list, (item, _) =>
                {
                    list.Add(GetItemReferenceValue(item));
                });
            }
            else if (collectionData.CollectionType == CollectionType.Array)
            {
                var array = Array.CreateInstance(collectionPropertyType.GetElementType(), collectionData.Collection.Count);

                SetValueToProperty(array, (item, index) =>
                {
                    array.SetValue(GetItemReferenceValue(item), index);
                });
            }

            object GetItemReferenceValue(object item)
            {
                var referenceElement = item as CollectionData<ReferenceData>;

                if (referenceElement == null)
                    return null;

                return GetReferenceValue(referenceElement.Type, deserializerData, referenceElement.Value);
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

        internal static void DeserializeComplexClass(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (target == null || property == null || property.Data == null)
                return;

            var complexData = property.Data as ComplexTypeData;
            if (Tr.ResolveType(complexData, out Type type))
            {
                var inst = Activator.CreateInstance(type);
                DeserializeTarget(inst, complexData.Properties, deserializerData);
                ReflectionUtils.SetMemberValue(target, inst, property.Name);
            }
        }

        internal static void DeserializeComplexCollection(object target, SerializedPropertyIR property, DeserializerData deserializerData)
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
                if (Tr.ResolveType(complexItem, out Type itemType))
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

            if (Tr.ResolveType(property, out Type type))
            {
                var collectionInstance = ReflectionUtils.GetDefaultValueInstance(type, collectionData.Collection.Count);

                if (collectionData.CollectionType == CollectionType.Dictionary)
                {
                    var dictionary = collectionInstance as IDictionary;
                    for (int i = 0; i < collectionData.Collection.Count; i++)
                    {
                        object DeserializeArgValue(ComplexTypeData complexArg)
                        {
                            object deserializedArgValue = null;

                            if (complexArg.ComplexType == SerializedType.Simple)
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
                            if (value is ReferenceData reference)
                            {
                                value = GetReferenceValue(complexItem.Type, deserializerData, reference);
                            }
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
                            if (item is ReferenceData reference)
                            {
                                item = GetReferenceValue(complexItem.Type, deserializerData, reference);
                            }

                            ReflectionUtils.SetMemberValueSafe(collectionInstance, item, default(MemberInfo), i);
                        });
                    }

                    ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
                }
            }
        }

        private static object GetReferenceValue(SerializedType type, DeserializerData deserializerData,
                                                ReferenceData refData)
        {
            if (type == SerializedType.None || refData == null || refData.Id == Guid.Empty)
                return null;

            switch (type)
            {
                case SerializedType.Component:
                    return GetReferenceValue(deserializerData?.ComponentsByID, refData.Id);
                case SerializedType.Actor:
                    return GetReferenceValue(deserializerData?.ActorsByID, refData.Id);
                case SerializedType.Asset:
                case SerializedType.TextureAsset:
                    return (Assets.GetAssetFromGuid(refData.Id) as TextureAsset)?.Texture;
                case SerializedType.SpriteAsset:
                    return GetSprite(refData as SpriteReferenceData);
                case SerializedType.RenderTextureAsset:
                case SerializedType.AudioClipAsset:
                case SerializedType.MaterialAsset:
                case SerializedType.ShaderAsset:
                case SerializedType.AnimationAsset:
                case SerializedType.AnimatorControllerAsset:
                case SerializedType.ScriptableObject:
                    return Assets.GetAssetFromGuid(refData.Id);
                default:
                    // Debug.Error($"Can't deserialize reference: '{type}' is not implemented.");
                    break;
            }

            return null;
        }

        private static Sprite GetSprite(SpriteReferenceData refData)
        {
            var asset = Assets.GetAssetFromGuid(refData.TextureId);
            var atlas = (asset as TextureAsset)?.Atlas;
            if (atlas != null)
            {
                var sprite = atlas.GetSprite(refData.AtlasIndex);
                return sprite;
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
