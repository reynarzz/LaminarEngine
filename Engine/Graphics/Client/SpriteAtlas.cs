using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SpriteAtlas : AssetResourceBase
    {
        private Sprite[] _sprites;
        public DefaultMetaFile _textureMeta;
        private Texture2D _targetTexture;
        public SpriteAtlas(DefaultMetaFile metadata, Texture2D targetTexture) : base(string.Empty, Guid.Empty)
        {
            int spritesLengthFromMetadata = 0; // TODO: get this from the texture metadata.
            _sprites = new Sprite[spritesLengthFromMetadata];
            _targetTexture = targetTexture; 
        }

        internal override void UpdateResource(object data, string path, Guid guid)
        {
            _textureMeta = (DefaultMetaFile)data;

            for (int i = 0; i < _sprites.Length; i++)
            {
                var sprite = _sprites[i];
                if(sprite != null)
                {
                    // TODO: only update valid sprites here, do not create new instances.
                    sprite.UpdateResource(null, string.Empty, guid);
                }
            }
        }

        public Sprite GetSprite(int index)
        {
            var sprite = _sprites[index];

            if (!sprite)
            {
                // TODO: Lazy load the sprite and add it to the array.
                //-- var spriteInfo = _textureMeta.GetSpriteInfo(index);
                //-- spriteInfo.uv;
                 sprite = new Sprite(_targetTexture);
                _sprites[index] = sprite;
            }
            return sprite;
        }
    }
}
