using Engine;
using Engine.Serialization;
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
    internal class SerializedEditorScene
    {
        public string Name { get; set; }
        public Guid RefId { get; set; }
        public List<ActorIR> ActorsData { get; set; }
        internal List<Actor> Actors { get; set; }
    }

    // Note: This is a very naive serializaton, renaming types, changing to another assembly will cause references to break.
    //       But for this development stage, this is fine, and will not be difficult to change later.
    internal static class SceneSerializer
    {
        internal struct SerializationOptions
        {
            public bool RemoveGameDLLComponentsFromActors;
            public bool CollectedPhysicalActors;
        }
        private readonly static List<Component> _componentsToRemove = new();
        internal static SceneIR SerializeScene(Scene scene)
        {
            var options = new SerializationOptions()
            {
                CollectedPhysicalActors = false,
                RemoveGameDLLComponentsFromActors = false
            };

            var sceneEditor = SerializeSceneEditor(scene, options);

            return new SceneIR()
            {
                Actors = sceneEditor.ActorsData
            };
        }
        internal static SerializedEditorScene SerializeSceneEditor(Scene scene, SerializationOptions options = default)
        {
            _componentsToRemove.Clear();

            var serializedScene = new SerializedEditorScene()
            {
                Name = scene.Name,
                RefId = scene.GetID(),
                ActorsData = new(),
                Actors = new(),
            };

            foreach (var actor in scene.RootActors)
            {
                SerializeActors(actor, serializedScene.Actors, serializedScene.ActorsData, options);
            }

            for (var i = 0; i < _componentsToRemove.Count; i++)
            {
                var component = _componentsToRemove[i];
                component.Actor.Components.Remove(component);
            }
            _componentsToRemove.Clear();
            return serializedScene;
        }

        internal static List<ActorIR> SerializeActorsTree(Actor actor)
        {
            var actorsTree = new List<ActorIR>();
            SerializeActors(actor, null, actorsTree, default);
            return actorsTree;
        }

        private static void SerializeActors(Actor actor, List<Actor> actors, List<ActorIR> actorsIR, SerializationOptions options)
        {
            if (!actor)
                return;

            if (options.CollectedPhysicalActors)
            {
                actors.Add(actor);
            }
            actorsIR.Add(GetActor(actor, options));

            foreach (var child in actor.Transform.Children)
            {
                SerializeActors(child.Actor, actors, actorsIR, options);
            }
        }

        private static ActorIR GetActor(Actor actor, SerializationOptions options)
        {
            return new ActorIR()
            {
                Name = actor.Name,
                IsActiveSelf = actor.IsActiveSelf,
                Layer = actor.Layer,
                ID = actor.GetID(),
                ParentID = actor.Transform?.Parent?.Actor?.GetID() ?? Guid.Empty,
                Components = GetAllComponentsData(actor, options),
            };
        }

        internal static List<ComponentIR> GetAllComponentsData(Actor actor, SerializationOptions options)
        {
            var componentsData = new List<ComponentIR>();

            bool IsFromGameDll(Component component)
            {
                return component.GetType().Assembly.GetName().Name.Contains(EditorPaths.GAME_PROJECT_NAME);
            }
            for (int i = 0; i < actor.Components.Count; i++)
            {
                componentsData.Add(GetComponentData(actor.Components[i]));
            }

            if (options.RemoveGameDLLComponentsFromActors)
            {
                for (int i = 0; i < actor.Components.Count; i++)
                {
                    if (IsFromGameDll(actor.Components[i]))
                    {
                        _componentsToRemove.Add(actor.Components[i]);
                    }
                }
            }

            return componentsData;
        }

        internal static ComponentIR GetComponentData(Component component)
        {
            return new ComponentIR()
            {
                ID = component.GetID(),
                TypeId = ReflectionUtils.GetStableGuid(component.GetType()),
                IsEnabled = component.IsEnabled,
                InternalType = ReflectionUtils.GetFullTypeName(component.GetType()),
                Properties = Serializer.Serialize(component)
            };
        }
    }
}