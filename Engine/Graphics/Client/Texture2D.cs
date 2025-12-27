using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;

namespace Engine
{
    public class Texture2D : Texture
    {
        public TextureAtlasData Atlas { get; } = new();
        private int _pixelsPerUnit;
        public int PixelPerUnit
        {
            get
            {
                _pixelsPerUnit = Math.Clamp(_pixelsPerUnit, 1, int.MaxValue);
                return _pixelsPerUnit;
            }
        }

        public static Texture2D White { get; } = Get1PixelTexture("WhiteTexture_Internal", [0xFF, 0xFF, 0xFF, 0xFF]);
        public static Texture2D Black { get; } = Get1PixelTexture("BlackTexture_Internal", [0x00, 0x00, 0x00, 0xFF]);

        
        public Texture2D(string path, Guid guid, TextureMode mode, TextureFilter filter, int width, int height, int channels, int pixelsPerUnit, byte[] data) :
                base(path, guid, mode, filter, width, height, channels, data)
        {
            if(pixelsPerUnit <= 0)
            {
                throw new ArgumentOutOfRangeException($"Invalid Pixels per unit '{pixelsPerUnit}' for texture: {path}");
            }
            _pixelsPerUnit = pixelsPerUnit;
        }

        public Texture2D(TextureMode mode, TextureFilter filter, int width, int height, int channels, int pixelsPerUnit, byte[] data) : 
            this(string.Empty, Guid.NewGuid(), mode, filter, width, height, channels, pixelsPerUnit, data)
        {
        }

        public Texture2D(TextureMode mode, TextureFilter filter, int width, int height, int channels, byte[] data) : 
            this(mode, filter, width, height, channels, 1, data)
        {
        }

        protected override IResourceHandle Create()
        {
            return GfxDeviceManager.Current.CreateTexture(new TextureDescriptor()
            {
                Width = Width,
                Height = Height,
                Channels = Channels,
                Buffer = Data,
                Mode = Mode,
                Filter = Filter
            });
        }

        private static Texture2D Get1PixelTexture(string name, byte[] color)
        {
            return new Texture2D(name, Guid.NewGuid(), TextureMode.Clamp, TextureFilter.Nearest, 1, 1, 4, 1, color);
        }
    }
}
