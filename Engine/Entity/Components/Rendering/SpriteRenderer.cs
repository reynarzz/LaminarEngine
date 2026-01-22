using Engine.Graphics;
using Engine.Types;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [UniqueComponent]
    public class SpriteRenderer : Renderer2D
    {
        [SerializedField]
        public override int SortOrder { get => base.SortOrder; set => base.SortOrder = value; }

        [SerializedField]
        public override Color Color { get => base.Color; set => base.Color = value; }

        [SerializedField]
        public override bool FlipX
        {
            get => base.FlipX;
            set
            {
                if (base.FlipX == value)
                {
                    return;
                }

                base.FlipX = value;
                if(Sprite != null)
                {
                    Sprite.GetAtlasCell().UpdateUvs(QuadUV.FlipUV(Sprite.GetAtlasCell().Uvs, value, FlipY));
                }
                RendererData.IsDirty = true;
            }
        }

        [SerializedField]
        public override bool FlipY
        {
            get => base.FlipY;
            set
            {
                if (base.FlipY == value)
                {
                    return;
                }

                base.FlipY = value;
                if(Sprite != null)
                {
                    Sprite.GetAtlasCell().UpdateUvs(QuadUV.FlipUV(Sprite.GetAtlasCell().Uvs, FlipX, value));
                }
                RendererData.IsDirty = true;
            }
        }
    }
}