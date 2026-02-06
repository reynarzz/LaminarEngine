using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class ActorIR
    {
        public string Name { get; set; }
        public int Layer { get; set; }
        public bool IsActiveSelf { get; set; }
        public Guid ID { get; set; }
        public Guid ParentID { get; set; } = Guid.Empty;
        public List<ComponentIR> Components { get; set; }
    }

    internal class ComponentIR
    {
        [Obsolete("Use TypeId")]
        public string TypeName { get; set; }
        public Guid TypeId { get; set; }
        public bool IsEnabled { get; set; }
        public Guid ID { get; set; }
        public List<SerializedPropertyIR> SerializedProperties { get; set; }
    }

    internal class SerializedPropertyIR
    {
        public string Name { get; set; }
        public SerializedType Type { get; set; }
        [Obsolete("Use TypeId")]
        public string InternalType { get; set; }
        public Guid TypeId { get; set; }
        public object Data { get; set; }
    }

    internal class SerializedItem
    {
        public SerializedType Type { get; set; }
    }

    internal class ReferenceData
    {
        public Guid Id { get; set; }
    }

    internal class SpriteReferenceData : ReferenceData
    {
        public int AtlasIndex { get; set; }
        public Guid TextureId { get; set; }
    }

    internal class CollectionPropertyData
    {
        public CollectionType CollectionType { get; set; }
        public List<object> Collection { get; set; } = new();
    }

    internal class DictionaryData<K, V> : SerializedItem
    {
        public SerializedType keyType { get; set; }
        public SerializedType ValueType { get; set; }

        public K Key { get; set; }
        public V Value { get; set; }
    }

    internal class ComplexDictionaryData<K, V> : SerializedItem
    {
        public K Key { get; set; }
        public V Value { get; set; }
    }

    internal class CollectionData<V> : SerializedItem
    {
        public V Value { get; set; }
    }

    internal class DelegateData : SerializedItem
    {
        internal class Subscriber
        {
            public Guid TypeId { get; set; }
            public string MethodName { get; set; }
            public ReferenceData Reference { get; set; }
        }
    }

    internal class ComplexTypeData
    {
        [Obsolete("Use TypeId")]
        public string TargetTypeName { get; set; }
        public Guid TypeId { get; set; }
        public SerializedType ComplexType { get; set; }
        public List<SerializedPropertyIR> Properties { get; set; }
    }

    internal enum SerializedType
    {
        None,
        EObject, // Reference to a generic EObject...
        Component,
        Actor,
        ComplexClass,
        /// <summary>
        /// Has multiple classes (any deep) and IObjects: classA -> classB -> reference.
        /// </summary>
        ComplexCollection,
        Asset,
        TextureAsset,
        SpriteAsset,
        RenderTextureAsset,
        AudioClipAsset,
        ShaderAsset,
        MaterialAsset,
        AnimationAsset,
        AnimatorControllerAsset,
        ScriptableObject,
        Delegate,

        /// <summary>
        /// Internal engine types: int, string, Color32, enums etc...
        /// </summary>
        Simple,
        [Obsolete]
        SimpleClass,
        [Obsolete]
        SimpleCollection,
        ReferenceCollection,
    }
}