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
                Levels = new()
            };

            var levelsCount = reader.ReadInt32();

            for (int i = 0; i < levelsCount; i++)
            {
                // Read level data
                var level = ReadLevel(reader);
                level.LevelIndex = i;
                tilemapData.Levels.Add(level.IID, level);
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

            level.Layers = new(layersCount);

            for (int i = 0; i < layersCount; i++)
            {
                var layer = ReadLayer(reader, level);
                layer.LayerIndex = i;
                level.Layers.Add(layer.IID, layer);
            }

            // Read level bounds.
            level.Bounds = EngineFileUtils.ReadStructNoAlloc<Bounds>(reader);

            return level;
        }

        private TilemapLevelLayer ReadLayer(BinaryReader reader, TilemapLevel level)
        {
            var layer = new TilemapLevelLayer();

            layer.Identifier = EngineFileUtils.ReadString(reader);
            layer.IID = EngineFileUtils.ReadGuidNoAlloc(reader);
            layer.SizeGridBased = EngineFileUtils.ReadStructNoAlloc<ivec2>(reader);
            layer.GridSize = reader.ReadInt32();
            layer.Opacity = reader.ReadSingle();
            layer.OffsetPx = EngineFileUtils.ReadStructNoAlloc<ivec2>(reader);
            layer.IsVisible = EngineFileUtils.ReadBool(reader);

            if (!layer.IsVisible)
            {
                return layer;
            }

            layer.Type = (TilemapLayerType)reader.ReadInt32();
            switch (layer.Type)
            {
                case TilemapLayerType.IntGrid:
                case TilemapLayerType.Tiles:
                case TilemapLayerType.AutoLayer:
                    ReadTiles(reader, layer);
                    break;
                case TilemapLayerType.Entities:
                    ReadEntities(reader, layer, level);
                    break;
                default:
                    throw new NotImplementedException(layer.Type.ToString());
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
            layer.TilesPosition = EngineFileUtils.ReadArray<vec2>(reader, tilesCount);
            var boxesCount = reader.ReadInt32();
            layer.CollisionBoxes = EngineFileUtils.ReadArray<Box>(reader, boxesCount);
            layer.Bounds = EngineFileUtils.ReadStructNoAlloc<Bounds>(reader);
        }

        private void ReadEntities(BinaryReader reader, TilemapLevelLayer layer, TilemapLevel level)
        {
            var entitiesCount = reader.ReadInt32();

            if (entitiesCount == 0)
                return;

            layer.Entities = new(entitiesCount);

            for (int i = 0; i < entitiesCount; i++)
            {
                var entity = new TilemapEntity();

                // Read identifier
                entity.Identifier = EngineFileUtils.ReadString(reader);

                // Read iid
                entity.IID = EngineFileUtils.ReadGuidNoAlloc(reader);

                entity.LayerIID = layer.IID;
                entity.LevelIID = level.IID;

                var tagsCount = reader.ReadInt32();
                if (tagsCount > 0)
                {
                    entity.Tags = new string[tagsCount];

                    for (int j = 0; j < tagsCount; j++)
                    {
                        // Read tag
                        entity.Tags[j] = EngineFileUtils.ReadString(reader);
                    }
                }

                // Read world position.
                entity.WorldPosition = EngineFileUtils.ReadStructNoAlloc<vec2>(reader);

                // Read pivot
                entity.Pivot = EngineFileUtils.ReadStructNoAlloc<vec2>(reader);

                var propertiesCount = reader.ReadInt32();

                layer.Entities.Add(entity.IID, entity);

                if (propertiesCount == 0)
                    continue;

                entity.Properties = new(propertiesCount);

                for (int j = 0; j < propertiesCount; j++)
                {
                    var property = ReadEntityProperty(reader);
                    entity.Properties.Add(property.Identifier, property);
                }
            }
        }

        private EntityPropertyData ReadEntityProperty(BinaryReader reader)
        {
            var propertyData = new EntityPropertyData();
            propertyData.Value = new EntityPropertyValue();

            // Read property identifier
            propertyData.Identifier = EngineFileUtils.ReadString(reader);

            // Read property type
            propertyData.Value.Type = (PropertyValueType)reader.ReadInt32();

            switch (propertyData.Value.Type)
            {
                case PropertyValueType.Unknown:
                    break;
                case PropertyValueType.String:
                    propertyData.Value.String = EngineFileUtils.ReadString(reader);
                    break;
                case PropertyValueType.Bool:
                    propertyData.Value.Bool = EngineFileUtils.ReadBool(reader);
                    break;
                case PropertyValueType.Int:
                    propertyData.Value.Int = reader.ReadInt32();
                    break;
                case PropertyValueType.Float:
                    propertyData.Value.Float = reader.ReadSingle();
                    break;
                case PropertyValueType.Vec2:
                    propertyData.Value.Vec2 = EngineFileUtils.ReadStructNoAlloc<vec2>(reader);
                    break;
                case PropertyValueType.Enum:
                    {
                        propertyData.Value.Enum = new EnumValue()
                        {
                            EnumName = EngineFileUtils.ReadString(reader),
                            EnumValStr = EngineFileUtils.ReadString(reader),
                        };
                    }
                    break;
                case PropertyValueType.Color:
                    propertyData.Value.Color = (Color32)(ColorPacketRGBA)reader.ReadUInt32();
                    break;
                case PropertyValueType.EntityRef:
                    propertyData.Value.EntityRef = EngineFileUtils.ReadStructNoAlloc<EntityRef>(reader);
                    break;
                case PropertyValueType.Tile:
                    propertyData.Value.Tile = EngineFileUtils.ReadStructNoAlloc<TileRef>(reader);
                    break;
                case PropertyValueType.StringArray:
                    propertyData.Value.StringArray = ReadStringArray(reader);
                    break;
                case PropertyValueType.BoolArray:
                    propertyData.Value.BoolArray = ReadArray<bool>(reader);
                    break;
                case PropertyValueType.IntArray:
                    propertyData.Value.IntArray = ReadArray<int>(reader);
                    break;
                case PropertyValueType.FloatArray:
                    propertyData.Value.FloatArray = ReadArray<float>(reader);
                    break;
                case PropertyValueType.Vec2Array:
                    propertyData.Value.Vec2Array = ReadArray<vec2>(reader);
                    break;
                case PropertyValueType.EnumArray:
                    propertyData.Value.EnumArray = ReadEnumArray(reader);
                    break;
                case PropertyValueType.ColorArray:
                    {
                        var colorUintArr = ReadArray<uint>(reader);

                        if (colorUintArr != null && colorUintArr.Length > 0)
                        {
                            var colors32Arr = new Color32[colorUintArr.Length];

                            for (uint i = 0; i < colorUintArr.Length; i++)
                            {
                                colors32Arr[i] = (Color32)(ColorPacketRGBA)colorUintArr[i];
                            }
                            propertyData.Value.ColorArray = colors32Arr;
                        }
                    }
                    break;
                case PropertyValueType.EntityRefArray:
                    propertyData.Value.EntityRefArray = ReadArray<EntityRef>(reader);
                    break;
                case PropertyValueType.TileArray:
                    propertyData.Value.TileArray = ReadArray<TileRef>(reader);
                    break;
                default:
                    throw new NotImplementedException(propertyData.Value.Type.ToString());
            }

            return propertyData;
        }

        private T[] ReadArray<T>(BinaryReader reader) where T : unmanaged
        {
            var count = reader.ReadInt32();
            if (count == 0)
                return null;

            return EngineFileUtils.ReadArray<T>(reader, count);
        }

        private string[] ReadStringArray(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            if (count == 0)
                return null;

            var arr = new string[count];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = EngineFileUtils.ReadString(reader);
            }

            return arr;
        }


        private EnumValue[] ReadEnumArray(BinaryReader reader)
        {
            var count = reader.ReadInt32();

            if (count == 0)
                return null;

            var enumName = EngineFileUtils.ReadString(reader);
            var enumsArr = new EnumValue[count];
            for (int i = 0; i < enumsArr.Length; i++)
            {
                enumsArr[i] = new EnumValue()
                {
                    EnumName = enumName,
                    EnumValStr = EngineFileUtils.ReadString(reader),
                };
            }

            return enumsArr;
        }

        public void UpdateAsset(ref readonly AssetInfo info, TilemapAsset asset, TilemapMeta meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
