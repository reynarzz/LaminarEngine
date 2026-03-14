using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    internal class DefaultInspectorDrawer : EditorDrawerBase
    {
        internal override void OnDraw(IObject target) 
        {
            DrawTitle(target);
        }
    }
}
