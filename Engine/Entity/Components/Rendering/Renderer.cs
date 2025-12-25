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
        internal abstract RendererData RendererData { get; protected private set; } 
        public Material Material { get => RendererData.Material; set => RendererData.Material = value; }

        protected override void OnAwake()
        {
            Transform.OnChanged += Transform_OnChanged;
            RendererData.IsDirty = true;
        }

        protected virtual void Transform_OnChanged(Transform obj)
        {
            // TODO: Careful with this, it could cause renderers to be dirty even when no meaningful transformation happens
            RendererData.IsDirty = true;
        }

        internal void MarkNotDirty()
        {
            RendererData.IsDirty = false;
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            RendererData.IsDirty = true;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
            RendererData.IsDirty = true;
            RendererData.OnDestroy();
        }

        internal virtual void Draw() { }

        protected internal override void OnDestroy()
        {
            base.OnDestroy();

            Transform.OnChanged -= Transform_OnChanged;
            RendererData.OnDestroy();
        }
    }
}