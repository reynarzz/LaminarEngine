using Engine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Layers
{
    internal class SceneLayer : LayerBase
    {
        public override Task<LayerInitResult> InitializeAsync()
        {
            SceneManager.Initialize();

            var data = EngineServices.GetService<EngineDataService>().GetProjectSettings();
            LayerMask.UpdateLayers(data.Physics.CollisionMatrix, data.LayerSettings.Layers);

            return Task.FromResult(LayerInitResult.Success);
        }

        internal override void UpdateLayer()
        {
            // NOTE/TODO: Maybe in the future I would like to update scripts that contain the attribute 'RunInEditModeAttribute'
            if (Application.IsInPlayMode)
            {
                SceneManager.UpdateScenes();
            }
        }

        public override void Close()
        {
            SceneManager.UnloadAll();
        }
    }
}
