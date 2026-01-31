using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    public abstract class ApplicationLayer : LayerBase
    {
        public sealed override Task InitializeAsync()
        {
            return MainThreadDispatcher.EnqueueAsync(OnInitialize);
        }

        protected abstract void OnInitialize();
        public virtual void OnFocusEnter() { }
        public virtual void OnFocusExit() { }
    }
}