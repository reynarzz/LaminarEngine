using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using GlmNet;
using Engine;

namespace Engine.Utils
{
    public class TextureAtlasUtils
    {
        public static TextureAtlasCell CreateTileBounds(int xPixel, int yPixel, int width, int height, float pivotX, float pivotY, int baseTextureWidth, int baseTextureHeight)
        {
            float x = xPixel / (float)baseTextureWidth;
            float y = yPixel / (float)baseTextureHeight;
            float nWidth = (float)width / (float)baseTextureWidth;
            float nHeight = (float)height / (float)baseTextureHeight;

            // adds pixel tolerance to remove bleeding pixel from adjacent tiles
            float ptX = (1.0f / (float)(baseTextureWidth * width)) / 2.0f;
            float ptY = (1.0f / (float)(baseTextureHeight * width)) / 2.0f;

            return new TextureAtlasCell()
            {
                ID = Guid.NewGuid(),
                Width = width,
                Height = height,
                Pivot = new vec2(pivotX, pivotY),
                XPixel = xPixel,
                YPixel = yPixel,
                Uvs = new QuadUV()
                {
                    BottomLeftUV = new vec2(x + ptX, y + ptY),                    // Bottom left
                    TopLeftUV = new vec2(x + ptX, y + nHeight - ptY),             // Top left
                    TopRightUV = new vec2(x + nWidth - ptX, y + nHeight - ptY),   // Top right
                    BottomRightUV = new vec2(x + nWidth - ptX, y + ptY),          // Bottom right
                },
            };
        }

        public static void SliceTiles(TextureAtlasData data, int tileWidth, int tileHeight, int textureWidth, int textureHeight,
                                      float pivotX = 0.5f, float pivotY = 0.5f)
        {
            if (tileWidth <= 0 || tileHeight <= 0)
            {
                Debug.Error("negative tile size is not supported");
                return;
            }

            int tilesX = textureWidth / tileWidth;
            int tilesY = textureHeight / tileHeight;

            if (tilesX <= 0 || tilesY <= 0)
                return;

            var atlasChunks = new TextureAtlasCell[tilesX * tilesY];
            int index = 0;

            for (int y = 0; y < tilesY; y++)
            {
                int flippedY = (tilesY - 1 - y) * tileHeight;

                for (int x = 0; x < tilesX; x++)
                {
                    atlasChunks[index++] = CreateTileBounds(x * tileWidth, flippedY, tileWidth, tileHeight,
                                                            pivotX, pivotY, textureWidth, textureHeight);
                }
            }

            data.SetChunks(atlasChunks);
        }
    }
}
