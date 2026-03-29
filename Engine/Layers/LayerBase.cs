using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    public enum EventType
    {
        WindowFocusEnter,
        WindowFocusExit,
    }
    public enum LayerInitializationType
    {
        Success,
        Error,
        InProgress
    }

    public struct LayerInitResult
    {
        public LayerInitializationType Message { get; set; }

        public static readonly LayerInitResult Success = new LayerInitResult() { Message = LayerInitializationType.Success };
        public static readonly LayerInitResult Error = new LayerInitResult() { Message = LayerInitializationType.Error };
        public static readonly LayerInitResult InProgress = new LayerInitResult() { Message = LayerInitializationType.InProgress };
    }

    public abstract class LayerBase
    {
        //internal abstract int Priority { get; set; }
        internal bool IsInitialized { get; protected private set; }
        public virtual Task<LayerInitResult> InitializeAsync() { return Task.FromResult(LayerInitResult.Success); }
        public virtual void Initialize() { }
        public abstract void Close();
        internal virtual void UpdateLayer() { }
        public virtual void OnPause() { }
        public virtual void OnResume() { }
        public virtual void OnEvent(EventType type, object value)
        {

        }
    }
}
