using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class Texture : Asset
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        [SerializedField] public TextureWrapMode Mode { get; protected set; }
        [SerializedField] public TextureFilterMode Filter { get; protected set; }
        public int Channels { get; protected set; }
        internal protected byte[] Data { get; internal set; }

        internal GfxResource NativeResource { get; protected private set; }

        internal Texture(Guid guid, TextureWrapMode mode, TextureFilterMode filter, int width, int height, int channels, byte[] data) : base(guid)
        {
            Width = width;
            Height = height;
            Channels = channels;
            Data = data;
            Mode = mode;
            Filter = filter;


        }

        internal Texture(string path, Guid guid, int width, int height, int channels, GfxResource nativeResource) : base(guid)
        {
            Width = width;
            Height = height;
            Channels = channels;

            NativeResource = nativeResource;
        }

        protected internal override void OnDestroy()
        {
            NativeResource.Dispose();
        }
        protected abstract void Create();
    }
}
