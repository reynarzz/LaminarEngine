using Engine.Graphics;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class Renderer2D : Renderer
    {
        private RendererData2D _renderData;
        internal override RendererData RendererData
        {
            get => _renderData;
            private protected set
            {
                _renderData = value as RendererData2D;
            }
        }

        internal override void OnInternalInitialize()
        {
            base.OnInternalInitialize();

           _renderData = new RendererData2D(GetID(), Transform, Draw, () => IsEnabled && Actor.IsActiveInHierarchy);
        }

        public virtual Color Color
        {
            get => _renderData.Color;
            set
            {
                if (_renderData.Color == value)
                {
                    return;
                }
                _renderData.Color = value;
            }
        }


        [SerializedField]
        public Sprite Sprite
        {
            get => _renderData.Sprite;
            set
            {
                if (_renderData.Sprite != null && _renderData.Sprite.Equals(value))
                    return;

                _renderData.Sprite = value;

                if (value)
                {
                    var chunk = value.GetAtlasCell();

                    float ppu = value.Texture.PixelPerUnit;
                    var width = (float)chunk.Width / ppu;
                    var height = (float)chunk.Height / ppu;

                    _renderData.Bounds = new Bounds()
                    {
                        Max = new GlmNet.vec3(width * 0.5f, height * 0.5f, 0),
                        Min = new GlmNet.vec3(-width * 0.5f, -height * 0.5f, 0),
                    };
                }
                else
                {
                    _renderData.Bounds = default;
                }

                _renderData.IsDirty = true;
            }
        }

        public virtual int SortOrder
        {
            get => _renderData.SortOrder;
            set
            {
                if (_renderData.SortOrder == value)
                    return;

                _renderData.SortOrder = value;
                _renderData.IsDirty = true;
            }
        }

        public virtual bool FlipX { get; set; }
        public virtual bool FlipY { get; set; }
    }
}