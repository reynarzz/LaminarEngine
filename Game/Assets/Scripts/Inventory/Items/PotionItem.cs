using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PotionItem : Item
    {
        public PotionItem(ItemFeatures features) : base(features) { }

        public override void Use(int amountMul, Inventory inventory)
        {
            (inventory as CharacterInventory).Life += Features.Amount * amountMul;
        }
    }
}