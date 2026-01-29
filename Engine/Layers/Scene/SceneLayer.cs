using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Layers
{
    internal class SceneLayer : LayerBase
    {
        public override Task InitializeAsync()
        {
            SceneManager.Initialize();

            return Task.CompletedTask;
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
