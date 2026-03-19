using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SpriteAtlas : Asset
    {
        private List<Sprite> _sprites;
        private TextureMetaFile _textureMeta;
        private readonly Texture2D _targetTexture;
        public int SpriteCount => _sprites.Count;
        public SpriteAtlas(TextureMetaFile metadata, Texture2D targetTexture, Guid refId) : base(refId)
        {
            _textureMeta = metadata;
            _targetTexture = targetTexture;
            _sprites = new List<Sprite>();
            CollectionsMarshal.SetCount(_sprites, metadata.AtlasData.ChunksCount);

            if (_sprites.Count == 0)
            {
                _sprites.Add(default);
            }
        }

        protected override void OnUpdateResource(object data, Guid id)
        {
            _SetID(id);

            _textureMeta = (TextureMetaFile)data;

            CollectionsMarshal.SetCount(_sprites, _textureMeta.AtlasData.ChunksCount);

            for (int i = 0; i < _textureMeta.AtlasData.ChunksCount; i++)
            {
                var sprite = _sprites[i];
                if (sprite != null)
                {
                    var cell = _textureMeta.AtlasData.GetCell(i);
                    sprite.UpdateResource(_targetTexture, cell, i);
                }
            }
        }

        public Sprite GetSprite(int index)
        {
            if (_textureMeta.AtlasData.ChunksCount == 0)
            {
                // Return default sprite if the texture is texture2D.
                var defaultSprite = _sprites[index];

                if (defaultSprite == null)
                {
                    var defaultCell = TextureAtlasCell.DefaultChunk;
                    defaultCell.ID = _targetTexture.GetID();
                    defaultCell.Width = _targetTexture.Width;
                    defaultCell.Height = _targetTexture.Height;
                    defaultSprite = CreateSprite(defaultCell, index);
                }
                return defaultSprite;
            }
            if (_textureMeta.AtlasData.ChunksCount <= index)
            {
                Debug.Error($"Invalid sprite index '{index}'. Out of range. Max'{_textureMeta.AtlasData.ChunksCount}'");
                return null;
            }

            var sprite = _sprites[index];

            if (!sprite)
            {
                // Lazy load the sprite and add it to the list.
                sprite = CreateSprite(_textureMeta.AtlasData.GetCell(index), index);
            }
            return sprite;
        }

        private Sprite CreateSprite(TextureAtlasCell cell, int index)
        {
            var sprite = new Sprite(_targetTexture, cell, index);
            _sprites[index] = sprite;

            return sprite;
        }
    }
}
