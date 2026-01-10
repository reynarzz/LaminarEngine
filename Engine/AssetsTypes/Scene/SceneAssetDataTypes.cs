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
        public bool IsActiveSelf { get; set; }
        public Guid ID { get; set; }
        public Guid ParentID { get; set; } = Guid.Empty;
        public List<ComponentDataSceneAsset> Components { get; set; }
    }

    internal class ComponentDataSceneAsset
    {
        public string TypeName { get; set; }
        public bool IsEnabled { get; set; }
        public Guid ID { get; set; }
        public List<SerializedPropertyData> SerializedProperties { get; set; }
    }

    internal class SerializedPropertyData
    {
        public string Name { get; set; }
        public SerializedType Type { get; set; }
        public object Data { get; set; }
    }

    internal class CollectionPropertyData
    {
        public object Metadata { get; set; }
        public List<object> Collection { get; set; } = new();
    }

    internal class DictionaryMetadata
    {

    }

    internal class ComplexTypeData
    {
        public string TargetTypeName { get; set; }
        public SerializedType ComplexType { get; set; }
        public List<SerializedPropertyData> Properties { get; set; }
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
        RenderTextureAsset,
        AudioClipAsset,
        MaterialAsset,
        AnimationAsset,
        AnimatorControllerAsset,
        ScriptableObject,

        /// <summary>
        /// Internal engine types: int, string, Color32, enums etc...
        /// </summary>
        Simple,
        SimpleClass,
        SimpleCollection,
        ReferenceCollection
    }
}