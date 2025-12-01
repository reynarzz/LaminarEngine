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

      
    }
}
