using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class SmallPotion : Item<SmallPotion>
    {
        public SmallPotion(ItemFeatures features) : base(features) { }

        protected override void Use(int requestedAmount, CharacterInventory inventory)
        {
            var times = requestedAmount == 0 ? 1 : requestedAmount;
            inventory.Life += Features.Amount * times;
        }
    }
}
