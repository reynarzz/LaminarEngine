using GlmNet;
using Engine;
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

        internal Sprite(Texture2D texture, TextureAtlasCell cell, int index) : base(CreateSpriteName(texture?.Name, index), cell.ID)
        {
            AtlasIndex = index;
            Texture = texture;
            _cell = cell;
        }

        internal Sprite(Texture2D texture) : this(0, texture)
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
                var cell = _cell;

                if (cell.Width <= 1 && cell.Height <= 1)
                {
                    cell = TextureAtlasCell.DefaultChunk;
                    cell.ID = GetID();
                    cell.Width = Texture.Width;
                    cell.Height = Texture.Height;
                }

                return cell;
            }

#if DEBUG
            Debug.Error($"Sprite: {Name}, doesn't have a texture attached, using default atlas chunk instead.");
#endif
            return TextureAtlasCell.DefaultChunk;
        }

        internal void UpdateResource(Texture2D texture, TextureAtlasCell cell, int index)
        {
            Texture = texture;
            Name = CreateSpriteName(texture.Name, index);
            _cell = cell;
            AtlasIndex = index;
            _SetID(cell.ID);
        }

        internal static string CreateSpriteName(string baseName, int index)
        {
            var postFix = (index > 0) ? $"({index})" : string.Empty;
            return $"{baseName}{postFix}";
        }
    }
}