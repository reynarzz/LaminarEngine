using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class CharacterInventory : Inventory
    {
        public static CharacterInventory Empty { get; } = new CharacterInventory(0);
        public int Life { get; set; }
        public CharacterInventory(int maxSlots) : base(maxSlots) { }
    }
}