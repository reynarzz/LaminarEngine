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

        public TextureAsset(Guid guid, Texture texture, SpriteAtlas atlas) : base(guid)
        {
            Texture = texture;
            Atlas = atlas;
        }

        protected override void OnUpdateResource(object data, Guid guid)
        {
            Texture.UpdateResource(data, guid);
        }
    }
}
