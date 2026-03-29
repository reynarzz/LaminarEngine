using Editor.Data;
using Engine;
using Engine.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Layers
{
    internal class ProjectLoaderLayer : LayerBase
    {
        public override Task<LayerInitResult> InitializeAsync()
        {
            if (!EditorConfigManager.IsProjectLoaded())
            {
                return Task.FromResult(LayerInitResult.InProgress);
            }

            return Task.FromResult(LayerInitResult.Success);
        }

        public override void Close()
        {
        }
    }
}
