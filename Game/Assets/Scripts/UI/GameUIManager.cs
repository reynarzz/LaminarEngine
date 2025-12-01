using Engine;
using Engine.GUI;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class GameUIManager : ScriptBehavior
    {
        public PauseMenu PauseMenu { get; private set; }
        public static PlayerHealthUI PlayerHealth { get; private set; }
        public static InventoryUI Inventory { get; private set; }
        private UICanvas _hudCanvas;

        protected override void OnAwake()
        {
            Actor.DontDestroyOnLoad(this);
            _hudCanvas = new Actor("HUD Canvas").AddComponent<UICanvas>();
            _hudCanvas.Transform.Parent = Transform;

            InitializeUIs();
        }

        private void InitializeUIs()
        {
            PlayerHealth = InitUI<PlayerHealthUI>("Player Health UI");
            Inventory = InitUI<InventoryUI>("Inventory");

            PauseMenu = InitUI<PauseMenu>("Pause menu");
        }

        private T InitUI<T>(string name) where T : GameUI
        {
            var ui = new Actor(name).AddComponent<T>();
            ui.Transform.Parent = _hudCanvas.Transform;
            return ui;
        }

        public void GoToMainMenu()
        {

        }

        public static UIText NewText(string name, string value, vec2 position, Transform parent)
        {
            var text = new Actor(name).AddComponent<UIText>();
            text.Transform.Parent = parent;
            text.Font = GameManager.DefaultFont;
            text.Material = GameManager.DefaultMaterial;
            text.SetText(value);
            text.Transform.LocalPosition = position;
            text.BlockEvents = false;
            text.ReceiveEvents = false;

            return text;
        }

        public static UIImage NewImage(string name, vec2 position, vec2 size, Color color, Transform parent)
        {
            var image = new Actor(name).AddComponent<UIImage>();
            image.Material = GameManager.DefaultMaterial;
            image.Transform.Parent = parent;
            image.RectTransform.Pivot = vec2.Half;
            image.RectTransform.Size = size;
            image.Transform.LocalPosition = position;
            image.Color = color;

            return image;
        }
    }
}
