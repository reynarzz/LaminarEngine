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
        private readonly RenderTargetDescriptor _desc = new();
        public RenderTexture(int width, int height) :
            base(string.Empty, Guid.NewGuid(), TextureMode.Clamp, TextureFilter.Nearest, width, height, 4, null)
        {
            Create();

        }

        public RenderTexture(int width, int height, TextureFilter filter, bool enableMipMaps) :
          base(string.Empty, Guid.NewGuid(), TextureMode.Clamp, filter, width, height, 4, null)
        {
            _desc.ColorTextureDescriptor.EnableMipMaps = enableMipMaps;
            Create();

        }

        public RenderTexture(int width, int height, TextureFilter filter, bool enableMipMaps, int samples) :
         base(string.Empty, Guid.NewGuid(), TextureMode.Clamp, filter, width, height, 4, null)
        {
            _desc.IsMultiSample = samples > 0;
            _desc.SamplesCount = samples;
            _desc.ColorTextureDescriptor.EnableMipMaps = enableMipMaps;

            Create();
        }


        public void UpdateTarget(int width, int height)
        {
            Width = width;
            Height = height;

            _desc.Width = width;
            _desc.Height = height;

            GfxDeviceManager.Current.UpdateResouce(NativeResource, _desc);
        }

        protected override void Create()
        {
            _desc.Width = Width;
            _desc.Height = Height;
            _desc.ColorTextureDescriptor.Width = Width;
            _desc.ColorTextureDescriptor.Height = Height;
            _desc.ColorTextureDescriptor.Mode = Mode;
            _desc.ColorTextureDescriptor.Filter = Filter;

            NativeResource = GfxDeviceManager.Current.CreateRenderTarget(_desc);
        }

        public byte[] ReadColorsRGBA()
        {
            //return GfxDeviceManager.Current.ReadRenderTargetColors(NativeResource);

            return null;
        }

        internal override void UpdateResource(object data, string path, Guid guid)
        {
            throw new NotImplementedException();
        }
    }
}
