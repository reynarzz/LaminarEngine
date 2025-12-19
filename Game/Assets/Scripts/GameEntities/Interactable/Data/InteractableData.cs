using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class InteractableData
    {
        public ItemId LockedBy { get; set; }
        public int LockedByAmount { get; set; }
        public Predicate<Player> InteractCondition { get; set; }
    }
}
