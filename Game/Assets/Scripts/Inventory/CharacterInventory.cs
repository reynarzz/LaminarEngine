using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class CharacterInventory : Inventory
    {
        public event Action<int> OnLifeChanged;

        private int _life;
        public int Life
        {
            get => _life; 
            set
            {
                if (_life == value)
                    return;
                _life = value;
                OnLifeChanged?.Invoke(_life);
            }
        }

        public CharacterInventory(int maxSlots) : base(maxSlots) { }
    }
}