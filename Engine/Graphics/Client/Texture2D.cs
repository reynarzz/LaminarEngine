using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Graphics;
using Engine;

namespace Engine
{
    public class Texture2D : Texture
    {
        private int _pixelsPerUnit;

        [SerializedField]
        public int PixelPerUnit
        {
            get
            {
                _pixelsPerUnit = Math.Clamp(_pixelsPerUnit, 1, int.MaxValue);
                return _pixelsPerUnit;
            }
            private set
            {
                _pixelsPerUnit = value;
            }
        }

        public static Texture2D White { get; } = Get1PixelTexture("WhiteTexture_Internal", [0xFF, 0xFF, 0xFF, 0xFF]);
        public static Texture2D Black { get; } = Get1PixelTexture("BlackTexture_Internal", [0x00, 0x00, 0x00, 0xFF]);
        private readonly TextureDescriptor _descriptor = new();
        public Texture2D(string path, Guid guid, TextureMode mode, TextureFilter filter, int width, int height, int channels, int pixelsPerUnit, byte[] data) :
                base(path, guid, mode, filter, width, height, channels, data)
        {
            if (pixelsPerUnit <= 0)
            {
                throw new ArgumentOutOfRangeException($"Invalid Pixels per unit '{pixelsPerUnit}' for texture: {path}");
            }
            _pixelsPerUnit = pixelsPerUnit;

            Create();
        }

        public Texture2D(TextureMode mode, TextureFilter filter, int width, int height, int channels, int pixelsPerUnit, byte[] data) :
            this(string.Empty, Guid.NewGuid(), mode, filter, width, height, channels, pixelsPerUnit, data)
        {
            Create();

        }

        public Texture2D(TextureMode mode, TextureFilter filter, int width, int height, int channels, byte[] data) :
            this(mode, filter, width, height, channels, 1, data)
        {

        }

        protected override void Create()
        {
            NativeResource = GfxDeviceManager.Current.CreateTexture(new TextureDescriptor()
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

        internal void UpdateResource(int width, int height, int xOffset, int yOffset, byte[] data)
        {
            Width = width;
            Height = height;
            Data = data;

            _descriptor.Width = width;
            _descriptor.Height = height;
            _descriptor.Channels = Channels;
            _descriptor.Buffer = data;
            _descriptor.Mode = Mode;
            _descriptor.Filter = Filter;
            _descriptor.XOffset = xOffset;
            _descriptor.YOffset = yOffset;

            GfxDeviceManager.Current.UpdateResouce(NativeResource, _descriptor);
        }
        internal override void UpdateResource(object data, string path, Guid guid)
        {
            var deserializedData = data as IO.TextureAssetBuilder.TextureDeserializedData;
            Width = deserializedData.Width;
            Height = deserializedData.Height;
            Mode = deserializedData.Config.Mode;
            Filter = deserializedData.Config.Filter;
            Channels = deserializedData.Channels;
            Data = deserializedData.Data;

            GfxDeviceManager.Current.UpdateResouce(NativeResource, new TextureDescriptor()
            {
                Width = Width,
                Height = Height,
                Channels = Channels,
                Buffer = Data,
                Mode = Mode,
                Filter = Filter
            });
        }
    }
}
