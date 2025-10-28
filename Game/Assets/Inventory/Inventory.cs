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
        public virtual int MaxSlots { get; set; }
        private readonly List<Item> _items = new();
        public int Count => _items.Count;
        protected event Action<Item> OnItemRemoved;

        public bool Add(Item item)
        {
            var currentItem = GetItem(item.Features.Id);

            if (currentItem != null && currentItem.Features.IsStackable &&
                (currentItem.Features.Amount + item.Features.Amount) < currentItem.Features.MaxDefault)
            {
                currentItem.Features.Amount += item.Features.Amount;
                return true;
            }

            var canAdd = _items.Count < MaxSlots;
            if (canAdd)
            {
                _items.Add(item.GetCopy());
            }

            return canAdd;
        }

        public void Remove(Item item)
        {
            if (_items.Remove(item))
            {
                OnItemRemoved?.Invoke(item);
            }
        }

        public void Drop(Item item)
        {
            if (_items.Remove(item))
            {
                Debug.Log("Instance into the world");
            }
        }

        public Item GetItem(int index)
        {
            return _items[index];
        }
        public Item GetItem(ItemId id)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Features.Id == id)
                {
                    return _items[i];
                }
            }
            return null;
        }
        public void DropAll()
        {
            foreach (Item item in _items)
            {
                Drop(item);
            }
            _items.Clear();
        }
    }
}
