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
        public virtual void Use(int amountMul, Inventory inventory) { }
    }
}