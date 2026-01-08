using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal static class SceneEditorDeserializer
    {
        private readonly static Dictionary<Guid, Actor> _actorsByID = new();
        private readonly static Dictionary<Guid, Component> _componentsByID = new();
        private readonly static List<(Component, ComponentDataSceneAsset)> _componentsToResolve = new();

        public static void DeserializeScene(IReadOnlyList<ActorDataSceneAsset> actors, WeakReference<Scene> scene)
        {
            _actorsByID.Clear();
            _componentsByID.Clear();
            _componentsToResolve.Clear();

            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];

                var actor = new Actor(actorData.Name, actorData.ID, scene);
                actor.Layer = actorData.Layer;
                // actor.AddComponent(typeof());
                _actorsByID.Add(actor.GetID(), actor);
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
        }

        private static void InstantiateActor()
        {

        }
    }
}
