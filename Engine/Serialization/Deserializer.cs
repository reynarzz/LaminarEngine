using Engine.Utils;
using Engine;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using Generated;

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
                var serializedType = property.Type;
                if (serializedType.IsEObject())
                {
                    DeserializeReferencedProperty(targetInstance, property, deserializerData);
                }
                else if (serializedType.IsSimple())
                {
                    DeserializeSimpleProperty(targetInstance, property);
                }
                else if (serializedType == SerializedType.ReferenceCollection)
                {
                    DeserializeReferenceCollectionProperty(targetInstance, property, deserializerData);
                }
                else if (serializedType == SerializedType.ComplexClass)
                {
                    DeserializeComplexClass(targetInstance, property, deserializerData);
                }
                else if (serializedType == SerializedType.ComplexCollection)
                {
                    DeserializeComplexCollection(targetInstance, property, deserializerData);
                }
                else if (serializedType == SerializedType.SimpleCollection)
                {
                    DeserializeSimpleCollection(targetInstance, property, deserializerData);
                }
                else
                {
                    // Debug.Error($"Cannot deserialize property of type: {property.Type}, please implement it.");
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
            object value = null;
            if (property.Type == SerializedType.Enum)
            {
                value = DeserializeEnum((VariantIRValue)property.Data);
            }
            else
            {
                value = ((VariantIRValue)property.Data).GetValueAsObject();
            }
            ReflectionUtils.SetMemberValue(target, value, property.Name);
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
                    var serializedItem = (DictionaryData)item;

                    object GetArgValue(object argData, SerializedType argSerializedType, Type argType)
                    {
                        if (argSerializedType.IsSimple())
                        {
                            if (argData != null && argData.GetType() != argType)
                            {
                                return DeserializeVariantValueSafe((VariantIRValue)argData);
                            }

                            return null;
                        }

                        return GetReferenceValue(argSerializedType, deserializerData, argData as ReferenceData);
                    }
                    //if (serializedItem.KeyType.IsSimple() && serializedItem.Key != null
                    //    && serializedItem.Key.GetType() != args[0].GetType())
                    //{
                    //    serializedItem.Key = serializedItem.Key;
                    //}
                    var key = GetArgValue(serializedItem.Key, serializedItem.KeyType, args[0]);
                    var value = GetArgValue(serializedItem.Value, serializedItem.ValueType, args[1]);

                    dictionary.Add(key, value);
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
        // TODO: This code is boxing like tyson in its prime.
        internal static void DeserializeSimpleCollection(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (target == null || property == null || property.Data == null)
                return;

            var collectionData = property.Data as CollectionPropertyData;
            if (collectionData == null)
            {
                Debug.EngineError("FATAL: Not a collection to deserialize!");
                return;
            }

            if (collectionData.Collection == null)
            {
                return;
            }
            if (Tr.ResolveType(property, out Type type))
            {
                var collectionInstance = ReflectionUtils.GetDefaultValueInstance(type);
                if (collectionData.CollectionType == CollectionType.Dictionary)
                {
                    var dictionary = collectionInstance as IDictionary;

                    foreach (var item in collectionData.Collection)
                    {
                        var itemObj = item as DictionaryDataSimple;
                        var key = DeserializeVariantValueSafe(itemObj.Key);
                        var value = DeserializeVariantValueSafe(itemObj.Value);

                        dictionary.Add(key, value);
                    }

                    ReflectionUtils.SetMemberValue(target, dictionary, property.Name);
                }
                else
                {
                    var variantCollection = collectionData.Collection as VariantIRValue[];
                    if(variantCollection.Length > 0)
                    {
                        if(collectionData.ItemsType == SerializedType.Enum)
                        {
                            // Having huge collections of enums is unlikelly, also, I do not want to complicate the code generator.
                            // is there are performance issues related to this, I will change it.
                            collectionInstance = ReflectionUtils.EnsureCount(collectionInstance, collectionData.Collection.Count);

                            for (int i = 0; i < variantCollection.Length; i++)
                            {
                                var itemObj = DeserializeVariantValueSafe(in variantCollection[i]);
                                ReflectionUtils.SetMemberValueSafe(collectionInstance, itemObj, default(MemberInfo), i);
                            }
                        }
                        else
                        {
                            collectionInstance = VariantCollectionWriter.Write(collectionInstance, variantCollection, collectionData.ItemsType, collectionData.CollectionType);
                        }
                    }
                }

                ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
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

            if (collectionData.Collection == null)
            {
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
                    var itemValue = GetComplexTypeDataSafe(itemInstance, complexItem, complexItem.ComplexType, deserializerData);
                    setValueCallback(itemValue);
                }
            }

            if (Tr.ResolveType(property, out Type type))
            {
                var collectionInstance = ReflectionUtils.GetDefaultValueInstance(type, collectionData.Collection.Count);

                if (collectionData.CollectionType == CollectionType.Dictionary)
                {
                    var dictionary = collectionInstance as IDictionary;
                    foreach (var collItem in collectionData.Collection)
                    {
                        object DeserializeArgValue(ComplexTypeData complexArg)
                        {
                            object deserializedArgValue = null;

                            if (complexArg.ComplexType.IsSimple())
                            {
                                // if (complexArg.Properties != null && complexArg.Properties.Count > 0)
                                {
                                    deserializedArgValue = GetComplexTypeDataSafe(complexArg.Properties?[0].Data ?? null, complexArg,
                                                                                  complexArg.ComplexType, deserializerData);
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

                        var complexItem = collItem as ComplexDictionaryData;

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
                    int colItemIndex = 0;
                    foreach (var colItem in collectionData.Collection)
                    {
                        var complexItem = colItem as CollectionData<ComplexTypeData>;

                        DeserializeItem(complexItem.Value, item =>
                        {
                            ReflectionUtils.SetMemberValueSafe(collectionInstance, item, default(MemberInfo), colItemIndex);
                        });
                        colItemIndex++;
                    }

                    ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
                }
            }
        }

        private static object GetComplexTypeDataSafe(object arg, ComplexTypeData argComplexData,
                                                     SerializedType argType, DeserializerData deserializedData)
        {
            if (arg == null)
                return null;

            if (arg is ReferenceData reference)
            {
                return GetReferenceValue(argType, deserializedData, reference);
            }
            else if (argComplexData.ComplexType.IsSimple())
            {
                if (argComplexData.ComplexType == SerializedType.Enum)
                {
                    return DeserializeEnum((VariantIRValue)arg);
                }

                return ((VariantIRValue)arg).GetValueAsObject();
            }

            return arg;
        }
        private static object DeserializeVariantValueSafe(in VariantIRValue variant)
        {
            if (variant.Kind == SerializedType.Enum)
            {
                return DeserializeEnum(variant);
            }

            return variant.GetValueAsObject();
        }

        private static Enum DeserializeEnum(in VariantIRValue variant)
        {
            if (variant.Kind != SerializedType.Enum)
            {
                Debug.EngineError("Is not enum!");
                return null;
            }

            if (Tr.ResolveType(variant.Enum, out var enumType))
            {
                return (Enum)Enum.ToObject(enumType, variant.Enum.EnumValue);
            }

            return null;
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
                case SerializedType.TextureAsset:
                    return (Assets.GetAssetFromGuid(refData.Id) as TextureAsset)?.Texture;
                case SerializedType.SpriteAsset:
                    return GetSprite(refData as SpriteReferenceData);

                // Rest of assets will be threated equal.
                case var t when t.IsAsset():
                    return Assets.GetAssetFromGuid(refData.Id);
                default:
                    Debug.Error($"Can't deserialize reference: '{type}' is not implemented.");
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
