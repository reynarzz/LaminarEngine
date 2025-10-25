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

        private static Sprite[] _sprites;
        protected override CollectibleConfig Init()
        {
            if(_sprites == null)
            {
                var tex = Assets.GetTexture("starkTileset.png");
                var tiles = TextureAtlasUtils.SliceSprites(tex, 16, 16);
                _sprites = new ArraySegment<Sprite>(tiles, 281, 4).ToArray();
            }
          
            _collectedClip = Assets.GetAudioClip("Audio/HALFTONE/Gameplay/Collectibles_2.wav");

            return new CollectibleConfig()
            {
                Item = GameItem.Coin,
                Amount = 1,
                IdleSprites = _sprites,
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
