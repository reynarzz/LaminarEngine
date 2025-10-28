using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class CoinItem : Item<CoinItem>
    {
        public CoinItem(ItemFeatures features) : base(features) { }

        protected override void Use(CharacterInventory inventory)
        {
            var item = inventory.GetItem(Features.Id);
          
            if (item != null && item.Features.IsStackable)
            {
                Features.Amount += 0;
            }
            else
            {
                inventory.Add(this);
            }
        }
    }
}
