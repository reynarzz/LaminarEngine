using GlmNet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct QuadUV
    {
        // Uv's for quad vertices.
        public vec2 BottomLeftUV;
        public vec2 TopLeftUV;
        public vec2 TopRightUV;
        public vec2 BottomRightUV;

        public static readonly QuadUV DefaultUVs = new QuadUV()
        {
            BottomLeftUV = new vec2(0, 0),
            TopLeftUV = new vec2(0, 1),
            TopRightUV = new vec2(1, 1),
            BottomRightUV = new vec2(1, 0)
        };

        public static QuadUV FlipUV(QuadUV uv, bool flipX, bool flipY)
        {
            QuadUV result = uv;

            if (flipX)
            {
                (result.BottomLeftUV, result.BottomRightUV) = (result.BottomRightUV, result.BottomLeftUV);
                (result.TopLeftUV, result.TopRightUV) = (result.TopRightUV, result.TopLeftUV);
            }

            if (flipY)
            {
                (result.TopLeftUV, result.BottomLeftUV) = (result.BottomLeftUV, result.TopLeftUV);
                (result.TopRightUV, result.BottomRightUV) = (result.BottomRightUV, result.TopRightUV);
            }

            return result;
        }
    }

    public struct TextureAtlasCell // Convert to class
    {
        public static TextureAtlasCell DefaultChunk = new TextureAtlasCell()
        {
            Pivot = new vec2(0.5f, 0.5f),
            Uvs = QuadUV.DefaultUVs,
            Width = 1,
            Height = 1,
        };

        public vec2 Pivot { get; set; }
        public QuadUV Uvs { get; set; }
        public int XPixel { get; set; }
        public int YPixel { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void UpdatePivot(vec2 pivot)
        {
            Pivot = pivot;
        }

        public void UpdateUvs(QuadUV uvs)
        {
            Uvs = uvs;
        }
    }

    public class TextureAtlasData
    {
        [JsonProperty] private TextureAtlasCell[] _chunks;
        [JsonIgnore] public int ChunksCount => _chunks?.Length ?? 0;

        // Serializer
        internal TextureAtlasData()
        {
        }

        public TextureAtlasData(int width, int height)
        {
            var defaultChunk = TextureAtlasCell.DefaultChunk;
            defaultChunk.Width = width;
            defaultChunk.Height = height;

            _chunks =
            [
                defaultChunk
            ];
        }

        public bool HasValidChunk(int chunkIndex)
        {
            return _chunks != null && _chunks.Length > chunkIndex;
        }

        public TextureAtlasCell GetCell(int index)
        {
            if (_chunks == null)
            {
                return TextureAtlasCell.DefaultChunk;
            }

            var isInvalidIndex = index >= _chunks.Length;
#if DEBUG
            if (isInvalidIndex)
            {
                Console.WriteLine($"invalid atlas chunk index: '{index}', Atlas Max: '{_chunks.Length}'");
                return TextureAtlasCell.DefaultChunk;
            }
#endif
            return _chunks[index];
        }

        public void UpdateChunk(int index, TextureAtlasCell chunk)
        {
            if (_chunks != null && _chunks.Length > index)
            {
                _chunks[index] = chunk;
            }
        }


        public void SetChunks(TextureAtlasCell[] chunks)
        {
            _chunks = chunks;
        }

    }
}