using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public struct InventorySlot
    {
        public Item item { get; private set; } = null;
        public int Amount { get; private set; } = 0;

        public InventorySlot(Item item, int amount)
        {
            this.item = item;
            Amount = amount;
        }
        public bool IsEmpty()
        {
            return item == null || (item.Features.RemoveAfterUse && Amount == 0) || item.Features.Id == ItemId.none;
        }
    }
}