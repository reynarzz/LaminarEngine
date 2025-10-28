using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PlayerInventory : CharacterInventory
    {
        public override int MaxSlots { get; set; }
        public Item[] QuickUseItems { get; }

        public PlayerInventory(int maxSlots, int maxQuickUseItems)
        {
            MaxSlots = maxSlots;
            QuickUseItems = new Item[maxQuickUseItems];
            OnItemRemoved += _OnItemRemoved;
        }

        private void _OnItemRemoved(Item item)
        {
            var index = Array.IndexOf(QuickUseItems, item);

            if (index >= 0)
            {
                QuickUseItems[index] = null;
            }
        }
    }
}