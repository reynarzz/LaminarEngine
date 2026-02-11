using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
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

        public Variant Simple { get; set; }
        public ReferenceData Reference { get; set; }
        public ComplexData Complex { get; set; }
        public CollectionData Collection { get; set; }
    }

    internal class ReferenceData
    {
        public SerializedType Type { get; set; }
        public Guid Id { get; set; }
    }

    internal class SpriteReferenceData : ReferenceData
    {
        public int AtlasIndex { get; set; }
        public Guid TextureId { get; set; }
    }

    internal abstract class CollectionData
    {
        public CollectionType CollectionType { get; set; }
        internal abstract int Count { get; }
    }

    internal abstract class DictionaryDataT<K, V> : CollectionData
    {
        [SerializedField] public K[] Keys { get; set; }
        [SerializedField] public V[] Values { get; set; }

        internal override int Count => Keys?.Length ?? 0;

        // Serializer
        protected DictionaryDataT() { }
        public DictionaryDataT(int size, CollectionType collectionType)
        {
            Keys = new K[size];
            Values = new V[size];
            CollectionType = collectionType;
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            for (int i = 0; i < Count; i++)
            {
                array.SetValue(new KeyValuePair<K, V>(Keys[i], Values[i]), index + i);
            }
        }

        public IEnumerator GetEnumerator()
        {
            if (Keys == null || Values == null) yield break;

            int length = Math.Min(Keys.Length, Values.Length);
            for (int i = 0; i < length; i++)
            {
                yield return new KeyValuePair<K, V>(Keys[i], Values[i]);
            }
        }
    }

    internal class DictionaryDataKVTypes<K, V, KT, VT> : DictionaryDataT<K, V>
    {
        public KT KeyType { get; set; }
        public VT ValueType { get; set; }
        protected DictionaryDataKVTypes() { }
        internal DictionaryDataKVTypes(int size, CollectionType collectionType) : base(size, collectionType) { }
    }
    internal class CollectionData<T, IT> : CollectionData
    {
        public T[] Value { get; set; }
        internal override int Count => Value?.Length ?? 0;
        public IT ItemsType { get; set; }

        protected CollectionData() { }
        protected CollectionData(int size, CollectionType collectionType)
        {
            Value = new T[size];
            CollectionType = collectionType;
        }
        public CollectionData(T[] value, IT itemsType, CollectionType collectionType)
        {
            Value = value;
            ItemsType = itemsType;
            CollectionType = collectionType;
        }
    }

    internal class DictionaryIRReferences : DictionaryDataKVTypes<object, object, SerializedType[], SerializedType[]>
    {
        protected DictionaryIRReferences() { }
        public DictionaryIRReferences(int size, CollectionType collectionType) : base(size, collectionType)
        {
            KeyType = new SerializedType[size];
            ValueType = new SerializedType[size];
        }
    }
    internal class DictionaryIRVariants : DictionaryDataKVTypes<Variant, Variant, SerializedType, SerializedType>
    {
        protected DictionaryIRVariants() { }
        public DictionaryIRVariants(int size, CollectionType collectionType) :
            base(size, collectionType)
        { }
    }
    internal class DictionaryIRComplexTypes : DictionaryDataT<ComplexData, ComplexData>
    {
        protected DictionaryIRComplexTypes() { }
        public DictionaryIRComplexTypes(int size, CollectionType collectionType) :
            base(size, collectionType)
        { }
    }
    internal class CollectionIRVariants : CollectionData<Variant, SerializedType>
    {
        protected CollectionIRVariants() { }
        internal CollectionIRVariants(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionIRVariants(Variant[] value, SerializedType itemsType, CollectionType colType) :
            base(value, itemsType, colType)
        { }
    }
    internal class CollectionIRReferences : CollectionData<ReferenceData, SerializedType[]>
    {
        protected CollectionIRReferences() { }
        internal CollectionIRReferences(int size, CollectionType colType) : base(size, colType)
        {
            ItemsType = new SerializedType[size];
        }
        public CollectionIRReferences(ReferenceData[] value, SerializedType[] itemsType, CollectionType collectionType) :
            base(value, itemsType, collectionType)
        { }
    }
    internal class CollectionIRComplexTypes : CollectionData<ComplexData, SerializedType>
    {
        protected CollectionIRComplexTypes() { }
        internal CollectionIRComplexTypes(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionIRComplexTypes(ComplexData[] value, CollectionType colType) : base(value, value != null && value.Length > 0 ?
                                                                                                SerializedType.ComplexClass :
                                                                                                SerializedType.None, colType)
        { }
    }
    internal class DelegateData
    {
        internal class Subscriber
        {
            public Guid TypeId { get; set; }
            public string MethodName { get; set; }
            public ReferenceData Reference { get; set; }
        }
    }

    internal class ComplexData
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
        UInt = SimpleFlag | (1008UL << 20),
        Float = SimpleFlag | (1009UL << 20),
        Double = SimpleFlag | (1010UL << 20),
        Long = SimpleFlag | (1011UL << 20),
        ULong = SimpleFlag | (1012UL << 20),
        Vec2 = SimpleFlag | (1100UL << 20),
        Vec3 = SimpleFlag | (1101UL << 20),
        Vec4 = SimpleFlag | (1102UL << 20),
        IVec2 = SimpleFlag | (1103UL << 20),
        IVec3 = SimpleFlag | (1104UL << 20),
        IVec4 = SimpleFlag | (1105UL << 20),
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
        SimpleCollection = CollectionFlag | (4001UL << 20),
        ReferenceCollection = CollectionFlag | (4002UL << 20),
        ComplexClass = ClassFlag | (4003UL << 20),
        Delegate = ClassFlag | (4004UL << 20)
    }

}