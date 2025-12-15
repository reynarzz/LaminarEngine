using Engine.Layers;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class InputLayerBase : LayerBase
    {
       public abstract TouchInput Touch { get; }
       public abstract vec2 MousePosition { get; }
       public abstract bool GetKey(KeyCode key);
       public abstract bool GetKeyDown(KeyCode key);
       public abstract bool GetKeyUp(KeyCode key);
       public abstract bool GetMouse(MouseButton button);
       public abstract bool GetMouseDown(MouseButton button);
       public abstract bool GetMouseUp(MouseButton button);
    }
}
