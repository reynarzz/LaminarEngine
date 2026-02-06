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
    internal class TextureAssetBuilder : AssetBuilderBase
    {
        internal override AssetResourceBase BuildAsset(AssetInfo info, AssetMetaFileBase meta, Guid guid, BinaryReader reader)
        {
            var data = GetData(reader, meta);

            var texture = new Texture2D(info.Path, guid, data.Config.Mode, data.Config.Filter,data.Width, data.Height,
                                        data.Channels, data.Config.PixelPerUnit, data.Data);

            return new TextureAsset(info.Path, guid, texture, new SpriteAtlas(meta as TextureMetaFile, texture, guid));
        }

        internal override void UpdateAsset(AssetResourceBase asset, AssetMetaFileBase meta, BinaryReader reader)
        {
            var textureAsset = asset as TextureAsset;
            var updatedPath = textureAsset.Texture.Path;
            var data = GetData(reader, meta);
            textureAsset.Texture.UpdateResource(data, updatedPath, meta.GUID);
        }

        internal class TextureDeserializedData
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int Channels { get; set; }
            public byte[] Data { get; set; }
            public TextureConfig Config { get; set; }
        }
        private TextureDeserializedData GetData(BinaryReader reader, AssetMetaFileBase meta)
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