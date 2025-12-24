using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    internal class RendererData
    {
        public Material Material { get; set; }
        public Mesh Mesh { get; set; }
        protected internal virtual bool IsDirty { get; protected set; } = true;
        internal event Action<Renderer> OnDestroyRenderer;
        public mat4 ModelMatrix { get; set; }
    }

    internal class RendererData2D : RendererData
    {
        private uint _colorpacket = Color.White;
        public bool IsBillboard { get; set; }
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
    }
}
