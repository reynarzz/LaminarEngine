using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Drawers
{
    internal class TilemapPropertyDrawer : IPropertyDrawer
    {
        bool IPropertyDrawer.Draw(string propertyName, object target, in object valueIn, out object valueOut)
        {
            
            valueOut = valueIn;
            return true;
        }
    }
}
