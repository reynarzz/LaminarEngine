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
        private static WeakReference<Transform> _transform;

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
                SetTransform(value);
            }
        }

        private static void SetTransform(EObject obj)
        {
            void Set(Transform transform)
            {
                if (_transform == null)
                    _transform = new WeakReference<Transform>(transform ? transform : null);
                else
                    _transform.SetTarget(transform);
            }

            if (obj)
            {
                if (obj is Actor actor)
                {
                    Set(actor.Transform);
                }
                else if (obj is Transform transform)
                {
                    Set(transform);
                }
            }
            else if (_transform != null)
            {
                _transform.SetTarget(null);
            }
        }

        public static Transform SelectedTransform()
        {
            if (_transform == null)
                return null;
            _transform.TryGetTarget(out var transform);
            return transform ? transform : null;
        }
    }
}
