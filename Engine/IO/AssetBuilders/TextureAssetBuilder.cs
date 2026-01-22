using Engine.Graphics;
using Engine.Layers;
using SharedTypes;
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

            return new Texture2D(info.Path, guid, (TextureMode)data.Config.Mode, (TextureFilter)data.Config.Filter,
                data.Width, data.Height, data.Channels, data.Config.PixelPerUnit, data.Data);
        }

        internal override void UpdateAsset(AssetResourceBase asset, AssetMetaFileBase meta, BinaryReader reader)
        {
            var texture = asset as Texture2D;
            var updatedPath = texture.Path; // TODO: use actual updated texture path
            var data = GetData(reader, meta);

            texture.UpdateResource(data, updatedPath, meta.GUID);
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