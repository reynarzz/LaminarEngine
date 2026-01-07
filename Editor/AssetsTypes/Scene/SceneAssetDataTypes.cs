using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class ActorDataSceneAsset
    {
        internal string Name { get; set; }
        internal Guid ParentID { get; set; } = Guid.Empty;
        internal int ParentIndex { get; set; } = 0;
        internal int Index { get; set; } = 0;
        internal Guid ID { get; set; }
        internal int Layer { get; set; }
        public List<ComponentDataSceneAsset> ComponentsData { get; private set; } = new();
    }

    internal class ComponentDataSceneAsset
    {
        internal string TypeName { get; set; }
        internal int ComponentIndex { get; set; }
        internal Guid ID { get; set; }
        internal List<ComponentSerializedProperty> SerializedProperties { get; private set; } = new();
    }

    internal class ComponentSerializedProperty
    {
        internal string Name { get; set; }
        internal PropertyType Type { get; set; }
        internal ComponentPropertyData Data { get; set; }
    }

    internal class ComponentPropertyData
    {

    }

    internal class EObjectTargetPropertyType : ComponentPropertyData
    {
        internal Guid ID { get; set; }
    }

    internal enum PropertyType
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