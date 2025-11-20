using Engine.Graphics;
using Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    /// <summary>
    /// Base class for all the renderers
    /// </summary>
    public abstract class Renderer : Component
    {
        public Material Material { get; set; }
        public Mesh Mesh { get; set; }
        protected internal virtual bool IsDirty { get; protected set; } = true;
        internal event Action<Renderer> OnDestroyRenderer;
        internal override void OnInitialize()
        {
            Transform.OnChanged += Transform_OnChanged;
            IsDirty = true;
        }

        protected virtual void Transform_OnChanged(Transform obj)
        {
            // TODO: Careful with this, it could cause renderers to be dirty even when no meaningful transformation happens
            IsDirty = true; 
        }

        internal void MarkNotDirty()
        {
            IsDirty = false;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            IsDirty = true;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            IsDirty = true;
            OnDestroyRenderer?.Invoke(this);
        }

        internal virtual void Draw() { }

        protected internal override void OnDestroy()
        {
            base.OnDestroy();

            Transform.OnChanged -= Transform_OnChanged;
            Debug.Warn("Destroy renderer");
            OnDestroyRenderer?.Invoke(this);
        }
    }
}