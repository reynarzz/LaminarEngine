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
        public virtual bool IsEnabled { get; }
        public Material Material { get; set; }
        public Mesh Mesh { get; set; }
        protected internal virtual bool IsDirty { get; set; } = true;
        internal event Action<RendererData> OnDestroyRenderer;

        internal void OnDestroy()
        {
            OnDestroyRenderer?.Invoke(this);
        }
        public mat4 ModelMatrix { get; set; }
        public bool PrivateBatch { get; set; }
        private readonly Guid _guid;
        private Action _onDraw;

        public RendererData(Guid id, Action onDraw)
        {
            _guid = id;
            _onDraw = onDraw;
        }
        public Guid GetID()
        {
            return _guid;
        }

        internal void MarkNotDirty()
        {
            IsDirty = false;
        }

        internal void Draw()
        {
            _onDraw?.Invoke();
        }

    }

    internal class RendererData2D : RendererData
    {
        private uint _colorpacket = Color.White;
        public bool IsBillboard { get; set; }
        public Transform Transform { get; }
   
        private Func<bool> _isEnabled;
        public override bool IsEnabled => _isEnabled?.Invoke() ?? false;

        //public RendererData2D(Guid id, Transform transform, Func<bool> isEnabled)
        //{

        //}
        public RendererData2D(Guid id, Transform transform, Action onDraw, Func<bool> isEnabled) : base(id, onDraw)
        {
            Transform = transform;
            _isEnabled = isEnabled;
        }
     
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
