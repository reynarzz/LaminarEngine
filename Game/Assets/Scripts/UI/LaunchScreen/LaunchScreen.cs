using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class LaunchScreen : ScriptBehavior
    {
        private Camera _camera;
        public override void OnAwake()
        {
            _camera = new Actor<Camera>("Camera").GetComponent<Camera>();
            OnComplete();
        }

        private void OnComplete()
        {
            SceneManager.Test_LoadScene(new Scene());
            new Actor<GameManager>("GameManager");
        }
    }
}
