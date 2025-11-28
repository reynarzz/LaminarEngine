using Engine.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SceneManager
    {
        internal static WeakReference<Scene> ActiveScene { get; private set; }
        internal static WeakReference<Scene> DontDestroyOnLoadScene { get; private set; }

        private static Scene _activeScene;

        private static readonly List<Scene> _scenesToDestroy = new();
        private static readonly List<Scene> _scenes = new();

        static SceneManager()
        {
            // First Scene is always the 'dontDestroyOnLoad' scene
            LoadSceneAdditive("DontDestroyOnLoad");
            LoadSceneAdditive("DefaultScene");

            _activeScene = _scenes[1];
            DontDestroyOnLoadScene = new WeakReference<Scene>(_scenes[0]);
            ActiveScene = new WeakReference<Scene>(_activeScene);
        }

        public static void LoadScene(string name)
        {
            PrintActorsInScene(_activeScene);
            ClearScenes();
            OnCleanUpUpdate();

            // TODO: Load scene from file (Probably will never be implemented since all scenes are built at runtime, without a editor)
            var scene = new Scene(name);
            _activeScene = scene;
            ActiveScene = new WeakReference<Scene>(_activeScene);
            _scenes.Add(scene);
        }

        private static void PrintActorsInScene(Scene scene)
        {
            void Actors(Actor actor)
            {
                Debug.Log(actor.Name);

                foreach (var item in actor.Transform.Children)
                {
                    Actors(item.Actor);
                }
            }

            for (int i = 0; i < _activeScene.RootActors.Count; i++)
            {
                Actors(_activeScene.RootActors[i]);
            }
            
        }
        private static void ClearScenes()
        {
            _scenesToDestroy.Clear();
            // Adds all scenes to destroy, except the 'DontDestroyOnLoadScene'
            for (int i = _scenes.Count - 1; i >= 1; --i)
            {
                _scenesToDestroy.Add(_scenes[i]);
                _scenes.RemoveAt(i);
            }
        }

        public static void LoadSceneAdditive(string name)
        {
            if (IsSceneAlreadyAdded(name))
                return;

            _scenes.Add(new Scene(name));
        }

        public void UnloadScene(string name)
        {
            // TODO: implement it
        }

        private static bool IsSceneAlreadyAdded(string name)
        {
            return _scenes.Exists(x => x?.Name.Equals(name) ?? false);
        }

        internal static void UpdateScenes()
        {
            for (int i = 0; i < _scenes.Count; i++)
            {
                var scene = _scenes[i];
                scene.Awake();
                scene.Start();
                scene.Update();
                scene.LateUpdate();
            }
        }

        internal static void FixedUpdate()
        {
            foreach (var scene in _scenes)
            {
                scene.FixedUpdate();
            }
        }

        internal static void OnDrawGizmos()
        {
            for (int i = 0; i < _scenes.Count; i++)
            {
                _scenes[i].OnDrawGizmos();
            }
        }

        internal static void OnCleanUpUpdate()
        {
            _activeScene.DeletePending();

            foreach (var scene in _scenesToDestroy)
            {
                scene.Destroy();
            }
            if (_scenesToDestroy.Count > 0)
            {
                // Note: this is provisional.
                // RenderingLayer.Test_ClearBatches();
                PhysicsLayer.Clear(); // Remove
                _scenesToDestroy.Clear();
            }
        }

        internal static IReadOnlyList<Actor> FindActorsByTag(string tag)
        {
            return FindAll<Actor, ActorTagMatcher, string>(tag, null);
        }

        internal static IReadOnlyList<Actor> FindActorsByTag(string tag, Actor rootActor)
        {
            return FindAll<Actor, ActorTagMatcher, string>(tag, rootActor);
        }

        internal static T FindComponent<T>(bool findDisabled) where T : Component
        {
            return Find<T, ComponentMatcher<T>, bool>(findDisabled);
        }

        internal static Actor FindActorByName(string name)
        {
            return Find<Actor, ActorMatcher, string>(name);
        }

        internal static List<T> FindAll<T>(bool findDisabled, Actor rootActor) where T : Component
        {
            return FindAll<T, ComponentMatcher<T>, bool>(findDisabled, rootActor);
        }

        internal static List<T> FindAll<T>(bool findDisabled) where T : Component
        {
            return FindAll<T, ComponentMatcher<T>, bool>(findDisabled, null);
        }

        internal static void FindAll<T>(List<T> elements, bool findDisabled) where T : Component
        {
            FindAll<T, ComponentMatcher<T>, bool>(findDisabled, elements, null);
        }
        internal static void FindAll<T>(List<T> elements, bool findDisabled, Actor rootActor) where T : Component
        {
            FindAll<T, ComponentMatcher<T>, bool>(findDisabled, elements, rootActor);
        }
        internal static void FindAll<T>(List<T> elements, Predicate<T> predicate) where T : Component
        {
            FindAll<T, ComponentMatcherFunc<T>, Predicate<T>>(predicate, elements, null);
        }

        internal static void FindAll<T>(List<T> elements, Actor rootActor, Predicate<T> predicate) where T : Component
        {
            FindAll<T, ComponentMatcherFunc<T>, Predicate<T>>(predicate, elements, rootActor);
        }

        private static List<T> FindAll<T, TMatcher, TComparer>(TComparer comparer, Actor rootActor) where T : EObject
                                                                            where TMatcher : struct, IMatcher<T, TComparer>
        {
            var list = new List<T>();
            FindAll<T, TMatcher, TComparer>(comparer, list, rootActor);
            return list;
        }

        private static T Find<T, TMatcher, IComparer>(IComparer comparer) where TMatcher : struct, IMatcher<T, IComparer>
                                                                    where T : EObject
        {
            // NOTE: this is being called by functions that might modify the scene list, so using a foreach will cause an exception.
            for (int i = 0; i < _scenes.Count; i++)
            {
                var scene = _scenes[i];
                var result = Find<T, TMatcher, IComparer>(scene, comparer);

                if (result != default)
                {
                    return result;
                }
            }

            return default;
        }

        private static T Find<T, TMatcher, IComparer>(Scene scene, IComparer comparer) where TMatcher : struct, IMatcher<T, IComparer>
                                                                                where T : EObject
        {
            var matcher = default(TMatcher);

            T Find(Actor actor)
            {
                var result = matcher.Invoke(actor, comparer);

                if (result)
                {
                    return result;
                }

                for (int i = 0; i < actor.Transform.Children.Count; i++)
                {
                    var found = Find(actor.Transform.Children[i].Actor);

                    if (found)
                    {
                        return found;
                    }
                }

                return default;
            }

            for (int i = 0; i < scene.RootActors.Count; i++)
            {
                var value = Find(scene.RootActors[i]);

                if (value)
                {
                    return value;
                }
            }

            return default;
        }
        private static void FindAll<T, TMatcher, TComparer>(TComparer comparer, List<T> elements, Actor rootActor) where T : EObject
                                                                                                            where TMatcher : struct, IMatcher<T, TComparer>
        {
            // NOTE: this is being called by functions that might modify the scene list, so using a foreach will cause an exception.
            for (int i = 0; i < _scenes.Count; i++)
            {
                var scene = _scenes[i];
                FindAll<T, TMatcher, TComparer>(scene, comparer, elements, rootActor);
            }
        }

        private static void FindAll<T, TMatcher, TComparer>(Scene scene, TComparer comparer, List<T> elements, Actor rootActor) where T : EObject
                                                                                                                         where TMatcher : struct, IMatcher<T, TComparer>
        {
            var matcher = default(TMatcher);

            void Find(Actor actor)
            {
                if (!actor) // After I put this, onDestroy is not being called.
                    return;

                var result = matcher.Invoke(actor, comparer);

                if (result && result.IsAlive)
                {
                    elements.Add(result);
                }

                for (int i = 0; i < actor.Transform.Children.Count; i++)
                {
                    Find(actor.Transform.Children[i].Actor);
                }
            }

            if (rootActor == null)
            {
                for (int i = 0; i < scene.RootActors.Count; i++)
                {
                    if (scene.RootActors[i])
                    {
                        Find(scene.RootActors[i]);
                    }
                }
            }
            else
            {
                Find(rootActor);
            }
        }
    }
}
