using Engine;
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
        protected override void OnAwake()
        {
            // Actor.DontDestroyOnLoad(this);

            PauseMenu = new Actor("Pause menu").AddComponent<PauseMenu>();

            PauseMenu.Transform.Parent = Transform;
        }

        public void GoToMainMenu()
        {

        }
    }
}
