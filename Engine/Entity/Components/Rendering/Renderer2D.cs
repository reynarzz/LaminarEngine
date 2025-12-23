using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class Renderer2D : Renderer
    {
        private uint _colorpacket = Color.White;

        [ExposeEditorField]
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

        [ExposeEditorField]
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


        private int _sortingOrder = 0;

        [ExposeEditorField]
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

        [ExposeEditorField]
        public virtual bool FlipX { get; set; }

        [ExposeEditorField]
        public virtual bool FlipY { get; set; }
    }
}