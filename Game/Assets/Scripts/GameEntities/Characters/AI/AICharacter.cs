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

            Inventory = new CharacterInventory(config.InventoryMaxSlots);

            if(config.StartInventoryValues != null)
            {
                foreach (var item in config.StartInventoryValues)
                {
                    Inventory.Add(item.Item, item.Amount);
                }
            }
            
            base.Init(config);
        }
    }
}
