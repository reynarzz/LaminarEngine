using Editor.Utils;
using Engine;
using GlmNet;
using ldtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
            String,
            Int,
            Float,
            Vec2,
            MultiLine
        }

        public struct EntityPropertyValue
        {
            public PropertyValueType Type { get; set; }
            public int IntValue { get; set; }
            public string StringValue { get; set; }
            public string EnumValue { get; set; }
            public float FloatValue { get; set; }
            public vec2 PointValue { get; set; }
        }

        /* Tilemap file format
        Header char[4]
        Version char[4]

        -Metadata
        Levels count (u32)
        Vertices chunk location (u64)
        Levels chunk location (u64)
        
        Vertices chunk size bytes (u64)
         
        */


        /*Level format
             identifier str len (u32)
             Identifier (string)
             iid str len (u32)
             iid (string)
             WorldX (s32)
             WorldY (s32)
             Level width px (u32)
             Level height px (u32)
             Depth (s32)
             Bounding box (vec4)
             Layers count (u32)
             Layers locations (u64[])
         */

        /* Layer format
            identifier str len (u32)
            Identifier (string)
            iid str len (u32)
            iid (string)
            Visible (bool)
            PixelsPerUnit (u32) (16px default)
            Grid based height (u32)
            Grid based width (u32)
            Grid size (u32)
            Opacity (float)
            Offset Pixels X (s32) 
            Offset Pixels Y (s32) 
            Bounding box (vec4)
            
            Layer type (s32)
            Tiles count (u32)
            Tiles worldPositions (vec2)
        */

        public const float DEFAULT_PIXEL_PER_UNIT = 16;

        AssetProccesResult IAssetProcessor.Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            var bytes = new byte[(int)reader.BaseStream.Length];
            reader.BaseStream.ReadExactly(bytes, 0, bytes.Length);
            var json = Encoding.UTF8.GetString(bytes);
            var ldtk = LdtkJson.FromJson(json);

            return new AssetProccesResult()
            {
                IsSuccess = true,
                //Data = Encoding.UTF8.GetBytes(reader.ReadToEnd())
            };
        }

        public void WriteTilemap(BinaryWriter writer, LdtkJson project, TilemapMeta meta)
        {
            for (var i = 0; i < project.Levels.Length; i++)
            {
                var level = project.Levels[i];
                var levelBounds = Bounds.GetInitialized();
                var levelConfig = meta.LevelConfig[i];

                for (int j = 0; j < level.LayerInstances.Length; j++)
                {
                    var layer = level.LayerInstances[j];
                    var gridSize = new vec2(layer.CWid, layer.CHei);

                    var texture = levelConfig != null ? Assets.GetAssetFromGuid(levelConfig.LayersTextureRef[j]) as Texture2D: null;

                    if (!layer.Visible)
                        continue;

                    switch (layer.Type)
                    {
                        case "AutoLayer":
                        case "IntGrid":
                            PaintLayerTiles(writer, level, j, layer, layer.AutoLayerTiles, texture, ref levelBounds);
                            break;
                        case "Tiles":
                            PaintLayerTiles(writer, level, j, layer, layer.GridTiles, texture, ref levelBounds);
                            break;
                        default:
                            break;
                    }

                    for (int k = 0; k < layer.EntityInstances.Length; k++)
                    {
                        var entity = layer.EntityInstances[k];

                    }
                }
            }
        }

        private void PaintLayerTiles(BinaryWriter writer, ldtk.Level level, int layerIndex, ldtk.LayerInstance layer,
                                     ldtk.TileInstance[] tiles, Texture2D texture, ref Bounds levelBounds)
        {
            int indexToDraw = tiles.Length * 6;
            var layerBounds = Bounds.GetInitialized();
            var tilesPositions = new vec2[tiles.Length];

            float ppu = texture?.PixelPerUnit ?? DEFAULT_PIXEL_PER_UNIT;

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

                WriteTile(writer, new Tile((int)tile.T, isFlippedX, isFlippedY), position, texture, ppu, ref layerBounds);
            }

            layerBounds.Min -= vec3.One * 0.5f;
            layerBounds.Max += vec3.One * 0.5f;
            layerBounds.Min.z = 0;
            layerBounds.Max.z = 0;

            ref var max = ref levelBounds.Max;
            ref var min = ref levelBounds.Min;
            levelBounds.Max = new vec3(Math.Max(max.x, layerBounds.Max.x), Math.Max(max.y, layerBounds.Max.y), 0);
            levelBounds.Min = new vec3(Math.Min(min.x, layerBounds.Min.x), Math.Min(min.y, layerBounds.Min.y), 0);
        }

        public void WriteTile(BinaryWriter writer, Tile tile, vec3 position, Texture2D texture, float ppu, ref Bounds layerBounds)
        {
            var chunk = Assets.GetSpriteAtlas(texture.Path).GetSprite(tile.Index).GetAtlasCell();

            var width = (float)chunk.Width / ppu;
            var height = (float)chunk.Height / ppu;

            var tileMatrix = new mat4(new vec4(1, 0, 0, position.x),
                                      new vec4(0, 1, 0, position.y),
                                      new vec4(0, 0, 1, position.z),
                                      new vec4(0, 0, 0, 1));

            chunk.Uvs = QuadUV.FlipUV(chunk.Uvs, tile.FlipX, tile.FlipY);

            QuadVertices vertices = default;
            GraphicsHelper.CreateQuad(ref vertices, chunk.Uvs, width, height, chunk.Pivot, Color.White, tileMatrix);

            WriteStruct(writer, vertices);

            ref var max = ref layerBounds.Max;
            ref var min = ref layerBounds.Min;
            layerBounds.Max = new vec3(Math.Max(max.x, position.x), Math.Max(max.y, position.y), 0);
            layerBounds.Min = new vec3(Math.Min(min.x, position.x), Math.Min(min.y, position.y), 0);
        }

        //Convert to world position, pixel unit division must be applied later.
        private vec2 ConvertToWorld_NoPixelUnit(long x, long y, ldtk.Level level, LayerInstance layer, bool isGridPos = false)
        {
            if (isGridPos)
            {
                x *= layer.GridSize;
                y *= layer.GridSize;
            }
            return new vec2(level.WorldX + x + layer.PxOffsetX, -level.WorldY + -y + -layer.PxOffsetY);
        }

        public static void WriteStruct<T>(BinaryWriter writer, in T value) where T : unmanaged
        {
            Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write(buffer, in value);
            writer.Write(buffer);
        }
    }
}