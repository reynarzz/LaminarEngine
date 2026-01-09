using Editor.Serialization;
using Newtonsoft.Json;
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
        public int ComponentIndex { get; set; }
        public List<ComponentSerializedProperty> SerializedProperties { get; set; }
    }

    internal class ComponentSerializedProperty
    {
        public string Name { get; set; }
        public SerializedType Type { get; set; }

        [JsonConverter(typeof(GFSDataProperty))]
        public object Data { get; set; }
    }

    internal class SerializedPropertyData 
    {
        public string TypeName { get; set; }
        public object Value { get; set; }
    }

    internal enum SerializedType
    {
        None,
        EObject, // Reference to a generic EObject...
        Component,
        Actor,
        Class,
        Collection,
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
        SimpleCollection,
        ReferenceCollection
    }
}