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

    public abstract class LayerBase
    {
        //internal abstract int Priority { get; set; }
        public virtual Task InitializeAsync() { return Task.CompletedTask; }
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
