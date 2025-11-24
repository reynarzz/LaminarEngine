using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal struct ComponentMatcher<T> : IMatcher<T, bool> where T : Component
    {
        public T Invoke(Actor actor, bool canAddDisabled)
        {
            if (!actor)
                return null;

            var comp = actor.GetComponent<T>();

            if (comp)
            {
                if (canAddDisabled)
                {
                    return comp;
                }
                else if (comp.Actor.IsActiveInHierarchy)
                {
                    return comp;
                }
            }

            return null;
        }
    }
    internal struct ComponentMatcherFunc<T> : IMatcher<T, Predicate<T>> where T : Component
    {
        public T Invoke(Actor actor, Predicate<T> func)
        {
            if (!actor)
                return null;

            var comp = actor.GetComponent<T>();
            if (!comp)
                return null;

            if (!func(comp))
            {
                return null;
            }
            else if (comp.Actor.IsActiveInHierarchy)
            {
                return comp;
            }

            return null;
        }
    }
}