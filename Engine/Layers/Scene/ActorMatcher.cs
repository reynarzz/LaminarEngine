using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal struct ActorMatcher : IMatcher<Actor, string>
    {
        public Actor Invoke(Actor actor, string comparer)
        {
            if (actor && actor.Name.Equals(comparer))
            {
                return actor;
            }
            return null;
        }
    }

    internal struct ActorTagMatcher : IMatcher<Actor, string>
    {
        public Actor Invoke(Actor actor, string comparer)
        {
            if (actor.Tag.Equals(comparer))
            {
                return actor;
            }
            return null;
        }
    }
}
