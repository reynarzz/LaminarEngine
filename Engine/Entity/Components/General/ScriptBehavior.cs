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
    /// Base class for all scripts.
    /// </summary>
    public abstract class ScriptBehavior : Component, IAwakeableComponent, IStartableComponent, IUpdatableComponent, ILateUpdatableComponent, IFixedUpdatableComponent, IDrawableGizmo
    {
        private readonly List<Coroutine> _coroutines = new();
        void IStartableComponent.OnStart() { OnStart(); }
        void ILateUpdatableComponent.OnLateUpdate() { OnLateUpdate(); }
        void IFixedUpdatableComponent.OnFixedUpdate() { OnFixedUpdate(); }
        void IDrawableGizmo.OnDrawGizmo() { OnDrawGizmo(); }
        void IUpdatableComponent.OnUpdate()
        {
            bool anyIncomplete = false;
            for (int i = 0; i < _coroutines.Count; i++)
            {
                var coroutine = _coroutines[i];
                coroutine.Update();

                if (!coroutine.IsCompleted)
                {
                    anyIncomplete = true;
                }
            }

            if (!anyIncomplete && _coroutines.Count > 0)
            {
                _coroutines.Clear();
            }

            OnUpdate();
        }
        protected virtual void OnStart() { }
        protected virtual void OnUpdate() { }
        protected virtual void OnLateUpdate() { }
        protected virtual void OnFixedUpdate() { }
        protected virtual void OnDrawGizmo() { }
        internal protected virtual void OnCollisionEnter2D(Collision2D collision) { }
        internal protected virtual void OnCollisionExit2D(Collision2D collision) { }
        internal protected virtual void OnCollisionStay2D(Collision2D collision) { }
        internal protected virtual void OnTriggerEnter2D(Collider2D collider) { }
        internal protected virtual void OnTriggerStay2D(Collider2D collider) { }
        internal protected virtual void OnTriggerExit2D(Collider2D collider) { }
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            var coroutine = new Coroutine(routine);
            _coroutines.Add(coroutine);
            return coroutine;
        }
        public void StopCoroutine(Coroutine coroutine)
        {
            if (_coroutines.Contains(coroutine))
            {
                coroutine.Stop();
                _coroutines.Remove(coroutine);
            }
        }
        public void StopAllCoroutines()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Stop();
            }
            _coroutines.Clear();
        }

        public Coroutine TimedExecute(Action callback, float seconds)
        {
            IEnumerator Delay(Action callback, float seconds)
            {
                yield return new WaitForSeconds(seconds);

                callback?.Invoke();
            }

            return StartCoroutine(Delay(callback, seconds));
        }
    }
}