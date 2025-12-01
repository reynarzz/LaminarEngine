using Engine;
using Engine.GUI;
using Engine.Types;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class InventoryUI : GameUI
    {
        private UIElement _inventory;

        protected override void OnAwake()
        {
            base.OnAwake();

            BuildInventory();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _inventory.Actor.IsActiveSelf = !_inventory.Actor.IsActiveSelf;
            }
        }

        private void BuildInventory()
        {
            _inventory = new Actor<UIElement, ContentSizeFitter>("Inventory").GetComponent<UIElement>();
            _inventory.Transform.LocalPosition = new vec3(500, 598);
            _inventory.Transform.Parent = Canvas.Transform;

            var inventory = GameUIManager.NewImage("Inventory image", default, new vec2(320, 240), Color.White, _inventory.Transform);
            var fitter = inventory.AddComponent<ContentSizeFitter>();
            inventory.RectTransform.Pivot = new vec2(0.5f, 0.5f);
            var img = inventory.GetComponent<UIImage>();
            img.Material = GameManager.DefaultMaterial;
            img.Sprite = new Sprite(Assets.GetTexture("pixel-ui_panel.png"));
            img.IsSliced = true;
            img.SlicedBorderResolution = 2.5f;

            var inventoryTitleText = GameUIManager.NewText("inventory title", "Inventory", new vec2(0, -52), _inventory.Transform);
            inventoryTitleText.Fit = TextFit.ExpandToFit;
            inventoryTitleText.FontSize = 30;
            inventoryTitleText.OutlineSize = 0;
            inventoryTitleText.FontResolution = 10;
            //inventoryTitleText.Transform.LocalScale = vec3.One * 0.3f;

            var horizontalLayout = new Actor("HorizontalRect").AddComponent<GridLayout>();
            horizontalLayout.Transform.Parent = inventory.Transform;
            horizontalLayout.Transform.LocalPosition = new vec3(0, 0);
            horizontalLayout.ResizeToFitVertical = true;
            horizontalLayout.ResizeToFitHorizontal = true;
            horizontalLayout.Spacing = 4;
            horizontalLayout.Padding = new Thickness(12);
            horizontalLayout.Padding.Top = 38;
            horizontalLayout.StartPivot = new vec2(0.5f, 0.5f);
            horizontalLayout.MaxPerRow = 10;
            horizontalLayout.ContentsSize = new vec2(46, 46);
            var slotSprite = new Sprite(Assets.GetTexture("pixel-ui_slot.png"));

            var parent = GameUIManager.NewImage("Quad1", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            parent.Sprite = slotSprite;
            var coins = GameTextureAtlases.GetAtlas("coin_currency");

            var iconContent = GameUIManager.NewImage("Content", default, new vec2(34, 34), Color.White, parent.Transform);
            iconContent.RectTransform.Pivot = new vec2(0.5f, 0.6f);
            var animator = iconContent.AddComponent<Animator>();
            var clip = new AnimationClip("Sprite");

            clip.AddCurve("Sprite", new SpriteCurve(7.0f, coins));
            animator.AddState(new AnimationState("Coin", clip));
            animator.OnUpdate += x =>
            {
                iconContent.Sprite = x.GetSprite("Sprite");
            };

            for (int i = 0; i < 19; i++)
            {
                GameUIManager.NewImage("IQuad: " + i, default, new vec2(100, 100), Color.White, horizontalLayout.Transform).Sprite = slotSprite;
            }
            horizontalLayout.RecalculateLayout();
            fitter.ResizeToFitChildren();
            _inventory.Actor.IsActiveSelf = false;
        }

    }
}
