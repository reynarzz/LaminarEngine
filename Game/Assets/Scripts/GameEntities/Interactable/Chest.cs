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
    public struct ItemAmountPair
    {
        public ItemId Item { get; set; }
        public int Amount { get; set; }
    }

    public class Chest : AnimatedInteractable<ChestData>
    {
        private bool _isOpened = false;
        protected override void OnAwake()
        {
            base.OnAwake();
            BoxCollider.Size = new vec2(1.7f, BoxCollider.Size.y);
            SpriteRenderer.Material = MaterialUtils.SpriteMaterial;
        }

        protected override void OnStart()
        {
            base.OnStart();
            SpriteRenderer.SortOrder = GameConsts.ChestRenderSorting;
        }

        public override void Init(ChestData data)
        {
            base.Init(data);

            if (data.ChestLoot != null)
            {
                var openAtlasId = data.ChestLoot.Length > 2 ? "chest_normal_fill_open" : "chest_small_fill_open";
                var idleAtlasId = data.ChestLoot.Length > 2 ? "chest_normal_idle" : "chest_small_idle";
                var collectedAtlasId = data.ChestLoot.Length > 2 ? "chest_normal_empty_open" : "chest_small_empty_open";

                SetAnims(idleAtlasId, openAtlasId, collectedAtlasId);
            }
            else
            {
                SetAnims("chest_normal_idle", "chest_normal_empty_open", "chest_normal_empty_open");
            }

            Animator.OnUpdate += x =>
            {
                SpriteRenderer.Sprite = x.GetSprite("Sprite");
            };
        }

        private void SetAnims(string idleAtlasId, string openAtlasId, string collectedIdleAtlasId)
        {
            if (!Animator)
            {
                Animator = GetComponent<Animator>();
            }
            var idleAnim = AnimatorUtils.AddState(Animator, "Idle", false);
            var openAnim = AnimatorUtils.AddState(Animator, "Open", false);
            var collectedIdle = AnimatorUtils.AddState(Animator, "CollectedIdle", false);

            idleAnim.Clip.AddCurve("Sprite", new SpriteCurve(11, GameTextures.GetAtlas(idleAtlasId)));
            openAnim.Clip.AddCurve("Sprite", new SpriteCurve(11, GameTextures.GetAtlas(openAtlasId)));
            //openAnim.Clip.AddEvent(openAnim.Clip.Duration, () => { });
            collectedIdle.Clip.AddCurve("Sprite", new SpriteCurve(11, GameTextures.GetAtlas(collectedIdleAtlasId)[^1]));

            SpriteRenderer.Sprite = GameTextures.GetAtlas(idleAtlasId)[0];
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetKeyDown(KeyCode.J))
            {
                SpriteRenderer.SortOrder = SpriteRenderer.SortOrder == 0? 3: 0;
            }
        }
        public override bool TryInteract(Player player)
        {
            if (!_isOpened && CanInteract(player))
            {
                _isOpened = true;

                IEnumerator Collect()
                {
                    //if (!player.Inventory.DoesFitInInventory())
                    //{
                    //    yield break;
                    //}
                    Animator.Play("Open");

                    yield return new WaitForSeconds(0.2f);

                    if (Data.ChestLoot != null)
                    {
                        if (Data.LockedBy == ItemId.none || player.Inventory.Use(Data.LockedBy))
                        {
                            foreach (var loot in Data.ChestLoot)
                            {
                                var added = player.Inventory.Add(loot.Item, loot.Amount);

                                if (added)
                                {
                                    // TODO: check how much of this item wasn't added.
                                }
                            }
                            Animator.Play("CollectedIdle");
                        }
                        else
                        {
                            Animator.Play("Idle");
                        }
                    }
                }

                StartCoroutine(Collect());

                return true;
            }
            return false;
        }
    }
}