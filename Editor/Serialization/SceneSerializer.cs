using Engine;
using Engine.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    internal class SerializedScene
    {
        public string Name { get; set; }
        public List<ActorDataSceneAsset> ActorsData { get; set; }
        [JsonIgnore]internal List<Actor> Actors { get; set; }
    }

    // Note: This is a very naive serializaton, renaming types, changing to another assembly will cause references to break.
    //       But for this development stage, this is fine, and will not be difficult to change later.
    internal static class SceneSerializer
    {
        internal struct SerializationOptions
        {
            public bool RemoveGameDLLComponentsFromActors;
            public bool CollectedPhysicalActors;
            public bool SerializeOnlyGameDLLComponents;
        }
        internal static SerializedScene SerializeScene(Scene scene, SerializationOptions options = default)
        {
            var serializedScene = new SerializedScene()
            {
                Name = scene.Name,
                ActorsData = new(),
                Actors = new()
            };

            void SerializeActors(Actor actor)
            {
                if (!actor)
                    return;

                if (options.CollectedPhysicalActors)
                {
                    serializedScene.Actors.Add(actor);
                }
                serializedScene.ActorsData.Add(GetActor(actor, options));

                foreach (var child in actor.Transform.Children)
                {
                    SerializeActors(child.Actor);
                }
            }

            foreach (var actor in scene.RootActors)
            {
                SerializeActors(actor);
            }

            return serializedScene;
        }

        internal static ActorDataSceneAsset GetActor(Actor actor, SerializationOptions options)
        {
            return new ActorDataSceneAsset()
            {
                Name = actor.Name,
                IsActiveSelf = actor.IsActiveSelf,
                Layer = actor.Layer,
                ID = actor.GetID(),
                ParentID = actor.Transform?.Parent?.Actor?.GetID() ?? Guid.Empty,
                Components = GetAllComponentsData(actor, options),
            };
        }

        internal static List<ComponentDataSceneAsset> GetAllComponentsData(Actor actor, SerializationOptions options)
        {
            var componentsData = new List<ComponentDataSceneAsset>();

            bool IsFromGameDll(Component component)
            {
                return component.GetType().Assembly.GetName().Name.Contains(EditorPaths.GAME_PROJECT_NAME);
            }
            for (int i = 0; i < actor.Components.Count; i++)
            {
                if (!options.SerializeOnlyGameDLLComponents || 
                    (options.SerializeOnlyGameDLLComponents && IsFromGameDll(actor.Components[i])))
                {
                    componentsData.Add(GetComponentData(actor.Components[i]));
                }
            }

            if (options.RemoveGameDLLComponentsFromActors)
            {
                for (int i = actor.Components.Count - 1; i >= 0; --i)
                {
                    if (IsFromGameDll(actor.Components[i]))
                    {
                        actor.Components.Remove(actor.Components[i]);
                    }
                }
            }

            return componentsData;
        }

        internal static ComponentDataSceneAsset GetComponentData(Component component)
        {
            return new ComponentDataSceneAsset()
            {
                ID = component.GetID(),
                IsEnabled = component.IsEnabled,
                TypeName = ReflectionUtils.GetFullTypeName(component.GetType()),
                SerializedProperties = Serializer.Serialize(component)
            };
        }
    }
}