using Engine;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections;
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
            static Items()
            {
            }

            public static Collectible[] InstantiateCollectible(ItemId item, int amount, bool instantiateAmount, vec2 position)
            {
                int count = (instantiateAmount ? amount : 1);
                var collectibles = new Collectible[count];

                IEnumerator InstantiateCollectibles()
                {
                    for (int i = 0; i < count; i++)
                    {
                        var angle = glm.radians(Random.Shared.Next(60, 120));
                        var dir = new vec2(MathF.Cos(angle), MathF.Sin(angle)) * 7;

                        var collectible = new Actor("Collectible_" + item.ToString()).AddComponent<Collectible>();
                        collectible.Transform.WorldPosition = position;

                        var audioClip = Assets.GetAudioClip("Audio/HALFTONE/Gameplay/Collectibles_2.wav");

                        collectible.Init(new Collectible.CollectibleConfig()
                        {
                            Item = item,
                            Amount = instantiateAmount ? 1 : amount,
                            Sprite = GameTextures.GetSprite(item.ToString()),
                            TriggerSize = new vec2(1f, 1f),
                            TargetLayer = LayerMask.NameToLayer(GameConsts.PLAYER),
                            CollectedAudioClip = audioClip,
                            ForceDir = dir
                        });

                        collectibles[i] = collectible;

                        yield return null;
                    }
                }
                GameManager.Instance.StartCoroutine(InstantiateCollectibles());

                return collectibles;
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