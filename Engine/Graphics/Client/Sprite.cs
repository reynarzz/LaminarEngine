using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Sprite : EObject
    {
        public Texture2D Texture { get; }
        public int AtlasIndex { get; }

        public Sprite() : this(0, Texture2D.White)
        {
        }
        public Sprite(Texture2D texture) : this(0, texture)
        {
        }
        public Sprite(int atlasIndex, Texture2D texture)
        {
            AtlasIndex = atlasIndex;
            Texture = texture;
        }
       
        public AtlasChunk GetAtlasChunk()
        {
            if (Texture)
            {
                var chunk = Texture.Atlas.GetChunk(AtlasIndex);

                if (chunk.Width <= 1 && chunk.Height <= 1)
                {
                    chunk.Width = Texture.Width;
                    chunk.Height = Texture.Height;
                }

                return chunk;
            }

#if DEBUG
            Debug.Error($"Sprite: {Name}, doesn't have a texture attached, using default atlas chunk instead.");
#endif
            return AtlasChunk.DefaultChunk;
        }
    }
}
