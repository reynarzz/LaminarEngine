using Engine.Layers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Base class for all scripts
    /// </summary>
    public abstract class ScriptBehavior : Component, IAwakeableComponent, IStartableComponent, IUpdatableComponent, ILateUpdatableComponent, IFixedUpdatableComponent
    {
        private readonly List<Coroutine> _coroutines = new();
        public virtual void OnAwake() { }
        public virtual void OnStart() { }

        void IUpdatableComponent.OnUpdate()
        {
            for (int i = 0; i < _coroutines.Count; i++)
            {
                _coroutines[i].Update();
            }

            OnUpdate();
        }
        public virtual void OnUpdate() { }

        public virtual void OnLateUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void OnCollisionEnter2D(Collision2D collision) { }
        public virtual void OnCollisionExit2D(Collision2D collision) { }
        public virtual void OnCollisionStay2D(Collision2D collision) { }
        public virtual void OnTriggerEnter2D(Collider2D collider) { }
        public virtual void OnTriggerStay2D(Collider2D collider) { }
        public virtual void OnTriggerExit2D(Collider2D collider) { }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = new Coroutine(routine);
            _coroutines.Add(coroutine);

            return coroutine;
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            coroutine.Stop();
        }
        public void StopAllCoroutine()
        {
            foreach (var coroutine in _coroutines)
            {
                StopCoroutine(coroutine);
            }
            _coroutines.Clear();
        }
    }
}