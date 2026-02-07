using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class SceneIR
    {
        public int Version { get; set; }
        public List<ActorIR> Actors { get; set; }
    }

    internal class ActorIR
    {
        public int Version { get; set; }
        public string Name { get; set; }
        public int Layer { get; set; }
        public bool IsActiveSelf { get; set; }
        public Guid ID { get; set; }
        public Guid ParentID { get; set; } = Guid.Empty;
        public List<ComponentIR> Components { get; set; }
    }

    internal class ComponentIR
    {
        public int Version { get; set; }
        public string InternalType { get; set; }
        public Guid TypeId { get; set; }
        public bool IsEnabled { get; set; }
        public Guid ID { get; set; }
        public List<SerializedPropertyIR> SerializedProperties { get; set; }
    }

    internal class SerializedPropertyIR
    {
        public string Name { get; set; }
        public SerializedType Type { get; set; }
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
        public string InternalType { get; set; }
        public Guid TypeId { get; set; }
        public SerializedType ComplexType { get; set; }
        public List<SerializedPropertyIR> Properties { get; set; }
    }

    [Flags]
    internal enum SerializedType : uint
    {
        None = 0,

        // Categories
        EObject = 1 << 0,   
        Asset = EObject | 1 << 1,     
        Simple = 1 << 2,    

        // EObjects
        Component = EObject | (1 << 3),
        Actor = EObject | (1 << 4),
        SpriteAsset = EObject | (1 << 5),

        // Assets
        TextureAsset = Asset | (1 << 6),
        MaterialAsset = Asset | (1 << 7),
        ShaderAsset = Asset | (1 << 8),
        AudioClipAsset = Asset | (1 << 9),
        AnimationAsset = Asset | (1 << 10),
        RenderTextureAsset = Asset | (1 << 11),
        AnimatorControllerAsset = Asset | (1 << 12),
        ScriptableObject = Asset | (1 << 13),

        // Collections
        ComplexCollection = 1 << 14,
        ReferenceCollection = 1 << 15,
        ComplexClass = 1 << 16,
        Delegate = 1 << 17,
    }

    internal enum SerializedSimpleType
    {
        None,
        Enum,
        Char,
        String,
        Bool,
        Byte,
        Short,
        UShort,
        Int,
        Uint,
        Float,
        Double,
        Long,
        Ulong,
        Vec2,
        Vec3,
        Vec4,
        Ivec2,
        Ivec3,
        Ivec4,
        Quat,
        Mat2,
        Mat3,
        Mat4,
        Color,
        Color32
    }
}