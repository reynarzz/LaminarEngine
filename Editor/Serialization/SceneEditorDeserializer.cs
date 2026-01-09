using Engine;
using Engine.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    internal static class SceneEditorDeserializer
    {
        private readonly static Dictionary<Guid, (Actor value, ActorDataSceneAsset data)> _actorsByID = new();
        private readonly static Dictionary<Guid, (Component value, ComponentDataSceneAsset data)> _componentsByID = new();

        public static void DeserializeScene(IReadOnlyList<ActorDataSceneAsset> actors, WeakReference<Scene> scene)
        {
            _actorsByID.Clear();
            _componentsByID.Clear();

            if (actors == null || actors.Count == 0)
                return;

            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];

                var actor = new Actor(actorData.Name, actorData.ID, scene);
                actor.Layer = actorData.Layer;
                actor.IsActiveSelf = actorData.IsActiveSelf;

                // actor.AddComponent(typeof());
                _actorsByID.Add(actor.GetID(), (actor, actorData));

                // Add components, but no deserialize yet.
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (ReflectionUtils.TryGetTypeFromName(componentData.TypeName, out var componentType))
                    {
                        var component = actor.AddComponent(componentType, componentData.ID, false);
                        component.IsEnabled = componentData.IsEnabled;

                        _componentsByID.Add(componentData.ID, (component, componentData));
                    }
                }
            }

            // Resolve parent-child relationship
            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];

                if (actorData.ParentID != Guid.Empty)
                {
                    _actorsByID[actorData.ID].value.Transform.Parent = _actorsByID[actorData.ParentID].value.Transform;
                }
            }

            // Deserialize components data, and resolve references.
            foreach (var (id, componentValue) in _componentsByID)
            {
                DeserializeComponent(componentValue.value, componentValue.data);
            }
        }

        private static void DeserializeComponent(Component component, ComponentDataSceneAsset data)
        {
            foreach (var property in data.SerializedProperties)
            {
                if (property.Type == SerializableType.Simple ||
                    property.Type == SerializableType.SimpleCollection)
                {
                    DeserializeSimpleProperty(component, property);
                }
                else if (property.Type == SerializableType.Component)
                {
                    DeserializeReferencedProperty(_componentsByID, component, property);
                }
                else if (property.Type == SerializableType.Actor)
                {
                    DeserializeReferencedProperty(_actorsByID, component, property);
                }
            }
        }

        private static void DeserializeReferencedProperty<V, D>(Dictionary<Guid, (V value, D data)> ids,
                                                                object target, ComponentSerializedProperty property)
        {
            if (property.Data == null)
            {
                Debug.EngineError("Serialization error: property data is null.");
                return;
            }

            var guid = (Guid)property.Data;

            if (ids.TryGetValue(guid, out var component))
            {
                ReflectionUtils.SetMemberValue(target, property.Name, component.value);
            }
            else
            {
                Debug.Error($"Could not deserialize value for component: {target.GetType().Name}, Property: {property.Name}");
            }
        }

        private static void DeserializeSimpleProperty(object target, ComponentSerializedProperty property)
        {
            if (property.Data == null)
            {
                return;
            }

            ReflectionUtils.SetMemberValue(target, property.Name, property.Data);
        }

        private static void InstantiateActor()
        {

        }
    }
}
