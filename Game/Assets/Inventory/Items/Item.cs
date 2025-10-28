using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class Item
    {
        public ItemFeatures Features { get; }
        public Item(ItemFeatures features)
        {
            Features = features;
        }
        public abstract Item GetCopy();
        protected virtual void Use(int requestedAmount, CharacterInventory inventory)
        {
            if (Features.DecreasesOnUse)
            {
                Features.Amount -= requestedAmount;
            }
        }
    }

    public abstract class Item<T> : Item where T : Item
    {
        protected Item(ItemFeatures features) : base(features) { }
        public override Item GetCopy()
        {
            return Activator.CreateInstance(typeof(T), Features) as Item;
        }
    }
}