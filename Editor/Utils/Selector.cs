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
                TryGetEObject(out var target);
                return target;
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
                if (!TryGetEObject(out var obj))
                    return null;

                if (obj is Actor actor)
                {
                    obj = actor.Transform;
                }

                return obj ? obj as Transform : null;
            }
        }

        private static bool TryGetEObject(out EObject obj)
        {
            obj = null;
            _selected?.TryGetTarget(out obj);

            if (!obj)
            {
                obj = null;
            }

            return obj;
        }
    }
}
