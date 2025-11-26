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
    public struct ChestLoot
    {
        public ItemId Item { get; set; }
        public int Amount { get; set; }
    }

    public class Chest : AnimatedInteractable
    {
        public ChestLoot[] ChestLoot { get; private set; }
        private bool _isOpened = false;
        public override void OnAwake()
        {
            base.OnAwake();
        }

        public void SetChestLoot(params ChestLoot[] loot)
        {
            ChestLoot = loot;

            if (ChestLoot != null)
            {
                var openAtlasId = ChestLoot.Length > 2 ? "chest_normal_fill_open" : "chest_small_fill_open";
                var idleAtlasId = ChestLoot.Length > 2 ? "chest_normal_idle" : "chest_small_idle";
                SetAnims(idleAtlasId, openAtlasId);
            }
            else
            {
                SetAnims("chest_normal_idle", "chest_normal_empty_open");
            }

            Animator.OnUpdate += x =>
            {
                SpriteRenderer.Sprite = x.GetSprite("Sprite");
            };
        }

        private void SetAnims(string idleAtlasId, string openAtlasId)
        {
            if (!Animator)
            {
                Animator = GetComponent<Animator>();
            }
            var idleAnim = AnimatorUtils.AddState(Animator, "Idle", false);
            var openAnim = AnimatorUtils.AddState(Animator, "Open", false);

            idleAnim.Clip.AddCurve("Sprite", new SpriteCurve(11, GameTextureAtlases.GetAtlas(idleAtlasId)));
            openAnim.Clip.AddCurve("Sprite", new SpriteCurve(11, GameTextureAtlases.GetAtlas(openAtlasId)));
        }

        public override bool TryInteract(Player player)
        {
            if (!_isOpened && CanInteract(player))
            {
                _isOpened = true;
                Animator.Play("Open");
                foreach (var loot in ChestLoot)
                {
                    player.Inventory.Add(loot.Item, loot.Amount);
                }
                return true;
            }
            return false;
        }
    }
}