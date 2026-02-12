using Engine.Graphics;
using Engine.Layers;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class TextureAssetBuilder : IAssetBuilder<TextureAsset, TextureMetaFile>
    {
        public TextureAsset BuildAsset(ref readonly AssetInfo info, TextureMetaFile meta, BinaryReader reader)
        {
            var data = GetData(reader, meta);

            var texture = new Texture2D(info.Path, meta.GUID, data.Config.Mode, data.Config.Filter,data.Width, data.Height,
                                        data.Channels, data.Config.PixelPerUnit, data.Data);

            return new TextureAsset(info.Path, meta.GUID, texture, new SpriteAtlas(meta, texture, meta.GUID));
        }

        public void UpdateAsset(ref readonly AssetInfo info, TextureAsset asset, TextureMetaFile meta, BinaryReader reader)
        {
            var data = GetData(reader, meta);
            asset.Texture.UpdateResource(data, info.Path, meta.GUID);
        }

        internal class TextureDeserializedData
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int Channels { get; set; }
            public byte[] Data { get; set; }
            public TextureConfig Config { get; set; }
        }
        private TextureDeserializedData GetData(BinaryReader reader, AssetMeta meta)
        {
            var width = reader.ReadInt32();
            var height = reader.ReadInt32();
            var comp = reader.ReadInt32();

            int imageSize = checked(width * height * comp);
            var imageData = new byte[imageSize];

            reader.BaseStream.ReadExactly(imageData);

            var texMeta = (TextureMetaFile)meta;
            return new TextureDeserializedData()
            {
                Width = width,
                Height = height,
                Channels = comp,
                Config = texMeta.Config,
                Data = imageData
            };
        }
    }
}