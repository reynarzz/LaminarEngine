using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Engine.Serialization
{
    internal class GFSDataProperty_polymorphism : JsonConverter
    {
        private const string _typeTag = "$type";
        private const string _valueTag = "$value";
        private const string _valueTypeTag = "$itemType";
        private const string _valueTypesTag = "$itemTypes";
        private const string _polymorphicTag = "polymorphic";
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var type = value.GetType();
            var typeId = $"{type.FullName}, {type.Assembly.GetName().Name}";

            var wrapper = new JObject
            {
                [_typeTag] = typeId
            };

            // Detect collections (exclude string)
            if (type != typeof(string) && value is IEnumerable enumerable)
            {
                Type declaredElementType = null;

                if (type.IsArray)
                {
                    declaredElementType = type.GetElementType();
                }
                else
                {
                    var iface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType &&
                                                                    i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

                    declaredElementType = iface?.GetGenericArguments()[0];
                }

                if (declaredElementType != null)
                {
                    bool hasDerived = false;

                    foreach (var item in enumerable)
                    {
                        if (item == null)
                            continue;

                        var runtimeType = item.GetType();

                        if (runtimeType != declaredElementType &&
                            declaredElementType.IsAssignableFrom(runtimeType))
                        {
                            hasDerived = true;
                            break;
                        }
                    }

                    if (hasDerived)
                    {
                        var elementTypes = new List<Type>();

                        foreach (var item in enumerable)
                        {
                            if (item != null)
                            {
                                elementTypes.Add(item.GetType());
                            }
                        }

                        wrapper[_valueTypeTag] = _polymorphicTag;
                        wrapper[_valueTypesTag] = new JArray(elementTypes.Select(t => $"{t.FullName}, {t.Assembly.GetName().Name}"));
                    }
                    else
                    {
                        wrapper[_valueTypeTag] = $"{declaredElementType.FullName}, {declaredElementType.Assembly.GetName().Name}";
                    }
                }
            }

            wrapper[_valueTag] = JToken.FromObject(value, serializer);
            wrapper.WriteTo(writer);
        }
        public override object ReadJson(JsonReader reader, Type objectType,
                                        object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.StartObject)
            {
                return serializer.Deserialize(reader);
            }

            JObject wrapper = JObject.Load(reader);

            if (!wrapper.TryGetValue(_typeTag, out var typeToken))
            {
                return wrapper.ToObject(objectType, serializer);
            }

            string typeName = typeToken.ToString();

            var type = Type.GetType(typeName);
            //return new SerializedPropertyData()
            //{
            //    TypeName = type.FullName,
            //    Value = wrapper[_valueTag].ToObject(type, serializer)
            //};

            if (wrapper[_valueTypeTag]?.ToString() == _polymorphicTag)
            {
                foreach (var t in wrapper[_valueTypesTag] as JArray)
                {
                    var itemType = Type.GetType(t.ToString());

                }
                return null;
            }
            else if (type != null)
            {
                return wrapper[_valueTag].ToObject(type, serializer);
            }

            Debug.Error($"Type name: '{typeName}' cannot be found, can't deserialize data");
            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }

    }
}

