using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class ActorDataSceneAsset
    {
        public string Name { get; set; }
        public int Layer { get; set; }
        public int ParentIndex { get; set; } = 0;
        public int Index { get; set; } = 0;
        public Guid ID { get; set; }
        public Guid ParentID { get; set; } = Guid.Empty;
        public List<ComponentDataSceneAsset> Components { get; set; }
    }

    internal class ComponentDataSceneAsset
    {
        public string TypeName { get; set; }
        public Guid ID { get; set; }
        public int ComponentIndex { get; set; }
        public List<ComponentSerializedProperty> SerializedProperties { get; set; }
    }

    internal class ComponentSerializedProperty
    {
        public string Name { get; set; }
        public SerializableType Type { get; set; }
        public SerializedPropertyData Data { get; set; }
    }

    internal abstract class SerializedPropertyData
    {
        public string TypeName { get; set; }
    }

    internal class EObjectSerializedProperty : SerializedPropertyData
    {
        public Guid ID { get; set; }
    }

    internal class SimpleSerializedProperty : SerializedPropertyData
    {
        public object Value { get; set; }
    }

    internal enum SerializableType
    {
        None,
        EObject, // Reference to a generic EObject...
        Component,
        Actor,
        Number,
        Class,
        Array,
        Dictionary,
        Asset,
        TextureAsset,
        RenderTexture,
        MaterialAsset,
        AnimationAsset,
        AnimatorAsset,
        ScriptableObject
    }
}