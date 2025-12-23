using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class Component : EObject, IComponent, IAwakeableComponent, IEnabledComponent
    {
        public Actor Actor { get; internal set; }
        public virtual Transform Transform
        {
            get
            {
                CheckIfValidObject(this);
                return Actor.Transform;
            }
        }
        internal Component()
        {
        }
        public override string Name { get => Actor?.Name ?? GetType().Name; set => Actor.Name = value; }

        private bool _isEnabled = true;

        public virtual bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value)
                {
                    return;
                }

                _isEnabled = value;

                if (Actor.IsActiveInHierarchy)
                {
                    if (_isEnabled)
                    {
                        OnEnabled();
                    }
                    else
                    {
                        OnDisabled();
                    }
                }
            }
        }
        void IAwakeableComponent.OnAwake() { OnAwake(); }
        public virtual void OnEnabled() { }
        public virtual void OnDisabled() { }
        protected virtual void OnAwake() { }

        public Component AddComponent(Type type)
        {
            CheckIfValidObject(this);
            return Actor.AddComponent(type);
        }

        public T AddComponent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>() where T : Component
        {
            CheckIfValidObject(this);
            return Actor.AddComponent<T>();
        }

        public void AddComponent<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T1,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T2>() where T1 : Component where T2 : Component
        {
            CheckIfValidObject(this);
            Actor.AddComponent<T1, T2>();
        }

        public void AddComponent<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T1,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T2,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T3>() where T1 : Component
                                                where T2 : Component
                                                where T3 : Component
        {
            CheckIfValidObject(this);
            Actor.AddComponent<T1, T2, T3>();
        }

        public void AddComponent<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T1,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T2,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T3,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T4>() where T1 : Component
                                                    where T2 : Component
                                                    where T3 : Component
                                                    where T4 : Component
        {
            CheckIfValidObject(this);
            Actor.AddComponent<T1, T2, T3, T4>();
        }

        public void AddComponent<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T1,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T2,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T3,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T4,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T5>() where T1 : Component
                                                        where T2 : Component
                                                        where T3 : Component
                                                        where T4 : Component
                                                        where T5 : Component
        {
            CheckIfValidObject(this);
            Actor.AddComponent<T1, T2, T3, T4, T5>();
        }

        public T GetComponent<T>() where T : class
        {
            CheckIfValidObject(this);
            return Actor.GetComponent<T>();
        }

        public Span<T> GetComponents<T>() where T : class
        {
            CheckIfValidObject(this);
            return Actor.GetComponents<T>();
        }

        public void GetComponents<T>(ref List<T> elements) where T : class
        {
            CheckIfValidObject(this);
            Actor.GetComponents(ref elements);
        }

        public void GetComponentsInChildren<T>(ref List<T> elements) where T : class
        {
            CheckIfValidObject(this);
            Actor.GetComponentsInChildren(ref elements);
        }

        public Span<T> GetComponentsInChildren<T>() where T : class
        {
            var elements = new List<T>();
            CheckIfValidObject(this);
            Actor.GetComponentsInChildren(ref elements);
            return CollectionsMarshal.AsSpan(elements);
        }

        public void GetComponentsInParents<T>(ref List<T> elements) where T : class
        {
            CheckIfValidObject(this);
            Actor.GetComponentsInParent(ref elements);
        }

        public Span<T> GetComponentsInParents<T>() where T : class
        {
            var elements = new List<T>();
            CheckIfValidObject(this);
            Actor.GetComponentsInParent(ref elements);
            return CollectionsMarshal.AsSpan(elements);
        }

        public T GetComponentInParents<T>() where T : class
        {
            CheckIfValidObject(this);
            return Actor.GetComponentInParent<T>();
        }
    }
}