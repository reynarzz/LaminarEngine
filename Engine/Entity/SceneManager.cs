using Engine.Data;
using Engine.Layers;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SceneManager
    {
        internal static Scene ActiveScene { get; private set; }
        internal static Scene DontDestroyOnLoadScene => _scenes[0];
        internal static Scene Dont => _scenes[0];

        private static readonly List<Scene> _scenesToDestroy = new();
        private static readonly List<Scene> _scenes = new();
        internal static IReadOnlyList<Scene> Scenes => _scenes;

        internal static void Initialize()
        {
            UnloadAll();
            // First Scene is always the 'DontDestroyOnLoad' scene
            CreateSceneInMemory("DontDestroyOnLoad");
        }

        internal static Scene CreateSceneInMemory(string name)
        {
            var scene = LoadEmptyScene(Guid.NewGuid());
            scene.Name = name;
            return scene;
        }

        private static Scene LoadEmptyScene(Guid refId)
        {
            ClearScenes();
            OnCleanupUpdate();

            var scene = new Scene(refId);
            ActiveScene = scene;
            _scenes.Add(scene);

            return scene;
        }

        private static bool IsActiveSceneValid()
        {
            return ActiveScene && _scenes.Contains(ActiveScene) && ActiveScene != DontDestroyOnLoadScene;
        }

        internal static Scene LoadEmptySceneAdditive(Guid refId)
        {
            var scene = new Scene(refId);
            _scenes.Add(scene);
            if (!IsActiveSceneValid())
            {
                ActiveScene = scene;
            }
            return scene;
        }
        public static void LoadScene(string name)
        {
            if (TryGetSceneAsset(name, out var sceneAsset))
            {
                if (!IsSceneAlreadyAdded(sceneAsset.GetID()))
                {
                    LoadScene(sceneAsset);
                }
                else
                {
                    Debug.Warn($"Scene '{name}' is already loaded.");
                }
            }
            else
            {
                Debug.Error($"Scene: '{name}' is not added to the build.");
            }
        }

        public static void LoadScene(Guid refId)
        {
            if (refId == Guid.Empty)
            {
                Debug.Error("Empty scene id");
                return;
            }

            var sceneAsset = Assets.GetAssetFromGuid(refId) as SceneAsset;
            LoadScene(sceneAsset);
        }

        public static void LoadScene(SceneAsset sceneAsset)
        {
            if (sceneAsset)
            {
                var scene = LoadEmptyScene(sceneAsset.GetID());
                SceneDeserializer.DeserializeScene(sceneAsset.SceneIR, scene);
            }
        }

        internal static void UnloadScene(Scene scene)
        {
            if (!_scenes.Contains(scene) || scene == _scenes[0])
                return;

            _scenesToDestroy.Add(scene);
            _scenes.Remove(scene);
        }

        internal static void ReloadScene(Guid refId)
        {
            var scene = _scenes.FirstOrDefault(x => x.GetID() == refId);
            if (!scene)
            {
                Debug.Error($"Scene '{refId}' is not already loaded, can't reload.");
                return;
            }

            var index = _scenes.IndexOf(scene);

            UnloadScene(scene);
            OnCleanupUpdate();
            LoadSceneAdditive(refId, index);
        }

        internal static void UnloadAll()
        {
            for (int i = 0; i < _scenes.Count; i++)
            {
                _scenes[i].IsPendingToDestroy = true;
                _scenesToDestroy.Add(_scenes[i]);
            }

            OnCleanupUpdate();
            _scenes.Clear();
        }

        private static void ClearScenes()
        {
            _scenesToDestroy.Clear();
            // Adds all scenes to destroy, except the 'DontDestroyOnLoadScene'
            for (int i = _scenes.Count - 1; i > 0; --i)
            {
                _scenesToDestroy.Add(_scenes[i]);
                _scenes[i].IsPendingToDestroy = true;
                _scenes.RemoveAt(i);
            }
        }

        internal static Scene LoadSceneAdditive(Guid refId, int index = -1)
        {
            if (IsSceneAlreadyAdded(refId) || !TryGetSceneAsset(refId, out var sceneAsset))
                return null;

            var scene = new Scene(refId);
            
            if(!ActiveScene || ActiveScene.GetID() == refId)
            {
                ActiveScene = scene;
            }
            SceneDeserializer.DeserializeScene(sceneAsset.SceneIR, scene);
            if (index < 0 || index >= _scenes.Count)
            {
                _scenes.Add(scene);
            }
            else
            {
                _scenes.Insert(index, scene);
            }

            return scene;
        }

        public void UnloadScene(string name)
        {
            // TODO: implement it
        }

        private static bool IsSceneAlreadyAdded(Guid refId)
        {
            if (refId == Guid.Empty)
            {
                return false;
            }
            return _scenes.Exists(x => x?.GetID() == refId);
        }
        private static bool TryGetSceneAsset(string sceneName, out SceneAsset scene)
        {
            return TryGetSceneAsset((assetInfo, _) => assetInfo.Name.Equals(sceneName), out scene);
        }
        private static bool TryGetSceneAsset(Guid refId, out SceneAsset scene)
        {
            return TryGetSceneAsset((_, id) => id == refId, out scene);
        }
        private static bool TryGetSceneAsset(Func<AssetInfo, Guid, bool> comparer, out SceneAsset scene)
        {
            scene = null;
            var sceneSettings = EngineServices.GetService<EngineDataService>().GetProjectSettings().SceneSettings;

            foreach (var refId in sceneSettings.GetAllScenesId())
            {
                var assetsInfo = IOLayer.Database.GetAssetInfo(refId);
                if (comparer(assetsInfo, refId))
                {
                    scene = Assets.GetAssetFromGuid(refId) as SceneAsset;
                    return true;
                }
            }
            return false;
        }

        internal static void OnCleanupUpdate()
        {
            for (int i = 0; i < _scenes.Count; i++)
            {
                _scenes[i].DeletePending();
            }

            foreach (var scene in _scenesToDestroy)
            {
                scene.Destroy();
            }
            if (_scenesToDestroy.Count > 0)
            {
                _scenesToDestroy.Clear();
            }
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
        internal static void OnPreRenderUpdate()
        {
            for (int i = 0; i < _scenes.Count; i++)
            {
                _scenes[i].OnPreRender();
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
        internal static Actor FindActorByID(Guid id)
        {
            return Find<Actor, ActorIDMatcher, Guid>(id);
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
