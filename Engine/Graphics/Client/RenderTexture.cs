using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;

namespace Engine
{
    public class RenderTexture : Texture
    {
        private static readonly RenderTargetDescriptor _desc = new();
        public RenderTexture(int width, int height) :
            base(string.Empty, Guid.NewGuid(), TextureMode.Clamp, width, height, 4, null)
        {
        }


        internal RenderTexture(GfxResource renderTarget, int width, int height) : 
            base(string.Empty, Guid.NewGuid(), width, height, 4, renderTarget)
        {
        }

        public void UpdateTarget(int width, int height)
        {
            Width = width;
            Height = height;

            _desc.Width = width;
            _desc.Height = height;
            GfxDeviceManager.Current.UpdateResouce(NativeResource, _desc);
        }

        protected override IResourceHandle Create()
        {
            _desc.Width = Width;
            _desc.Height = Height;

           return GfxDeviceManager.Current.CreateRenderTarget(_desc);
        }

        public byte[] ReadColorsRGBA()
        {
            return GfxDeviceManager.Current.ReadRenderTargetColors(NativeResource);
        }
    }
}
