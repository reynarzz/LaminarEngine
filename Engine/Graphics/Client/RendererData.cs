using GlmNet;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    internal class RendererData
    {
        internal event Action<RendererData> OnDestroyRenderer;
        public Material Material { get; set; }
        public Mesh Mesh { get; set; }
        protected internal virtual bool IsDirty { get; set; } = true;
        public virtual bool IsEnabled { get; set; }
        public bool PrivateBatch { get; set; }
        public Guid ID { get; set; }
        public Bounds Bounds;

        private Action _onDraw;

        public RendererData(Guid id, Action onDraw)
        {
            ID = id;
            _onDraw = onDraw;
        }
        public Guid GetID()
        {
            return ID;
        }

        internal void MarkNotDirty()
        {
            IsDirty = false;
        }

        internal void Draw()
        {
            _onDraw?.Invoke();
        }
        internal void OnDestroy()
        {
            OnDestroyRenderer?.Invoke(this);
        }
    }

    internal class RendererData2D : RendererData
    {
        private uint _colorpacket = Color.White;
        public bool IsBillboard { get; set; }
        public Transform Transform { get; set; }

        private Func<bool> _isEnabledFunc;
        private bool _isEnabled = true;
        public override bool IsEnabled
        {
            get
            {
                return _isEnabledFunc?.Invoke() ?? _isEnabled;
            }
            set
            {
                if(_isEnabledFunc == null)
                {
                    _isEnabled = value;
                }
            }
        }

        //public RendererData2D(Guid id, Transform transform, Func<bool> isEnabled)
        //{

        //}
        public RendererData2D(Guid id, Transform transform) : this(id, transform, null, null)
        {
        }
        public RendererData2D(Guid id, Transform transform, Action onDraw, Func<bool> isEnabled) : base(id, onDraw)
        {
            Transform = transform;
            _isEnabledFunc = isEnabled;
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
