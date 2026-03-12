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
                return target ? target : null;
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

        public static Transform SelectedTransform()
        {
            if (_selected == null)
                return null;
            _selected.TryGetTarget(out var obj);
            if (obj is Actor actor)
            {
                return actor.Transform;
            }

            return obj ? obj as Transform : null;
        }
    }
}
