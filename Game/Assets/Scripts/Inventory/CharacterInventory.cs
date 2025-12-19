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
                var life = Math.Clamp(value, 0, MaxLife);

                if (_life == life)
                    return;
                _life = life;
                OnLifeChanged?.Invoke(_life);
            }
        }
        public int MaxLife { get; set; }
        public CharacterInventory(int maxSlots) : base(maxSlots) { }

    }
}