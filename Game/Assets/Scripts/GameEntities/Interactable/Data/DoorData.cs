using GlmNet;
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
        public bool ConsumeItem { get; set; }
        public vec2 TargetPosition { get; internal set; }
        public int TargetLevelIndex { get; internal set; }
        public int CurrentLevel { get; internal set; }
    }
}
