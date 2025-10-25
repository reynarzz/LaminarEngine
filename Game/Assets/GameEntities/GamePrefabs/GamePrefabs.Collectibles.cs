using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal partial class GamePrefabs
    {
        public static class Collectibles
        {
            private static Material _collectiblesDefault;

            static Collectibles()
            {
                _collectiblesDefault = new Material(new Shader(Assets.GetText("Shaders/SpriteVert.vert").Text, Assets.GetText("Shaders/SpriteFrag.frag").Text));
                _collectiblesDefault.Name = "Collectibles Default";

                var PlayerMatPass = _collectiblesDefault.Passes.ElementAt(0);
                PlayerMatPass.Stencil.Enabled = true;
                PlayerMatPass.Stencil.Func = StencilFunc.Always;
                PlayerMatPass.Stencil.Ref = 3;
                PlayerMatPass.Stencil.ZPassOp = StencilOp.Replace;
            }

            public static CoinCollectible InstantiateCoin(vec2 position)
            {
                var coin = new Actor("Coin").AddComponent<CoinCollectible>();
                coin.Transform.WorldPosition = position;
                return coin;
            }
        }
    }
}