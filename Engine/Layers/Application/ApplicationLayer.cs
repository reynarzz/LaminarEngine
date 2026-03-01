using Engine.Data;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    public abstract class ApplicationLayer : LayerBase
    {
        public sealed override Task InitializeAsync()
        {
#if SHIP_BUILD
            bool TryGetSceneAssetSafe(SceneSettings settings, out SceneAsset asset)
            {
                asset = null;
                for (int i = 0; i < settings.ScenesRelease.Length; i++)
                {
                    asset = (SceneAsset)Assets.GetAssetFromGuid(settings.ScenesRelease[i]);

                    if (asset)
                    {
                        return true;
                    }
                }

                return false;
            }
            Debug.Log("Load Launch Scene");
            var sceneSettings = EngineServices.GetService<EngineDataService>().GetProjectSettings().SceneSettings;
            SceneManager.Initialize();
            var scene = (SceneAsset)Assets.GetAssetFromGuid(sceneSettings.LaunchScene);

            if (!scene)
            {
                Debug.Warn("Launch scene wasn't found, make sure it's added to the project settings.");

                if(!TryGetSceneAssetSafe(sceneSettings, out scene))
                {
                    Debug.Warn("No valid scenes were found to load.");
                    return Task.FromCanceled(new CancellationToken(true));
                }
            }

            SceneDeserializer.DeserializeScene(scene.SceneIR, SceneManager.ActiveScene);
#endif
            return MainThreadDispatcher.EnqueueAsync(OnInitialize);
        }

        protected abstract void OnInitialize();
        public virtual void OnFocusEnter() { }
        public virtual void OnFocusExit() { }
    }
}