using Editor;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    [PropertyDrawer(nameof(TilemapRenderer.Options))]
    internal class TilemapPropertyDrawer : PropertyDrawer<TilemapRenderer>
    {
        protected internal override bool DrawProperty(Type type, string name, object target, in object valueIn, 
                                                      out object valueOut, Func<bool> defaultPropertyDrawer)
        {
            valueOut = valueIn;
            return false;
        }
    }
}