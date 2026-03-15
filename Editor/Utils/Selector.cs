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
        private static WeakReference<EObject> _selected;

        public static EObject Selected
        {
            get
            {
                if (_selected == null)
                    return null;

                _selected.TryGetTarget(out var target);
                return target && target.IsAlive ? target : null;
            }
            set
            {
                if (_selected == null)
                {
                    _selected = new WeakReference<EObject>(value);
                }
                else
                {
                    _selected.SetTarget(value);
                }
            }
        }

        public static Transform Transform
        {
            get
            {
                if (_selected == null || !_selected.TryGetTarget(out var obj))
                {
                    return null;
                }

                if (obj is Actor actor)
                {
                    if (!actor) // Checks if is alive.
                    {
                        return null;
                    }
                    else
                    {
                        obj = actor.Transform;
                    }
                }

                return obj ? obj as Transform : null;
            }
        }
    }
}
