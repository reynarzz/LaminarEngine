using Editor.Utils;
using Engine;
using Engine.IO;
using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Editor.Cooker
{
    internal class TilemapAssetProcessor : IAssetProcessor
    {
        public class Tilemap
        {
            public int Version { get; set; }

            public TilemapLevel[] Levels { get; set; }
        }

        public class TilemapLevel
        {
            public string Identifier { get; set; }
            public string IID { get; set; }
            public ivec2 WorldPosition { get; set; }
            public ivec2 SizeInPixels { get; set; }
            public int Depth { get; set; }
            public Bounds Bounds { get; set; }
            public TilemapLevelLayer[] Layers { get; set; }
        }

        public enum TilemapLayerType
        {
            IntGrid,
            Entities,
            Tiles,
            AutoLayer
        }

        public class TilemapLevelLayer
        {
            public string Identifier { get; set; }
            public string IID { get; set; }
            public ivec2 SizeGridBased { get; set; }
            public int GridSize { get; set; }
            public float Opacity { get; set; }
            public ivec2 Offset { get; set; }
            public Bounds Bounds { get; set; }
            public TilemapLayerType Type { get; set; }
            public vec2[] TilesPosition { get; set; }
            public TilemapEntity[] Entities { get; set; }
            internal Vertex[] Vertices { get; set; }
            internal int IndicesToDraw { get; set; }

            public ReadOnlySpan<vec2> GetTilesPosition()
            {
                return TilesPosition;
            }
        }

        public class TilemapEntity
        {
            public string Identifier { get; set; }

            // Instance identifier
            public string IID { get; set; }

            public vec2 Pivot { get; set; }
            public string[] Tags { get; set; }
            public ivec2 SizeInPixels { get; set; }
            public ivec2 WorldPosition { get; set; }
        }

        public class EntityProperty
        {
            public string Identifier { get; set; }
            public EntityPropertyValue Value { get; set; }
        }

        public enum PropertyValueType
        {
            Unknown,
            String,
            Bool,
            Int,
            Float,
            Vec2,
            Enum,
            Color,
            EntityRef,
            Tile,

            StringArray,
            BoolArray,
            IntArray,
            FloatArray,
            Vec2Array,
            EnumArray,
            ColorArray,
            EntityRefArray,
            TileArray,
            ComplexJson
        }

        public struct EntityPropertyValue
        {
            public PropertyValueType Type { get; set; }
            public int IntValue { get; set; }
            public string StringValue { get; set; }
            public EnumValue EnumValue { get; set; }
            public float FloatValue { get; set; }
            public float BoolValue { get; set; }
            public vec2 PointValue { get; set; }
            public EntityRef EntityRef { get; set; }
        }

        public struct EnumValue
        {
            public string EnumTitle { get; set; }
            public string EnumValStr { get; set; }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EntityRef
        {
            public Guid entityIid;
            public Guid layerIid;
            public Guid levelIid;
            public Guid worldIid;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TileRef
        {
            public int TilesetUid;
            public ivec2 SizePx;
            public ivec2 PositionPx;
        }
        /* Tilemap file format
        Version char[4]

        -Metadata
        Levels count (u32)
        Levels (Level format)
        */

        /*Level format
             identifier str len (u32)
             Identifier (string)
             iid (guid)
             WorldX (s32)
             WorldY (s32)
             Level width px (u32)
             Level height px (u32)
             Depth (s32)
             BGColor (u32)
             Layers count (u32)
             Layers (Layer format)
             Bounding box (vec4)
         */

        /* Layer format
            identifier str len (u32)
            Identifier (string)
            iid (guid)
            Visible (bool)
            PixelsPerUnit (u32) (16px default)
            Grid based size (ivec2)
            Grid size (u32)
            Opacity (float)
            Offset Pixels X (s32)
            Offset Pixels Y (s32)

            Layer type (s32)
            Tiles count (u32)
            Indices to draw (u32)

            Tiles vertices (Vertex[])
            Tiles worldPositions (vec2[])
            Bounding box (vec4)
        */

        /* Entity format
            identifier str len (u32)
            identifier (string)
            iid (guid)
            tags count (s32)
            tags (string)
            worldPosition (vec2)
            pivot (vec2)
            properties data count (s32)
         */

        // TODO: FIX:
        // 1-Meta arrays for levels and layers should be always in sync.
        // 2-I have to read texture metadata directly from here since AssetDatabase is not ready before importing all the assets.


        private readonly Dictionary<Guid, TextureMetaFile> _texturesMeta = new();

        private const float DEFAULT_PIXEL_PER_UNIT = 16;
        private const int VERSION = 1;

        AssetProccesResult IAssetProcessor.Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            var bytes = new byte[(int)reader.BaseStream.Length];
            reader.BaseStream.ReadExactly(bytes, 0, bytes.Length);
            var json = Encoding.UTF8.GetString(bytes);
            var ldtk = LdtkJson.FromJson(json);

            using var mem = new MemoryStream();
            using var writer = new BinaryWriter(mem);

            // Version
            writer.Write(VERSION);

            try
            {
                WriteTilemap(writer, ldtk, meta as TilemapMeta);
            }
            catch (Exception e)
            {
                Debug.Error(e);

                return new AssetProccesResult()
                {
                    IsSuccess = false,
                    Data = null,
                };
            }

            return new AssetProccesResult()
            {
                IsSuccess = true,
                Data = mem.ToArray()
            };
        }

        public void WriteTilemap(BinaryWriter writer, LdtkJson project, TilemapMeta meta)
        {
            // Levels count
            writer.Write(project.Levels.Length);

            for (var i = 0; i < project.Levels.Length; i++)
            {
                var level = project.Levels[i];
                var levelBounds = Bounds.GetInitialized();
                var levelConfig = meta.LevelConfig?.Length == project.Levels.Length ? meta.LevelConfig[i] : null;

                EditorUtils.WriteString(writer, level.Identifier);
                EditorUtils.WriteGuidNoAlloc(writer, Guid.Parse(level.Iid));

                // Level position
                EditorUtils.WriteStruct(writer, new ivec2((int)level.WorldX, (int)level.WorldY));

                // Level size
                EditorUtils.WriteStruct(writer, new ivec2((int)level.PxWid, (int)level.PxHei));

                // Level depth
                writer.Write((int)level.WorldDepth);

                // Level Background color
                writer.Write(HexToRgbaUint(level.BgColor));

                // Layers count
                writer.Write(level.LayerInstances.Length);

                for (int j = 0; j < level.LayerInstances.Length; j++)
                {
                    var layer = level.LayerInstances[j];
                    TextureMetaFile textureMeta = null;

                    if (levelConfig != null && levelConfig.LayersTextureRef.Length > j)
                    {
                        var guid = levelConfig.LayersTextureRef[j];

                        if (guid != Guid.Empty)
                        {
                            if (!_texturesMeta.TryGetValue(guid, out textureMeta))
                            {
                                var assetPath = EditorIOLayer.Database.GetAssetInfo(guid).Path;
                                textureMeta = EditorAssetUtils.GetMetaFromAssetPath(assetPath, AssetType.Texture) as TextureMetaFile;
                                _texturesMeta.Add(guid, textureMeta);
                            }
                        }
                    }

                    float ppu = textureMeta?.Config?.PixelPerUnit ?? DEFAULT_PIXEL_PER_UNIT;

                    // Layer Identifier
                    EditorUtils.WriteString(writer, layer.Identifier);

                    // Layer IID
                    EditorUtils.WriteGuidNoAlloc(writer, Guid.Parse(layer.Iid));

                    // Layer visible
                    EditorUtils.WriteBool(writer, layer.Visible);

                    // Pixels per unit.
                    writer.Write(ppu);

                    // Grid based size
                    EditorUtils.WriteStruct(writer, new ivec2((int)layer.CWid, (int)layer.CHei));

                    // Grid size
                    writer.Write((int)layer.GridSize);

                    // Opacity
                    writer.Write((float)layer.Opacity);

                    // Total Offset in pixels
                    EditorUtils.WriteStruct(writer, new ivec2((int)layer.PxTotalOffsetX, (int)layer.PxTotalOffsetY));

                    if (!layer.Visible)
                        continue;

                    var layerType = Enum.Parse<TilemapLayerType>(layer.Type);

                    // Layer type
                    writer.Write((int)layerType);

                    switch (layerType)
                    {
                        case TilemapLayerType.AutoLayer:
                        case TilemapLayerType.IntGrid:
                            WriteLayerTiles(writer, level, layer, layer.AutoLayerTiles, textureMeta, ppu, ref levelBounds);
                            break;
                        case TilemapLayerType.Tiles:
                            WriteLayerTiles(writer, level, layer, layer.GridTiles, textureMeta, ppu,
                                ref levelBounds);
                            break;
                        case TilemapLayerType.Entities:
                            WriteEntityInstances(writer, level, layer, ppu);
                            break;
                        default:
                            break;
                    }
                }

                EditorUtils.WriteStruct(writer, levelBounds);
            }

            _texturesMeta.Clear();
        }

        private void WriteEntityInstances(BinaryWriter writer, ldtk.Level level, ldtk.LayerInstance layer, float pixelPerUnit)
        {
            T[] ParseJsonArray<T>(object value)
            {
                if (value != null)
                {
                    return JsonConvert.DeserializeObject<T[]>(value.ToString());
                }

                return Array.Empty<T>();
            }

            void WriteArray<T>(BinaryWriter writer, T[] arr) where T : unmanaged
            {
                // Write Count
                writer.Write(arr.Length);

                if (arr.Length > 0)
                {
                    // Write array
                    EditorUtils.WriteSpan(writer, arr);
                }
            }

            foreach (var entity in layer.EntityInstances)
            {
                // Entity identifier
                EditorUtils.WriteString(writer, entity.Identifier);

                // Write entity iiD
                EditorUtils.WriteGuidNoAlloc(writer, Guid.Parse(entity.Iid));

                // Tags count
                writer.Write(entity.Tags.Length);

                // Entity Tags
                foreach (var tag in entity.Tags)
                {
                    EditorUtils.WriteString(writer, tag);
                }

                var worldPosition = ConvertToWorld(entity.Px[0], entity.Px[1], pixelPerUnit, level, layer);

                // Entity position
                EditorUtils.WriteStruct(writer, worldPosition);

                // Write pivot
                EditorUtils.WriteStruct(writer, new vec2((float)entity.Pivot[0], (float)entity.Pivot[1]));

                // Write Properties count
                writer.Write(entity.FieldInstances.Length);
                foreach (var field in entity.FieldInstances)
                {
                    // Field identifier
                    EditorUtils.WriteString(writer, field.Identifier);

                    var ValType = field.Type;
                    var propType = ParsePropertyType(field.Type);

                    // Property type
                    writer.Write((int)propType);

                    Debug.Log(ValType);
                    if (propType == PropertyValueType.Color)
                    {
                        // Write Color
                        writer.Write(DeserializeColorSafe(field.Value));
                    }
                    else if (propType == PropertyValueType.String)
                    {
                        string str = null;
                        if (field.Value != null)
                        {
                            str = field.Value.ToString();
                        }

                        // Write string
                        EditorUtils.WriteString(writer, str);
                    }
                    else if (propType == PropertyValueType.Float)
                    {
                        float floatVal = field.Value != null ? Convert.ToSingle(field.Value) : 0;

                        // Write Float
                        writer.Write(floatVal);
                    }
                    else if (propType == PropertyValueType.Bool)
                    {
                        bool boolVal = field.Value != null ? (bool)field.Value : false;

                        // Write Bool 
                        EditorUtils.WriteBool(writer, boolVal);
                    }
                    else if (propType == PropertyValueType.Int)
                    {
                        int intVal = field.Value != null ? (int)(long)field.Value : 0;
                        // Write int 
                        writer.Write(intVal);
                    }
                    else if (propType == PropertyValueType.Vec2)
                    {
                        vec2 pointWorldPos = default;

                        if (field.Value != null)
                        {
                            var gridPoint = JsonConvert.DeserializeObject<GridPoint>(field.Value.ToString());
                            pointWorldPos = ConvertToWorld(gridPoint.Cx, gridPoint.Cy, pixelPerUnit, level, layer, true);
                        }

                        // Write point coords.
                        EditorUtils.WriteStruct(writer, pointWorldPos);
                    }
                    else if (propType == PropertyValueType.Enum)
                    {
                        var enumFieldName = field.Type.Substring(field.Type.IndexOf('.') + 1);
                        var enumVal = field?.Value?.ToString() ?? string.Empty;

                        // Enum field name
                        EditorUtils.WriteString(writer, enumFieldName);

                        // Enum value str
                        EditorUtils.WriteString(writer, enumVal);
                    }
                    else if (propType == PropertyValueType.Tile)
                    {
                        var tileRef = GetTileRefStruct(DeserializeJsonFieldSafe<ldtk.TilesetRectangle>(field));

                        // Write Tile ref
                        EditorUtils.WriteStruct(writer, tileRef);
                    }
                    else if (propType == PropertyValueType.EntityRef)
                    {
                        var entityRef = DeserializeEntityRefSafe(field);

                        // Write entity ref
                        WriteEntityRef(writer, entityRef);
                    }
                    else if (propType == PropertyValueType.EnumArray)
                    {
                        var startIndex = field.Type.IndexOf('.') + 1;
                        var count = field.Type.LastIndexOf('>') - startIndex;
                        var enumFieldName = field.Type.Substring(startIndex, count);
                        var enumArrVals = ParseJsonArray<string>(field.Value);
                        // Enums field name
                        EditorUtils.WriteString(writer, enumFieldName);

                        // Enums count
                        writer.Write(enumArrVals.Length);

                        // Enums str
                        foreach (var str in enumArrVals)
                        {
                            EditorUtils.WriteString(writer, str);
                        }
                    }
                    else if (propType == PropertyValueType.Vec2Array)
                    {
                        var points = ParseJsonArray<GridPoint>(field.Value);
                        // Points count
                        writer.Write(points.Length);

                        foreach (var point in points)
                        {
                            var pointWorldPos = ConvertToWorld(point.Cx, point.Cy, pixelPerUnit, level, layer, true);

                            // Points coord
                            EditorUtils.WriteStruct(writer, pointWorldPos);
                        }
                    }
                    else if (propType == PropertyValueType.IntArray)
                    {
                        // Int array
                        WriteArray(writer, ParseJsonArray<int>(field.Value));
                    }
                    else if (propType == PropertyValueType.BoolArray)
                    {
                        // Bool array
                        WriteArray(writer, ParseJsonArray<bool>(field.Value));
                    }
                    else if (propType == PropertyValueType.FloatArray)
                    {
                        // Float array
                        WriteArray(writer, ParseJsonArray<float>(field.Value));
                    }
                    else if (propType == PropertyValueType.StringArray)
                    {
                        var arrVal = ParseJsonArray<string>(field.Value);

                        // String array len
                        writer.Write(arrVal.Length);

                        // Write str array 
                        foreach (var str in arrVal)
                        {
                            EditorUtils.WriteString(writer, str);
                        }
                    }
                    else if (propType == PropertyValueType.EntityRefArray)
                    {
                        var arrVal = ParseJsonArray<EntityRef>(field.Value);

                        // Entity ref count
                        writer.Write(arrVal.Length);

                        // Write EntityRef array
                        foreach (var entityRef in arrVal)
                        {
                            WriteEntityRef(writer, entityRef);
                        }
                    }
                    else if (propType == PropertyValueType.ColorArray)
                    {
                        var arrVal = ParseJsonArray<string>(field.Value);

                        // Color count
                        writer.Write(arrVal.Length);

                        // Write color array
                        foreach (var coloStr in arrVal)
                        {
                            writer.Write(DeserializeColorSafe(coloStr));
                        }
                    }
                    else if (propType == PropertyValueType.TileArray)
                    {
                        var tilesRefArr = ParseJsonArray<ldtk.TilesetRectangle>(field.Value);

                        if (tilesRefArr == null || tilesRefArr.Length == 0)
                        {
                            writer.Write((int)0);
                        }
                        else
                        {
                            // Write tile count
                            writer.Write(tilesRefArr.Length);

                            // Write tile array
                            foreach (var tile in tilesRefArr)
                            {
                                EditorUtils.WriteStruct(writer, GetTileRefStruct(tile));
                            }
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("Unknown property type: " + propType);
                    }
                }
            }
        }

        private void WriteEntityRef(BinaryWriter writer, EntityRef entityRef)
        {
            // Write EntityRef
            EditorUtils.WriteGuidNoAlloc(writer, entityRef.entityIid);
            EditorUtils.WriteGuidNoAlloc(writer, entityRef.layerIid);
            EditorUtils.WriteGuidNoAlloc(writer, entityRef.levelIid);
            EditorUtils.WriteGuidNoAlloc(writer, entityRef.worldIid);
        }

        private uint DeserializeColorSafe(object value)
        {
            uint color = uint.MaxValue;

            if (value != null)
            {
                color = HexToRgbaUint(value.ToString());
            }

            return color;
        }

        private EntityRef DeserializeEntityRefSafe(FieldInstance field)
        {
            EntityRef entityRef = default;
            if (field.Value != null)
            {
                entityRef = JsonConvert.DeserializeObject<EntityRef>(field.Value.ToString());
            }
            return entityRef;
        }

        private T DeserializeJsonFieldSafe<T>(FieldInstance field)
        {
            if (field.Value != null)
            {
                return JsonConvert.DeserializeObject<T>(field.Value.ToString());
            }

            return default;
        }
        private TileRef GetTileRefStruct(ldtk.TilesetRectangle tileRefInst)
        {
            var tileDef = new TileRef()
            {
                TilesetUid = -1
            };

            if (tileRefInst != null)
            {
                tileDef = new TileRef()
                {
                    TilesetUid = (int)tileRefInst.TilesetUid,
                    PositionPx = new ivec2((int)tileRefInst.X, (int)tileRefInst.Y),
                    SizePx = new ivec2((int)tileRefInst.W, (int)tileRefInst.H)
                };
            }

            return tileDef;
        }
        private PropertyValueType ParsePropertyType(string type)
        {
            if (type.StartsWith("LocalEnum."))
            {
                return PropertyValueType.Enum;
            }
            else if (type.StartsWith("Array<LocalEnum."))
            {
                return PropertyValueType.EnumArray;
            }

            switch (type)
            {
                case "Int":
                    return PropertyValueType.Int;
                case "Float":
                    return PropertyValueType.Float;
                case "Bool":
                    return PropertyValueType.Bool;
                case "String":
                case "Multilines":
                case "FilePath":
                    return PropertyValueType.String;
                case "Point":
                    return PropertyValueType.Vec2;
                case "Color":
                    return PropertyValueType.Color;
                case "EntityRef":
                    return PropertyValueType.EntityRef;
                case "Tile":
                    return PropertyValueType.Tile;
                case "Array<Point>":
                    return PropertyValueType.Vec2Array;
                case "Array<Int>":
                    return PropertyValueType.IntArray;
                case "Array<Bool>":
                    return PropertyValueType.BoolArray;
                case "Array<Float>":
                    return PropertyValueType.FloatArray;
                case "Array<EntityRef>":
                    return PropertyValueType.EntityRefArray;
                case "Array<Color>":
                    return PropertyValueType.ColorArray;
                case "Array<String>":
                case "Array<Multilines>":
                case "Array<FilePath>":
                    return PropertyValueType.StringArray;
                case "Array<Tile>":
                    return PropertyValueType.TileArray;
                default:
                    throw new Exception("Unknown property type: " + type);
            }
        }

        private void WriteLayerTiles(BinaryWriter writer, ldtk.Level level, ldtk.LayerInstance layer,
            ldtk.TileInstance[] tiles, TextureMetaFile texture, float ppu, ref Bounds levelBounds)
        {
            int indexToDraw = tiles.Length * 6;
            var layerBounds = Bounds.GetInitialized();
            var tilesPositions = new vec2[tiles.Length];

            // Tiles count
            writer.Write(tiles.Length);

            // Indices to draw
            writer.Write(indexToDraw);

            for (int i = 0; i < tiles.Length; i++)
            {
                var tile = tiles[i];
                bool isFlippedX = (tile.F & 1) != 0 || tile.F == 3;
                bool isFlippedY = (tile.F & 2) != 0 || tile.F == 3;

                float tilePxX = tile.Px[0];
                float tilePxY = tile.Px[1];

                float worldX = level.WorldX + tilePxX + layer.PxTotalOffsetX;
                float worldY = -level.WorldY + -tilePxY + -layer.PxTotalOffsetY;

                var position = new vec3(worldX / ppu, worldY / ppu, 0);

                tilesPositions[i] = new vec2(position.x, position.y);

                WriteTile(writer, new Tile((int)tile.T, isFlippedX, isFlippedY), position, texture, ppu,
                    ref layerBounds);
            }

            if (tiles.Length > 0)
            {
                // Tiles positions
                EditorUtils.WriteSpan(writer, tilesPositions);
            }

            layerBounds.Min -= vec3.One * 0.5f;
            layerBounds.Max += vec3.One * 0.5f;
            layerBounds.Min.z = 0;
            layerBounds.Max.z = 0;

            EditorUtils.WriteStruct(writer, layerBounds);

            ref var max = ref levelBounds.Max;
            ref var min = ref levelBounds.Min;
            levelBounds.Max = new vec3(Math.Max(max.x, layerBounds.Max.x), Math.Max(max.y, layerBounds.Max.y), 0);
            levelBounds.Min = new vec3(Math.Min(min.x, layerBounds.Min.x), Math.Min(min.y, layerBounds.Min.y), 0);
        }

        public void WriteTile(BinaryWriter writer, Tile tile, vec3 position, TextureMetaFile texture, float ppu,
            ref Bounds layerBounds)
        {
            var chunk = TextureAtlasCell.DefaultChunk;

            if (texture != null)
            {
                chunk = texture?.AtlasData?.GetCell(tile.Index) ?? TextureAtlasCell.DefaultChunk;
            }

            var width = (float)chunk.Width / ppu;
            var height = (float)chunk.Height / ppu;

            var tileMatrix = new mat4(new vec4(1, 0, 0, position.x),
                new vec4(0, 1, 0, position.y),
                new vec4(0, 0, 1, position.z),
                new vec4(0, 0, 0, 1));

            chunk.Uvs = QuadUV.FlipUV(chunk.Uvs, tile.FlipX, tile.FlipY);

            QuadVertices vertices = default;
            GraphicsHelper.CreateQuad(ref vertices, chunk.Uvs, width, height, chunk.Pivot, Color.White, tileMatrix);

            EditorUtils.WriteStruct(writer, vertices);

            ref var max = ref layerBounds.Max;
            ref var min = ref layerBounds.Min;
            layerBounds.Max = new vec3(Math.Max(max.x, position.x), Math.Max(max.y, position.y), 0);
            layerBounds.Min = new vec3(Math.Min(min.x, position.x), Math.Min(min.y, position.y), 0);
        }

        //Convert to world position.
        private vec2 ConvertToWorld(long x, long y, float pixelPerUnit, ldtk.Level level, LayerInstance layer,
            bool isGridPos = false)
        {
            // NOTE: Points from 'entity fields data' are in grid position.
            if (isGridPos)
            {
                x *= layer.GridSize;
                y *= layer.GridSize;
            }

            return new vec2(level.WorldX + x + layer.PxOffsetX, -level.WorldY + -y + -layer.PxOffsetY) / pixelPerUnit;
        }

        public static uint HexToRgbaUint(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException("Invalid hex color.");
            }

            hex = hex.TrimStart('#');

            if (hex.Length != 6)
            {
                throw new ArgumentException("Hex color must be 6 characters (RRGGBB).");
            }

            var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return (uint)(ColorPacketRGBA)new Color32(r, g, b, 255);
        }
    }
}