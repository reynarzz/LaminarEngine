using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class Selector
    {
        public static EObject Selected { get; set; }

        public static Transform SelectedTransform()
        {
            if(Selected)
            {
                if(Selected is Actor actor)
                {
                    return actor.Transform;
                }
                else if (Selected is Transform transform)
                {
                    return transform;
                }
            }

            return null;
        }
    }
}
