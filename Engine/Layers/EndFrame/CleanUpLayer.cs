using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    internal class CleanUpLayer : LayerBase
    {
        public static bool CleaningUp { get; private set; }

        public override Task Initialize()
        {
            return Task.CompletedTask;
        }

        internal override void UpdateLayer()
        {
            CleaningUp = true;
            SceneManager.OnCleanupUpdate();
            CleaningUp = false;
        }

        public override void Close()
        {
        }

    }
}
