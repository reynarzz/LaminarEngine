using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal interface IMatcher<T, TComparer>
    {
        T Invoke(Actor actor, TComparer comparer);
    }
}
