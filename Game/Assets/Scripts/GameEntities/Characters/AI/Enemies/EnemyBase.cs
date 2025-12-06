using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal abstract class EnemyBase : AICharacter
    {
        public override void Init(CharacterConfig config)
        {
            base.Init(config);

            if(config.StartInventoryValues != null && config.StartInventoryValues.Length > 0)
            {
                for(int i = 0; i < config.StartInventoryValues.Length; i++)
                {
                    ref var val = ref config.StartInventoryValues[i];
                    Inventory.Add(val.Item, val.Amount, true);
                }
            }
        }
    }
}