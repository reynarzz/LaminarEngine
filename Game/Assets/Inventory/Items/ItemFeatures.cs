using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class ItemFeatures
    {
        public ItemId Id { get; set; }
        public int Amount { get; set; }
        public int MaxPerSlot { get; set; }
        public float SecondsToDisappear { get; set; }
        public bool MultipleSlots { get; set; }
        public bool IsStackable { get; set; }
        public bool AutoConsumable { get; set; } // Should consume right when obtaining it? it will not be added to the inventory if true.
        public bool DecreasesOnUse { get; set; }  // Can the amount decrease after use.
        public bool UserCanUseIt { get; set; }
        public bool UserCanRemove { get; set; }  // If AutoConsumable is false
        public bool RemoveAfterUse { get; set; }
    };
}