using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Exceptions;
using Engine.Layers;

namespace Engine
{
    public abstract class EObject : IObject
    {
        public virtual string Name { get; set; } = DefaultObjectName;

        private const string DefaultObjectName = "Object";

        private Guid _id;

        internal bool IsAlive { get; set; } = true;
        internal bool IsPendingToDestroy { get; set; } = false;

        internal EObject()
        {
            _id = Guid.NewGuid();
        }

        internal EObject(string name)
        {
            Name = name;
            _id = Guid.NewGuid();
        }

        internal EObject(Guid id)
        {
            _id = id;
        }
        internal EObject(string name, Guid id)
        {
            Name = name;
            _id = id;
        }

        public Guid GetID()
        {
            return _id;
        }

        public static implicit operator bool(EObject obj)
        {
            return obj != null && obj.IsAlive && !obj.IsPendingToDestroy;
        }

        protected bool CheckIfValidObject(EObject obj)
        {
            if (CleanUpLayer.CleaningUp)
                return false;

            if (!obj.IsAlive)
            {
                Debug.Error($"Can't use destroyed object of type: '{obj.GetType().Name}'");
            }
#if SHOW_ENGINE_MESSAGES
            else if (obj.IsPendingToDestroy)
            {
                Debug.Error($"Object of type: '{obj.GetType().Name}' was marked to be destroyed, please don't use it.");
            }
#endif

            return true;
        }

#if DEBUG && SHOW_ENGINE_MESSAGES
        ~EObject()
        {
            Debug.Warn($"Mem Destroy: ({GetType().Name}, {_guid}), name: {Name}");
        }
#endif

        protected internal virtual void OnDestroy() { }
        public bool IsValid()
        {
            return this;
        }


        // Remove this.
        internal void _SetID(Guid id)
        {
            _id = id;
        }
    }
}