using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SpriteAtlas : AssetResourceBase
    {
        private List<Sprite> _sprites;
        private TextureMetaFile _textureMeta;
        private readonly Texture2D _targetTexture;
        public int SpriteCount => _sprites.Count;
        public SpriteAtlas(TextureMetaFile metadata, Texture2D targetTexture, Guid id) : base(targetTexture.Path, id)
        {
            _textureMeta = metadata;
            _targetTexture = targetTexture;
            _sprites = new List<Sprite>();
            CollectionsMarshal.SetCount(_sprites, metadata.AtlasData.ChunksCount);
        }

        internal override void UpdateResource(object data, string path, Guid id)
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
            if (_textureMeta.AtlasData.ChunksCount <= index)
            {
                Debug.Error("Invalid sprite index. Out of range");
                return null;
            }

            var sprite = _sprites[index];

            if (!sprite)
            {
                // Lazy load the sprite and add it to the list.
                var chunk = _textureMeta.AtlasData.GetCell(index);
                sprite = new Sprite(_targetTexture, chunk, index);
                _sprites[index] = sprite;
            }
            return sprite;
        }
    }
}
