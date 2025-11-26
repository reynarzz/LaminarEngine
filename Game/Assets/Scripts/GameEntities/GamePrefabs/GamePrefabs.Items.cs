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
        public static class Items
        {
            private static Material _collectiblesDefault;

            static Items()
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
                var collectible = new Actor("Collectible_" + item.ToString()).AddComponent<Collectible>();
                collectible.Transform.WorldPosition = position;

                var tiles = GameTextureAtlases.GetAtlas(item.ToString());
                var audioClip = Assets.GetAudioClip("Audio/HALFTONE/Gameplay/Collectibles_2.wav");

                collectible.Init(new Collectible.CollectibleConfig()
                {
                    Item = item,
                    Amount = 1,
                    IdleSprites = tiles,
                    CollectedSprites = null,
                    TriggerSize = new vec2(0.8f, 0.8f),
                    AnimFPS = 7,
                    TargetLayer = LayerMask.NameToLayer(GameConsts.PLAYER),
                    CollectedAudioClip = audioClip
                });

                return collectible;
            }

            public static Chest InstantiateChest(vec2 position, ChestData data)
            {
                var chest = new Actor("Chest").AddComponent<Chest>();
                chest.GetComponent<SpriteRenderer>().SortOrder = 4;
                chest.Transform.WorldPosition = position;
                chest.Init(data);
                return chest;
            }
        }
    }
}