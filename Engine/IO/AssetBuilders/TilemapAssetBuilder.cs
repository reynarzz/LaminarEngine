using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class TilemapAssetBuilder : IAssetBuilder<TilemapAsset, TilemapMeta>
    {
        public TilemapAsset BuildAsset(ref readonly AssetInfo info, TilemapMeta meta, BinaryReader reader)
        {

            var tilemapData = new TilemapData()
            {
                // Read version
                Version = reader.ReadUInt32(),

                // Read levels count
                Levels = new TilemapLevel[reader.ReadInt32()]
            };

            for (int i = 0; i < tilemapData.Levels.Length; i++)
            {
                // Read level data
                tilemapData.Levels[i] = ReadLevel(reader);
            }
            return new TilemapAsset(info.Path, meta.GUID, tilemapData);
        }

        private TilemapLevel ReadLevel(BinaryReader reader)
        {
            var level = new TilemapLevel();

            // Read identifier
            level.Identifier = EngineFileUtils.ReadString(reader);
            // Read iid
            level.IID = EngineFileUtils.ReadGuidNoAlloc(reader);

            // Read world position in pixels
            level.WorldPositionPx = EngineFileUtils.ReadStructNoAlloc<ivec2>(reader);

            // Read size in pixels
            level.SizePx = EngineFileUtils.ReadStructNoAlloc<ivec2>(reader);

            // Read world depth
            level.Depth = reader.ReadInt32();

            // Read background color
            level.BackgroundColor = (Color32)(ColorPacketRGBA)reader.ReadUInt32();

            var layersCount = reader.ReadInt32();

            level.Layers = new TilemapLevelLayer[layersCount];

            for (int i = 0; i < layersCount; i++)
            {
                level.Layers[i] = ReadLayer(reader);
            }

            return level;
        }

        private TilemapLevelLayer ReadLayer(BinaryReader reader)
        {
            var layer = new TilemapLevelLayer();

            layer.Identifier = EngineFileUtils.ReadString(reader);
            layer.IID = EngineFileUtils.ReadGuidNoAlloc(reader);
            layer.IsVisible = EngineFileUtils.ReadBool(reader);
            layer.SizeGridBased = EngineFileUtils.ReadStructNoAlloc<ivec2>(reader);
            layer.Opacity = reader.ReadSingle();
            layer.OffsetPx = EngineFileUtils.ReadStructNoAlloc<ivec2>(reader);

            if (!layer.IsVisible)
            {
                return layer;
            }

            var layerType = (TilemapLayerType)reader.ReadInt32();
            switch (layerType)
            {
                case TilemapLayerType.IntGrid:
                case TilemapLayerType.Tiles:
                case TilemapLayerType.AutoLayer:

                    break;
                case TilemapLayerType.Entities:
                    break;
                default:
                    break;
            }

            return layer;
        }

        private void ReadTiles(BinaryReader reader, TilemapLevelLayer layer)
        {
            var tilesCount = reader.ReadInt32();

            if (tilesCount == 0)
                return;

            layer.IndicesToDraw = reader.ReadInt32();
            layer.Vertices = EngineFileUtils.ReadArray<Vertex>(reader, tilesCount * 4);
            layer.TilesPosition = new vec2[tilesCount];

        }

        private void ReadEntities()
        {

        }
        public void UpdateAsset(ref readonly AssetInfo info, TilemapAsset asset, TilemapMeta meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
