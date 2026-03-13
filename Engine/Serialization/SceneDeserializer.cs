using Engine;
using Engine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{

    internal class SceneDeserializer :
#if SHIP_BUILD
        SceneDeserializer<TypeResolver>
#else
        SceneDeserializer<CombinedTypeResolver>
#endif
    { }
    internal class SceneDeserializer<T> where T : ITypeResolver
    {
        private readonly static Dictionary<Guid, (Actor value, ActorIR data)> _actorsByID = new();
        private readonly static Dictionary<Guid, (Component value, ComponentIR data)> _componentsByID = new();

        private static readonly List<Component> _initializationComponents = new();
        private static readonly Dictionary<Guid, Component> _existantComponents = new();
        public static void DeserializeSceneComponents(IReadOnlyList<Actor> actors, IReadOnlyList<ActorIR> actorsData)
        {
            _actorsByID.Clear();
            _componentsByID.Clear();
            _existantComponents.Clear();

            if (actorsData == null || actorsData.Count == 0)
                return;

            for (int i = 0; i < actorsData.Count; i++)
            {
                var actorData = actorsData[i];

                _actorsByID[actorData.ID] = (actors[i], actorData);

                for (int j = 0; j < actors[i].Components.Count; j++)
                {
                    var existantComp = actors[i].Components[j];
                    _existantComponents.Add(existantComp.GetID(), existantComp);
                }
            }

            //foreach (var actor in actors)
            //{
            //    // Add current alive components.
            //    foreach (var component in actor.Components)
            //    {
            //        _componentsByID.Add(component.GetID(), (component, new ComponentDataSceneAsset() 
            //        {
            //              ID = component.GetID(),
            //              IsEnabled = component.IsEnabled,
            //              TypeName = ReflectionUtils.GetFullTypeName(component.GetType()),
            //        }));
            //    }
            //}

            for (int i = 0; i < actorsData.Count; i++)
            {
                var actorData = actorsData[i];
                var actor = _actorsByID[actorData.ID].value;
                // Add components, but no deserialize yet.
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (T.ResolveType(componentData, out var componentType))
                    {
                        // TODO: fix the component initialization for the ones that auto add other required components.
                        // When 'Application.IsInPlayMode' is on, it will auto add components, making this not usable.

                        Component component = null;

                        if (!_existantComponents.TryGetValue(componentData.ID, out component))
                        {
                            component = actor.AddComponent(componentType, componentData.ID, false,
                                                           componentData.IsEnabled, true, out var isPendingToInitialize);
#if DEBUG
                            component.Transform.SyncLocalEulerDelta(true);
#endif
                        }

                        _componentsByID.Add(componentData.ID, (component, componentData));
                    }
                }

                // Resolve required properties
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (_componentsByID.TryGetValue(componentData.ID, out var comp))
                    {
                        var component = comp.value;

                        var reqProperties = ReflectionUtils.GetAllMembersWithAttribute<RequiredPropertyAttribute>(component.GetType())?.ToList();

                        if (reqProperties != null && reqProperties.Count > 0)
                        {
                            for (int k = 0; k < reqProperties.Count; k++)
                            {
                                var prop = reqProperties[k];
                                var reqComponent = component.GetComponent(ReflectionUtils.GetMemberType(prop));
                                ReflectionUtils.SetMemberValue(component, prop, reqComponent);
                            }
                        }
                    }
                }
            }

            var deserializerData = new DeserializerData()
            {
                ActorsByID = _actorsByID,
                ComponentsByID = _componentsByID,
            };

            // Deserialize components data, and resolve references.
            foreach (var (id, componentValue) in _componentsByID)
            {
                Deserializer.DeserializeTarget(componentValue.value, componentValue.data.Properties, deserializerData);
            }

            _actorsByID.Clear();
            _componentsByID.Clear();
            _existantComponents.Clear();
        }
        public static void DeserializeScene(SceneIR sceneIr, Scene scene)
        {
            DeserializeScene(sceneIr.Actors, scene, false);
        }
        public static void DeserializeScene(List<ActorIR> actors, Scene scene)
        {
            DeserializeScene(actors, scene, true);
        }

        /// <summary>
        /// Deserialize actors to the scene.
        /// </summary>
        /// <param name="actors"></param>
        /// <param name="scene"></param>
        /// <param name="newIds"></param>
        /// <returns>Root actors (actors without parent)</returns>
        public static List<Actor> DeserializeScene(List<ActorIR> actors, Scene scene, bool collectRootActors)
        {
            _actorsByID.Clear();
            _componentsByID.Clear();
            _initializationComponents.Clear();

            if (actors == null || actors.Count == 0)
                return null;

            List<Actor> rootActors = null;

            if (collectRootActors)
            {
                rootActors = new List<Actor>();
            }
            // Instantiate all the actors.
            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];
                var actor = new Actor(actorData.Name, actorData.ID, scene);
                actor.Layer = actorData.Layer;
                actor.IsActiveSelf = actorData.IsActiveSelf;
                _actorsByID.Add(actor.GetID(), (actor, actorData));
            }

            if (collectRootActors)
            {
                // NOTE: This truly collects the root actors without relying on IRData since
                //       actors could be deserialized without the real parent (aka: duplicate children).
                for (int i = 0; i < actors.Count; i++)
                {
                    var actorData = actors[i];
                    if (actorData.ParentID == Guid.Empty || !_actorsByID.ContainsKey(actorData.ParentID) )
                    {
                        rootActors.Add(_actorsByID[actorData.ID].value);
                    }
                }
            }

            // Resolve parent-child relationship
            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];

                if (actorData.ParentID != Guid.Empty && _actorsByID.TryGetValue(actorData.ParentID, out var parent))
                {
                    _actorsByID[actorData.ID].value.Transform.Parent = parent.value.Transform;
                }
            }

            // Note: loops through the actors again after the hierarchy is completed to add the components.
            //      This is important in playmode since components could be added to a "enable later" queue, if actors are disabled
            //      by itself or in the hierarchy.
            for (int i = 0; i < actors.Count; i++)
            {
                var actorData = actors[i];
                var actor = _actorsByID[actorData.ID].value;

                // Add components, but no deserialize yet.
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (T.ResolveType(componentData, out var componentType))
                    {
                        // TODO: fix the component initialization for the ones that auto add other required components.
                        // When 'Application.IsInPlayMode' is on, it will auto add components, making this not usable.

                        var component = actor.AddComponent(componentType, componentData.ID, false,
                                                           componentData.IsEnabled, true, out var isPendingToInitialize);
#if DEBUG
                        component.Transform.SyncLocalEulerDelta(true);
#endif
                        if (!isPendingToInitialize)
                        {
                            if (Application.IsInPlayMode)
                            {
                                _initializationComponents.Add(component);
                            }
                            else
                            {
                                // Debug.Log(component.GetType() + " Not adding, pending for later");
                            }
                        }

                        _componentsByID.Add(componentData.ID, (component, componentData));
                    }
                }

                // Resolve required properties
                for (int j = 0; j < actorData.Components.Count; j++)
                {
                    var componentData = actorData.Components[j];

                    if (_componentsByID.TryGetValue(componentData.ID, out var comp))
                    {
                        var component = comp.value;

                        var reqProperties = ReflectionUtils.GetAllMembersWithAttribute<RequiredPropertyAttribute>(component.GetType())?.ToList();

                        if (reqProperties != null && reqProperties.Count > 0)
                        {
                            for (int k = 0; k < reqProperties.Count; k++)
                            {
                                var prop = reqProperties[k];
                                var reqComponent = component.GetComponent(ReflectionUtils.GetMemberType(prop));
                                ReflectionUtils.SetMemberValue(component, prop, reqComponent);
                            }
                        }
                    }
                }
            }

            var deserializerData = new DeserializerData()
            {
                ActorsByID = _actorsByID,
                ComponentsByID = _componentsByID,
            };

            // Deserialize components data, and resolve references.
            foreach (var (id, componentValue) in _componentsByID)
            {
                Deserializer.DeserializeTarget(componentValue.value, componentValue.data.Properties, deserializerData);
            }

            if (Application.IsInPlayMode)
            {
                foreach (var component in _initializationComponents)
                {
                    try
                    {
                        (component as IAwakeableComponent).OnAwake();
                    }
                    catch (Exception e)
                    {
                        Debug.Error(e);
                    }

                    try
                    {
                        if (component.IsEnabled)
                        {
                            (component as IEnabledComponent).OnEnabled();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Error(e);
                    }
                }
                _initializationComponents.Clear();
                _actorsByID.Clear();
                _componentsByID.Clear();
            }

            return rootActors;
        }
    }
}
