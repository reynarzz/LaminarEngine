using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class AICharacter : Character, IAIStateMachineContext
    {
        public Character Target { get; set; }
        public ITargetDetector Detector { get; private set; }
        public override void Init(CharacterConfig config)
        {
            Detector = AddComponent<CircleTargetDetector>();
            Inventory = new CharacterInventory(5);
            base.Init(config);
        }
    }
}
