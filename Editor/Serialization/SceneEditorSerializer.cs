using Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal static class SceneEditorSerializer
    {
        internal static List<ActorDataSceneAsset> SerializeScene(Scene scene)
        {
            int actorIndex = 0;
            var actors = new List<ActorDataSceneAsset>();

            void SerializeActors(Actor actor)
            {
                if (!actor)
                    return;

                actors.Add(GetActor(actor, ref actorIndex));

                foreach (var child in actor.Transform.Children)
                {
                    SerializeActors(child.Actor);
                }
            }
            
            foreach (var actor in scene.RootActors)
            {
                SerializeActors(actor);
            }

            return actors;
        }

        internal static ActorDataSceneAsset GetActor(Actor actor, ref int index)
        {
            return new ActorDataSceneAsset()
            {
                Name = actor.Name,
                Layer = actor.Layer,
                ID = actor.GetID(),
                Index = index++,
                ParentID = actor.Transform?.Parent?.GetID() ?? Guid.Empty,
                ComponentsData = GetAllComponentsData(actor),
            };
        }

        internal static List<ComponentDataSceneAsset> GetAllComponentsData(Actor actor)
        {
            var componentsData = new List<ComponentDataSceneAsset>();

            for (int i = 0; i < actor.Components.Count; i++)
            {
                componentsData.Add(GetComponentData(actor.Components[i], i));
            }
            return componentsData;
        }

        internal static ComponentDataSceneAsset GetComponentData(Component component, int index)
        {
            return new ComponentDataSceneAsset()
            {
                ComponentIndex = index,
                ID = component.GetID(),
                TypeName = component.GetType().Name,

            };
        }
    }
}
