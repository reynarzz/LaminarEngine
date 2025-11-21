using Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    [UniqueComponent]
    public abstract class UIGraphicsElement : UIElement
    {
        internal abstract void OnCanvasDraw(UICanvas canvas);
    }
}