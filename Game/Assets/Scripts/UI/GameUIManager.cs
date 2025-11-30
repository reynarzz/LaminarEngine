using Engine;
using Engine.GUI;
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
        private UICanvas _hudCanvas;
        public static PlayerHealthUI PlayerHealthUI { get; private set; } 
        protected override void OnAwake()
        {
            Actor.DontDestroyOnLoad(this);

            _hudCanvas = new Actor("HUD Canvas").AddComponent<UICanvas>();

            _hudCanvas.Transform.Parent = Transform;

            InitializeUIs();

            PauseMenu = new Actor("Pause menu").AddComponent<PauseMenu>();
            PauseMenu.Transform.Parent = Transform;

        }

        private void InitializeUIs()
        {
            PlayerHealthUI = InitPlayerHealtUI(_hudCanvas.Transform);
        }

        private PlayerHealthUI InitPlayerHealtUI(Transform parentCanvas)
        {
            var healthUI = new Actor("Player Health UI").AddComponent<PlayerHealthUI>();
            healthUI.Transform.Parent = parentCanvas;
            return healthUI;
        }

        public void GoToMainMenu()
        {

        }
    }
}
