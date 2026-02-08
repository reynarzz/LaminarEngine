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
        public int Version { get; set; } = 1;
        public string Name { get; set; }
        public int Layer { get; set; }
        public bool IsActiveSelf { get; set; }
        public Guid ID { get; set; }
        public Guid ParentID { get; set; } = Guid.Empty;
        public List<ComponentIR> Components { get; set; }
    }

    internal class ComponentIR
    {
        public int Version { get; set; } = 1;
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

    internal class DictionaryData : SerializedItem
    {
        public SerializedType KeyType { get; set; }
        public SerializedType ValueType { get; set; }

        public object Key { get; set; }
        public object Value { get; set; }
    }

    internal class ComplexDictionaryData : SerializedItem
    {
        public ComplexTypeData Key { get; set; }
        public ComplexTypeData Value { get; set; }
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
    internal enum SerializedType : ulong
    {
        None = 0,

        // Trait flags (Bits 0-19)
        // Category buckets.
        SimpleFlag = 1L << 0,              // 1
        EObjectFlag = 1L << 1,             // 2
        AssetNoEObjectFlag = 1L << 2,      // 4
        CollectionFlag = 1L << 3,          // 8
        ClassFlag = 1L << 4,               // 16
        AssetFlag = EObjectFlag | AssetNoEObjectFlag, // 6

        // ID Shift (20 Bits)
        // We use a 20-bit shift to move the Identity into the 'Millions' range.

        // Simple Types
        Enum = SimpleFlag | (1000UL << 20),
        Char = SimpleFlag | (1001UL << 20),
        String = SimpleFlag | (1002UL << 20),
        Bool = SimpleFlag | (1003UL << 20),
        Byte = SimpleFlag | (1004UL << 20),
        Short = SimpleFlag | (1005UL << 20),
        UShort = SimpleFlag | (1006UL << 20),
        Int = SimpleFlag | (1007UL << 20),
        Uint = SimpleFlag | (1008UL << 20),
        Float = SimpleFlag | (1009UL << 20),
        Double = SimpleFlag | (1010UL << 20),
        Long = SimpleFlag | (1011UL << 20),
        Ulong = SimpleFlag | (1012UL << 20),
        Vec2 = SimpleFlag | (1100UL << 20),
        Vec3 = SimpleFlag | (1101UL << 20),
        Vec4 = SimpleFlag | (1102UL << 20),
        Ivec2 = SimpleFlag | (1103UL << 20),
        Ivec3 = SimpleFlag | (1104UL << 20),
        Ivec4 = SimpleFlag | (1105UL << 20),
        Quat = SimpleFlag | (1106UL << 20),
        Mat2 = SimpleFlag | (1107UL << 20),
        Mat3 = SimpleFlag | (1108UL << 20),
        Mat4 = SimpleFlag | (1109UL << 20),
        Color = SimpleFlag | (1110UL << 20),
        Color32 = SimpleFlag | (1111UL << 20),

        // EObjects
        Component = EObjectFlag | (2000UL << 20),
        Actor = EObjectFlag | (2001UL << 20),

        // Assets 
        SpriteAsset = AssetFlag | (3000UL << 20),
        TextureAsset = AssetFlag | (3001UL << 20),
        MaterialAsset = AssetFlag | (3002UL << 20),
        ShaderAsset = AssetFlag | (3003UL << 20),
        AudioClipAsset = AssetFlag | (3004UL << 20),
        AnimationAsset = AssetFlag | (3005UL << 20),
        AnimatorControllerAsset = AssetFlag | (3006UL << 20),
        RenderTextureAsset = AssetFlag | (3007UL << 20),
        ScriptableObject = AssetFlag | (3008UL << 20),

        // Collections and classes
        ComplexCollection = CollectionFlag | (4000UL << 20),
        ReferenceCollection = CollectionFlag | (4001UL << 20),
        ComplexClass = ClassFlag | (4002UL << 20),
        Delegate = ClassFlag | (4003UL << 20)
    }
}