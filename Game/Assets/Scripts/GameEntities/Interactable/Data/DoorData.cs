using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class DoorData : InteractableData
    {
        public int Level { get; set; }
        public ItemId LockedBy { get; set; }
        public int LockedAmount { get; set; }
        public bool ConsumeItem { get; set; }
    }
}
