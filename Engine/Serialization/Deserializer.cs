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
        internal static void DeserializeTarget(object targetInstance, IReadOnlyList<SerializedPropertyIR> properties, DeserializerData deserializerData)
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
                    Debug.Error($"Cannot deserialize property of type: {property.Type}, please implement it.");
                }
            }
        }

        private static void DeserializeReferencedProperty(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (property.Reference == null)
            {
                // Debug.Warn($"Deserialization error: property '{property.Name}' data is null.");
                return;
            }
            //if(Guid.TryParse((string)property.Data, out var guid))

            var refData = property.Reference;

            var referenceValue = GetReferenceValue(property.Type, deserializerData, refData);

            if (referenceValue != null)
            {
                ReflectionUtils.SetMemberValue(target, referenceValue, property.Name);
            }
        }

        private static void DeserializeSimpleProperty(object target, SerializedPropertyIR property)
        {
            if (property.Simple.Kind == SerializedType.None)
            {
                return;
            }
            object value = null;
            if (property.Type == SerializedType.Enum)
            {
                value = ReflectionUtils.DeserializeEnum<Tr>(property.Simple);
            }
            else
            {
                value = property.Simple.GetValueAsObject();
            }
            ReflectionUtils.SetMemberValue(target, value, property.Name);
        }

        private static void DeserializeReferenceCollectionProperty(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (property.Collection == null)
                return;

            Type collectionPropertyType = null;
            CollectionData collectionData = property.Collection;

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
            }
            else
            {
                collectionPropertyType = ReflectionUtils.GetMemberType(target.GetType(), property.Name);
            }

            if (collectionPropertyType == null || collectionData == null || collectionData == null)
            {
                return;
            }

            if (collectionData.CollectionType == CollectionType.Dictionary)
            {
                object GetArgValue(object argData, SerializedType argSerializedType, Type argType)
                {
                    if (argSerializedType.IsSimple())
                    {
                        if (argData != null && argData.GetType() != argType)
                        {
                            return ReflectionUtils.DeserializeVariantValueSafe<Tr>((VariantIRValue)argData);
                        }

                        return null;
                    }

                    return GetReferenceValue(argSerializedType, deserializerData, argData as ReferenceData);
                }

                var args = collectionPropertyType.GetGenericArguments();
                var dictType = typeof(Dictionary<,>).MakeGenericType(args[0], args[1]);

                var dictionary = (IDictionary)Activator.CreateInstance(dictType);

                var referenceCollection = collectionData as DictionaryIRReferences;

                for (int i = 0; i < referenceCollection.Count; i++)
                {
                    var rawKey = referenceCollection.Keys[i];
                    var keyType = referenceCollection.KeyType[i];
                    var rawValue = referenceCollection.Values[i];
                    var valueType = referenceCollection.ValueType[i];

                    var key = GetArgValue(rawKey, keyType, args[0]);
                    var value = GetArgValue(rawValue, valueType, args[1]);

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
                var array = Array.CreateInstance(collectionPropertyType.GetElementType(), collectionData.Count);

                SetValueToProperty(array, (item, index) =>
                {
                    array.SetValue(GetItemReferenceValue(item), index);
                });
            }

            object GetItemReferenceValue(ReferenceData reference)
            {
                if (reference == null)
                    return null;

                return GetReferenceValue(reference.Type, deserializerData, reference);
            }

            void SetValueToProperty(object collectionInstance, Action<ReferenceData, int> setCollectionValueCallback)
            {
                int collIndex = 0;
                var referenceCol = collectionData as CollectionIRReferences;
                foreach (var item in referenceCol.Value)
                {
                    setCollectionValueCallback(item, collIndex);
                    collIndex++;
                }

                ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
            }
        }

        internal static void DeserializeComplexClass(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (target == null || property == null || property.ComplexClass == null)
                return;

            var complexData = property.ComplexClass;
            if (Tr.ResolveType(complexData, out Type type))
            {
                var inst = Activator.CreateInstance(type);
                DeserializeTarget(inst, complexData.Properties, deserializerData);
                ReflectionUtils.SetMemberValue(target, inst, property.Name);
            }
        }
        internal static void DeserializeSimpleCollection(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (target == null || property == null || property.Collection == null)
                return;

            var collectionData = property.Collection;
            if (collectionData == null)
            {
                Debug.EngineError("FATAL: Not a collection to deserialize!");
                return;
            }

            if (collectionData.Count == 0)
            {
                return;
            }
            if (Tr.ResolveType(property, out Type type))
            {
                var collectionInstance = ReflectionUtils.GetDefaultValueInstance(type);
                if (collectionData.CollectionType == CollectionType.Dictionary)
                {
                    var dictionarySimple = collectionData as DictionaryIRVariants;

                    // NOTE: Only enums will be boxed.
                    if (dictionarySimple.Count > 0 && (dictionarySimple.KeyType == SerializedType.Enum ||
                                                       dictionarySimple.ValueType == SerializedType.Enum))
                    {
                        var dictionary = collectionInstance as IDictionary;
                        for (int i = 0; i < dictionarySimple.Count; i++)
                        {
                            var key = ReflectionUtils.DeserializeVariantValueSafe<Tr>(dictionarySimple.Keys[i]);
                            var value = ReflectionUtils.DeserializeVariantValueSafe<Tr>(dictionarySimple.Values[i]);
                            dictionary.Add(key, value);
                        }
                    }
                    else
                    {
                        collectionInstance = VariantCollectionWriter.Write(collectionInstance, dictionarySimple.Keys, dictionarySimple.Values,
                                                                           dictionarySimple.KeyType, dictionarySimple.ValueType);
                    }

                    ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
                }
                else
                {
                    var variantCollection = collectionData as CollectionIRVariants;
                    if (variantCollection.Count > 0)
                    {
                        if (variantCollection.ItemsType == SerializedType.Enum)
                        {
                            // Having huge collections of enums is unlikelly, also, I do not want to complicate the code generator.
                            // if we have performance issues related to this, I will change it.
                            collectionInstance = ReflectionUtils.EnsureCount(collectionInstance, collectionData.Count);

                            for (int i = 0; i < variantCollection.Count; i++)
                            {
                                var itemObj = ReflectionUtils.DeserializeVariantValueSafe<Tr>(in variantCollection.Value[i]);
                                ReflectionUtils.SetMemberValueSafe(collectionInstance, itemObj, default(MemberInfo), i);
                            }
                        }
                        else
                        {
                            collectionInstance = VariantCollectionWriter.Write(collectionInstance, variantCollection.Value, variantCollection.ItemsType, collectionData.CollectionType);
                        }
                    }
                }

                ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
            }
        }

        internal static void DeserializeComplexCollection(object target, SerializedPropertyIR property, DeserializerData deserializerData)
        {
            if (target == null || property == null || property.Collection == null)
                return;

            var collectionData = property.Collection;

            if (collectionData == null)
            {
                Debug.EngineError("FATAL: Not a collection to deserialize!");
                return;
            }

            if (collectionData == null || collectionData.Count == 0)
            {
                return;
            }

            void DeserializeItem(ComplexClassData complexItem, Action<object> setValueCallback)
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
                var collectionInstance = ReflectionUtils.GetDefaultValueInstance(type, collectionData.Count);

                if (collectionData.CollectionType == CollectionType.Dictionary)
                {
                    var dictionary = collectionInstance as IDictionary;
                    var complexDictionary = collectionData as DictionaryIRComplexTypes;
                    for (int i = 0; i < complexDictionary.Count; i++)
                    {
                        object DeserializeArgValue(ComplexClassData complexArg)
                        {
                            object deserializedArgValue = null;

                            if (complexArg.ComplexType.IsSimple())
                            {
                                //if (complexArg.Properties != null && complexArg.Properties.Count > 0)
                                {
                                    deserializedArgValue = ReflectionUtils.DeserializeVariantValueSafe<Tr>(complexArg.Properties[0].Simple);
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

                        var key = DeserializeArgValue(complexDictionary.Keys[i]);
                        var value = DeserializeArgValue(complexDictionary.Values[i]);

                        if (key != null && !dictionary.Contains(key))
                        {
                            dictionary.Add(key, value);
                        }
                    }

                    ReflectionUtils.SetMemberValue(target, dictionary, property.Name);
                }
                else
                {
                    collectionInstance = ReflectionUtils.EnsureCount(collectionInstance, collectionData.Count);
                    int colItemIndex = 0;
                    var complexCol = collectionData as CollectionIRComplexTypes;
                    foreach (var complexItem in complexCol.Value)
                    {
                        DeserializeItem(complexItem, item =>
                        {
                            ReflectionUtils.SetMemberValueSafe(collectionInstance, item, default(MemberInfo), colItemIndex);
                        });
                        colItemIndex++;
                    }

                    ReflectionUtils.SetMemberValue(target, collectionInstance, property.Name);
                }
            }
        }

        private static object GetComplexTypeDataSafe(object arg, ComplexClassData argComplexData,
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
                    return ReflectionUtils.DeserializeEnum<Tr>((VariantIRValue)arg);
                }

                return ((VariantIRValue)arg).GetValueAsObject();
            }

            return arg;
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
