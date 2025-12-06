using Engine;
using GlmNet;
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
        public event Action<Inventory> OnInventoryChanged;
        public Inventory(int maxSlots)
        {
            MaxSlots = maxSlots;

            if (maxSlots > 0)
            {

            }
            _slots = new InventorySlot[maxSlots];

            for (int i = 0; i < _slots.Length; i++)
            {
                _slots[i] = new InventorySlot();
            }
        }

        public bool Add(ItemId id, int amount, bool forceAddInventory = false)
        {
            if (id == ItemId.none)
                return false;

            Item item = ItemsDatabase.GetItem(id);
            if (item == null)
            {
                Debug.Error($"Item: '{id}' is not added to the '_itemTypeMapper' dictionary, or 'itemsDatabase.csv'.");
                return false;
            }

            if (item.Features.AutoConsumable && !forceAddInventory)
            {
                item.Use(amount, this);
                return true;
            }

            var slotIndex = GetFirstSlotIndex(item.Features.Id);
            var currentSlot = default(InventorySlot);

            if (slotIndex >= 0)
            {
                currentSlot = _slots[slotIndex];
            }

            if (!currentSlot.IsEmpty() && item.Features.IsStackable && (currentSlot.Amount + amount) < item.Features.MaxPerSlot)
            {
                _slots[slotIndex] = new InventorySlot(currentSlot.item, currentSlot.Amount + amount);
                OnInventoryChanged?.Invoke(this);
                return true;
            }

            return AllocateItemInNewSlots(item, amount);
        }

        private List<(int slotIndex, int amount)> GetSlotsWhereItemFits(Item item, int amount)
        {
            var slots = new List<(int, int)>();
            for (int i = 0; i < Slots.Count; i++)
            {
                int amountToAdd = 0;
                var slot = Slots[i];
                if (slot.IsEmpty())
                {
                    amountToAdd = amount > item.Features.MaxPerSlot ? item.Features.MaxPerSlot : amount;
                    slots.Add((i, amountToAdd));
                }
                else if (item.Features.IsStackable && item.Features.Id == slot.item.Features.Id && slot.Amount < item.Features.MaxPerSlot)
                {
                    amountToAdd = item.Features.MaxPerSlot - slot.Amount;

                    slots.Add((i, amountToAdd));
                }

                amount -= amountToAdd;
                if (amount <= 0)
                    break;
            }

            if (amount > 0)
            {
                return null;
            }
            return slots;
        }

        public bool DoesFitInInventory(ItemId id, int amount)
        {
            var fitSlots = GetSlotsWhereItemFits(ItemsDatabase.GetItem(id), amount);
            return fitSlots == null || fitSlots.Count == 0;
        }
        private bool AllocateItemInNewSlots(Item item, int amount)
        {
            var fitSlots = GetSlotsWhereItemFits(item, amount);
            if (fitSlots == null || fitSlots.Count == 0)
            {
                return false;
            }

            foreach (var slotFitValue in fitSlots)
            {
                var slot = _slots[slotFitValue.slotIndex];
                _slots[slotFitValue.slotIndex] = new InventorySlot(item, slot.Amount + slotFitValue.amount);
            }

            OnInventoryChanged?.Invoke(this);
            return true;
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

        public void Drop(int slotIndex, vec2 position)
        {
            if (_slots != null)
            {
                var slot = _slots[slotIndex];

                if (!slot.IsEmpty())
                {
                    Debug.Log("Drop: " + slot.item.Features.Id);
                    GamePrefabs.Items.InstantiateCollectible(slot.item.Features.Id, slot.Amount, true, position);
                }

                _slots[slotIndex] = default;

                OnInventoryChanged?.Invoke(this);
            }
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
            if (index >= 0)
            {
                return _slots[index].Amount;
            }
            return 0;
        }
        public bool Use(ItemId id, int amountMul = 1)
        {
            if (id == ItemId.none)
                return false;

            var index = GetFirstSlotIndex(id);
            if (index >= 0)
            {
                return Use(index, amountMul);
            }
            return false;
        }

        public bool Use(int slotIndex, int amountMul = 1)
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

                OnInventoryChanged?.Invoke(this);

                return true;
            }

            return false;
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

        public void DropAll(vec2 position)
        {
            if (_slots != null)
            {
                for (int i = 0; i < _slots.Length; i++)
                {
                    Drop(i, position);
                }
            }
        }
    }
}
