using FontStashSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    internal class FontTextureManager : ITexture2DManager
    {
        private TextureDescriptor _sharedDescriptor;
        internal FontTextureManager()
        {
            _sharedDescriptor = new TextureDescriptor();
        }

        public object CreateTexture(int width, int height)
        {
            return new Texture2D(TextureMode.Clamp, TextureFilter.Nearest, width, height, 4, null);
        }

        public Point GetTextureSize(object texture)
        {
            var tex2D = texture as Texture2D;
            return new Point(tex2D.Width, tex2D.Height);
        }

        public void SetTextureData(object texture, Rectangle bounds, byte[] data)
        {
            var tex2D = texture as Texture2D;
            tex2D.Data = data;
            _sharedDescriptor.XOffset = bounds.Left;
            _sharedDescriptor.YOffset = bounds.Top;
            _sharedDescriptor.Width = bounds.Width;
            _sharedDescriptor.Height = bounds.Height;
            _sharedDescriptor.Buffer = data;

            //tex2D.UpdateResource(bounds.Width, bounds.Height, bounds.Left, bounds.Top, data); // Why updating directly doesn't work?
            GfxDeviceManager.Current.UpdateResouce(tex2D.NativeResource, _sharedDescriptor);
        }
    }
}
