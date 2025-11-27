using Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Actor : EObject
    {
        public string Tag { get; set; }
        public int Layer { get; set; } = 0;

        internal WeakReference<Scene> Scene { get; private set; }
        private List<Component> _components;
        internal List<Component> Components => _components;
        private List<IComponent> _toDeleteComponents = new();

        private Transform _transform;
        public Transform Transform
        {
            get { CheckIfValidObject(this); return _transform; }
            private set
            {
                CheckIfValidObject(this);
                _transform = value;
            }
        }

        private bool _isEnabled = true;
        private bool _isActiveInHierarchy = true;
        public bool IsActiveInHierarchy
        {
            get => _isActiveInHierarchy;
            private set
            {
                if (_isActiveInHierarchy == value)
                    return;

                _isActiveInHierarchy = value;

                foreach (var component in _components)
                {
                    if (value && component.IsEnabled)
                    {
                        component.OnEnabled();
                    }
                    else if (!value)
                    {
                        component.OnDisabled();
                    }
                }
            }
        }

        public bool IsActiveSelf
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                RecalculateHierarchyActivation();
            }
        }

        private List<Component> _pendingToDeleteComponents;
        private List<IComponent> _onAwakePendingComponents;
        private List<IComponent> _onEnablePendingComponents;
        private List<IComponent> _onStartPendingComponents;
        private static readonly Action<IAwakeableComponent> _awakeAction = x => { if (x.Actor.IsActiveInHierarchy) x.OnAwake(); };
        private static readonly Action<IEnabledComponent> _enabledAction = x => { if (x.Actor.IsActiveInHierarchy) x.OnEnabled(); };
        private static readonly Action<IStartableComponent> _startAction = x => x.OnStart();
        private static readonly Action<IUpdatableComponent> _updateAction = x => x.OnUpdate();
        private static readonly Action<ILateUpdatableComponent> _lateUpdateAction = x => x.OnLateUpdate();
        private static readonly Action<IDrawableGizmo> _drawGizmoUpdateAction = x => x.OnDrawGizmo();
        private static readonly Action<IFixedUpdatableComponent> _fixedUpdateAction = x => x.OnFixedUpdate();
        
        private static readonly Func<Actor, List<IComponent>> _getAwakePending = a => a._onAwakePendingComponents;
        private static readonly Func<Actor, List<IComponent>> _getEnablePending = a => a._onEnablePendingComponents;
        private static readonly Func<Actor, List<IComponent>> _getStartPending = a => a._onStartPendingComponents;

     
        public Actor() : this(string.Empty, string.Empty)
        {
        }

        public Actor(string name) : this(name, string.Empty)
        {
        }

        public Actor(string name, string id) : base(name, id)
        {
            if (string.IsNullOrEmpty(name))
            {
                Name = "Actor";
            }
            else
            {
                Name = name;
            }

            _components = new List<Component>();
            _onAwakePendingComponents = new();
            _onStartPendingComponents = new();
            _pendingToDeleteComponents = new();
            _onEnablePendingComponents = new();
            _transform = AddComponent<Transform>();

            Scene = SceneManager.ActiveScene;
            Scene.TryGetTarget(out var scene);
            scene.RegisterRootActor(this);
        }

        public Component AddComponent(Type type)
        {
            CheckIfValidObject(this);

            if (!IsValidComponent(type))
            {
                return default;
            }
            else if (type.IsAssignableFrom(typeof(Transform)) && _components.Count > 0)
            {
                return Transform;
            }

            var isUnique = type.GetCustomAttribute<UniqueComponentAttribute>() != null;
            if (isUnique)
            {
                for (int i = 0; i < _components.Count; i++)
                {
                    var alreadyAdded = _components[i].GetType().IsAssignableFrom(type);
                    if (alreadyAdded)
                    {
#if SHOW_ENGINE_WARNS
                        Debug.Warn($"Can't add component of type '{type.Name}', it should only appear once in an Actor. This will return the current one.");
#endif
                        return _components[i];
                    }
                }
            }

            // Get all attributes from current type, and parent classes
            IEnumerable<T> GetAllAttributes<T>(Type type) where T : Attribute
            {
                var result = new List<T>();

                while (type != null && type != typeof(object))
                {
                    result.AddRange(type.GetCustomAttributes(typeof(T), inherit: false).Cast<T>());
                    type = type.BaseType;
                }

                return result;
            }

            var required = GetAllAttributes<RequiredComponentAttribute>(type);
            if (required != null)
            {
                foreach (var requiredAttrib in required)
                {
                    foreach (var componentsTypes in requiredAttrib.RequiredComponents)
                    {
                        AddComponent(componentsTypes);
                    }
                }
            }

            var component = Activator.CreateInstance(type) as Component;
            component.Actor = this;
            _components.Add(component);
            
            if (component is IStartableComponent start)
            {
                _onStartPendingComponents.Add(start);
            }

            if (!IsActiveInHierarchy || !IsActiveSelf)
            {
                _onAwakePendingComponents.Add(component);
                _onEnablePendingComponents.Add(component);
            }
            else
            {
                (component as IAwakeableComponent).OnAwake();

                // NOTE: All components are enabled by default, so this if is unncessary right now,
                //       however, in the future I might add a flag to addComponents that are disabled by default.
                if (component.IsEnabled)
                {
                    (component as IEnabledComponent).OnEnabled();
                }
            }

            return component;
        }

        public T AddComponent<T>() where T : Component
        {
            CheckIfValidObject(this);
            return AddComponent(typeof(T)) as T;
        }

        public void AddComponent<T1, T2>() where T1 : Component where T2 : Component
        {
            CheckIfValidObject(this);
            AddComponent<T1>();
            AddComponent<T2>();
        }

        public void AddComponent<T1, T2, T3>() where T1 : Component
                                                where T2 : Component
                                                where T3 : Component
        {
            CheckIfValidObject(this);
            AddComponent<T1, T2>();
            AddComponent<T3>();
        }

        public void AddComponent<T1, T2, T3, T4>() where T1 : Component
                                                    where T2 : Component
                                                    where T3 : Component
                                                    where T4 : Component
        {
            CheckIfValidObject(this);
            AddComponent<T1, T2, T3>();
            AddComponent<T4>();
        }

        public void AddComponent<T1, T2, T3, T4, T5>() where T1 : Component
                                                        where T2 : Component
                                                        where T3 : Component
                                                        where T4 : Component
                                                        where T5 : Component
        {
            CheckIfValidObject(this);
            AddComponent<T1, T2, T3, T4>();
            AddComponent<T5>();
        }

        public Component GetComponent(Type type)
        {
            CheckIfValidObject(this);
            for (int i = 0; i < _components.Count; i++)
            {
                if (type.IsAssignableFrom(_components[i].GetType()))
                {
                    return _components[i];
                }
            }

            return null;
        }

        public T GetComponent<T>() where T : class
        {
            return GetComponent(typeof(T)) as T;
        }

        public void GetComponents<T>(ref List<T> elements) where T : class
        {
            CheckIfValidObject(this);

            for (int i = 0; i < _components.Count; i++)
            {
                if (typeof(T).IsAssignableFrom(_components[i].GetType()))
                {
                    if (elements == null)
                    {
                        elements = new List<T>();
                    }
                    elements.Add(_components[i] as T);
                }
            }
        }

        public void GetComponentsInChildren<T>(ref List<T> elements) where T : class
        {
            CheckIfValidObject(this);

            void GetComponents(ref List<T> elements, Actor actor)
            {
                for (int i = 0; i < actor._components.Count; i++)
                {
                    if (typeof(T).IsAssignableFrom(actor._components[i].GetType()))
                    {
                        if (elements == null)
                        {
                            elements = new List<T>();
                        }
                        elements.Add(actor._components[i] as T);
                    }
                }

                foreach (var child in actor.Transform.Children)
                {
                    GetComponents(ref elements, child.Actor);
                }
            }

            GetComponents(ref elements, this);
        }

        public void GetComponentsInParent<T>(ref List<T> elements) where T : class
        {
            CheckIfValidObject(this);

            void GetComponents(ref List<T> elements, Actor actor)
            {
                for (int i = 0; i < actor._components.Count; i++)
                {
                    if (typeof(T).IsAssignableFrom(actor._components[i].GetType()))
                    {
                        if (elements == null)
                        {
                            elements = new List<T>();
                        }
                        elements.Add(actor._components[i] as T);
                    }
                }

                GetComponents(ref elements, actor.Transform.Parent.Actor);
            }

            GetComponents(ref elements, this);
        }

        public T GetComponentInParent<T>() where T : class
        {
            CheckIfValidObject(this);

            T GetComponents(Actor actor)
            {
                if (!actor)
                {
                    return default;
                }

                for (int i = 0; i < actor._components.Count; i++)
                {
                    if (typeof(T).IsAssignableFrom(actor._components[i].GetType()))
                    {
                        return actor._components[i] as T;
                    }
                }

                return GetComponents(actor.Transform.Parent?.Actor);
            }

            return GetComponents(this);
        }

        public Span<T> GetComponents<T>() where T : class
        {
            var components = new List<T>();
            GetComponents(ref components);
            return CollectionsMarshal.AsSpan(components);
        }

        private bool IsValidComponent(Type component)
        {
            return component != null &&
                   component.IsClass &&
                   typeof(Component).IsAssignableFrom(component) &&
                   typeof(Component) != component;

        }

        public static void Destroy(Actor actor)
        {
            if (actor == null || !actor.IsAlive || actor.IsPendingToDestroy)
            {
                Console.WriteLine("Can't destroy invalid actor.");
                return;
            }

            void PendingToDestroyNotify(Actor actor)
            {
                // Notify own components
                for (int i = 0; i < actor._components.Count; i++)
                {
                    actor._components[i].IsPendingToDestroy = true;
                }

                // Notify children components
                for (int i = 0; i < actor.Transform.Children.Count; i++)
                {
                    PendingToDestroyNotify(actor.Transform.Children[i].Actor);
                }

                actor.IsPendingToDestroy = true;
            }

            PendingToDestroyNotify(actor);
        }

        public static void Destroy(Component component)
        {
            if (component is Transform) // Can't destroy transform.
                return;

            foreach (var comp in component.Actor._components)
            {
                if (comp != component)
                {
                    var att = comp.GetType().GetCustomAttribute<RequiredComponentAttribute>();

                    if (att != null && att.RequiredComponents.Contains(component.GetType()))
                    {
                        Debug.Warn($"Can't remove component: '{component.GetType()}', it is used by others");
                        return;
                    }
                }
            }


            if (component && !component.IsPendingToDestroy)
            {
                AddToDestroyList(component);
            }
            else
            {
                Debug.Info("Can't destroy and already destroyed component");
            }
        }

        private static void AddToDestroyList(Component component)
        {
            component.IsPendingToDestroy = true;
            component.Actor._pendingToDeleteComponents.Add(component);
        }

        public static IReadOnlyList<Actor> FindAllByTag(string tag)
        {
            return SceneManager.FindActorsByTag(tag);
        }

        public static IReadOnlyList<T> FindAllByType<T>(bool findDisabled = false) where T : Component
        {
            return SceneManager.FindAll<T>(findDisabled);
        }
        public static void DontDestroyOnLoad(Component component)
        {
            DontDestroyOnLoad(component.Actor);
        }
        public static void DontDestroyOnLoad(Actor actor)
        {
            if (!actor)
                return;

            // Remove from current scene.
            actor.Scene.TryGetTarget(out var scene);
            scene.RemoveActor(actor);

            // Add to 'DontDestroyOnLoad' scene.
            SceneManager.DontDestroyOnLoadScene.TryGetTarget(out var dontDestroyOnLoadscene);
            dontDestroyOnLoadscene.RegisterRootActor(actor);
            actor.Scene = SceneManager.DontDestroyOnLoadScene;
        }

        private static void DestroyComponentNoNotify(Component component)
        {
            if (component == null || !component.IsAlive)
            {
                Debug.Error($"Can't destroy and already destroyed component. {component.GetType().Name}");
                return;
            }

            component.Actor._components.Remove(component);
            component.Actor._onStartPendingComponents.Remove(component);
            component.Actor._onAwakePendingComponents.Remove(component);
            component.Actor._onEnablePendingComponents.Remove(component);

            component.Actor = null;
            component.IsAlive = false;
        }

        internal void Awake()
        {
            UpdateScriptBeginEvent(this, _getEnablePending, _enabledAction);
            UpdateScriptBeginEvent(this, _getAwakePending, _awakeAction);
        }

        internal void Start()
        {
            UpdateScriptBeginEvent(this, _getStartPending, _startAction);
        }

        internal void Update()
        {
            UpdateScriptsFunction(this, _updateAction);
        }

        internal void LateUpdate()
        {
            UpdateScriptsFunction(this, _lateUpdateAction);
        }

        internal void FixedUpdate()
        {
            UpdateScriptsFunction(this, _fixedUpdateAction);
        }

        internal void OnDrawGizmoUpdate()
        {
            UpdateScriptsFunction(this, _drawGizmoUpdateAction, true);
        }

        private void UpdateScriptBeginEvent<T>(Actor actor, Func<Actor, List<IComponent>> getPendingComponents,
                                                         Action<T> action) where T : class, IComponent
        {
            if (actor && actor.IsActiveInHierarchy)
            {
                var components = getPendingComponents(actor);
                actor._toDeleteComponents.Clear();

                for (int i = 0; i < components.Count; ++i)
                {
                    if (components[i] is T component && component.IsValid() && component.IsEnabled)
                    {
                        if (actor.IsActiveInHierarchy)
                        {
#if DEBUG
                            try
                            {
                                action(component);
                            }
                            catch (Exception e)
                            {
                                Debug.Error(e);
                            }
#else
                            action(component);
#endif
                        }
                    }

                    actor._toDeleteComponents.Add(components[i]);
                }

                for (int i = 0; i < actor._toDeleteComponents.Count; i++)
                {
                    components.Remove(actor._toDeleteComponents[i]);
                }

                for (int i = 0; i < actor.Transform.Children.Count; i++)
                {
                    UpdateScriptBeginEvent(actor.Transform.Children[i].Actor, getPendingComponents, action);
                }
            }
        }

        private void UpdateScriptsFunction<T>(Actor actor, Action<T> action, bool updateIfDisabled = false) where T : class, IComponent
        {
            if (actor && actor.IsActiveInHierarchy)
            {
                foreach (var component in actor._components)
                {
                    var comp = component as T;
                    if (comp != null && comp.IsValid() && (comp.IsEnabled || updateIfDisabled))
                    {
#if DEBUG
                        try
                        {
                            action(comp);
                        }
                        catch (Exception e)
                        {
                            Debug.Error(e);
                        }
#else
                        action(comp);
#endif
                    }
                }

                for (int i = 0; i < actor.Transform.Children.Count; i++)
                {
                    UpdateScriptsFunction(actor.Transform.Children[i].Actor, action);
                }
            }
        }

        public static Actor Find(string name)
        {
            return SceneManager.FindActorByName(name);
        }

        internal bool Contains(Component component)
        {
            return _components.Contains(component);
        }

        internal void DeletePending()
        {
            if (IsPendingToDestroy)
            {
                void OnDestroyEventNotify(Actor actor)
                {
                    /* Notify own components, inverse loop so client components can have the transform component available
                       (also transform's onChange event will not be cleared)*/
                    for (int i = actor._components.Count - 1; i >= 0; i--)
                    {
#if DEBUG
                        try
                        {
                            actor._components[i].OnDestroy();
                        }
                        catch (Exception e)
                        {
                            Debug.Error(e);
                        }
#else
                        actor._components[i].OnDestroy();
#endif
                    }

                    // Notify children components
                    for (int i = 0; i < actor.Transform.Children.Count; i++)
                    {
                        OnDestroyEventNotify(actor.Transform.Children[i].Actor);
                    }
                }

                void OnCleanUpChildren(Actor actor)
                {
                    for (int i = actor._components.Count - 1; i >= 0; i--)
                    {
                        DestroyComponentNoNotify(actor._components[i]);
                    }

                    for (int i = 0; i < actor.Transform.Children.Count; i++)
                    {
                        OnCleanUpChildren(actor.Transform.Children[i].Actor);
                    }

                    actor.IsAlive = false;
                    actor.Scene.TryGetTarget(out var scene);
                    scene.RemoveActor(actor);
                    // actor.Scene = null;
                }

                OnDestroyEventNotify(this);
                OnCleanUpChildren(this);

                IsPendingToDestroy = false;
            }

            if (_pendingToDeleteComponents.Count > 0)
            {
                for (int i = _pendingToDeleteComponents.Count - 1; i >= 0; --i)
                {
                    var component = _pendingToDeleteComponents[i];
#if DEBUG
                    try
                    {
                        if (component != null)
                        {
                            component.OnDestroy();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Error(e);
                    }
#else
                    if (component != null)
                    {
                        component.OnDestroy();
                    }
#endif
                    DestroyComponentNoNotify(component);
                }

                _pendingToDeleteComponents.Clear();
            }

            // Check if a child ask to be removed.
            for (int i = 0; i < Transform.Children.Count; i++)
            {
                Transform.Children[i].Actor.DeletePending();
            }
        }
        internal void RecalculateHierarchyActivation()
        {
            void UpdateActiveInHierarchy(Actor actor)
            {
                var parent = actor.Transform.Parent;
                actor.IsActiveInHierarchy = (parent != null ? parent.Actor.IsActiveInHierarchy && actor.IsActiveSelf : actor.IsActiveSelf);

                for (int i = 0; i < actor.Transform.Children.Count; i++)
                {
                    UpdateActiveInHierarchy(actor.Transform.Children[i].Actor);
                }
            }

            UpdateActiveInHierarchy(this);
        }

        public void OnDestroy()
        {

            foreach (var component in _components)
            {
                AddToDestroyList(component);
            }
        }

    }

    public class Actor<T1> : Actor where T1 : Component
    {
        public Actor() : this(string.Empty, string.Empty) { }
        public Actor(string name) : this(name, string.Empty) { }
        public Actor(string name, string id) : base(name, id) => AddComponent<T1>();
    }

    public class Actor<T1, T2> : Actor where T1 : Component where T2 : Component
    {
        public Actor() : this(string.Empty, string.Empty) { }
        public Actor(string name) : this(name, string.Empty) { }
        public Actor(string name, string id) : base(name, id) => AddComponent<T1, T2>();
    }

    public class Actor<T1, T2, T3> : Actor where T1 : Component where T2 : Component where T3 : Component
    {
        public Actor() : this(string.Empty, string.Empty) { }
        public Actor(string name) : this(name, string.Empty) { }
        public Actor(string name, string id) : base(name, id) => AddComponent<T1, T2, T3>();
    }

    public class Actor<T1, T2, T3, T4> : Actor where T1 : Component where T2 : Component where T3 : Component where T4 : Component
    {
        public Actor() : this(string.Empty, string.Empty) { }
        public Actor(string name) : this(name, string.Empty) { }
        public Actor(string name, string id) : base(name, id) => AddComponent<T1, T2, T3, T4>();
    }

    public class Actor<T1, T2, T3, T4, T5> : Actor where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component
    {
        public Actor() : this(string.Empty, string.Empty) { }
        public Actor(string name) : this(name, string.Empty) { }
        public Actor(string name, string id) : base(name, id) => AddComponent<T1, T2, T3, T4, T5>();
    }
}
