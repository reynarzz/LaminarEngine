using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Inventory
    {
        public int MaxSlots { get; }
        private readonly InventorySlot[] _slots;
        public IReadOnlyList<InventorySlot> Slots => _slots;
        protected event Action<Item> OnItemRemoved;
        public Inventory(int maxSlots)
        {
            MaxSlots = maxSlots;
            _slots = new InventorySlot[maxSlots];

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = new InventorySlot();
            }
        }

        public bool Add(ItemId id, int amount)
        {
            Item item = ItemsDatabase.GetItem(id);
            var slotIndex = GetFirstSlotIndex(item.Features.Id);
            var currentSlot = default(InventorySlot);
            
            if(slotIndex >= 0)
            {
                currentSlot = _slots[slotIndex];
            }

            if (!currentSlot.IsEmpty() && item.Features.IsStackable && (currentSlot.Amount + amount) < item.Features.MaxPerSlot)
            {
                _slots[slotIndex] = new InventorySlot(currentSlot.item, currentSlot.Amount + amount);
                return true;
            }

            var emptySlotIndex = GetFirstEmptySlotIndex();
            var canAdd = emptySlotIndex >= 0;
            if (canAdd)
            {
                _slots[emptySlotIndex] = new InventorySlot(item, amount);
            }

            return canAdd;
        }

        private int GetFirstEmptySlotIndex()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].IsEmpty())
                    return i;
            }

            return -1;
        }

        public void Drop(int slotIndex)
        {
            var slot = _slots[slotIndex];

            // TODO: Instance into the world.


            _slots[slotIndex] = default;
        }

        public InventorySlot GetSlot(int index)
        {
            return _slots[index];
        }
        public int GetFirstSlotIndex(ItemId id)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var slot = _slots[i];

                if (!slot.IsEmpty() && slot.item.Features.Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        public int GetItemCount(ItemId id)
        {
            var index = GetFirstSlotIndex(id);
            if(index >= 0)
            {
                return _slots[index].Amount;
            }
            return 0;
        }
        public void Use(int slotIndex, int amountMul = 1)
        {
            var slot = GetSlot(slotIndex);

            if (!slot.IsEmpty())
            {
                var useCount = Math.Clamp(amountMul, 0, Math.Min(slot.Amount, slot.item.Features.MaxPerSlot));

                slot.item.Use(useCount, this);

                if (slot.item.Features.DecreasesOnUse)
                {
                    _slots[slotIndex] = new InventorySlot(slot.item, slot.Amount - useCount);
                }
            }
        }

        public InventorySlot GetFirstSlot(ItemId id)
        {
            var index = GetFirstSlotIndex(id);

            if (index >= 0)
            {
                return _slots[index];
            }

            return default;
        }

        public void DropAll()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                Drop(i);
            }
        }
    }
}
