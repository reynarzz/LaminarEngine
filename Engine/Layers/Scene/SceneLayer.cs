using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Layers
{
    internal class SceneLayer : LayerBase
    {
        public override void Initialize()
        {
            SceneManager.Initialize();
        }

        internal override void UpdateLayer()
        {
            SceneManager.UpdateScenes();
        }

        public override void Close()
        {
            SceneManager.UnloadAll();
        }
    }
}
