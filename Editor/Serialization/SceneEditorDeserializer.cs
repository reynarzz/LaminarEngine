using Engine;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal static class SceneEditorDeserializer
    {
        private readonly static Dictionary<Guid, Actor> _actorsByID = new();
        private readonly static Dictionary<Guid, (Component component, ComponentDataSceneAsset data)> _componentsByID = new();

        public static void DeserializeScene(IReadOnlyList<ActorDataSceneAsset> actors, WeakReference<Scene> scene)
        {
            _actorsByID.Clear();
            _componentsByID.Clear();

            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];

                var actor = new Actor(actorData.Name, actorData.ID, scene);
                actor.Layer = actorData.Layer;
                // actor.AddComponent(typeof());
                _actorsByID.Add(actor.GetID(), actor);

                // Add components, but no deserialize yet.
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (ReflectionUtils.TryGetTypeFromName(componentData.TypeName, out var componentType))
                    {
                        var component = actor.AddComponent(componentType, componentData.ID);
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
                    _actorsByID[actorData.ID].Transform.Parent = _actorsByID[actorData.ParentID].Transform;
                }
            }

            // Deserialize components data, and resolve references.
            foreach (var (id, componentValue) in _componentsByID)
            {
                DeserializeComponent(componentValue.component, componentValue.data);
            }
        }

        private static void DeserializeComponent(Component component, ComponentDataSceneAsset data)
        {

        }
        private static void InstantiateActor()
        {

        }
    }
}
