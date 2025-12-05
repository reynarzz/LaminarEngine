using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PortalData : InteractableData
    {
        public vec2 TargetPos { get; set; }
        public bool IsArriveOnly { get; set; }
        public ItemId LockedBy { get; internal set; }
        public int LockedAmount { get; internal set; }
    }
}
