using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class ChestData : InteractableData
    {
        public ItemAmountPair[] ChestLoot { get; set; }
    }
}
