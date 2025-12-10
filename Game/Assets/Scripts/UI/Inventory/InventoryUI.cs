using Engine;
using Engine.GUI;
using Engine.Types;
using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    [RequireComponent(typeof(UICanvas))]
    public class InventoryUI : GameUI
    {
        [RequiredProperty] private UICanvas _canvas;
        private UIElement _inventory;
        private bool _show = false;
        private float _showT = 0;
        private bool _isShowing;
        private ContentSizeFitter _fitter;

        private InventorySlotUI[] _slotsUI;
        private GridLayout _gridLayout;
        private bool _isInitialized;

        protected override void OnAwake()
        {
            base.OnAwake();

            BuildInventory();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            //if (Input.GetKeyDown(KeyCode.Tab))
            //{
            //    _show = !_show;
            //    Show(_show);
            //}
        }

        private void BuildInventory()
        {
            _inventory = new Actor<UIElement>("Inventory").GetComponent<UIElement>();
            _inventory.Transform.Parent = _canvas.Transform;

            var inventoryImage = UiUtils.NewImage("Inventory image", default, new vec2(320, 240), Color.White, _inventory.Transform);
            _fitter = inventoryImage.AddComponent<ContentSizeFitter>();
            inventoryImage.Material = MaterialUtils.Instance.WobbleMaterial;
            //inventoryImage.Sprite = new Sprite(Assets.GetTexture("pixel-ui_panel.png"));
            inventoryImage.IsSliced = true;
            inventoryImage.SlicedBorderResolution = 2.5f;
            //inventoryImage.Color = new Color(0,0,0,  0.1f);


            var inventoryTitleText = UiUtils.NewText("inventory title", "Inventory", new vec2(0, -52), _inventory.Transform);
            inventoryTitleText.Fit = TextFit.ExpandToFit;
            inventoryTitleText.FontSize = 30;
            inventoryTitleText.OutlineSize = 0;
            inventoryTitleText.FontResolution = 10;
            //inventoryTitleText.Transform.LocalScale = vec3.One * 0.3f;

            _gridLayout = new Actor("HorizontalRect").AddComponent<GridLayout>();
            _gridLayout.Transform.Parent = inventoryImage.Transform;
            _gridLayout.Transform.LocalPosition = new vec3(0, 0);
            _gridLayout.ResizeToFitVertical = true;
            _gridLayout.ResizeToFitHorizontal = true;
            _gridLayout.Spacing = 4;
            _gridLayout.Padding = new Thickness(12);
            _gridLayout.Padding.Top = 38;
            _gridLayout.StartPivot = new vec2(0.5f, 0.5f);
            _gridLayout.MaxPerRow = 3;
            _gridLayout.ContentsSize = new vec2(46, 46);

            _inventory.Transform.LocalPosition = new vec3(120, 180);

            // _inventory.Actor.IsActiveSelf = false;
        }

        public void InitInventory(Inventory inventory)
        {
            if (_isInitialized)
            {
                UpdateInventory(inventory);
                inventory.OnInventoryChanged -= UpdateInventory;
                inventory.OnInventoryChanged += UpdateInventory;
                return;
            }

            _slotsUI = new InventorySlotUI[inventory.MaxSlots];
            for (int i = 0; i < _slotsUI.Length; i++)
            {
                var slotUI = new InventorySlotUI();
                slotUI.BackgroundImage = UiUtils.NewImage("InventorySlot:" + i, default, new vec2(100, 100), Color.White, _gridLayout.Transform);
                slotUI.BackgroundImage.Sprite = GameTextures.GetSprite("inventory_slot");
                slotUI.IconImage = UiUtils.NewImage("Content", default, new vec2(36, 36), Color.White, slotUI.BackgroundImage.Transform);
                slotUI.IconImage.RectTransform.Pivot = new vec2(0.5f, 0.5f);
                _slotsUI[i] = slotUI;

                //var animator = iconContent.AddComponent<Animator>();
                //var clip = new AnimationClip("Sprite");

                //clip.AddCurve("Sprite", new SpriteCurve(7.0f, GameTextureAtlases.GetAtlas("coin_currency")));
                //animator.AddState(new AnimationState("Coin", clip));
                //animator.OnUpdate += x =>
                //{
                //    iconContent.Sprite = x.GetSprite("Sprite");
                //};

            }

            UpdateInventory(inventory);

            inventory.OnInventoryChanged -= UpdateInventory;
            inventory.OnInventoryChanged += UpdateInventory;
            _isInitialized = true;

        }

        public void UpdateInventory(Inventory inventory)
        {
            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                var slotData = inventory.Slots[i];
                var slotUI = _slotsUI[i];

                slotUI.IconImage.IsEnabled = !slotData.IsEmpty();
                slotUI.IconImage.Color = Color.Red;
                if (!slotData.IsEmpty())
                {
                    slotUI.IconImage.Color = Color.White;
                    slotUI.IconImage.Sprite = GameTextures.GetSprite(slotData.item.Features.Id.ToString());
                }
            }

            _gridLayout.RecalculateLayout();
            _fitter.ResizeToFitChildren();
        }

        private void Show(bool show)
        {
            var hidePos = new vec2(-120, 160);
            var showPos = new vec2(120, 160);

            IEnumerator AnimateShow(vec2 from, vec2 to)
            {
                _showT = 0f;
                while (_showT < 1f)
                {
                    var eased = Easing.Apply(EasingType.EaseInOutCubic, from, to, _showT);
                    eased = Mathf.Lerp((vec2)_inventory.Transform.LocalPosition, to, _showT);

                    _inventory.Transform.LocalPosition = new vec3(eased);

                    _showT += Time.DeltaTime;

                    yield return null;
                }

                _inventory.Transform.LocalPosition = new vec2(to);
                _isShowing = false;
            }

            if (_isShowing)
            {
                StopAllCoroutines();
            }
            StartCoroutine(AnimateShow(show ? hidePos : showPos,
                                       show ? showPos : hidePos));

            _isShowing = true;
        }
    }
}
