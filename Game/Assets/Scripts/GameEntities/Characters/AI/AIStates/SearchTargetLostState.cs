using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class SearchTargetLostState<T> : StateBase<T> where T : AICharacter
    {
        // TODO: use last known target position, and walk there, look around, if target is not there, go back to base.
    }
}
