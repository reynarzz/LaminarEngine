using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class Renderer2D : Renderer
    {
        private int _sortingOrder = 0;
        public int SortOrder
        {
            get => _sortingOrder;
            set
            {
                if (_sortingOrder == value)
                    return;

                _sortingOrder = value;
                IsDirty = true;
            }
        }
        private uint _colorpacket = Color.White;
        public Color Color
        {
            get => _colorpacket;
            set
            {
                if (_colorpacket == value)
                {
                    return;
                }
                _colorpacket = value;
                IsDirty = true;
            }
        }

        private Sprite _sprite;
        public Sprite Sprite
        {
            get => _sprite;
            set
            {
                if (_sprite != null && _sprite.Equals(value))
                    return;

                IsDirty = true;
                _sprite = value;
            }
        }

        public virtual bool FlipX { get; set; }
        public virtual bool FlipY { get; set; }
    }
}