using Engine;
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

            var detector = new Actor("EnemyTargetDetector").AddComponent<CircleTargetDetector>();
            detector.Transform.Parent = Transform;
            Detector = detector;

            Inventory = new CharacterInventory(5);
            base.Init(config);
        }
    }
}
