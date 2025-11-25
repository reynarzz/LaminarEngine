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

            public static Collectible InstantiateCollectible(ItemId item, vec2 position)
            {
                var collectible = new Actor("Collectible_"+ item.ToString()).AddComponent<Collectible>();
                collectible.Transform.WorldPosition = position;

                // TODO: this depends on the type of the collectible
                var tex = Assets.GetTexture("starkTileset.png");
                var tiles = TextureAtlasUtils.SliceSprites(tex, 16, 16, 281, 1);
                var audioClip = Assets.GetAudioClip("Audio/HALFTONE/Gameplay/Collectibles_2.wav");

                collectible.Init(new Collectible.CollectibleConfig()
                {
                    Item = item,
                    Amount = 1,
                    IdleSprites = [ tiles[0]],
                    CollectedSprites = null,
                    TriggerSize = new vec2(0.8f, 0.8f),
                    AnimFPS = 7,
                    TargetLayer = LayerMask.NameToLayer(GameLayers.PLAYER),
                    CollectedAudioClip = audioClip
                });

                return collectible;
            }
        }
    }
}