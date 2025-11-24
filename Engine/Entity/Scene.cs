using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Scene : EObject
    {
        private readonly Action<Actor> _onAwake = x => x.Awake();
        private readonly Action<Actor> _onStart = x => x.Start();
        private readonly Action<Actor> _onUpdate = x => x.Update();
        private readonly Action<Actor> _onLateUpdate = x => x.LateUpdate();
        private readonly Action<Actor> _onFixedUpdate = x => x.FixedUpdate();


        private readonly List<Actor> _rootActors = new();
        internal IReadOnlyList<Actor> RootActors => _rootActors;
        public Scene()
        {
            Name = "Scene";
        }
        public Scene(string name)
        {
            Name = name;
        }
        internal void RegisterRootActor(Actor actor)
        {
            _rootActors.Add(actor);
        }

        internal void UnregisterRootActor(Actor actor)
        {
            _rootActors.Remove(actor);
        }

        internal IReadOnlyList<Actor> GetRootActors()
        {
            return _rootActors;
        }

        internal void RemoveActor(Actor actor)
        {
            if (actor.Transform.Parent != null)
            {
                actor.Transform.Parent.RemoveChild(actor.Transform);
            }
            else
            {
                UnregisterRootActor(actor);
            }
        }


        
        internal void Awake()
        {
            CallActorFunc(_onAwake);
        }

        internal void Start()
        {
            CallActorFunc(_onStart);
        }

        internal void Update()
        {
            CallActorFunc(_onUpdate);
        }

        internal void LateUpdate()
        {
            CallActorFunc(_onLateUpdate);
        }

        internal void FixedUpdate()
        {
            CallActorFunc(_onFixedUpdate);
        }

        private void CallActorFunc(Action<Actor> callback)
        {
            for (int i = 0; i < _rootActors.Count; i++)
            {
                var actor = _rootActors[i];
                if (actor && actor.IsActiveInHierarchy)
                {
                    callback(actor);
                }
            }
        }

        internal void DeletePending()
        {
            for (int i = _rootActors.Count - 1; i >= 0; --i)
            {
                _rootActors[i].DeletePending();
            }
        }

        internal void Destroy()
        {
            for (int i = 0; i < _rootActors.Count; i++)
            {
                var actor = _rootActors[i];
                actor.OnDestroy(); // Adds to the deletePending's list
                actor.DeletePending(); // Removes all references
            }

            _rootActors.Clear();
        }
    }
}
