using Engine;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class CoinCollectible : CollectibleBase
    {
        private AudioClip _collectedClip;

        protected override CollectibleConfig Init()
        {
            var tex = Assets.GetTexture("starkTileset.png");
            var tiles = TextureAtlasUtils.SliceSprites(tex, 16, 16, 281, 4);

            _collectedClip = Assets.GetAudioClip("Audio/HALFTONE/Gameplay/Collectibles_2.wav");

            return new CollectibleConfig()
            {
                Item = GameItem.Coin,
                Amount = 1,
                IdleSprites = tiles,
                CollectedSprites = null,
                TriggerSize = new vec2(1, 1),
                AnimFPS = 7,
                TargetLayer = LayerMask.NameToLayer(GameLayers.PLAYER)
            };
        }

        public override void OnTargetCollided(bool collision)
        {
            if (collision)
            {
                AudioSource.PlayOneShot(_collectedClip, 0.2f);
                Collect();
                Disable();
            }
        }
    }
}
