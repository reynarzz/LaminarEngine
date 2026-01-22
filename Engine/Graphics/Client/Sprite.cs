using GlmNet;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Sprite : EObject
    {
        public Texture2D Texture { get; private set; }
        internal int AtlasIndex { get; private set; }
        private TextureAtlasCell _cell;

        internal Sprite(Texture2D texture, TextureAtlasCell cell, int index) : base(CreateSpriteName(texture, index), cell.ID)
        {
            AtlasIndex = index;
            Texture = texture;
            _cell = cell;
        }

        public Sprite(Texture2D texture) : this(0, texture)
        {
            Name = texture?.Name;
        }
        public Sprite(int atlasIndex, Texture2D texture) : base(texture.Path, Guid.NewGuid())
        {
            AtlasIndex = atlasIndex;
            Texture = texture;
            Name = texture?.Name;
        }

        internal TextureAtlasCell GetAtlasCell()
        {
            if (Texture)
            {
                var chunk = _cell;

                if (chunk.Width <= 1 && chunk.Height <= 1)
                {
                    chunk = TextureAtlasCell.DefaultChunk;
                    chunk.ID = GetID();
                    chunk.Width = Texture.Width;
                    chunk.Height = Texture.Height;
                }

                return chunk;
            }

#if DEBUG
            Debug.Error($"Sprite: {Name}, doesn't have a texture attached, using default atlas chunk instead.");
#endif
            return TextureAtlasCell.DefaultChunk;
        }

        internal void UpdateResource(Texture2D texture, TextureAtlasCell cell, int index)
        {
            Texture = texture;
            Name = CreateSpriteName(texture, index);
            _cell = cell;
            AtlasIndex = index;
            _SetID(cell.ID);
        }

        private static string CreateSpriteName(Texture2D texture, int index)
        {
            var postFix = (index > 0) ? $"[{index}]" : string.Empty;
            return $"{texture.Name}{postFix}";
        }
    }
}