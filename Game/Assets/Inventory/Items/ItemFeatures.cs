using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class ItemFeatures
    {
        public ItemId Id;
        public int Amount;
        public int MaxPerSlot;
        public float SecondsToDisappear;
        public bool IsStackable;
        public bool AutoConsumable; // Should consume right when obtaining it? it will not be added to the inventory if true.
        public bool DecreasesOnUse;  // Can the amount decrease after use.
        public bool UserCanUseIt;
        public bool UserCanRemove;  // If AutoConsumable is false
        public bool RemoveAfterUse;
    };
}