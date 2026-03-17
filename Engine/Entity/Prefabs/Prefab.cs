using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Prefab : AssetResourceBase
    {
        [SerializedField] internal ActorIR[] Actors { get; protected private set; }

        internal Prefab(Guid guid) : base(guid)
        {
        }

        public virtual Actor Instantiate()
        {
            Debug.Log("Give me just the actor");
            return null;
        }

        protected override void OnUpdateResource(object data, Guid guid)
        {

        }
    }

    public class Prefab<T> : Prefab where T : Component
    {
        internal Prefab(Guid guid) : base(guid) { }

        public new T Instantiate()
        {
            return base.Instantiate()?.GetComponent<T>();
        }
    }
}
