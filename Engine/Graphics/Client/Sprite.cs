using GlmNet;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct SpriteUpdateData
    {
        internal int Index;
        internal Texture2D Texture;
        internal TextureAtlasCell Cell;
    }
    public class Sprite : SubAsset<SpriteUpdateData>
    {
        public Texture2D Texture { get; private set; }
        internal int AtlasIndex { get; private set; }
        private TextureAtlasCell _cell;
        public override string Name
        {
            get => CreateSpriteName(Texture?.Name, AtlasIndex);
            set { }
        }
        
        internal Sprite(Texture2D texture, TextureAtlasCell cell, int index) : base(texture.GetID())
        {
            AtlasIndex = index;
            Texture = texture;
            _cell = cell;
        }

        internal Sprite(Texture2D texture) : this(0, texture)
        {
            Name = texture?.Name;
        }
        public Sprite(int atlasIndex, Texture2D texture) : base(texture.GetID())
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

        internal static string CreateSpriteName(string baseName, int index)
        {
            var postFix = (index > 0) ? $"({index})" : string.Empty;
            return $"{baseName}{postFix}";
        }

        internal override void UpdateResource(SpriteUpdateData data, Guid guid)
        {
            Texture = data.Texture;
            Name = CreateSpriteName(Texture.Name, data.Index);
            _cell = data.Cell;
            AtlasIndex = data.Index;
        }
    }
}