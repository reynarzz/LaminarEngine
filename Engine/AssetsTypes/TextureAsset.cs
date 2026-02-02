using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class TextureAsset : AssetResourceBase
    {
        internal override bool IsCacheHardReference { get; private protected set; } = true;
        public Texture Texture { get; }
        public SpriteAtlas Atlas { get; }

        public TextureAsset(string path, Guid guid, Texture texture, SpriteAtlas atlas) : base(path, guid)
        {
            Texture = texture;
            Atlas = atlas;
        }

        internal override void UpdateResource(object data, string path, Guid guid)
        {
            Texture.UpdateResource(data, path, guid);
        }
    }
}
