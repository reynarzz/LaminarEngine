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
        public int SceneVersion { get; set; } = 1;
        public int ActorsVersion { get; set; } = 1;
        public int ComponentsVersion { get; set; } = 1;

        public List<ActorIR> Actors { get; set; }
    }

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
        public string InternalType { get; set; }
        public Guid TypeId { get; set; }
        public bool IsEnabled { get; set; }
        public Guid ID { get; set; }
        public SerializedPropertyIR[] Properties { get; set; }
    }

    internal class SerializedPropertyIR
    {
        public string Name { get; set; }
        public SerializedType Type { get; set; }
        public string InternalType { get; set; }
        public Guid TypeId { get; set; }

        public Variant Simple { get; set; }
        public ReferenceData Reference { get; set; }
        public ClassData Class { get; set; }
        public CollectionData Collection { get; set; }
    }

    internal class ReferenceData
    {
        public SerializedType Type { get; set; }
        public Guid RefId { get; set; }
    }

    internal class SpriteReferenceData : ReferenceData
    {
        public int AtlasIndex { get; set; }
        public Guid TexRefId { get; set; }
    }

    internal abstract class CollectionData
    {
        public CollectionType CollectionType { get; set; }
        internal abstract int Count { get; }
    }
    internal interface IItemType<T> 
    {
        public T ItemsType { get; }
    }

    internal abstract class CollectionData<T, IT> : CollectionData, IItemType<IT>
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
    internal class CollectionData<T> : CollectionData<T, SerializedType>
    {
        protected CollectionData() { }
        public CollectionData(T[] value, SerializedType itemsType, CollectionType colType) : base(value, itemsType, colType) { }
    }

    internal abstract class DictionaryDataT<K, V> : CollectionData
    {
        [SerializedField] public K[] Keys { get; set; }
        [SerializedField] public V[] Values { get; set; }

        internal override int Count => Keys?.Length ?? 0;

        // Serializer
        protected DictionaryDataT() { }
        // NOTE: the collectionType is to support orderedDictionary in the future.
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
 
    internal class DictionaryReference<K, V> : DictionaryDataKVTypes<K, V, SerializedType[], SerializedType[]>
    {
        protected DictionaryReference() { }
        public DictionaryReference(int size, CollectionType collectionType) : base(size, collectionType)
        {
            KeyType = new SerializedType[size];
            ValueType = new SerializedType[size];
        }
    }
    // Remove this and use the concrete ones.------
    internal class DictionaryIRReferences : DictionaryReference<object, object>
    {
        protected DictionaryIRReferences() { }
        public DictionaryIRReferences(int size, CollectionType collectionType) : base(size, collectionType) { }
    }
    //---------------------


    internal class DictionaryKeyRefValueRef : DictionaryDataKVTypes<ReferenceData, ReferenceData, SerializedType[], SerializedType[]>
    {
        protected DictionaryKeyRefValueRef() { }
        public DictionaryKeyRefValueRef(int size, CollectionType collectionType) : base(size, collectionType)
        {
            KeyType = new SerializedType[size];
            ValueType = new SerializedType[size];
        }
    }
    internal class DictionaryKeyRefValueSimple : DictionaryDataKVTypes<ReferenceData, Variant, SerializedType[], SerializedType>
    {
        protected DictionaryKeyRefValueSimple() { }
        public DictionaryKeyRefValueSimple(int size, CollectionType collectionType) : base(size, collectionType) { }
    }
    internal class DictionaryKeyRefValueClass : DictionaryDataKVTypes<ReferenceData, ClassData, SerializedType[], SerializedType>
    {
        protected DictionaryKeyRefValueClass() { }
        public DictionaryKeyRefValueClass(int size, CollectionType collectionType) : base(size, collectionType) { }
    }
    internal class DictionaryKeySimpleValueRef : DictionaryDataKVTypes<Variant, ReferenceData, SerializedType, SerializedType[]>
    {
        protected DictionaryKeySimpleValueRef() { }
        public DictionaryKeySimpleValueRef(int size, CollectionType collectionType) : base(size, collectionType) { }
    }
    internal class DictionaryKeyClassValueRef : DictionaryDataKVTypes<ClassData, ReferenceData, SerializedType, SerializedType[]>
    {
        protected DictionaryKeyClassValueRef() { }
        public DictionaryKeyClassValueRef(int size, CollectionType collectionType) : base(size, collectionType) { }
    }
    internal class DictionarySimple : DictionaryDataKVTypes<Variant, Variant, SerializedType, SerializedType>
    {
        protected DictionarySimple() { }
        public DictionarySimple(int size, CollectionType collectionType) :
            base(size, collectionType)
        { }
    }
    internal class DictionaryClass : DictionaryDataKVTypes<ClassData, ClassData, SerializedType, SerializedType>
    {
        protected DictionaryClass() { }
        public DictionaryClass(int size, CollectionType collectionType) :
            base(size, collectionType)
        { }
    }
    internal class CollectionReferences : CollectionData<ReferenceData, SerializedType[]>
    {
        protected CollectionReferences() { }
        internal CollectionReferences(int size, CollectionType colType) : base(size, colType)
        {
            ItemsType = new SerializedType[size];
        }
        public CollectionReferences(ReferenceData[] value, SerializedType[] itemsType, CollectionType collectionType) :
            base(value, itemsType, collectionType)
        { }
    }
    internal class CollectionClasses : CollectionData<ClassData, SerializedType>
    {
        protected CollectionClasses() { }
        internal CollectionClasses(int size, CollectionType collectionType) : base(size, collectionType) { }
        public CollectionClasses(ClassData[] value, CollectionType colType) : base(value, value != null && value.Length > 0 ?
                                                                                                SerializedType.ComplexClass :
                                                                                                SerializedType.None, colType)
        { }
    }
    internal class ClassData
    {
        public string InternalType { get; set; }
        public Guid TypeId { get; set; }
    }

    internal class DelegateClassData : ClassData
    {
        internal class Subscriber
        {
            public Guid TypeId { get; set; }
            public string MethodName { get; set; }
            public ReferenceData Reference { get; set; }
        }
    }


    // TODO: After the refactoring, ClassData still behaves as ComplexClass, I will spli concerns once I implement delegates.
    internal class ComplexClass : ClassData
    {
        public SerializedType ClassType { get; set; }
        public SerializedPropertyIR[] Properties { get; set; }
    }


    [Flags]
    internal enum SerializedType : ulong
    {
        None = 0,

        // Trait flags
        // Category buckets.
        SimpleFlag = 1L << 0,              // 1
        EObjectFlag = 1L << 1,             // 2
        AssetFlag = 1L << 2,                // 6
        CollectionFlag = 1L << 3,          // 8
        ClassFlag = 1L << 4,               // 16

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
        ASSETS_START = AssetFlag | EObjectFlag | (2999UL << 20), // Assets start

        SpriteAsset = AssetFlag | EObjectFlag | (3000UL << 20),
        TextureAsset = AssetFlag | EObjectFlag | (3001UL << 20),
        MaterialAsset = AssetFlag | EObjectFlag | (3002UL << 20),
        ShaderAsset = AssetFlag | EObjectFlag | (3003UL << 20),
        AudioClipAsset = AssetFlag | EObjectFlag | (3004UL << 20),
        AnimationAsset = AssetFlag | EObjectFlag | (3005UL << 20),
        AnimatorControllerAsset = AssetFlag | EObjectFlag | (3006UL << 20),
        RenderTextureAsset = AssetFlag | EObjectFlag | (3007UL << 20),
        ScriptableObject = AssetFlag | EObjectFlag | (3008UL << 20),
        Tilemap = AssetFlag | EObjectFlag | (3009UL << 20),
        Prefab = AssetFlag | EObjectFlag | (3010UL << 20),
        Scene = AssetFlag | EObjectFlag | (3011UL << 20),
        Font = AssetFlag | EObjectFlag | (3012UL << 20),


        ASSETS_END = AssetFlag | EObjectFlag | (3999UL << 20), // max asset boundary

        // Collections and classes
        ComplexCollection = CollectionFlag | (4000UL << 20),
        SimpleCollection = CollectionFlag | (4001UL << 20),
        ReferenceCollection = CollectionFlag | (4002UL << 20),
        ComplexClass = ClassFlag | (4003UL << 20),
        Delegate = ClassFlag | (4004UL << 20)
    }
}